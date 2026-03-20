/* Dark mode toggle */
document.addEventListener('DOMContentLoaded', function() {
    const toggle = document.getElementById('themeToggle');
    const icon = document.getElementById('themeIcon');
    const html = document.documentElement;
    const stored = localStorage.getItem('theme');

    if (stored === 'dark') {
        html.setAttribute('data-theme', 'dark');
        if (icon) icon.className = 'bi bi-moon-stars-fill';
    }

    if (toggle) {
        toggle.addEventListener('click', function() {
            const current = html.getAttribute('data-theme');
            if (current === 'dark') {
                html.setAttribute('data-theme', 'light');
                if (icon) icon.className = 'bi bi-sun-fill';
                localStorage.setItem('theme', 'light');
            } else {
                html.setAttribute('data-theme', 'dark');
                if (icon) icon.className = 'bi bi-moon-stars-fill';
                localStorage.setItem('theme', 'dark');
            }
        });
    }
});
