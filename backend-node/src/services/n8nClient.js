const N8N_APPROVAL_WEBHOOK_URL = process.env.N8N_APPROVAL_WEBHOOK_URL;

async function sendApprovalEvent(payload){
    if(!N8N_APPROVAL_WEBHOOK_URL) {
        throw new Error("N8N_APPROVAL_WEBHOOK_URL is not defined");
    }

    const response = await fetch(N8N_APPROVAL_WEBHOOK_URL, {
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