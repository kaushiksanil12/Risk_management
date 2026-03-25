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

        // Charts - Modernized Colors
        const statusColors = ['#64748b','#3b82f6','#0ea5e9','#10b981','#ef4444','#f59e0b','#334155'];
        const buColors = ['#6366f1','#8b5cf6','#d946ef','#f43f5e','#f97316','#eab308'];
        const catColors = ['#06b6d4','#14b8a6','#22c55e','#84cc16','#eab308','#f59e0b'];
        const fyColors = ['#3b82f6','#10b981','#8b5cf6','#f43f5e'];

        renderChart('statusChart', 'doughnut', d.risksByStatus || [], 'status', 'cnt', statusColors);
        renderChart('buChart', 'bar', d.risksByBU || [], 'buName', 'cnt', buColors);
        renderChart('catChart', 'doughnut', d.risksByCategory || [], 'riskCatName', 'cnt', catColors);
        renderChart('fyChart', 'bar', d.risksByFY || [], 'fyName', 'cnt', fyColors);

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

    const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
    Chart.defaults.color = isDark ? '#cccccc' : '#6b7280';
    
    // Modern grid and border matching the sleek UI
    const gridColor = isDark ? 'rgba(255,255,255,0.05)' : 'rgba(0,0,0,0.05)';
    const surfaceColor = isDark ? '#252526' : '#ffffff';

    new Chart(ctx, {
        type: type,
        data: {
            labels: data.map(d => d[labelKey] || ''),
            datasets: [{
                data: data.map(d => d[valueKey]),
                backgroundColor: colors.slice(0, data.length),
                borderWidth: type === 'bar' ? 0 : 3,
                borderColor: type === 'bar' ? 'transparent' : surfaceColor,
                borderRadius: type === 'bar' ? 6 : 0,
                barThickness: type === 'bar' ? 32 : undefined,
                hoverOffset: type !== 'bar' ? 8 : 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: type === 'doughnut' ? '70%' : undefined,
            plugins: {
                legend: { display: type !== 'bar', position: 'bottom', labels: { padding: 20, usePointStyle: true, pointStyle: 'circle' } },
                tooltip: {
                    backgroundColor: isDark ? 'rgba(0,0,0,0.8)' : 'rgba(255,255,255,0.9)',
                    titleColor: isDark ? '#fff' : '#000',
                    bodyColor: isDark ? '#fff' : '#000',
                    padding: 12,
                    cornerRadius: 8,
                    displayColors: true
                }
            },
            scales: type === 'bar' ? {
                y: { 
                    beginAtZero: true, 
                    ticks: { stepSize: 1, padding: 10 },
                    grid: { color: gridColor, drawBorder: false }
                },
                x: { 
                    grid: { display: false, drawBorder: false },
                    ticks: { padding: 10 }
                }
            } : {}
        }
    });
}
