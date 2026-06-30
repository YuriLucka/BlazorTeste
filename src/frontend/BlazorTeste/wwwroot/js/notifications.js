window.sinderpBadge = {
    set: (count) => {
        if ('setAppBadge' in navigator) navigator.setAppBadge(count).catch(() => {});
    },
    clear: () => {
        if ('clearAppBadge' in navigator) navigator.clearAppBadge().catch(() => {});
    }
};

window.sinderpNotifications = {
    getPermission: () => Notification.permission,

    requestAndNotify: async (title, body, icon) => {
        if (!('Notification' in window)) return 'unsupported';

        let permission = Notification.permission;
        if (permission === 'default') {
            permission = await Notification.requestPermission();
        }
        if (permission !== 'granted') return permission;

        const n = new Notification(title, {
            body,
            icon: icon ?? '/favicon.png',
            badge: '/favicon.png',
            tag: 'sinderp-' + Date.now()
        });
        setTimeout(() => n.close(), 6000);
        return 'granted';
    }
};
