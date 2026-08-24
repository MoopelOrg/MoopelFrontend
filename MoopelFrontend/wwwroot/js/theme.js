window.moopelTheme = window.moopelTheme || {};

window.moopelTheme.applyTheme = function (theme) {
    if (!theme || typeof theme !== "object") {
        return;
    }

    const styleId = "moopel-theme-overrides";
    const entries = Object.entries(theme).filter(([key, value]) => typeof key === "string" && typeof value === "string" && key.startsWith("--"));

    if (entries.length === 0) {
        const existing = document.getElementById(styleId);
        if (existing) {
            existing.remove();
        }
        return;
    }

    let style = document.getElementById(styleId);
    if (!style) {
        style = document.createElement("style");
        style.id = styleId;
        document.head.appendChild(style);
    }

    style.textContent = `:root { ${entries.map(([key, value]) => `${key}: ${value};`).join(" ")} }`;
};

window.moopelTheme.saveTheme = function (theme) {
    if (!theme || typeof theme !== "object") {
        return;
    }

    localStorage.setItem("moopel-theme", JSON.stringify(theme));
    window.moopelTheme.applyTheme(theme);
};

window.moopelTheme.clearTheme = function () {
    localStorage.removeItem("moopel-theme");

    const style = document.getElementById("moopel-theme-overrides");
    if (style) {
        style.remove();
    }
};

window.moopelTheme.getTheme = function () {
    const raw = localStorage.getItem("moopel-theme");
    if (!raw) {
        return null;
    }

    try {
        const parsed = JSON.parse(raw);
        return parsed && typeof parsed === "object" ? parsed : null;
    } catch {
        return null;
    }
};

window.moopelTheme.applyThemeFromStorage = function () {
    const theme = window.moopelTheme.getTheme();
    if (theme) {
        window.moopelTheme.applyTheme(theme);
    }
};
