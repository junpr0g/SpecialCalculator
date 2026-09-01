window.localStorageInterop = {
    containsKey: function (key) {
        return localStorage.getItem(key) !== null;
    },
    getItem: function (key) {
        return localStorage.getItem(key);
    },
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    }
};

window.themeInterop = {
    applyTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
    }
};
