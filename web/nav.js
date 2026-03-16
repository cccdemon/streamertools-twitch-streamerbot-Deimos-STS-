// ════════════════════════════════════════════════════════
// CHAOS CREW – Shared Navigation
// nav.js – einbinden in alle Admin-Seiten
// ════════════════════════════════════════════════════════

(function() {
  var PAGES = [
    { href: 'giveaway-admin.html',  label: 'ADMIN PANEL',   group: 'giveaway' },
    { href: 'stats.html',           label: 'STATISTIKEN',   group: 'giveaway' },
    { href: 'giveaway-test.html',   label: 'TEST CONSOLE',  group: 'tools' },
    { href: 'tests/test-runner.html', label: 'TEST SUITE',  group: 'tools' },
    { sep: true },
    { href: 'streamerbot.html',     label: 'C# ACTIONS',    group: 'tools', color: 'gold' },
    { sep: true },
    { href: 'giveaway-overlay.html', label: 'GW OVERLAY',   group: 'obs', obs: true },
    { href: 'giveaway-join.html',   label: 'JOIN ANIM',     group: 'obs', obs: true },
    { href: 'chat.html',            label: 'HUD CHAT',      group: 'obs', obs: true },
    { href: 'spacefight.html',      label: 'RAUMKAMPF',     group: 'obs', obs: true },
  ];

  var base = window._navBase || '';
  var currentPage = window.location.pathname.split('/').pop() || 'index.html';
  if (currentPage === '') currentPage = 'index.html';

  var nav = document.createElement('nav');
  nav.className = 'cc-nav';

  // Home-Link
  var home = document.createElement('a');
  home.href = base + 'index.html';
  home.className = 'cc-nav-home';
  home.innerHTML =
    '<svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">' +
      '<path d="M6 1L11 5.5V11H8V8H4V11H1V5.5L6 1Z" stroke="currentColor" stroke-width="1.2" fill="none"/>' +
    '</svg>' +
    'CHAOS CREW';
  if (currentPage === 'index.html') home.classList.add('active');
  nav.appendChild(home);

  var items = document.createElement('div');
  items.className = 'cc-nav-items';

  PAGES.forEach(function(p) {
    if (p.sep) {
      var sep = document.createElement('div');
      sep.className = 'cc-nav-sep';
      items.appendChild(sep);
      return;
    }

    var a = document.createElement('a');
    // Normalize href for comparison (strip query string)
    var hrefBase = p.href.split('?')[0];
    var isCurrent = (currentPage === hrefBase) ||
                    (currentPage === '' && p.href === 'index.html');

    a.href = base + p.href;
    a.className = 'cc-nav-item' +
      (p.color ? ' ' + p.color : '') +
      (isCurrent ? ' active' : '');

    if (p.obs) {
      a.innerHTML = p.label + '<span class="nav-obs">OBS</span>';
      a.target = '_blank';
    } else {
      a.textContent = p.label;
    }

    items.appendChild(a);
  });

  nav.appendChild(items);

  // Nav als erstes Element nach <body> einfügen
  var body = document.body;
  body.insertBefore(nav, body.firstChild);
})();
