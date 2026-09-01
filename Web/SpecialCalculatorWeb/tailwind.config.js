/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml,cs}",
    "./wwwroot/index.html"
  ],
  theme: {
    extend: {
      screens: {
        'calc-lg': '980px',
      },
      colors: {
        calc: {
          page: 'var(--calc-page-bg)',
          card: 'var(--calc-card-bg)',
          'card-alt': 'var(--calc-card-alt-bg)',
          header: 'var(--calc-header-bg)',
          stroke: 'var(--calc-stroke)',
          accent: 'var(--calc-accent-stroke)',
          primary: 'var(--calc-primary-text)',
          secondary: 'var(--calc-secondary-text)',
          body: 'var(--calc-body-text)',
          input: 'var(--calc-input-text)',
          'input-bg': 'var(--calc-input-bg)',
          dropdown: 'var(--calc-dropdown-bg)',
          button: 'var(--calc-button-bg)',
          placeholder: 'var(--calc-placeholder)',
          'dropdown-option': 'var(--calc-dropdown-option-text)',
          total: 'var(--calc-total-band)',
          'tab-bar': 'var(--calc-tab-bar-bg)',
          'admin-tab-active': 'var(--calc-admin-tab-active-bg)',
          'app-tab-active': 'var(--calc-app-tab-active-bg)',
          'app-tab-active-text': 'var(--calc-app-tab-active-text)',
          'admin-panel': 'var(--calc-admin-panel-bg)',
          'app-panel': 'var(--calc-app-panel-bg)',
          'admin-input': 'var(--calc-admin-input-bg)',
          'app-input': 'var(--calc-app-input-bg)',
          'app-input-stroke': 'var(--calc-app-input-stroke)',
          'app-primary': 'var(--calc-app-primary-text)',
          'app-body': 'var(--calc-app-body-text)',
          'app-picker': 'var(--calc-app-picker-text)',
          'primary-btn': 'var(--calc-primary-btn)',
          'app-primary-btn': 'var(--calc-app-primary-btn)',
          error: 'var(--calc-error)',
          success: 'var(--calc-success)',
          'app-success': 'var(--calc-app-success)',
        }
      },
      borderRadius: {
        calc: '14px',
        'calc-sm': '10px',
        'calc-xs': '8px',
      },
      fontFamily: {
        sans: ['Segoe UI', 'system-ui', '-apple-system', 'sans-serif'],
      },
      maxWidth: {
        app: '1200px',
      },
    },
  },
  plugins: [],
};
