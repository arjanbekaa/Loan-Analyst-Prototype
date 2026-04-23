from fastapi import FastAPI
from app.models import AnalyzeRequest, AnalyzeResponse
from app.analysis import analyze_loan_risk

app = FastAPI(title="Loan Analyst AI Service", version="0.1.0")

@app.get("/health")
def health_check():
    return {"status": "ok"}

@app.post("/analyze", response_model=AnalyzeResponse)
def analyze(request: AnalyzeRequest):
    return analyze_loan_risk(request)
