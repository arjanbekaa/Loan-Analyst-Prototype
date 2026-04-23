from typing import List, Literal
from pydantic import BaseModel, Field

class AnalyzeRequest(BaseModel):
    applicantId: str = Field(..., min_length=1)
    monthlyIncome: float = Field(..., gt=0)
    monthlyDebtPayments: float = Field(..., ge=0)
    creditScore: int = Field(..., ge=300, le=850)
    loanAmount: float = Field(..., gt=0)
    loanTermMonths: int = Field(..., gt=0)

class AnalysisMetrics(BaseModel):
    dti: float
    estimatedMonthlyPayment: float

class AnalyzeResponse(BaseModel):
    applicantId: str
    riskLevel: Literal["LOW", "MEDIUM", "HIGH"]
    riskScore: int
    reasons: List[str]
    metric: AnalysisMetrics
