let ownerList = [];
let clienteList = [];
let currentStep = 1;

document.addEventListener('DOMContentLoaded', init);

async function init() {
    suppressAutocomplete();
    bindEvents();
    setMaxDataApertura();
    setMinDateChiusura();
    await loadRilevataDa();
    await loadStati();
    await loadPartiInteressate();

    if (window.NC_EDIT_ID) {
        await openRevisiona(window.NC_EDIT_ID);
    } else {
        await caricaNumeroNC();
    }
}

function bindEvents() {
    document.getElementById('btnStep1Next').addEventListener('click', step1Next);
    document.getElementById('btnStep2Back').addEventListener('click', () => goToStep(1));
    document.getElementById('btnStep2Next').addEventListener('click', step2Next);
    document.getElementById('btnStep3Back').addEventListener('click', () => goToStep(2));
    document.getElementById('btnSalva').addEventListener('click', salvaForm);

    // Owner tag input
    document.getElementById('ownerInput').addEventListener('keydown', e => {
        if (e.key === 'Enter' || e.key === ',') {
            e.preventDefault();
            const v = e.target.value.trim();
            if (v && !ownerList.includes(v)) { ownerList.push(v); renderOwnerTags(); }
            e.target.value = '';
        }
    });
    document.getElementById('ownerTags').addEventListener('click', e => {
        if (e.target.classList.contains('tag-remove')) {
            ownerList.splice(+e.target.dataset.i, 1);
            renderOwnerTags();
        }
    });
    document.getElementById('ownerWrap').addEventListener('click', () =>
        document.getElementById('ownerInput').focus());

    // Cliente tag input
    document.getElementById('clienteInput').addEventListener('keydown', e => {
        if (e.key === 'Enter' || e.key === ',') {
            e.preventDefault();
            const v = e.target.value.trim();
            if (v && !clienteList.includes(v)) { clienteList.push(v); renderClienteTags(); }
            e.target.value = '';
        }
    });
    document.getElementById('clienteTags').addEventListener('click', e => {
        if (e.target.classList.contains('tag-remove')) {
            clienteList.splice(+e.target.dataset.i, 1);
            renderClienteTags();
        }
    });
    document.getElementById('clienteWrap').addEventListener('click', () =>
        document.getElementById('clienteInput').focus());

    // Sedi checkboxes
    document.querySelectorAll('#sediBox input[type="checkbox"]').forEach(cb => {
        cb.addEventListener('change', syncSedi);
    });
}

function suppressAutocomplete() {
    const randomize = el => {
        if (el.tagName !== 'INPUT') return;
        if (el.type === 'hidden' || el.type === 'checkbox') return;
        el.setAttribute('autocomplete', 'new-password');
        if (!el.dataset.nameFixed) {
            el.name = (el.name || el.id || 'f') + '_' + Math.random().toString(36).slice(2, 7);
            el.dataset.nameFixed = '1';
        }
    };
    document.querySelectorAll('input').forEach(randomize);
}

function setMaxDataApertura()  { document.getElementById('dataApertura').setAttribute('max', todayISO()); }
function setMinDateChiusura()  { document.getElementById('dataPrevistaChiusura').setAttribute('min', todayISO()); }

function step1Next() {
    if (!validateStep1()) {
        toast('Compila tutti i campi obbligatori (*)', 'error');
        setChipError(1);
        document.querySelector('#step1 .field-error')?.scrollIntoView({ behavior:'smooth', block:'center' });
        return;
    }
    goToStep(2);
}
function step2Next() {
    if (!validateStep2()) {
        toast('Compila tutti i campi obbligatori (*)', 'error');
        setChipError(2);
        document.querySelector('#step2 .field-error')?.scrollIntoView({ behavior:'smooth', block:'center' });
        return;
    }
    populateRiepilogo();
    goToStep(3);
}

function goToStep(n) {
    currentStep = n;
    [1,2,3].forEach(i => {
        document.getElementById(`step${i}`).style.display = (i === n) ? '' : 'none';
        const chip = document.querySelector(`.stepchip[data-step="${i}"]`);
        chip.classList.remove('active','done','error');
        if (i === n) chip.classList.add('active');
        if (i <  n) chip.classList.add('done');
    });
    window.scrollTo({ top:0, behavior:'smooth' });
}
window.goToStep = goToStep;

function setChipError(step) {
    const chip = document.querySelector(`.stepchip[data-step="${step}"]`);
    if (chip) { chip.classList.remove('active','done'); chip.classList.add('error'); }
}

function clearErrors() {
    document.querySelectorAll('.field-error').forEach(el => el.classList.remove('field-error'));
    document.querySelectorAll('.field-error-msg').forEach(el => el.remove());
}
function showError(wrap, msg) {
    if (!wrap) return;
    wrap.classList.add('field-error');
    if (!wrap.querySelector('.field-error-msg')) {
        const span = document.createElement('span');
        span.className = 'field-error-msg'; span.textContent = msg;
        wrap.appendChild(span);
    }
}
function getWrap(id) { const el = document.getElementById(id); return el ? el.closest('div') : null; }

function validateStep1() {
    clearErrors(); let ok = true;
    // Item di Riferimento ora NON obbligatorio
    [['stato','Stato'],['isoNorma','ISO / Norma'],
     ['rilevataDa','Rilevata da'],['dataApertura','Data Apertura'],
     ['dataPrevistaChiusura','Data Prevista Chiusura']].forEach(([id,lbl]) => {
        const el = document.getElementById(id);
        if (!el || !el.value.trim()) { showError(getWrap(id), `${lbl} è obbligatorio`); ok = false; }
    });
    const da = document.getElementById('dataApertura').value;
    if (da && da > todayISO()) { showError(getWrap('dataApertura'),'La Data Apertura non può essere futura'); ok = false; }
    return ok;
}

function validateStep2() {
    clearErrors(); let ok = true;
    // Risoluzione ora NON obbligatoria
    [['descrizione','Descrizione'],['causa','Causa']].forEach(([id,lbl]) => {
        const el = document.getElementById(id);
        if (!el || !el.value.trim()) { showError(getWrap(id), `${lbl} è obbligatorio`); ok = false; }
    });
    if (!ownerList.length) {
        showError(document.getElementById('ownerWrap').closest('div'),'Owner è obbligatorio'); ok = false;
    }
    if (!document.getElementById('dipartimento').value.trim()) {
        showError(getWrap('dipartimento'),'Dipartimento Coinvolto è obbligatorio'); ok = false;
    }
    const checks = document.querySelectorAll('#partiBox .check-label input[type="checkbox"]');
    if (!Array.from(checks).some(cb => cb.checked) && !document.getElementById('piAltri').value.trim()) {
        showError(document.getElementById('partiBox').closest('div'),'Seleziona almeno una Parte Interessata'); ok = false;
    }
    return ok;
}

function populateRiepilogo() {
    const g = id => document.getElementById(id), gv = id => g(id)?.value || '—';
    g('r_numeroNc').textContent = g('numeroNcDisplay').value || '—';
    g('r_stato')   .textContent = gv('stato');
    g('r_iso')     .textContent = gv('isoNorma');
    g('r_item')    .textContent = gv('itemRif');
    g('r_rilevata').textContent = gv('rilevataDa');
    g('r_dataAp')  .textContent = fmtDate(gv('dataApertura'));
    g('r_dataPrev').textContent = fmtDate(gv('dataPrevistaChiusura'));
    g('r_dataEff') .textContent = fmtDate(gv('dataEffettivaChiusura'));
    g('r_desc')    .textContent = gv('descrizione');
    g('r_causa')   .textContent = gv('causa');
    g('r_risol')   .textContent = gv('risoluzione');
    g('r_azioni')  .textContent = gv('azioniCorrettive');
    g('r_docsgq')  .textContent = gv('docSgq');
    g('r_owner')   .textContent = ownerList.length ? ownerList.join(', ') : '—';
    g('r_dip')     .textContent = gv('dipartimento');
    g('r_dataRev') .textContent = fmtDate(gv('dataRevisione'));
    g('r_noteRev') .textContent = gv('noteRevisione');
    g('r_parti')   .textContent = parsePartiInteressate(g('partiInteressateValue').value);
    g('r_cliente') .textContent = clienteList.length ? clienteList.join(', ') : '—';
    g('r_sedi')    .textContent = parseSedi(g('sediValue').value);
}

function renderOwnerTags() {
    const c = document.getElementById('ownerTags');
    c.innerHTML = '';
    ownerList.forEach((name,i) => {
        const tag = document.createElement('span');
        tag.className = 'tag';
        tag.innerHTML = `${escHtml(name)} <span class="tag-remove" data-i="${i}">×</span>`;
        c.appendChild(tag);
    });
    document.getElementById('ownerValue').value = JSON.stringify(ownerList);
}

function renderClienteTags() {
    const c = document.getElementById('clienteTags');
    c.innerHTML = '';
    clienteList.forEach((name,i) => {
        const tag = document.createElement('span');
        tag.className = 'tag';
        tag.innerHTML = `${escHtml(name)} <span class="tag-remove" data-i="${i}">×</span>`;
        c.appendChild(tag);
    });
    document.getElementById('clienteValue').value = JSON.stringify(clienteList);
}

function syncPI() {
    const checks = document.querySelectorAll('#partiBox .check-label input[type="checkbox"]');
    const sel = [];
    checks.forEach(cb => { if (cb.checked) sel.push(cb.dataset.valore); });
    document.getElementById('partiInteressateValue').value =
        JSON.stringify({ selezionati: sel, altri: document.getElementById('piAltri').value });
}

function syncSedi() {
    const checks = document.querySelectorAll('#sediBox input[type="checkbox"]');
    const sel = [];
    checks.forEach(cb => { if (cb.checked) sel.push(cb.dataset.sede); });
    document.getElementById('sediValue').value = JSON.stringify(sel);
}

function parseSedi(json) {
    try {
        const arr = JSON.parse(json || '[]');
        return Array.isArray(arr) && arr.length ? arr.join(', ') : '—';
    } catch { return '—'; }
}

async function collectForm() {
    syncPI();
    syncSedi();
    const g = id => document.getElementById(id);
    return {
        stato: g('stato').value,
        isoNormaRiferimento: g('isoNorma').value.trim() || null,
        itemRiferimento: g('itemRif').value.trim() || null,
        rilevataDa: g('rilevataDa').value || null,
        dataApertura: g('dataApertura').value || null,
        dataPrevistaChiusura: g('dataPrevistaChiusura').value || null,
        dataEffettivaChiusura: g('dataEffettivaChiusura').value || null,
        descrizione: g('descrizione').value.trim() || null,
        causa: g('causa').value.trim() || null,
        risoluzione: g('risoluzione').value.trim() || null,
        azioniCorrettive: g('azioniCorrettive').value.trim() || null,
        docSgqModificati: g('docSgq').value.trim() || null,
        owner: g('ownerValue').value || null,
        dipartimentoCoinvolto: g('dipartimento').value || null,
        partiInteressate: g('partiInteressateValue').value || null,
        cliente: g('clienteValue').value || null,
        sedi: g('sediValue').value || null,
        dataRevisione: g('dataRevisione').value || null,
        noteRevisione: g('noteRevisione').value.trim() || null
    };
}

async function loadRilevataDa() {
    try {
        const v = await (await fetch(API_NC_RILEVATA_DA)).json();
        const sel = document.getElementById('rilevataDa');
        v.forEach(x => { const o = document.createElement('option'); o.value = x.valore; o.textContent = x.valore; sel.appendChild(o); });
    } catch { }
}
async function loadStati() {
    try {
        const v = await (await fetch(API_NC_STATI)).json();
        const sel = document.getElementById('stato');
        v.forEach(x => { const o = document.createElement('option'); o.value = x.nome; o.textContent = x.nome; sel.appendChild(o); });
    } catch { }
}
async function loadPartiInteressate() {
    try {
        const v = await (await fetch(API_NC_PARTI)).json();
        const box = document.getElementById('partiBox');
        const altr = box.querySelector('.pi-altri');
        const lblFixed = document.createElement('label');
        lblFixed.className = 'check-label';
        lblFixed.innerHTML = `<input type="checkbox" data-valore="Legal &amp; Compliance" /> Legal &amp; Compliance`;
        lblFixed.querySelector('input').addEventListener('change', syncPI);
        box.insertBefore(lblFixed, altr);
        v.forEach(x => {
            const lbl = document.createElement('label');
            lbl.className = 'check-label';
            lbl.innerHTML = `<input type="checkbox" data-valore="${x.valore}" /> ${x.valore}`;
            lbl.querySelector('input').addEventListener('change', syncPI);
            box.insertBefore(lbl, altr);
        });
    } catch (err) { console.warn('Errore Parti Interessate:', err); }
}

async function caricaNumeroNC(tent = 0) {
    try {
        const d = await (await fetch(API_NC_PROSSIMO)).json();
        document.getElementById('ncNumeroDisplay').textContent = d.numero;
        document.getElementById('numeroBadge').style.display = '';
        document.getElementById('numeroNcDisplay').value = formatNumeroNC(d.numero);
    } catch {
        if (tent < 5) setTimeout(() => caricaNumeroNC(tent + 1), 800);
    }
}

async function openRevisiona(id) {
    try {
        const res = await fetch(API_NC_BY_ID(id));
        if (!res.ok) throw new Error('NC non trovata');
        const nc = await res.json();
        populateForm(nc);
        document.getElementById('revizioneSection').style.display = '';
        document.getElementById('riepilogoRevizioneSection').style.display = '';
        goToStep(1);
    } catch (err) { toast('Errore: ' + err.message, 'error'); }
}

function populateForm(nc) {
    const g = id => document.getElementById(id);
    g('ncId').value = nc.id;
    g('ncNumero').value = nc.numero;
    g('stato').value = nc.stato || '';
    g('isoNorma').value = nc.isoNormaRiferimento || '';
    g('itemRif').value = nc.itemRiferimento || '';
    g('rilevataDa').value = nc.rilevataDa || '';
    g('dataApertura').value = nc.dataApertura ? nc.dataApertura.slice(0,10) : '';
    g('dataPrevistaChiusura').value = nc.dataPrevistaChiusura ? nc.dataPrevistaChiusura.slice(0,10) : '';
    g('dataEffettivaChiusura').value = nc.dataEffettivaChiusura ? nc.dataEffettivaChiusura.slice(0,10) : '';
    g('descrizione').value = nc.descrizione || '';
    g('causa').value = nc.causa || '';
    g('risoluzione').value = nc.risoluzione || '';
    g('azioniCorrettive').value = nc.azioniCorrettive || '';
    g('docSgq').value = nc.docSgqModificati || '';
    g('dipartimento').value = nc.dipartimentoCoinvolto || '';
    g('dataRevisione').value = nc.dataRevisione ? nc.dataRevisione.slice(0,10) : '';
    g('noteRevisione').value = nc.noteRevisione || '';
    try { ownerList = JSON.parse(nc.owner || '[]'); } catch { ownerList = []; }
    renderOwnerTags();
    try { clienteList = JSON.parse(nc.cliente || '[]'); } catch { clienteList = []; }
    renderClienteTags();
    try {
        const pi = JSON.parse(nc.partiInteressate || '{}');
        document.querySelectorAll('#partiBox .check-label input[type="checkbox"]').forEach(cb => {
            if (Array.isArray(pi.selezionati)) cb.checked = pi.selezionati.includes(cb.dataset.valore);
            else {
                if (cb.dataset.valore === 'Legal & Compliance') cb.checked = !!pi.legalCompliance;
                else if (cb.dataset.valore === 'Diego Pisa')    cb.checked = !!pi.diegoPisa;
                else cb.checked = false;
            }
        });
        g('piAltri').value = pi.altri || '';
    } catch { }
    syncPI();
    try {
        const sediArr = JSON.parse(nc.sedi || '[]');
        document.querySelectorAll('#sediBox input[type="checkbox"]').forEach(cb => {
            cb.checked = Array.isArray(sediArr) && sediArr.includes(cb.dataset.sede);
        });
    } catch { }
    syncSedi();
    g('ncNumeroDisplay').textContent = nc.numero;
    g('numeroBadge').style.display = '';
    g('numeroNcDisplay').value = formatNumeroNC(nc.numero);
    clearErrors();
}

async function salvaForm() {
    const id = document.getElementById('ncId').value;
    const data = await collectForm();
    try {
        const res = id
            ? await fetch(API_NC_BY_ID(id), { method:'PUT',  headers:{'Content-Type':'application/json'}, body: JSON.stringify(data) })
            : await fetch(API_NC,           { method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify(data) });
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const saved = await res.json();

        document.getElementById('homeTitolo').textContent =
            id ? 'Revisione salvata con successo' : `${saved.numero}^ NC compilata con successo`;
        document.querySelectorAll('.steps, #ncForm, #numeroBadge').forEach(el => el && (el.style.display = 'none'));
        document.getElementById('panelSuccesso').style.display = '';
    } catch (err) { toast('Errore: ' + err.message, 'error'); }
}
