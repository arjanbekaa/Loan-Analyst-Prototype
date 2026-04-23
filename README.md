# LoanAnalyst

LoanAnalyst is a local prototype for an operator-facing loan review workflow. It uses a Node.js API for authentication, role-based access, applicant data, approval actions, and webhook integration; a Python FastAPI service for loan risk analysis; and a Unity UGUI client for the reviewer/manager interface.

## Project Structure

```text
LoanAnalyst/
  backend-node/        Node.js API gateway and business routes
  ai-service-python/   FastAPI risk analysis service
  unity-client/        Unity operator-facing frontend
  docs/                Build notes and supporting documentation
```

## Prerequisites

- Node.js and npm
- Python 3.11+ recommended
- Unity 6 / Unity Editor with TextMeshPro imported
- Optional: n8n if testing approval webhooks

## Backend Setup

From `backend-node`:

```powershell
npm install
npm run dev
```

Expected backend URL:

```text
http://localhost:4000
```

Create `backend-node/.env` from `backend-node/.env.example`:

```env
PORT=4000
JWT_SECRET=replace-with-a-local-development-secret
JWT_EXPIRES_IN=1h
ANALYSIS_BASE_URL=http://127.0.0.1:8000
N8N_APPROVAL_WEBHOOK_URL=http://localhost:5678/webhook/loan-approved
```

## Python Analysis Service Setup

From `ai-service-python`:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
python -m uvicorn app.main:app --reload --port 8000
```

Expected Python service URL:

```text
http://127.0.0.1:8000
```

## Unity Client Setup

Open this Unity project:

```text
unity-client/Loan Analyst Client
```

Generate UI scenes from the Unity menu:

```text
Tools -> LoanAnalyst -> UI Generator -> Generate All Screens
```

Make sure the build scene order is:

```text
LoginScene
ApplicantsScene
ApplicantDetailScene
```

Test users:

```text
reviewer1 / reviewer123
manager1 / manager123
```

Reviewer users can view applicants and run analysis. Manager users can also approve applicants.

## API Test Commands

Health check:

```powershell
curl http://localhost:4000/health
```

Login as manager:

```powershell
curl -Method POST http://localhost:4000/auth/login `
  -ContentType "application/json" `
  -Body '{"username":"manager1","password":"manager123"}'
```

Fetch applicants with a JWT:

```powershell
curl http://localhost:4000/applicants `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" }
```

Run analysis:

```powershell
curl -Method POST http://localhost:4000/applicants/a1/analyze `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" } `
  -ContentType "application/json" `
  -Body '{"monthlyIncome":3200,"monthlyDebtPayments":1000,"creditScore":710,"loanAmount":12000,"loanTermMonths":36}'
```

Approve as manager:

```powershell
curl -Method POST http://localhost:4000/applicants/a1/approve `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN_HERE" } `
  -ContentType "application/json" `
  -Body '{}'
```

## Manual Test Checklist

1. Start the Python service on port `8000`.
2. Start the Node backend on port `4000`.
3. Open Unity and run `LoginScene`.
4. Log in as `reviewer1`; verify applicants load and approve is disabled or unavailable.
5. Log out and log in as `manager1`; verify approve is visible and clickable.
6. Open an applicant, run analysis, then approve.
7. Confirm backend responses are shown clearly in Unity status/error text.

## Notes

This is a local development prototype. The current data store is in-memory, so applicant approval state resets when the backend restarts. Secrets in `.env` are development values and should be replaced before any production deployment.
