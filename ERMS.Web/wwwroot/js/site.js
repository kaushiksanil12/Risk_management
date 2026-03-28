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

// ── Wizard: Quarter Rating Score Calculation ──────────────────────────────

const ERMS_SCORE_MAP = {
    'Low (1)': 1, 'Low (2)': 2, 'Moderate (3)': 3,
    'High (4)': 4, 'Critical (5)': 5,
    'Rare (1)': 1, 'Unlikely (2)': 2, 'Probable (3)': 3,
    'Likely (4)': 4, 'Almost Certain (5)': 5
};

function calculateRating(score) {
    if (score <= 3)  return 'Low';
    if (score <= 6)  return 'Medium Low';
    if (score <= 9)  return 'Medium';
    if (score <= 14) return 'Medium High';
    return 'High';
}

function calculateScore(impactVal, likelihoodVal) {
    const i = ERMS_SCORE_MAP[impactVal]    || 0;
    const l = ERMS_SCORE_MAP[likelihoodVal] || 0;
    return i * l;
}

function initQuarterRatingCalculation() {
    ['GrossImpact', 'GrossLikelihood', 'ResidualImpact', 'ResidualLikelihood']
        .forEach(id => {
            const el = document.getElementById(id);
            if (el) el.addEventListener('change', updateScores);
        });
}

function updateScores() {
    const gi = document.getElementById('GrossImpact')?.value;
    const gl = document.getElementById('GrossLikelihood')?.value;
    if (gi && gl) {
        const gs = calculateScore(gi, gl);
        const gsEl = document.getElementById('GrossScore');
        const grEl = document.getElementById('GrossRating');
        if (gsEl) gsEl.value = gs;
        if (grEl) grEl.value = calculateRating(gs);
    }
    const ri = document.getElementById('ResidualImpact')?.value;
    const rl = document.getElementById('ResidualLikelihood')?.value;
    if (ri && rl) {
        const rs = calculateScore(ri, rl);
        const rsEl = document.getElementById('ResidualScore');
        const rrEl = document.getElementById('ResidualRating');
        if (rsEl) rsEl.value = rs;
        if (rrEl) rrEl.value = calculateRating(rs);
    }
}

