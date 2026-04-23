require("dotenv").config();
const { config, validateEnv } = require("./config/env");
const app = require("./app");

validateEnv();

app.listen(config.port, () => {
    console.log(`Backend running on http://localhost:${config.port}`);
});
