window.moopelAuthCookies = {
    get: function (name) {
        const encodedName = encodeURIComponent(name) + "=";
        const cookies = document.cookie ? document.cookie.split(";") : [];

        for (const cookie of cookies) {
            const trimmed = cookie.trim();
            if (trimmed.startsWith(encodedName)) {
                return decodeURIComponent(trimmed.substring(encodedName.length));
            }
        }

        return null;
    },

    set: function (name, value, days) {
        const maxAge = Math.max(1, Number(days) || 30) * 24 * 60 * 60;
        document.cookie = `${encodeURIComponent(name)}=${encodeURIComponent(value)}; path=/; max-age=${maxAge}; samesite=lax`;
    },

    remove: function (name) {
        document.cookie = `${encodeURIComponent(name)}=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; samesite=lax`;
    }
};