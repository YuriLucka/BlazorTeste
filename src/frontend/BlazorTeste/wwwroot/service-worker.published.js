// Blazor WASM offline cache — used only in published builds
const cacheNamePrefix = 'sinderp-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest?.version ?? 'v1'}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm$/, /\.html$/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.svg$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

async function onInstall(event) {
    self.skipWaiting();
    const assetsRequests = self.assetsManifest?.assets
        ?.filter(asset => offlineAssetsInclude.some(p => p.test(asset.url)))
        ?.filter(asset => !offlineAssetsExclude.some(p => p.test(asset.url)))
        ?.map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' })) ?? [];
    await caches.open(cacheName).then(c => c.addAll(assetsRequests));
}

async function onActivate(event) {
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);
    const cachedResponse = await caches.match(event.request, { ignoreSearch: true });
    return cachedResponse ?? fetch(event.request);
}

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
