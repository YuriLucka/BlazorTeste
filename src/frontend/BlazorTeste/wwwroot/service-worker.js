// Dev SW — bypass HTTP cache for framework files with known-cached-404 issue
self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', e => e.waitUntil(self.clients.claim()));
self.addEventListener('fetch', event => {
  if (event.request.url.includes('_framework/')) {
    event.respondWith(fetch(event.request.url, {cache: 'no-store'}));
  }
});
