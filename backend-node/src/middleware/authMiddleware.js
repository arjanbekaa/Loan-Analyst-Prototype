const jwt = require("jsonwebtoken");
const { config } = require("../config/env");

function requireAuth(req, res, next) {
  const authHeader = req.headers.authorization;

  if (!authHeader || !authHeader.startsWith("Bearer ")) {
    return res.status(401).json({ message: "missing or invalid authorization header" });
  }

  const token = authHeader.split(" ")[1];

  try {
    const payload = jwt.verify(token, config.jwtSecret);
    req.user = {
      ...payload,
      id: payload.id || payload.sub,
    };
    return next();
  } catch (err) {
    return res.status(401).json({ message: "invalid or expired token" });
  }
}

module.exports = { requireAuth };
