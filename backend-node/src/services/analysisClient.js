const { config } = require("../config/env");

async function analyzeWithPython(payload) {
  const response = await fetch(`${config.analysisBaseUrl}/analyze`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Python analysis failed (${response.status}): ${errorText}`);
  }

  return response.json();
}

module.exports = { analyzeWithPython };
