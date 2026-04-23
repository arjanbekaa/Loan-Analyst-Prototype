# LoanAnalyst Build Log

## Overview

LoanAnalyst is a small internal operations prototype built to simulate an AI-assisted lending workflow platform using Unity, Node.js, Python, and n8n.

The project was developed as a focused end-to-end system covering authentication, role-based access control, backend APIs, service integration, and workflow automation.

---

## Phase 1 — Backend Foundation

### Completed

- Created project structure for:
  - `backend-node`
  - `ai-service-python`
  - `unity-client`
  - `docs`

- Initialized Node.js / Express backend

- Implemented core backend features:
  - `POST /auth/login`
  - JWT authentication
  - Auth middleware
  - Role-based middleware (RBAC)

- Implemented protected applicant endpoints:
  - `GET /applicants`
  - `GET /applicants/:id`

### Key Concepts Practiced

- Express routing
- Middleware pipeline
- JWT token flow
- Authentication vs authorization
- Protected API design

---

## Phase 2 — Python Analysis Service

### Completed

- Built separate Python FastAPI service

- Implemented:
  - `GET /health`
  - `POST /analyze`

- Added rule-based loan risk scoring

- Returns structured JSON including:
  - risk level
  - risk score
  - analysis reasons
  - financial metrics

- Integrated Node.js backend with Python service

### Key Concepts Practiced

- FastAPI fundamentals
- Service-to-service communication
- JSON contracts
- Microservice separation
- Backend orchestration

---

## Phase 3 — Approval Workflow + Automation

### Completed

- Implemented protected approval endpoint:

  - `POST /applicants/:id/approve`

- Restricted approval to manager role only

- Added in-memory approval tracking:
  - status
  - approved by
  - timestamp

- Integrated n8n webhook workflow

- Approval events now trigger automation flow

### Key Concepts Practiced

- RBAC enforcement
- Workflow automation
- Event-driven backend actions
- Secure role restrictions

---

## Phase 4 — Unity Frontend

### Completed

- Unity login interface
- Token-based API requests
- Applicant list UI
- Applicant detail view
- Analyze action
- Approve action
- Role-based UI behavior

### Key Concepts Practiced

- Unity REST integration
- Frontend/backend communication
- Async request handling
- Internal tool UI workflows

---

## Final Architecture

Unity Client  
→ Node.js API Backend  
→ Python Analysis Service

Node.js API Backend  
→ n8n Workflow Automation

---

## Purpose of This Project

Built to strengthen hands-on experience in:

- backend systems
- authentication / RBAC
- Python services
- workflow automation
- Unity internal tools
- full-stack product integration

---

## Status

Prototype complete and functional as a local end-to-end system.