/* Dashboard JS */
async function loadDashboard(userId, adminFlag) {
    try {
        const result = await apiCall('GET', `/api/dashboard/summary?userId=${userId}&adminFlag=${adminFlag}`);
        if (!result.success) { showToast(result.message, 'danger'); return; }
        const d = result.data;

        document.getElementById('totalRisks').textContent = d.totalRisks || 0;

        // KPI from status counts
        const statusMap = {};
        (d.risksByStatus || []).forEach(s => statusMap[s.status] = s.cnt);
        document.getElementById('highImpact').textContent = (d.highAlerts || []).length;
        document.getElementById('pendingReview').textContent = (statusMap['Submitted'] || 0) + (statusMap['UnderReview'] || 0);
        document.getElementById('closedRisks').textContent = statusMap['Closed'] || 0;

        // Charts
        renderChart('statusChart', 'doughnut', d.risksByStatus || [], 'status', 'cnt', ['#95a5a6','#3498db','#17a2b8','#27ae60','#e74c3c','#f39c12','#2c3e50']);
        renderChart('buChart', 'bar', d.risksByBU || [], 'buName', 'cnt', ['#2E86C1','#1A5276','#148F77','#2980B9','#27AE60','#D4AC0D']);
        renderChart('catChart', 'pie', d.risksByCategory || [], 'riskCatName', 'cnt', ['#E74C3C','#F39C12','#27AE60','#2E86C1','#8E44AD','#C0392B']);
        renderChart('fyChart', 'bar', d.risksByFY || [], 'fyName', 'cnt', ['#1ABC9C','#2ECC71','#3498DB','#9B59B6']);

        // Alerts table
        const tb = document.getElementById('alertsBody');
        if (!d.highAlerts || d.highAlerts.length === 0) {
            tb.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No high impact alerts</td></tr>';
        } else {
            const impactBadge = { 'High': 'bg-danger', 'Medium': 'bg-warning text-dark', 'Low': 'bg-success' };
            const statusBadge = { 'Draft': 'bg-secondary', 'Submitted': 'bg-primary', 'Approved': 'bg-success', 'Rejected': 'bg-danger', 'Closed': 'bg-dark' };
            tb.innerHTML = d.highAlerts.map(a => `<tr>
                <td><a href="/Risk/Detail/${a.riskId}">${a.riskTitle}</a></td>
                <td>${a.buName}</td><td>${a.riskCatName}</td>
                <td><span class="badge ${impactBadge[a.impactLevel] || 'bg-secondary'} rounded-pill">${a.impactLevel}</span></td>
                <td><span class="badge ${statusBadge[a.status] || 'bg-secondary'} rounded-pill">${a.status}</span></td>
                <td>${new Date(a.createdDate).toLocaleDateString()}</td>
            </tr>`).join('');
        }
    } catch (e) {
        console.error('Dashboard load error:', e);
        showToast('Failed to load dashboard data.', 'danger');
    }
}

function renderChart(canvasId, type, data, labelKey, valueKey, colors) {
    const ctx = document.getElementById(canvasId);
    if (!ctx || !data.length) return;

    new Chart(ctx, {
        type: type,
        data: {
            labels: data.map(d => d[labelKey] || ''),
            datasets: [{
                data: data.map(d => d[valueKey]),
                backgroundColor: colors.slice(0, data.length),
                borderWidth: type === 'bar' ? 0 : 2,
                borderColor: type === 'bar' ? 'transparent' : '#fff',
                borderRadius: type === 'bar' ? 6 : 0,
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: type !== 'bar', position: 'bottom', labels: { padding: 16, usePointStyle: true } }
            },
            scales: type === 'bar' ? {
                y: { beginAtZero: true, ticks: { stepSize: 1 } },
                x: { grid: { display: false } }
            } : {}
        }
    });
}
