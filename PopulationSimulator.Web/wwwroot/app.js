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
