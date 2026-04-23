const { config } = require("../config/env");

async function sendApprovalEvent(payload){
    const response = await fetch(config.n8nApprovalWebhookUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    });

    if (!response.ok) {
        const text = await response.text();
        console.error("Failed to send approval event to n8n:", response.status, text);
    }

    return response.json().catch(() => ({}));
}

module.exports = { sendApprovalEvent };
