const express = require("express");
const cors = require("cors");
const morgan = require("morgan");
const authRoutes = require("./routes/authRoutes");
const aplicatnRoutes = require("./routes/applicantRoutes");

const app = express();

app.use(cors());
app.use(express.json());
app.use(morgan("dev"));

app.use("/auth", authRoutes);
app.use("/applicants", aplicatnRoutes);

app.get("/health", (req, res) => {
  res.status(200).json({
    ok: true,
    service: "backend-node",
  });
});

module.exports = app;
