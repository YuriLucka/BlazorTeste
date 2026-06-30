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
        if (permission === 'default') permission = await Notification.requestPermission();
        if (permission !== 'granted') return permission;
        const n = new Notification(title, {
            body,
            icon: icon ?? '/favicon.png',
            badge: '/favicon.png',
            tag: 'sinderp-' + Date.now()
        });
        setTimeout(() => n.close(), 6000);
        return 'granted';
    },

    gerarImagemCobranca: async (status, nome, valor, vencimento, avatarUrl) => {
        // 360×180 = 2:1 — ratio exato que Chrome usa para notification image
        const W = 360, H = 180;
        const canvas = document.createElement('canvas');
        canvas.width = W; canvas.height = H;
        const ctx = canvas.getContext('2d');

        const cores  = { pago: '#2e7d32', vencido: '#c62828', pendente: '#e65100', cancelado: '#546e7a' };
        const labels = { pago: '✓  PAGO', vencido: '⚠  VENCIDO', pendente: '●  PENDENTE', cancelado: '✕  CANCELADO' };
        const cor   = cores[status]  ?? '#1565c0';
        const label = labels[status] ?? status.toUpperCase();

        // fundo
        ctx.fillStyle = cor;
        ctx.fillRect(0, 0, W, H);

        // faixa de status no topo
        ctx.fillStyle = 'rgba(0,0,0,0.20)';
        ctx.fillRect(0, 0, W, 48);

        // avatar circular (canto direito, centralizado verticalmente)
        const avatarR  = 52;
        const avatarCx = W - avatarR - 12;
        const avatarCy = H / 2 + 10;  // ligeiramente abaixo do centro
        let avatarOk = false;
        if (avatarUrl) {
            try {
                await new Promise((resolve) => {
                    const img = new Image();
                    try { const u = new URL(avatarUrl, location.origin); if (u.origin !== location.origin) img.crossOrigin = 'anonymous'; } catch (_) {}
                    img.onload = () => {
                        const sq  = Math.min(img.width, img.height);
                        const sx  = (img.width  - sq) / 2;
                        const sy  = (img.height - sq) / 2;
                        const d   = avatarR * 2;
                        ctx.save();
                        ctx.beginPath();
                        ctx.arc(avatarCx, avatarCy, avatarR, 0, Math.PI * 2);
                        ctx.clip();
                        ctx.drawImage(img, sx, sy, sq, sq, avatarCx - avatarR, avatarCy - avatarR, d, d);
                        ctx.restore();
                        // borda
                        ctx.save();
                        ctx.beginPath();
                        ctx.arc(avatarCx, avatarCy, avatarR + 2.5, 0, Math.PI * 2);
                        ctx.strokeStyle = 'rgba(255,255,255,0.65)';
                        ctx.lineWidth = 3;
                        ctx.stroke();
                        ctx.restore();
                        avatarOk = true;
                        resolve();
                    };
                    img.onerror = resolve;
                    img.src = avatarUrl;
                });
            } catch (_) {}
        }

        const textRight = avatarOk ? avatarCx - avatarR - 14 : W - 14;

        // label status
        ctx.fillStyle = '#fff';
        ctx.font = 'bold 22px system-ui, sans-serif';
        ctx.textAlign = 'left';
        ctx.textBaseline = 'middle';
        ctx.fillText(label, 16, 24);

        // nome (2 linhas se necessário)
        ctx.font = '14px system-ui, sans-serif';
        ctx.fillStyle = 'rgba(255,255,255,0.93)';
        ctx.textBaseline = 'top';
        const maxW = textRight - 16;
        const words = nome.split(' ');
        let line1 = '', line2 = '';
        for (const w of words) {
            const test = line1 ? line1 + ' ' + w : w;
            if (ctx.measureText(test).width <= maxW) { line1 = test; }
            else { line2 = line2 ? line2 + ' ' + w : w; }
        }
        ctx.fillText(line1, 16, 58);
        if (line2) ctx.fillText(line2, 16, 76);

        // valor
        ctx.font = 'bold 20px system-ui, sans-serif';
        ctx.fillStyle = '#fff';
        ctx.fillText(valor, 16, 106);

        // vencimento
        ctx.font = '13px system-ui, sans-serif';
        ctx.fillStyle = 'rgba(255,255,255,0.75)';
        ctx.fillText('Vencimento: ' + vencimento, 16, 134);

        return canvas.toDataURL('image/png');
    },

    // Uses SW showNotification — supports `actions` array (max 2), requires HTTPS + SW
    // opts.actions: { [actionId]: url } — mapped to SW data so notificationclick can navigate
    // opts.actionButtons: [{ action, title, icon? }] — displayed as buttons in notification
    sendWithActions: async (opts) => {
        if (!('Notification' in window)) return 'unsupported';
        let permission = Notification.permission;
        if (permission === 'default') permission = await Notification.requestPermission();
        if (permission !== 'granted') return permission;
        if (!('serviceWorker' in navigator)) return 'no-sw';
        const sw = await navigator.serviceWorker.ready;
        await sw.showNotification(opts.title, {
            body:               opts.body,
            icon:               opts.icon ?? '/favicon.png',
            badge:              '/favicon.png',
            image:              opts.image ?? undefined,
            silent:             opts.silent ?? false,
            requireInteraction: opts.requireInteraction ?? false,
            renotify:           opts.renotify ?? false,
            tag:                opts.tag ?? ('sinderp-' + Date.now()),
            timestamp:          opts.timestamp ?? Date.now(),
            actions:            opts.actionButtons ?? [],
            data: {
                url:     opts.url ?? '/',
                actions: opts.actions ?? {}
            }
        });
        return 'granted';
    },

    send: async (opts) => {
        if (!('Notification' in window)) return 'unsupported';
        let permission = Notification.permission;
        if (permission === 'default') permission = await Notification.requestPermission();
        if (permission !== 'granted') return permission;

        const n = new Notification(opts.title, {
            body:                opts.body,
            icon:                opts.icon ?? '/favicon.png',
            badge:               '/favicon.png',
            image:               opts.image ?? undefined,
            silent:              opts.silent ?? false,
            requireInteraction:  opts.requireInteraction ?? false,
            renotify:            opts.renotify ?? false,
            tag:                 opts.tag ?? ('sinderp-' + Date.now()),
            timestamp:           opts.timestamp ?? Date.now(),
        });

        n.onclick = () => {
            if (opts.url) window.open(opts.url, '_self');
            window.focus();
            n.close();
        };

        if (!opts.requireInteraction) setTimeout(() => n.close(), opts.duration ?? 6000);
        return 'granted';
    }
};
