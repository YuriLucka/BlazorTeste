// Dev SW — bypass HTTP cache for framework files with known-cached-404 issue
self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', e => e.waitUntil(self.clients.claim()));
self.addEventListener('fetch', event => {
  if (event.request.url.includes('_framework/')) {
    event.respondWith(fetch(event.request.url, {cache: 'no-store'}));
  }
});

self.addEventListener('notificationclick', event => {
    const action = event.action;
    const url = action
        ? (event.notification.data?.actions?.[action] ?? '/')
        : (event.notification.data?.url ?? '/');
    event.notification.close();
    event.waitUntil(
        self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then(clientList => {
            for (const client of clientList) {
                if (client.url.includes(self.location.origin) && 'focus' in client) {
                    client.navigate(url);
                    return client.focus();
                }
            }
            return self.clients.openWindow(url);
        })
    );
});
