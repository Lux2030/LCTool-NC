document.addEventListener('DOMContentLoaded', async () => {
    const id = window.NC_ID;
    if (!id) {
        toast('ID Non Conformità non specificato', 'error');
        return;
    }
    try {
        const res = await fetch(API_NC_BY_ID(id));
        if (!res.ok) throw new Error('NC non trovata');
        const nc = await res.json();
        renderDettagli(nc);
    } catch (err) {
        document.getElementById('dettagliTitolo').textContent = 'Errore';
        document.getElementById('dettagliContent').innerHTML =
            `<p style="color:red">${escHtml(err.message)}</p>`;
    }
});

function parseSedi(json) {
    try {
        const arr = JSON.parse(json || '[]');
        return Array.isArray(arr) && arr.length ? arr.join(', ') : '—';
    } catch { return '—'; }
}

function parseCliente(json) {
    try {
        const arr = JSON.parse(json || '[]');
        return Array.isArray(arr) && arr.length ? arr.join(', ') : '—';
    } catch { return json || '—'; }
}

function renderDettagli(nc) {
    document.getElementById('dettagliTitolo').textContent =
        `Dettagli — ${formatNumeroNC(nc.numero)}`;

    const owners = (() => {
        try { return JSON.parse(nc.owner || '[]').join(', '); } catch { return '—'; }
    })();
    const pi = parsePartiInteressate(nc.partiInteressate);
    const clienteStr = parseCliente(nc.cliente);
    const sediStr = parseSedi(nc.sedi);

    const urlRevisiona = appUrl(`NuovaNC?id=${encodeURIComponent(nc.id)}`);

    document.getElementById('dettagliContent').innerHTML = `
        <div style="display:flex;gap:10px;margin-bottom:20px;flex-wrap:wrap;">
            <a class="btn primary" href="${urlRevisiona}">✏️ Revisiona</a>
            <button class="btn danger" id="btnElimina">🗑️ Elimina NC</button>
        </div>
        <div class="dettagli-section">
            <div class="dettagli-section-title">Intestazione</div>
            <div class="dettagli-grid">
                <div class="dettagli-kv"><span class="dk">Numero NC</span><span class="dv">${formatNumeroNC(nc.numero)}</span></div>
                <div class="dettagli-kv"><span class="dk">Stato</span><span class="dv">${escHtml(nc.stato || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">ISO / Norma</span><span class="dv">${escHtml(nc.isoNormaRiferimento || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Item di Riferimento</span><span class="dv">${escHtml(nc.itemRiferimento || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Rilevata da</span><span class="dv">${escHtml(nc.rilevataDa || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Data Apertura</span><span class="dv">${fmtDate(nc.dataApertura)}</span></div>
                <div class="dettagli-kv"><span class="dk">Data Prevista Chiusura</span><span class="dv">${fmtDate(nc.dataPrevistaChiusura)}</span></div>
                <div class="dettagli-kv"><span class="dk">Data Effettiva Chiusura</span><span class="dv">${fmtDate(nc.dataEffettivaChiusura)}</span></div>
                <div class="dettagli-kv"><span class="dk">Creato da</span><span class="dv">${escHtml(nc.creatoDa || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Revisionato da</span><span class="dv">${escHtml(nc.revisionatoDa || '—')}</span></div>
            </div>
        </div>
        <div class="dettagli-section">
            <div class="dettagli-section-title">Dettagli NC</div>
            <div class="dettagli-grid">
                <div class="dettagli-kv full"><span class="dk">Descrizione</span><span class="dv">${escHtml(nc.descrizione || '—')}</span></div>
                <div class="dettagli-kv full"><span class="dk">Causa</span><span class="dv">${escHtml(nc.causa || '—')}</span></div>
                <div class="dettagli-kv full"><span class="dk">Risoluzione</span><span class="dv">${escHtml(nc.risoluzione || '—')}</span></div>
                <div class="dettagli-kv full"><span class="dk">Azioni Correttive</span><span class="dv">${escHtml(nc.azioniCorrettive || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Doc SGQ Modificati</span><span class="dv">${escHtml(nc.docSgqModificati || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Owner</span><span class="dv">${escHtml(owners)}</span></div>
                <div class="dettagli-kv"><span class="dk">Dipartimento Coinvolto</span><span class="dv">${escHtml(nc.dipartimentoCoinvolto || '—')}</span></div>
                <div class="dettagli-kv"><span class="dk">Parti Interessate</span><span class="dv">${escHtml(pi)}</span></div>
                <div class="dettagli-kv"><span class="dk">Cliente</span><span class="dv">${escHtml(clienteStr)}</span></div>
                <div class="dettagli-kv"><span class="dk">Sedi</span><span class="dv">${escHtml(sediStr)}</span></div>
            </div>
        </div>
        ${(nc.dataRevisione && new Date(nc.dataRevisione).getFullYear() > 1900) || nc.noteRevisione ? `
        <div class="dettagli-section">
            <div class="dettagli-section-title">Revisione</div>
            <div class="dettagli-grid">
                <div class="dettagli-kv"><span class="dk">Data Revisione</span><span class="dv">${fmtDate(nc.dataRevisione)}</span></div>
                <div class="dettagli-kv full"><span class="dk">Note Revisione</span><span class="dv">${escHtml(nc.noteRevisione || '—')}</span></div>
            </div>
        </div>` : ''}`;

    document.getElementById('btnElimina').addEventListener('click', () => eliminaNC(nc.id, nc.numero));
}

async function eliminaNC(id, numero) {
    if (!confirm(`Sei sicuro di voler eliminare la NC ${formatNumeroNC(numero)}?\nL'operazione non può essere annullata.`)) return;
    try {
        // Fix errore 405: usa POST /delete invece di DELETE (WebDAV su IIS blocca DELETE)
        const res = await fetch(API_NC_DELETE_POST(id), { method: 'POST' });
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        toast('NC eliminata con successo', 'success');
        setTimeout(() => { window.location.href = appUrl('Archivio'); }, 1200);
    } catch (err) {
        toast('Errore durante eliminazione: ' + err.message, 'error');
    }
}
