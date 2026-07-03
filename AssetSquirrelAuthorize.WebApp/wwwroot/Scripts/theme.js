// Theme switcher: plain global functions, called via inline HTML "onclick"
// attributes (not Blazor @onclick) so the toggle also works on statically
// rendered pages (e.g. /Account/Login) that have no live Blazor circuit.

function getStoredTheme() {
    return localStorage.getItem('as-theme');
}

function updateThemeToggleIcon(theme) {
    var icon = document.getElementById('theme-toggle-icon');
    if (icon) {
        icon.className = theme === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
    }
}

function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('as-theme', theme);
    updateThemeToggleIcon(theme);
}

function toggleTheme() {
    var current = document.documentElement.getAttribute('data-theme') || 'dark';
    setTheme(current === 'dark' ? 'light' : 'dark');
}

function initThemeToggleIcon() {
    updateThemeToggleIcon(document.documentElement.getAttribute('data-theme') || 'dark');
}
