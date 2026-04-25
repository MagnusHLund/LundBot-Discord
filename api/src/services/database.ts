import { PrismaClient } from '@prisma/client';

let prisma: PrismaClient;

/**
 * Get or create singleton instance of Prisma client
 */
export function getPrismaClient(): PrismaClient {
  if (!prisma) {
    prisma = new PrismaClient({
      log: process.env.NODE_ENV === 'development' ? ['query', 'error', 'warn'] : ['error'],
    });
  }
  return prisma;
}

/**
 * Disconnect Prisma client
 */
export async function disconnectPrisma(): Promise<void> {
  if (prisma) {
    await prisma.$disconnect();
  }
}

// Export Prisma types
export * from '@prisma/client';
