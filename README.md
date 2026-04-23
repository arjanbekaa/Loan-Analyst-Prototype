# Loan Analyst Prototype

Independent personal prototype built to explore how an internal lending workflow platform could be structured using Unity, Node.js, Python, and n8n.

This project simulates a role-based loan review system where staff users can authenticate, review applicants, run automated risk analysis, and approve cases through secured backend actions.

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
- n8n webhook notification flow  

## Architecture

Unity Client  
→ Node.js API Backend  
→ Python Analysis Service  

Node.js API Backend  
→ n8n Webhook Workflow  

## Demo

YouTube walkthrough included in repository sharing materials.

## Notes

- This is a local prototype focused on architecture demonstration.
- Uses mock in-memory data for applicants and demo users.
- Authentication and users are simplified for prototype purposes.

## Local Setup

Full setup instructions available in:

```text
docs/setup.md
