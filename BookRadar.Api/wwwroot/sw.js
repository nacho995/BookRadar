// Service worker: cachea la carcasa de la app. v2 (al cambiar el nombre,
// el navegador descarta la caché vieja en el próximo activate).
const CACHE = 'bookradar-v4';
const SHELL = ['/', 'styles.css', 'app.js', 'manifest.webmanifest', 'icon.svg'];

self.addEventListener('install', (e) => {
  e.waitUntil(caches.open(CACHE).then((c) => c.addAll(SHELL)));
  self.skipWaiting();
});

self.addEventListener('activate', (e) => {
  e.waitUntil(caches.keys().then((keys) =>
    Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k)))));
});

self.addEventListener('fetch', (e) => {
  const url = new URL(e.request.url);
  if (url.pathname.startsWith('/api/')) return;   // datos: siempre red
  e.respondWith(caches.match(e.request).then((hit) => hit || fetch(e.request)));
});
