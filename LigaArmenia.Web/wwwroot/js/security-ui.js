(() => {
  // 1. Mostrar / ocultar contraseña
  document.querySelectorAll('[data-toggle-password]').forEach((btn) => {
    btn.addEventListener('click', () => {
      const targetId = btn.getAttribute('data-toggle-password');
      const input = targetId ? document.getElementById(targetId) : null;
      if (!input) return;

      const isPassword = input.getAttribute('type') === 'password';
      input.setAttribute('type', isPassword ? 'text' : 'password');
      btn.setAttribute('aria-pressed', isPassword ? 'true' : 'false');

      const eyeOpen = btn.querySelector('[data-eye-open]');
      const eyeClosed = btn.querySelector('[data-eye-closed]');
      if (eyeOpen && eyeClosed) {
        eyeOpen.classList.toggle('d-none', isPassword);
        eyeClosed.classList.toggle('d-none', !isPassword);
      }
    });
  });

  // 2. Seleccionar / deseleccionar permisos por módulo
  document.querySelectorAll('[data-module-toggle]').forEach((btn) => {
    btn.addEventListener('click', () => {
      const moduleKey = btn.getAttribute('data-module-toggle');
      const checkboxes = document.querySelectorAll(
        `input[type="checkbox"][data-perm-module="${moduleKey}"]:not(:disabled)`
      );
      if (checkboxes.length === 0) return;

      const allChecked = Array.from(checkboxes).every((cb) => cb.checked);
      checkboxes.forEach((cb) => {
        cb.checked = !allChecked;
      });
    });
  });

  // 3. Modal de confirmación reutilizable para formularios sensibles
  const modalEl = document.getElementById('laConfirmModal');
  if (modalEl && typeof bootstrap !== 'undefined') {
    const modal = new bootstrap.Modal(modalEl);
    const titleEl = modalEl.querySelector('[data-confirm-modal-title]');
    const messageEl = modalEl.querySelector('[data-confirm-modal-message]');
    const confirmBtn = modalEl.querySelector('[data-confirm-modal-accept]');
    let pendingForm = null;

    document.querySelectorAll('form[data-confirm-message]').forEach((form) => {
      form.addEventListener('submit', (e) => {
        if (form.dataset.confirmed === 'true') {
          form.dataset.confirmed = 'false';
          return;
        }

        e.preventDefault();
        pendingForm = form;

        if (titleEl) {
          titleEl.textContent = form.dataset.confirmTitle || 'Confirmar acción';
        }
        if (messageEl) {
          messageEl.textContent = form.dataset.confirmMessage || '¿Deseas continuar con esta operación?';
        }
        if (confirmBtn) {
          confirmBtn.textContent = form.dataset.confirmButton || 'Confirmar';
          confirmBtn.className = form.dataset.confirmVariant === 'danger'
            ? 'la-btn la-btn-danger'
            : 'la-btn la-btn-primary';
        }

        modal.show();
      });
    });

    if (confirmBtn) {
      confirmBtn.addEventListener('click', () => {
        if (!pendingForm) return;
        pendingForm.dataset.confirmed = 'true';
        modal.hide();
        pendingForm.submit();
      });
    }
  }

  // 4. Sección dinámica "Información del vocal" al seleccionar/deseleccionar el rol VOCAL
  const vocalSection = document.querySelector('[data-vocal-section]');
  const vocalRoleCheckbox = document.querySelector('input[type="checkbox"][name="RoleIds"][data-role-name="VOCAL"]');
  if (vocalSection && vocalRoleCheckbox) {
    const syncVocalSection = () => {
      vocalSection.classList.toggle('d-none', !vocalRoleCheckbox.checked);
    };

    syncVocalSection();
    vocalRoleCheckbox.addEventListener('change', syncVocalSection);
  }
})();
