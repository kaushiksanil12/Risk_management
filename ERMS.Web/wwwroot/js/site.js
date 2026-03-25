/* === site.js === */
// Sidebar toggle
document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.getElementById('sidebar');
    const toggle = document.getElementById('sidebarToggle');
    const mobileToggle = document.getElementById('mobileToggle');

    if (toggle) {
        toggle.addEventListener('click', function() {
            sidebar.classList.toggle('collapsed');
            localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
        });
    }
    if (mobileToggle) {
        mobileToggle.addEventListener('click', function() {
            sidebar.classList.toggle('show');
        });
    }

    // Restore state
    if (localStorage.getItem('sidebarCollapsed') === 'true') {
        sidebar?.classList.add('collapsed');
    }

    // Force all modals to body to completely prevent overlapping issues
    document.querySelectorAll('.modal').forEach(function(m) {
        document.body.appendChild(m);
    });
});

// Global API caller
async function apiCall(method, endpoint, body) {
    const url = endpoint.startsWith('http') ? endpoint : (window.API_BASE || 'http://localhost:5000') + endpoint;
    console.log(`[API] ${method} ${url}`);
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'X-User-Id': (window.USER_ID || '').toString(),
            'X-Admin-Flag': (window.ADMIN_FLAG || 'N').toString()
        }
    };

    // Note: In MVC, we might prefer passing these from the view during the call
    // For now, let's keep it flexible.
    
    if (body) options.body = JSON.stringify(body);
    
    const response = await fetch(url, options);
    return response.json();
}

// Toast helper
function showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const icons = { success: 'bi-check-circle-fill', danger: 'bi-x-circle-fill', warning: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' };
    const colors = { success: '#27ae60', danger: '#e74c3c', warning: '#f39c12', info: '#2e86c1' };

    const toast = document.createElement('div');
    toast.className = 'toast show toast-msg';
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="toast-header" style="border-left:4px solid ${colors[type] || colors.info};border-radius:10px 10px 0 0">
            <i class="bi ${icons[type] || icons.info} me-2" style="color:${colors[type]}"></i>
            <strong class="me-auto">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
            <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
        </div>
        <div class="toast-body">${message}</div>
    `;
    container.appendChild(toast);

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 4000);
}
