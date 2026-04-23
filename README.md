# Loan Analyst Prototype

Loan Analyst Prototype is a local end-to-end internal operations system built with **Unity, Node.js, Python, and n8n**. It simulates a role-based loan review workflow where staff users can log in, review applicants, run automated risk analysis, and approve cases through secured backend actions.

## Tech Stack

- **Unity (C#)** – operator-facing client UI  
- **Node.js / Express** – main backend API, authentication, RBAC, applicant routes  
- **Python / FastAPI** – loan risk analysis service  
- **n8n** – approval workflow automation  

## Core Features

- JWT authentication  
- Role-based access control (Reviewer / Manager)  
- Applicant dashboard and detail flow  
- Automated loan risk analysis  
- Manager-only approval actions  
- n8n webhook automation after approval  

## Architecture

Unity Client  
→ Node.js API Backend  
→ Python Analysis Service  

Node.js API Backend  
→ n8n Webhook Workflow  

## Demo Flow

1. Login as Reviewer or Manager  
2. View applicants  
3. Open applicant details  
4. Run automated analysis  
5. Approve as Manager  
6. Trigger workflow automation  

## Local Setup

Full setup instructions available in:

```text
docs/setup.md
