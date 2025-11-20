// Dark mode functionality
window.darkMode = {
    toggle: function () {
        document.body.classList.toggle('dark-mode');
        const isDark = document.body.classList.contains('dark-mode');
        localStorage.setItem('darkMode', isDark ? 'enabled' : 'disabled');
        return isDark;
    },

    init: function () {
        const darkMode = localStorage.getItem('darkMode');
        if (darkMode === 'enabled') {
            document.body.classList.add('dark-mode');
            return true;
        }
        return false;
    },

    isDarkMode: function () {
        return document.body.classList.contains('dark-mode');
    }
};

// Initialize dark mode on page load
window.addEventListener('DOMContentLoaded', function () {
    window.darkMode.init();
});

// Download file functionality for exports
window.downloadFile = function (filename, base64Content, contentType) {
    // Convert base64 to bytes
    const byteCharacters = atob(base64Content);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // Create blob
    const blob = new Blob([byteArray], { type: contentType });

    // Create download link
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;

    // Trigger download
    document.body.appendChild(link);
    link.click();

    // Cleanup
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
};

// Keyboard shortcuts
window.setupKeyboardShortcuts = function (dotNetHelper) {
    document.addEventListener('keydown', function (e) {
        // Space = Pause/Resume
        if (e.code === 'Space' && e.target.tagName !== 'INPUT' && e.target.tagName !== 'TEXTAREA') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('HandleKeyPress', 'Space');
        }
        // + or = = Speed up
        else if ((e.code === 'Equal' || e.code === 'NumpadAdd') && !e.shiftKey) {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('HandleKeyPress', 'Plus');
        }
        // - = Slow down
        else if (e.code === 'Minus' || e.code === 'NumpadSubtract') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('HandleKeyPress', 'Minus');
        }
    });
};

// Initialize Bootstrap tooltips
window.initializeTooltips = function () {
    // Wait for Bootstrap to be available
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    }
};

// Refresh tooltips after UI updates
window.refreshTooltips = function () {
    // Dispose old tooltips
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(el => {
        const tooltip = bootstrap.Tooltip.getInstance(el);
        if (tooltip) {
            tooltip.dispose();
        }
    });

    // Reinitialize
    window.initializeTooltips();
};

// Notification system
window.showNotification = function (message, type = 'info', duration = 5000) {
    // Create toast container if it doesn't exist
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `notification-toast ${type} p-3 mb-2 rounded`;
    toast.innerHTML = `
        <div class="d-flex align-items-center">
            <div class="flex-grow-1">
                <strong>${getNotificationIcon(type)}</strong> ${message}
            </div>
            <button type="button" class="btn-close ms-2" aria-label="Close" onclick="this.parentElement.parentElement.remove()"></button>
        </div>
    `;

    container.appendChild(toast);

    // Auto-remove after duration
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => toast.remove(), 300);
    }, duration);
};

function getNotificationIcon(type) {
    switch (type) {
        case 'success': return '✅';
        case 'warning': return '⚠️';
        case 'danger': return '❌';
        case 'info': return 'ℹ️';
        default: return 'ℹ️';
    }
}

// Chart.js functionality
window.charts = {
    instances: {},

    createPopulationChart: function (canvasId, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        // Destroy existing chart
        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');
        this.instances[canvasId] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Living Population',
                    data: data.living,
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    tension: 0.1,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    },
                    title: {
                        display: true,
                        text: 'Population Over Time'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    },

    createBirthDeathChart: function (canvasId, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');
        this.instances[canvasId] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Births',
                    data: data.births,
                    borderColor: 'rgb(54, 162, 235)',
                    backgroundColor: 'rgba(54, 162, 235, 0.1)',
                    tension: 0.1
                }, {
                    label: 'Deaths',
                    data: data.deaths,
                    borderColor: 'rgb(255, 99, 132)',
                    backgroundColor: 'rgba(255, 99, 132, 0.1)',
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    },
                    title: {
                        display: true,
                        text: 'Birth & Death Rates'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    },

    createInventionTimelineChart: function (canvasId, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');
        this.instances[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Inventions Discovered',
                    data: data.inventions,
                    backgroundColor: 'rgba(255, 206, 86, 0.6)',
                    borderColor: 'rgba(255, 206, 86, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Invention Discovery Timeline'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    },

    createEducationChart: function (canvasId, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');
        this.instances[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Education Levels',
                    data: data.values,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.6)',
                        'rgba(54, 162, 235, 0.6)',
                        'rgba(255, 206, 86, 0.6)',
                        'rgba(75, 192, 192, 0.6)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'right'
                    },
                    title: {
                        display: true,
                        text: 'Education Distribution'
                    }
                }
            }
        });
    },

    updatePopulationChart: function (canvasId, data) {
        if (this.instances[canvasId]) {
            this.instances[canvasId].data.labels = data.labels;
            this.instances[canvasId].data.datasets[0].data = data.living;
            this.instances[canvasId].update('none'); // Update without animation for performance
        } else {
            this.createPopulationChart(canvasId, data);
        }
    },

    updateBirthDeathChart: function (canvasId, data) {
        if (this.instances[canvasId]) {
            this.instances[canvasId].data.labels = data.labels;
            this.instances[canvasId].data.datasets[0].data = data.births;
            this.instances[canvasId].data.datasets[1].data = data.deaths;
            this.instances[canvasId].update('none');
        } else {
            this.createBirthDeathChart(canvasId, data);
        }
    },

    updateInventionTimelineChart: function (canvasId, data) {
        if (this.instances[canvasId]) {
            this.instances[canvasId].data.labels = data.labels;
            this.instances[canvasId].data.datasets[0].data = data.inventions;
            this.instances[canvasId].update('none');
        } else {
            this.createInventionTimelineChart(canvasId, data);
        }
    },

    updateEducationChart: function (canvasId, data) {
        if (this.instances[canvasId]) {
            this.instances[canvasId].data.labels = data.labels;
            this.instances[canvasId].data.datasets[0].data = data.values;
            this.instances[canvasId].update('none');
        } else {
            this.createEducationChart(canvasId, data);
        }
    }
};
