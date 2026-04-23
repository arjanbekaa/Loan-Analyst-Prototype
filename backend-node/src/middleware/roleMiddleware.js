function requireRole(...allowedRoles) {
  return (req, res, next) => {
    if (!req.user || !req.user.role) {
      return res.status(401).json({ message: "unauthenticated user context" });
    }

    if (!allowedRoles.includes(req.user.role)) {
      return res.status(403).json({ message: "forbidden: insufficient role" });
    }

    return next();
  };
}

module.exports = { requireRole };
