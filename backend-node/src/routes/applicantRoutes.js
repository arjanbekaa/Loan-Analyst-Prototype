const express = require("express");
const { analyzeWithPython } = require("../services/analysisClient");
const { applicants } = require("../data/applicants");
const { requireAuth } = require("../middleware/authMiddleware");
const { requireRole } = require("../middleware/roleMiddleware");
const { sendApprovalEvent } = require("../services/n8nClient");

const router = express.Router();

router.get("/", requireAuth, requireRole("reviewer", "manager"), (req, res) => {
  return res.status(200).json({ applicants });
});

router.post(
  "/:id/analyze",
  requireAuth,
  requireRole("reviewer", "manager"),
  async (req, res) => {
  try {
    const applicantId = req.params.id;

    const {
      monthlyIncome,
      monthlyDebtPayments,
      creditScore,
      loanAmount,
      loanTermMonths,
    } = req.body;

    const analysis = await analyzeWithPython({
      applicantId,
      monthlyIncome,
      monthlyDebtPayments,
      creditScore,
      loanAmount,
      loanTermMonths,
    });

    return res.status(200).json(analysis);
  } catch (err) {
    return res.status(502).json({
      message: "Failed to analyze applicant via Python service",
      error: err.message,
    });
  }
});

router.post(
  "/:id/approve",
  requireAuth,
  requireRole("manager"),
  async (req, res) => {
    const applicant = applicants.find(a => a.id === req.params.id);

    if(!applicant) {
      return res.status(404).json({ message: "Applicant not found" });
    }

    if(applicant.status === "approved" || applicant.decision === "approved") {
      return res.status(400).json({ message: "Applicant already approved" });
    }

    const approvedAt = new Date().toISOString();
    const approvedBy = {
      id: req.user.id,
      username: req.user.username,
      role: req.user.role,
    };

    applicant.status = "approved";
    applicant.decision = "approved";
    applicant.approvedAt = approvedAt;
    applicant.approvedBy = approvedBy;

    try {
      await sendApprovalEvent({
        event: "loan.approved",
        applicantId: applicant.id,
        applicantName: applicant.fullName,
        status: applicant.status,
        approvedAt,
        approvedBy,
      });
    } catch (err) {
      return res.status(502).json({
        message: "applicant approved, but failed to notify n8n",
        error: err.message,
        applicant,
      });
    }

    return res.status(200).json({ message: "Applicant approved successfully", applicant });
  }
)

module.exports = router;
