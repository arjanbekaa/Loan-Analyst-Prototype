const DEFAULT_PORT = 4000;
const DEFAULT_ANALYSIS_BASE_URL = "http://127.0.0.1:8000";

function readEnv(name, fallback) {
  const value = process.env[name]?.trim();

  if (value) {
    return value;
  }

  return fallback;
}

function requireEnv(name) {
  const value = readEnv(name);

  if (!value) {
    throw new Error(`[config] Missing required environment variable: ${name}`);
  }

  return value;
}

function parsePort(value) {
  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : DEFAULT_PORT;
}

function validateEnv() {
  requireEnv("JWT_SECRET");
  requireEnv("N8N_APPROVAL_WEBHOOK_URL");
}

const config = {
  port: parsePort(readEnv("PORT", String(DEFAULT_PORT))),
  jwtSecret: requireEnv("JWT_SECRET"),
  jwtExpiresIn: readEnv("JWT_EXPIRES_IN", "1h"),
  analysisBaseUrl: readEnv("ANALYSIS_BASE_URL", DEFAULT_ANALYSIS_BASE_URL),
  n8nApprovalWebhookUrl: requireEnv("N8N_APPROVAL_WEBHOOK_URL"),
};

module.exports = { config, validateEnv };
