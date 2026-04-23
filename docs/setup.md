# LoanAnalyst Setup Guide

This guide explains how to run the full local LoanAnalyst stack: Node.js backend, Python analysis service, Unity client, and optional n8n approval webhook.

## 1. Project Layout

```text
LoanAnalyst/
  backend-node/        Node.js API, auth, applicants, approval routes
  ai-service-python/   FastAPI loan analysis service
  unity-client/        Unity UGUI operator client
  docs/                Setup notes and build documentation
```

## 2. Required Tools

- Node.js and npm
- Python 3.11 or newer recommended
- Unity Editor with TextMeshPro installed/imported
- PowerShell for the commands below
- Optional: n8n for approval webhook testing

## 3. Backend Configuration

Work in:

```text
LoanAnalyst/backend-node
```

Create `backend-node/.env` from `backend-node/.env.example`:

```env
PORT=4000
JWT_SECRET=replace-with-a-local-development-secret
JWT_EXPIRES_IN=1h
ANALYSIS_BASE_URL=http://127.0.0.1:8000
N8N_APPROVAL_WEBHOOK_URL=http://localhost:5678/webhook/loan-approved
```

Install dependencies:

```powershell
cd backend-node
npm install
```

Start backend:

```powershell
npm run dev
```

Expected URL:

```text
http://localhost:4000
```

Quick health check:

```powershell
curl http://localhost:4000/health
```

Expected response:

```json
{"ok":true,"service":"backend-node"}
```

## 4. Python Analysis Service

Work in:

```text
LoanAnalyst/ai-service-python
```

Create and activate virtual environment:

```powershell
cd ai-service-python
python -m venv .venv
.\.venv\Scripts\Activate.ps1
```

Install dependencies:

```powershell
pip install -r requirements.txt
```

Start service:

```powershell
python -m uvicorn app.main:app --reload --port 8000
```

Expected URL:

```text
http://127.0.0.1:8000
```

Quick health check:

```powershell
curl http://127.0.0.1:8000/health
```

## 5. Unity Client Setup

Open this Unity project:

```text
LoanAnalyst/unity-client/Loan Analyst Client
```

Generate all UI scenes:

```text
Tools -> LoanAnalyst -> UI Generator -> Generate All Screens
```

If generating one scene at a time:

```text
Tools -> LoanAnalyst -> UI Generator -> Generate Login Scene UI
Tools -> LoanAnalyst -> UI Generator -> Generate Applicants Scene UI
Tools -> LoanAnalyst -> UI Generator -> Generate Applicant Details Scene UI
```

Required scene order:

```text
LoginScene
ApplicantsScene
ApplicantDetailScene
```

Run from:

```text
Assets/Scenes/LoginScene.unity
```

## 6. Test Accounts

```text
reviewer1 / reviewer123
manager1 / manager123
```

Reviewer permissions:

```text
Can log in, view applicants, open details, and run analysis.
Cannot approve applicants.
```

Manager permissions:

```text
Can log in, view applicants, open details, run analysis, and approve applicants.
```

## 7. API Test Flow

Login as manager:

```powershell
$login = curl -Method POST http://localhost:4000/auth/login `
  -ContentType "application/json" `
  -Body '{"username":"manager1","password":"manager123"}'
```

Copy the returned `token` value and use it as `YOUR_TOKEN_HERE`.

Fetch applicants:

```powershell
curl http://localhost:4000/applicants `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" }
```

Analyze applicant:

```powershell
curl -Method POST http://localhost:4000/applicants/a1/analyze `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" } `
  -ContentType "application/json" `
  -Body '{"monthlyIncome":3200,"monthlyDebtPayments":1000,"creditScore":710,"loanAmount":12000,"loanTermMonths":36}'
```

Approve applicant:

```powershell
curl -Method POST http://localhost:4000/applicants/a1/approve `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" } `
  -ContentType "application/json" `
  -Body '{}'
```

## 8. Unity Manual Test Checklist

1. Start Python service on port `8000`.
2. Start Node backend on port `4000`.
3. Open Unity project.
4. Generate UI scenes if needed.
5. Open `LoginScene`.
6. Press Play.
7. Log in as `reviewer1`.
8. Confirm applicant list loads.
9. Open applicant details.
10. Confirm analysis works.
11. Confirm approve is disabled or blocked for reviewer.
12. Log out or restart Play mode.
13. Log in as `manager1`.
14. Open applicant details.
15. Confirm approve button is visible and enabled.
16. Approve applicant.
17. Confirm success or webhook error is displayed clearly.

## 9. Optional n8n Webhook Test

Start n8n locally and enable a webhook workflow at:

```text
http://localhost:5678/webhook/loan-approved
```

Then approve an applicant as manager. The Node backend should send an approval event to the webhook.

If n8n is not running, approval may return a webhook-related error depending on backend behavior.

## 10. Common Issues

Backend cannot sign JWT:

```text
Check that backend-node/.env contains JWT_SECRET.
```

Unity cannot connect to backend:

```text
Confirm Node is running on http://localhost:4000.
Confirm ApiConfig.BaseUrl is http://localhost:4000.
```

Analyze fails:

```text
Confirm Python service is running on http://127.0.0.1:8000.
Confirm ANALYSIS_BASE_URL is set in backend-node/.env.
```

Approve button missing or disabled:

```text
Log in as manager1 / manager123.
Regenerate Applicant Details Scene UI.
Confirm ApplicantDetailController has approveButton assigned.
```

Unity Input System error:

```text
Use UnityEngine.InputSystem.Keyboard.current instead of UnityEngine.Input.GetKeyDown.
The project is configured for the Input System package.
```

## 11. Development Notes

- Applicant data is currently in-memory.
- Approval state resets when the Node backend restarts.
- `.env` values are development-only.
- Unity UI is generated through editor tooling, not manually assembled.
