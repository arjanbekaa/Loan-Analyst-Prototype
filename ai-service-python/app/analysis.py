from app.models import AnalyzeRequest, AnalyzeResponse, AnalysisMetrics

def analyze_loan_risk(payload: AnalyzeRequest) -> AnalyzeResponse:
    estimated_monthly_installment = payload.loanAmount / payload.loanTermMonths
    dti = (payload.monthlyDebtPayments + estimated_monthly_installment) / payload.monthlyIncome

    risk_score = 0
    reasons: list[str] = []

    # Credit score rules
    if(payload.creditScore < 580):
        risk_score += 50
        reasons.append("Low credit score")
    elif(payload.creditScore < 670):
        risk_score += 25
        reasons.append("Fair credit score")
    else:
        reasons.append("Good credit score")
    
    # Debt-to-income rules
    if dti > 0.5:
        risk_score += 40
        reasons.append("High debt-to-income ratio")
    elif dti > 0.35:
        risk_score += 35
        reasons.append("Moderate debt-to-income ratio")
    else:
        reasons.append("Low debt-to-income ratio")

    # Loan size vs income rules
    loan_income_ratio = payload.loanAmount / (payload.monthlyIncome * 12)
    if loan_income_ratio > 1.0:
        risk_score += 20
        reasons.append("Loan amount is too high compared to annual income")
    elif loan_income_ratio > 0.5:
        risk_score += 20
        reasons.append("Loan amount is moderately high compared to annual income")
    else:
        reasons.append("Loan amount is reasonable compared to annual income")

    risk_score = min(risk_score, 100)

    if risk_score >= 70:
        risk_level = "HIGH"
    elif risk_score >= 40:
        risk_level = "MEDIUM"
    else:
        risk_level = "LOW"

    return AnalyzeResponse(
        applicantId=payload.applicantId,
        riskLevel=risk_level,
        riskScore=risk_score,
        reasons=reasons,
        metric=AnalysisMetrics(
            dti=round(dti, 2),
            estimatedMonthlyPayment=round(estimated_monthly_installment, 2)
        )
    )