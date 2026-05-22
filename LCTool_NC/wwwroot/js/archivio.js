let allNcList = [];

document.addEventListener('DOMContentLoaded', init);

async function init() {
    await popolaFiltriDropdown();
    await loadArchivio();
    bindEvents();
}

function bindEvents() {
    document.getElementById('btnRicerca') .addEventListener('click', applicaFiltri);
    document.getElementById('btnPulisci') .addEventListener('click', pulisciFiltri);
    document.getElementById('btnAggiorna').addEventListener('click', loadArchivio);
    document.getElementById('btnEsportaExcel').addEventListener('click', esportaExcel);
    document.getElementById('btnEsportaPDF').addEventListener('click', esportaPDF);
}

async function popolaFiltriDropdown() {
    try {
        const valori = await (await fetch(API_NC_STATI)).json();
        const sel = document.getElementById('filtroStato');
        valori.forEach(v => {
            const o = document.createElement('option');
            o.value = v.nome; o.textContent = v.nome;
            sel.appendChild(o);
        });
    } catch { }
    try {
        const valori = await (await fetch(API_NC_RILEVATA_DA)).json();
        const sel = document.getElementById('filtroRilevataDa');
        valori.forEach(v => {
            const o = document.createElement('option');
            o.value = v.valore; o.textContent = v.valore;
            sel.appendChild(o);
        });
    } catch { }
}

async function loadArchivio() {
    const container = document.getElementById('archivioTabella');
    container.innerHTML = '<tr><td colspan="8" style="text-align:center;padding:40px;color:#999">Caricamento…</td></tr>';
    try {
        const res = await fetch(API_NC);
        allNcList = await res.json();
        const btn = document.getElementById('btnCounter');
        document.getElementById('ncCount').textContent = allNcList.length;
        btn.style.display = allNcList.length > 0 ? '' : 'none';
        applicaFiltri();
    } catch (err) {
        container.innerHTML = `<tr><td colspan="8" style="color:red;padding:20px">Errore: ${escHtml(err.message)}</td></tr>`;
    }
}

function applicaFiltri() {
    const numero  = document.getElementById('filtroNumero').value.trim().toLowerCase();
    const stato   = document.getElementById('filtroStato').value.toLowerCase();
    const rilev   = document.getElementById('filtroRilevataDa').value.toLowerCase();
    const owner   = document.getElementById('filtroOwner').value.trim().toLowerCase();
    const dataDal = document.getElementById('filtroDataDal').value;
    const dataAl  = document.getElementById('filtroDataAl').value;

    let filtrata = allNcList.filter(nc => {
        if (numero && !String(nc.numero).toLowerCase().includes(numero)) return false;
        if (stato  && (nc.stato || '').toLowerCase() !== stato) return false;
        if (rilev  && (nc.rilevataDa || '').toLowerCase() !== rilev) return false;
        if (owner) {
            let owners = '';
            try { owners = JSON.parse(nc.owner || '[]').join(' ').toLowerCase(); } catch { }
            if (!owners.includes(owner)) return false;
        }
        if (dataDal && nc.dataApertura && nc.dataApertura.slice(0, 10) < dataDal) return false;
        if (dataAl  && nc.dataApertura && nc.dataApertura.slice(0, 10) > dataAl)  return false;
        return true;
    });

    const count = document.getElementById('risultatiCount');
    count.textContent = filtrata.length === allNcList.length
        ? `${allNcList.length} Non Conformità totali`
        : `${filtrata.length} di ${allNcList.length} Non Conformità`;

    renderTabellaNC(filtrata);
}

function pulisciFiltri() {
    ['filtroNumero', 'filtroOwner', 'filtroDataDal', 'filtroDataAl']
        .forEach(id => document.getElementById(id).value = '');
    document.getElementById('filtroStato').value = '';
    document.getElementById('filtroRilevataDa').value = '';
    applicaFiltri();
}

function parseOwners(json) {
    try { return JSON.parse(json || '[]').join(', ') || '—'; } catch { return '—'; }
}

function renderTabellaNC(list) {
    const tbody = document.getElementById('archivioTabella');
    if (!list.length) {
        tbody.innerHTML = '<tr><td colspan="8" style="text-align:center;padding:40px;color:#999">Nessuna Non Conformità trovata con i filtri selezionati.</td></tr>';
        return;
    }
    tbody.innerHTML = list.map(nc => {
        const owners = parseOwners(nc.owner);
        const statoClass = (nc.stato || '').toLowerCase() === 'chiusa' ? 'chiusa' : 'aperta';
        const urlDettagli  = appUrl(`Dettagli?id=${encodeURIComponent(nc.id)}`);
        const urlRevisiona = appUrl(`NuovaNC?id=${encodeURIComponent(nc.id)}`);
        return `<tr>
            <td><strong>${formatNumeroNC(nc.numero)}</strong></td>
            <td><span class="nc-stato-badge ${statoClass}">${escHtml(nc.stato || '—')}</span></td>
            <td>${escHtml(nc.isoNormaRiferimento || '—')}</td>
            <td>${escHtml(nc.rilevataDa || '—')}</td>
            <td>${fmtDate(nc.dataApertura)}</td>
            <td>${escHtml(owners)}</td>
            <td>${escHtml(nc.creatoDa || '—')}</td>
            <td class="nc-azioni">
                <a class="btn secondary btn-sm" href="${urlDettagli}" title="Dettagli">🔍</a>
                <a class="btn primary btn-sm" href="${urlRevisiona}" title="Revisiona">✏️</a>
                <button class="btn danger btn-sm" onclick="eliminaNC(${nc.id},${nc.numero})" title="Elimina">🗑️</button>
            </td>
        </tr>`;
    }).join('');
}

async function eliminaNC(id, numero) {
    if (!confirm(`Sei sicuro di voler eliminare la NC ${formatNumeroNC(numero)}?\nL'operazione non può essere annullata.`)) return;
    try {
        const res = await fetch(API_NC_BY_ID(id), { method: 'DELETE' });
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        toast('NC eliminata con successo', 'success');
        await loadArchivio();
    } catch (err) {
        toast('Errore durante eliminazione: ' + err.message, 'error');
    }
}

// ── EXPORT EXCEL ─────────────────────────────────────────────────────────────
function esportaExcel() {
    const rows = getExportRows();
    if (!rows.length) { toast('Nessun dato da esportare', 'error'); return; }

    const header = ['Numero NC','Stato','ISO/Norma','Item Riferimento','Rilevata Da',
                    'Data Apertura','Data Prev. Chiusura','Data Eff. Chiusura',
                    'Descrizione','Causa','Risoluzione','Azioni Correttive',
                    'Doc SGQ','Owner','Dipartimento','Parti Interessate',
                    'Cliente','Sedi','Creato Da','Revisionato Da'];

    const csvRows = [header, ...rows.map(nc => [
        nc.numero,
        nc.stato || '',
        nc.isoNormaRiferimento || '',
        nc.itemRiferimento || '',
        nc.rilevataDa || '',
        fmtDate(nc.dataApertura),
        fmtDate(nc.dataPrevistaChiusura),
        fmtDate(nc.dataEffettivaChiusura),
        nc.descrizione || '',
        nc.causa || '',
        nc.risoluzione || '',
        nc.azioniCorrettive || '',
        nc.docSgqModificati || '',
        parseOwners(nc.owner),
        nc.dipartimentoCoinvolto || '',
        parsePartiInteressate(nc.partiInteressate),
        parseOwners(nc.cliente),
        parseSediExport(nc.sedi),
        nc.creatoDa || '',
        nc.revisionatoDa || ''
    ])];

    const csvContent = csvRows.map(row =>
        row.map(cell => `"${String(cell).replace(/"/g, '""')}"`).join(';')
    ).join('\r\n');

    const bom = '\uFEFF';
    const blob = new Blob([bom + csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `NC_Archivio_${todayISO()}.csv`;
    a.click();
    URL.revokeObjectURL(url);
    toast('File Excel (CSV) scaricato', 'success');
}

// ── EXPORT PDF ────────────────────────────────────────────────────────────────
function esportaPDF() {
    const rows = getExportRows();
    if (!rows.length) { toast('Nessun dato da esportare', 'error'); return; }

    const now = new Date().toLocaleDateString('it-IT');
    let html = `<!DOCTYPE html><html><head><meta charset="UTF-8">
    <title>Archivio NC</title>
    <style>
        body { font-family: Arial, sans-serif; font-size: 10px; margin: 20px; }
        h1 { font-size: 16px; margin-bottom: 4px; }
        p { color: #666; margin-bottom: 12px; font-size: 10px; }
        table { width: 100%; border-collapse: collapse; }
        th { background: #111; color: white; padding: 6px 8px; text-align: left; font-size: 9px; }
        td { padding: 5px 8px; border-bottom: 1px solid #eee; vertical-align: top; }
        tr:nth-child(even) td { background: #f9f9f9; }
        .badge { display:inline-block; padding:2px 8px; border-radius:10px; font-size:8px; font-weight:bold; }
        .aperta { background:#fef3c7; color:#92400e; }
        .chiusa { background:#d1fae5; color:#065f46; }
    </style></head><body>
    <h1>Archivio Non Conformità</h1>
    <p>Esportato il ${now} — ${rows.length} NC</p>
    <table>
    <thead><tr>
        <th>NC#</th><th>Stato</th><th>ISO/Norma</th><th>Rilevata Da</th>
        <th>Data Apertura</th><th>Owner</th><th>Descrizione</th><th>Creato Da</th>
    </tr></thead><tbody>`;

    rows.forEach(nc => {
        const owners = parseOwners(nc.owner);
        const sc = (nc.stato||'').toLowerCase() === 'chiusa' ? 'chiusa' : 'aperta';
        html += `<tr>
            <td><strong>${nc.numero}</strong></td>
            <td><span class="badge ${sc}">${escHtml(nc.stato||'—')}</span></td>
            <td>${escHtml(nc.isoNormaRiferimento||'—')}</td>
            <td>${escHtml(nc.rilevataDa||'—')}</td>
            <td>${fmtDate(nc.dataApertura)}</td>
            <td>${escHtml(owners)}</td>
            <td>${escHtml((nc.descrizione||'—').substring(0,80))}${(nc.descrizione||'').length>80?'…':''}</td>
            <td>${escHtml(nc.creatoDa||'—')}</td>
        </tr>`;
    });

    html += `</tbody></table></body></html>`;

    const blob = new Blob([html], { type: 'text/html;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const win = window.open(url, '_blank');
    if (win) {
        win.addEventListener('load', () => {
            win.focus();
            win.print();
        });
    }
    setTimeout(() => URL.revokeObjectURL(url), 30000);
    toast('PDF aperto per la stampa', 'success');
}

function getExportRows() {
    // Usa i dati filtrati correnti
    const numero  = document.getElementById('filtroNumero').value.trim().toLowerCase();
    const stato   = document.getElementById('filtroStato').value.toLowerCase();
    const rilev   = document.getElementById('filtroRilevataDa').value.toLowerCase();
    const owner   = document.getElementById('filtroOwner').value.trim().toLowerCase();
    const dataDal = document.getElementById('filtroDataDal').value;
    const dataAl  = document.getElementById('filtroDataAl').value;

    return allNcList.filter(nc => {
        if (numero && !String(nc.numero).toLowerCase().includes(numero)) return false;
        if (stato  && (nc.stato || '').toLowerCase() !== stato) return false;
        if (rilev  && (nc.rilevataDa || '').toLowerCase() !== rilev) return false;
        if (owner) {
            let owners = '';
            try { owners = JSON.parse(nc.owner || '[]').join(' ').toLowerCase(); } catch { }
            if (!owners.includes(owner)) return false;
        }
        if (dataDal && nc.dataApertura && nc.dataApertura.slice(0, 10) < dataDal) return false;
        if (dataAl  && nc.dataApertura && nc.dataApertura.slice(0, 10) > dataAl)  return false;
        return true;
    });
}

function parseSediExport(json) {
    try {
        const arr = JSON.parse(json || '[]');
        return Array.isArray(arr) ? arr.join(', ') : '';
    } catch { return ''; }
}
