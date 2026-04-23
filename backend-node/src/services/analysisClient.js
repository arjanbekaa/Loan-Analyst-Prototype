const ANALYSIS_BASE_URL = process.env.ANALYSIS_BASE_URL || "http://127.0.0.1:8000";

async function analyzeWithPython(payload) {
  const response = await fetch(`${ANALYSIS_BASE_URL}/analyze`, {
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
