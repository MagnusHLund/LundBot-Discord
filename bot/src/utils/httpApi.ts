import { createServer, IncomingMessage, ServerResponse } from 'node:http';
import { URL } from 'node:url';
import { createHash } from 'node:crypto';
import type { Client } from 'discord.js';
import { getPrismaClient } from '../services/database.js';

const API_PORT = Number(process.env.BOT_API_PORT ?? '3000');
const MAX_BODY_SIZE = 1024 * 1024;

type JsonRecord = Record<string, unknown>;

type TrafficEventType = 'visit' | 'invite-click';

function sendJson(res: ServerResponse, statusCode: number, payload: JsonRecord): void {
  res.writeHead(statusCode, {
    'Content-Type': 'application/json; charset=utf-8',
  });
  res.end(JSON.stringify(payload));
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
        WebsiteTrafficId: existingTraffic.WebsiteTrafficId,
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

async function getTextChannel(client: Client, channelId: string) {
  const channel = await client.channels.fetch(channelId).catch(() => null);

  if (!channel || !('send' in channel) || typeof channel.send !== 'function') {
    return null;
  }

  return channel;
}

export function startHttpApi(client: Client): void {
  const server = createServer(async (req, res) => {
    const requestUrl = new URL(req.url ?? '/', `http://${req.headers.host ?? 'localhost'}`);

    if (requestUrl.pathname === '/health' && req.method === 'GET') {
      sendJson(res, 200, { ok: true, ready: client.isReady() });
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
