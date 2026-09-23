const fs = require('fs');

// Launcher extra para correr en sandboxes/CI sin sandbox de Chrome
// disponible (ver docs/decisiones.md, sección Frontend/UX — mismo tipo de
// gap de entorno que zone.js). Local con Chrome normal no necesita esto.
//
// Si no hay CHROME_BIN seteado pero existe chromium-browser del sistema
// (común en distros sin "google-chrome"), se usa automáticamente.
if (!process.env.CHROME_BIN) {
  const fallback = ['/usr/bin/chromium-browser', '/usr/bin/chromium', '/usr/bin/google-chrome']
    .find(path => fs.existsSync(path));
  if (fallback) process.env.CHROME_BIN = fallback;
}

module.exports = function (config) {
  config.set({
    frameworks: ['jasmine'],
    plugins: [require('karma-jasmine'), require('karma-chrome-launcher')],
    customLaunchers: {
      ChromeHeadlessCI: {
        base: 'ChromeHeadless',
        flags: ['--no-sandbox', '--disable-gpu']
      }
    }
  });
};
