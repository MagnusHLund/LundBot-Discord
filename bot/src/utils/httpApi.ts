import { createServer, IncomingMessage, ServerResponse } from 'node:http';
import { URL } from 'node:url';
import { createHash } from 'node:crypto';
import type { Client } from 'discord.js';
import { getPrismaClient } from '../services/database.js';

const API_PORT = Number(process.env.BOT_API_PORT ?? '3000');
const WEB_TRAFFIC_CHANNEL_ID = process.env.WEB_TRAFFIC_CHANNEL_ID?.trim() ?? '';
const MAX_BODY_SIZE = 1024 * 1024;
const MAX_TRAFFIC_MESSAGE_LENGTH = 1900;
const MAX_TRAFFIC_ROWS_IN_MESSAGE = 1000;

type JsonRecord = Record<string, unknown>;

type TrafficEventType = 'visit' | 'invite-click';

function sendJson(res: ServerResponse, statusCode: number, payload: JsonRecord): void {
  res.writeHead(statusCode, {
    'Content-Type': 'application/json; charset=utf-8',
  });
  res.end(JSON.stringify(payload));
}

function setCorsHeaders(res: ServerResponse): void {
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PATCH, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization, X-Api-Key');
  res.setHeader('Access-Control-Max-Age', '3600');
}

async function readJsonBody(req: IncomingMessage): Promise<JsonRecord> {
  return await new Promise((resolve, reject) => {
    let raw = '';

    req.on('data', (chunk: Buffer) => {
      raw += chunk.toString('utf8');

      if (Buffer.byteLength(raw, 'utf8') > MAX_BODY_SIZE) {
        reject(new Error('Request body too large'));
        req.destroy();
      }
    });

    req.on('end', () => {
      if (!raw.trim()) {
        resolve({});
        return;
      }

      try {
        resolve(JSON.parse(raw) as JsonRecord);
      } catch {
        reject(new Error('Invalid JSON body'));
      }
    });

    req.on('error', reject);
  });
}

function getRequestIp(req: IncomingMessage): string {
  const forwarded = req.headers['x-forwarded-for'];
  if (typeof forwarded === 'string' && forwarded.trim()) {
    return forwarded.split(',')[0].trim();
  }

  const remoteAddress = req.socket.remoteAddress;
  if (remoteAddress) {
    return remoteAddress;
  }

  return '0.0.0.0';
}

function hashIp(ip: string): Buffer {
  return createHash('sha256').update(ip).digest();
}

function getCurrentUtcWeekBounds(referenceDate: Date): { weekStart: Date; weekEnd: Date } {
  const current = new Date(referenceDate);
  const day = current.getUTCDay();
  const dayFromMonday = (day + 6) % 7;

  current.setUTCHours(0, 0, 0, 0);
  current.setUTCDate(current.getUTCDate() - dayFromMonday);

  const weekStart = current;
  const weekEnd = new Date(weekStart);
  weekEnd.setUTCDate(weekEnd.getUTCDate() + 7);

  return { weekStart, weekEnd };
}

function splitTrafficContent(content: string): string[] {
  if (content.length <= MAX_TRAFFIC_MESSAGE_LENGTH) {
    return [content];
  }

  const lines = content.split('\n');
  const chunks: string[] = [];
  let currentChunk = '';

  for (const line of lines) {
    const lineWithNewline = `${line}\n`;

    if (
      currentChunk.length > 0 &&
      currentChunk.length + lineWithNewline.length > MAX_TRAFFIC_MESSAGE_LENGTH
    ) {
      chunks.push(currentChunk.trimEnd());
      currentChunk = lineWithNewline;
      continue;
    }

    if (lineWithNewline.length > MAX_TRAFFIC_MESSAGE_LENGTH) {
      if (currentChunk.length > 0) {
        chunks.push(currentChunk.trimEnd());
        currentChunk = '';
      }

      let start = 0;
      while (start < line.length) {
        const part = line.slice(start, start + MAX_TRAFFIC_MESSAGE_LENGTH);
        chunks.push(part);
        start += MAX_TRAFFIC_MESSAGE_LENGTH;
      }
      continue;
    }

    currentChunk += lineWithNewline;
  }

  if (currentChunk.trim().length > 0) {
    chunks.push(currentChunk.trimEnd());
  }

  return chunks.length > 0 ? chunks : [content];
}

async function syncWebTrafficMessages(client: Client): Promise<void> {
  if (!WEB_TRAFFIC_CHANNEL_ID) {
    return;
  }

  const channel = await getTextChannel(client, WEB_TRAFFIC_CHANNEL_ID);
  if (!channel) {
    console.warn(`Web traffic channel not found or not writable: ${WEB_TRAFFIC_CHANNEL_ID}`);
    return;
  }

  const prisma = getPrismaClient();
  const { weekStart, weekEnd } = getCurrentUtcWeekBounds(new Date());

  const [totalVisits, inviteClicks, latestTrafficRows, existingWebTrafficMessages] =
    await prisma.$transaction([
      prisma.websiteTraffic.count({
        where: {
          createdAt: {
            gte: weekStart,
            lt: weekEnd,
          },
        },
      }),
      prisma.websiteTraffic.count({
        where: {
          clickedInviteButton: true,
          createdAt: {
            gte: weekStart,
            lt: weekEnd,
          },
        },
      }),
      prisma.websiteTraffic.findMany({
        where: {
          createdAt: {
            gte: weekStart,
            lt: weekEnd,
          },
        },
        orderBy: { createdAt: 'desc' },
        take: MAX_TRAFFIC_ROWS_IN_MESSAGE,
      }),
      prisma.webTrafficMessages.findMany({
        where: {
          createdAt: {
            gte: weekStart,
            lt: weekEnd,
          },
        },
        orderBy: { webTrafficMessagesId: 'asc' },
      }),
    ]);

  const trafficLines = latestTrafficRows.map(
    (row, index) =>
      `${index + 1}. ${row.createdAt.toISOString()} | invite=${row.clickedInviteButton ? 'yes' : 'no'}`
  );

  const overflowNote =
    totalVisits > latestTrafficRows.length
      ? `\n\nShowing latest ${latestTrafficRows.length} of ${totalVisits} records.`
      : '';

  const weekLabel = `${weekStart.toISOString().slice(0, 10)} to ${new Date(weekEnd.getTime() - 1).toISOString().slice(0, 10)}`;

  const content =
    `# Website Traffic\n` +
    `Week (UTC): ${weekLabel}\n` +
    `Total Visits: ${totalVisits}\n` +
    `Invite Clicks: ${inviteClicks}\n\n` +
    `## Entries\n` +
    `${trafficLines.length > 0 ? trafficLines.join('\n') : 'No entries yet.'}` +
    overflowNote;

  const chunks = splitTrafficContent(content);
  const sharedLength = Math.min(existingWebTrafficMessages.length, chunks.length);

  for (let index = 0; index < sharedLength; index += 1) {
    const existingMessage = existingWebTrafficMessages[index];
    const targetContent = chunks[index];
    const discordMessage = await channel.messages
      .fetch(existingMessage.discordMessageId)
      .catch(() => null);

    if (discordMessage) {
      await discordMessage.edit({ content: targetContent });
      continue;
    }

    const replacementMessage = await channel.send({ content: targetContent });
    await prisma.webTrafficMessages.update({
      where: {
        webTrafficMessagesId: existingMessage.webTrafficMessagesId,
      },
      data: {
        discordMessageId: replacementMessage.id,
      },
    });
  }

  if (chunks.length > existingWebTrafficMessages.length) {
    for (let index = existingWebTrafficMessages.length; index < chunks.length; index += 1) {
      const newMessage = await channel.send({ content: chunks[index] });
      await prisma.webTrafficMessages.create({
        data: {
          discordMessageId: newMessage.id,
        },
      });
    }
  }

  if (existingWebTrafficMessages.length > chunks.length) {
    const extraMessages = existingWebTrafficMessages.slice(chunks.length);

    for (const extraMessage of extraMessages) {
      const discordMessage = await channel.messages
        .fetch(extraMessage.discordMessageId)
        .catch(() => null);
      if (discordMessage) {
        await discordMessage.delete().catch(() => undefined);
      }
    }

    await prisma.webTrafficMessages.deleteMany({
      where: {
        webTrafficMessagesId: {
          in: extraMessages.map((message) => message.webTrafficMessagesId),
        },
      },
    });
  }
}

async function forwardTrafficToBot(hashedIp: Buffer, eventType: TrafficEventType) {
  const prisma = getPrismaClient();
  const clickedInviteButton = eventType === 'invite-click';

  const existingTraffic = await prisma.websiteTraffic.findFirst({
    where: {
      hashedIp,
    },
  });

  if (existingTraffic) {
    await prisma.websiteTraffic.update({
      where: {
        websiteTrafficId: existingTraffic.websiteTrafficId,
      },
      data: {
        clickedInviteButton: existingTraffic.clickedInviteButton || clickedInviteButton,
      },
    });

    return {
      saved: false,
      eventType,
      hashedIpHex: hashedIp.toString('hex'),
    };
  }

  await prisma.websiteTraffic.create({
    data: {
      hashedIp,
      clickedInviteButton,
    },
  });

  return {
    saved: true,
  };
}

async function isDatabaseReachable(): Promise<boolean> {
  const prisma = getPrismaClient();

  try {
    await prisma.$queryRaw`SELECT 1`;
    return true;
  } catch (error) {
    console.warn('Health check database probe failed:', error);
    return false;
  }
}

async function getTextChannel(client: Client, channelId: string) {
  const channel = await client.channels.fetch(channelId).catch(() => null);

  if (!channel || !('send' in channel) || typeof channel.send !== 'function') {
    return null;
  }

  return channel;
}

export function startHttpApi(client: Client): void {
  const server = createServer(async (req, res) => {
    setCorsHeaders(res);

    if (req.method === 'OPTIONS') {
      res.writeHead(204);
      res.end();
      return;
    }

    const requestUrl = new URL(req.url ?? '/', `http://${req.headers.host ?? 'localhost'}`);

    if (requestUrl.pathname === '/health' && req.method === 'GET') {
      sendJson(res, 200, {
        ok: true,
        status: 'alive',
        ready: client.isReady(),
        uptimeSeconds: Math.floor(process.uptime()),
      });
      return;
    }

    if (requestUrl.pathname === '/ready' && req.method === 'GET') {
      const databaseReady = await isDatabaseReachable();
      const ready = client.isReady() && databaseReady;

      sendJson(res, ready ? 200 : 503, {
        ok: ready,
        status: ready ? 'ready' : 'degraded',
        ready: client.isReady(),
        database: databaseReady,
        uptimeSeconds: Math.floor(process.uptime()),
      });
      return;
    }

    if (!client.isReady()) {
      sendJson(res, 503, { ok: false, error: 'Bot is not ready yet' });
      return;
    }

    try {
      if (
        requestUrl.pathname === '/traffic/visit' &&
        (req.method === 'GET' || req.method === 'POST')
      ) {
        const ip = getRequestIp(req);
        const hashedIp = hashIp(ip);
        await forwardTrafficToBot(hashedIp, 'visit');
        await syncWebTrafficMessages(client);

        sendJson(res, 200, {
          ok: true,
        });
        return;
      }

      if (
        requestUrl.pathname === '/traffic/invite-click' &&
        (req.method === 'GET' || req.method === 'POST')
      ) {
        const ip = getRequestIp(req);
        const hashedIp = hashIp(ip);
        await forwardTrafficToBot(hashedIp, 'invite-click');
        await syncWebTrafficMessages(client);

        sendJson(res, 200, {
          ok: true,
        });
        return;
      }

      if (requestUrl.pathname === '/message' && req.method === 'POST') {
        const body = await readJsonBody(req);
        const channelId = typeof body.channelId === 'string' ? body.channelId.trim() : '';
        const content = typeof body.content === 'string' ? body.content.trim() : '';

        if (!channelId || !content) {
          sendJson(res, 400, { ok: false, error: 'channelId and content are required' });
          return;
        }

        const channel = await getTextChannel(client, channelId);
        if (!channel) {
          sendJson(res, 404, { ok: false, error: 'Channel not found or not text-based' });
          return;
        }

        const message = await channel.send({ content });
        sendJson(res, 201, {
          ok: true,
          action: 'send',
          channelId,
          messageId: message.id,
        });
        return;
      }

      if (requestUrl.pathname === '/message' && req.method === 'PATCH') {
        const body = await readJsonBody(req);
        const channelId = typeof body.channelId === 'string' ? body.channelId.trim() : '';
        const messageId = typeof body.messageId === 'string' ? body.messageId.trim() : '';
        const content = typeof body.content === 'string' ? body.content.trim() : '';

        if (!channelId || !messageId || !content) {
          sendJson(res, 400, { ok: false, error: 'channelId, messageId and content are required' });
          return;
        }

        const channel = await getTextChannel(client, channelId);
        if (!channel || !('messages' in channel)) {
          sendJson(res, 404, { ok: false, error: 'Channel not found or not editable' });
          return;
        }

        const message = await channel.messages.fetch(messageId).catch(() => null);
        if (!message) {
          sendJson(res, 404, { ok: false, error: 'Message not found' });
          return;
        }

        await message.edit({ content });
        sendJson(res, 200, {
          ok: true,
          action: 'edit',
          channelId,
          messageId,
        });
        return;
      }

      sendJson(res, 404, { ok: false, error: 'Route not found' });
    } catch (error) {
      console.error('HTTP API error:', error);
      sendJson(res, 500, { ok: false, error: 'Internal server error' });
    }
  });

  server.listen(API_PORT, () => {
    console.info(`✓ HTTP API listening on port ${API_PORT}`);
  });
}
