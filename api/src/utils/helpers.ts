/**
 * Utility functions for bot operations
 */

/**
 * Format error messages for logging
 */
export function formatError(error: unknown): string {
  if (error instanceof Error) {
    return `${error.name}: ${error.message}`;
  }
  return String(error);
}

/**
 * Create a formatted timestamp for logging
 */
export function getTimestamp(): string {
  return new Date().toISOString();
}

/**
 * Log with timestamp
 */
export function logWithTimestamp(
  level: 'info' | 'warn' | 'error' | 'debug',
  message: string
): void {
  const timestamp = getTimestamp();
  const levelMap = {
    info: 'ℹ️',
    warn: '⚠️',
    error: '❌',
    debug: '🐛',
  };

  console[level === 'info' ? 'info' : level](`[${timestamp}] ${levelMap[level]} ${message}`);
}
