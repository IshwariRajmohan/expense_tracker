const fs = require('fs');
const path = require('path');

function loadEnv() {
  const paths = [
    path.join(__dirname, '.env'),
    path.join(__dirname, '..', '.env'),
    path.join(__dirname, '..', '..', '.env')
  ];
  for (const envPath of paths) {
    if (fs.existsSync(envPath)) {
      const lines = fs.readFileSync(envPath, 'utf8').split('\n');
      for (const line of lines) {
        if (!line || line.trim().startsWith('#')) continue;
        const separatorIndex = line.indexOf('=');
        if (separatorIndex > 0) {
          const key = line.substring(0, separatorIndex).trim();
          const value = line.substring(separatorIndex + 1).trim().replace(/^['"]|['"]$/g, '');
          process.env[key] = value;
        }
      }
      break;
    }
  }
}

loadEnv();

const proxyConfig = {
  "/api": {
    "target": process.env.BACKEND_URL || "http://localhost:5032",
    "secure": false,
    "changeOrigin": true
  }
};

module.exports = proxyConfig;
