// BookRadar — frontend en JS plano. Sin frameworks: fetch + DOM.

const $ = (sel) => document.querySelector(sel);

const searchInput = $('#search');
const resultsEl   = $('#results');
const recsSection = $('#recs-section');
const recsEl      = $('#recs');
const statusEl    = $('#status');

const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)').matches;

/* ============ portadas (Open Library Covers API) ============ */
function coverUrl(book, size = 'M') {
  if (!book.key) return null;
  const olid = book.key.split('/').pop();           // "/works/OL123W" → "OL123W"
  return `https://covers.openlibrary.org/w/olid/${olid}-${size}.jpg?default=false`;
}

function coverEl(book, large = false) {
  const wrap = document.createElement('span');
  wrap.className = 'cover' + (large ? ' cover-lg' : '');
  const url = coverUrl(book, large ? 'L' : 'M');
  if (url) {
    const img = document.createElement('img');
    img.loading = 'lazy';
    img.alt = '';
    img.src = url;
    // si Open Library no tiene portada → letra capital como placeholder
    img.onerror = () => { img.remove(); wrap.appendChild(coverFallback(book)); };
    wrap.appendChild(img);
  } else {
    wrap.appendChild(coverFallback(book));
  }
  return wrap;
}

function coverFallback(book) {
  const ph = document.createElement('span');
  ph.className = 'cover-fallback';
  ph.textContent = (book.title ?? '?').charAt(0).toUpperCase();
  return ph;
}

/* ============ búsqueda ============ */
function debounce(fn, ms) {
  let timer;
  return (...args) => { clearTimeout(timer); timer = setTimeout(() => fn(...args), ms); };
}

async function searchBooks(term) {
  if (!term || term.trim().length < 2) { resultsEl.innerHTML = ''; setStatus(''); return; }
  setStatus('Buscando…');
  try {
    const res = await fetch(`/api/books?search=${encodeURIComponent(term)}`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    renderResults(await res.json());
  } catch (err) {
    setStatus(`Error hablando con la API: ${err.message}`);
  }
}

function renderResults(books) {
  resultsEl.innerHTML = '';
  if (books.length === 0) { setStatus('Nada en tu catálogo con ese título.'); return; }
  setStatus(`${books.length} resultado${books.length > 1 ? 's' : ''} — clica uno`);
  books.forEach((book, i) => resultsEl.appendChild(bookCard(book, i)));
}

/* ============ tarjetas ============ */
function bookCard(book, index) {
  const card = document.createElement('button');
  card.className = 'card';
  card.style.animationDelay = `${index * 70}ms`;

  card.appendChild(coverEl(book));

  const body = document.createElement('span');
  body.className = 'card-body';
  let inner = `
    <span class="card-title">${escapeHtml(book.title)}</span>
    <span class="card-author">${escapeHtml(book.author)}</span>`;
  if (book.desc) inner += `<span class="card-desc">${escapeHtml(book.desc)}</span>`;
  if (book.score !== undefined) {
    const pct = Math.round(book.score * 100);
    inner += `
      <span class="affinity">
        <span class="affinity-track"><span class="affinity-bar" data-w="${pct}"></span></span>
        <span class="affinity-label">${pct}% afín</span>
      </span>`;
  }
  body.innerHTML = inner;
  card.appendChild(body);

  card.addEventListener('click', () => loadRecommendations(book));
  return card;
}

/* ============ recomendaciones ============ */
async function loadRecommendations(book) {
  recsSection.hidden = false;
  $('#ref-title').textContent = book.title;
  $('#ref-author').textContent = book.author;
  $('#ref-desc').textContent = book.desc ?? '';
  const refCover = $('#ref-cover');
  refCover.replaceWith(coverEl(book, true));
  document.querySelector('#ref-card .cover-lg').id = 'ref-cover';

  recsEl.innerHTML = '';
  for (let i = 0; i < 5; i++) {
    const sk = document.createElement('div');
    sk.className = 'skeleton';
    recsEl.appendChild(sk);
  }
  recsSection.scrollIntoView({ behavior: reducedMotion ? 'auto' : 'smooth', block: 'start' });

  try {
    const res = await fetch(`/api/books/${book.id}/recommendations`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const recs = await res.json();
    recsEl.innerHTML = '';
    recs.forEach((rec, i) => recsEl.appendChild(bookCard(rec, i)));
    animateAffinityBars();
  } catch (err) {
    recsEl.innerHTML = `<p class="muted">Error: ${escapeHtml(err.message)}</p>`;
  }
}

function animateAffinityBars() {
  requestAnimationFrame(() =>
    document.querySelectorAll('.affinity-bar').forEach((bar) => {
      bar.style.width = `${bar.dataset.w}%`;
    }));
}

/* ============ utilidades ============ */
// NUNCA interpoles texto externo en innerHTML sin escapar (XSS)
function escapeHtml(s) {
  const div = document.createElement('div');
  div.textContent = s ?? '';
  return div.innerHTML;
}

function setStatus(msg) { statusEl.textContent = msg; }

searchInput.addEventListener('input', debounce((e) => searchBooks(e.target.value), 300));

/* ============ PWA ============ */
if ('serviceWorker' in navigator) {
  navigator.serviceWorker.register('sw.js');
}
