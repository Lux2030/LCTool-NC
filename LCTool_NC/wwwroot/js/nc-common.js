window.APP_BASE_URL = window.APP_BASE_URL || "/";

window.appUrl = function (path) {
    return new URL(
        String(path).replace(/^\/+/, ""),
        new URL(window.APP_BASE_URL, window.location.origin)
    ).toString();
};

window.API_NC             = appUrl('api/nonconformita');
window.API_NC_STATI       = appUrl('api/nonconformita/stati');
window.API_NC_RILEVATA_DA = appUrl('api/nonconformita/rilevata-da');
window.API_NC_PARTI       = appUrl('api/nonconformita/parti-interessate');
window.API_NC_PROSSIMO    = appUrl('api/nonconformita/prossimo-numero');
window.API_NC_BY_ID       = (id) => appUrl(`api/nonconformita/${id}`);
window.API_NC_UPDATE      = (id) => appUrl(`api/nonconformita/${id}/update`);
window.API_NC_DELETE_POST = (id) => appUrl(`api/nonconformita/${id}/delete`);
window.API_NC_DR_PREVIEW  = appUrl('api/nonconformita/data-retention/anteprima');
window.API_NC_DR_RUN      = appUrl('api/nonconformita/data-retention');

window.todayISO = function () {
    return new Date().toISOString().split('T')[0];
};

window.fmtDate = function (iso) {
    if (!iso) return '—';
    try {
        const d = new Date(iso);
        if (isNaN(d.getTime()) || d.getFullYear() < 1900) return '—';
        return d.toLocaleDateString('it-IT');
    } catch { return '—'; }
};

window.escHtml = function (s) {
    return String(s || '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
};

window.formatNumeroNC = function (n) { return `${n}`; };

window.toast = function (msg, type) {
    const t = document.getElementById('toast');
    if (!t) return;
    t.textContent = msg;
    t.className = 'toast show ' + (type || '');
    setTimeout(() => { t.className = 'toast'; }, 3200);
};

window.parsePartiInteressate = function (json) {
    try {
        const p = JSON.parse(json || '{}');
        if (Array.isArray(p.selezionati)) {
            const parts = [...p.selezionati];
            if (p.altri) parts.push(p.altri);
            return parts.join(', ') || '—';
        }
        const a = [];
        if (p.legalCompliance) a.push('Legal & Compliance');
        if (p.diegoPisa) a.push('Diego Pisa');
        if (p.altri) a.push(p.altri);
        return a.join(', ') || '—';
    } catch { return '—'; }
};

window.parseSediExport = function(json) {
    try {
        const arr = JSON.parse(json || '[]');
        return Array.isArray(arr) ? arr.join(', ') : '';
    } catch { return ''; }
};
