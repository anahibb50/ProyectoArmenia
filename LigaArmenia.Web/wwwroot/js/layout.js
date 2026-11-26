(() => {
  const STORAGE_KEY = 'la_sidebar_collapsed';
  const body = document.body;

  if (window.innerWidth >= 992 && localStorage.getItem(STORAGE_KEY) === '1') {
    body.classList.add('la-sidebar-collapsed');
  }

  const desktopToggle = document.querySelector('[data-sidebar-toggle]');
  if (desktopToggle) {
    desktopToggle.addEventListener('click', () => {
      if (window.innerWidth < 992) {
        body.classList.toggle('la-mobile-nav-open');
        return;
      }

      const collapsed = body.classList.toggle('la-sidebar-collapsed');
      localStorage.setItem(STORAGE_KEY, collapsed ? '1' : '0');
    });
  }

  const backdrop = document.querySelector('[data-sidebar-backdrop]');
  if (backdrop) {
    backdrop.addEventListener('click', () => {
      body.classList.remove('la-mobile-nav-open');
    });
  }

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
      body.classList.remove('la-mobile-nav-open');
    }
  });
})();
