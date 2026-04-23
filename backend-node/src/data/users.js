// Local-only demo auth store for the prototype.
// Plain-text credentials are intentional mock data and must not be reused in production.
const users = [
  {
    id: "u1",
    username: "reviewer1",
    password: "reviewer123",
    role: "reviewer",
    displayName: "Rita Reviewer",
  },
  {
    id: "u2",
    username: "manager1",
    password: "manager123",
    role: "manager",
    displayName: "Manny Manager",
  },
];

module.exports = { users };
