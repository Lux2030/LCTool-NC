using LCTool_NC.Data;
using LCTool_NC.Models;
using Microsoft.EntityFrameworkCore;

namespace LCTool_NC.Services;

public class NonConformitaService
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public NonConformitaService(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<NonConformita>> GetAllAsync()
        => await _db.NonConformita.OrderByDescending(n => n.Numero).ToListAsync();

    public async Task<NonConformita?> GetByIdAsync(int id)
        => await _db.NonConformita.FindAsync(id);

    public async Task<int> GetProssimoNumeroAsync()
    {
        if (!await _db.NonConformita.AnyAsync()) return 1;
        return await _db.NonConformita.MaxAsync(n => n.Numero) + 1;
    }

    public async Task<NonConformita> CreateAsync(NonConformita nc)
    {
        var user = _currentUser.GetUserName();

        nc.Numero = await GetProssimoNumeroAsync();
        nc.CreatedAt = DateTime.Now;
        nc.CreatoDa = user;
        nc.ModificatoDa = user;

        if (HasRevisione(nc))
            nc.RevisionatoDa = user;

        _db.NonConformita.Add(nc);
        await _db.SaveChangesAsync();
        return nc;
    }

    public async Task<NonConformita?> UpdateAsync(int id, NonConformita updated)
    {
        var existing = await _db.NonConformita.FindAsync(id);
        if (existing == null) return null;

        var user = _currentUser.GetUserName();

        var revisionePrima = HasRevisione(existing);
        var revisioneDopo = HasRevisione(updated);
        var revisioneCambiata =
            existing.DataRevisione != updated.DataRevisione ||
            existing.NoteRevisione != updated.NoteRevisione;

        existing.Stato = updated.Stato;
        existing.IsoNormaRiferimento = updated.IsoNormaRiferimento;
        existing.ItemRiferimento = updated.ItemRiferimento;
        existing.RilevataDa = updated.RilevataDa;
        existing.DataApertura = updated.DataApertura;
        existing.DataPrevistaChiusura = updated.DataPrevistaChiusura;
        existing.DataEffettivaChiusura = updated.DataEffettivaChiusura;
        existing.Descrizione = updated.Descrizione;
        existing.Causa = updated.Causa;
        existing.Risoluzione = updated.Risoluzione;
        existing.AzioniCorrettive = updated.AzioniCorrettive;
        existing.DocSgqModificati = updated.DocSgqModificati;
        existing.Owner = updated.Owner;
        existing.DipartimentoCoinvolto = updated.DipartimentoCoinvolto;
        existing.PartiInteressate = updated.PartiInteressate;
        existing.Cliente = updated.Cliente;
        existing.Sedi = updated.Sedi;
        existing.DataRevisione = updated.DataRevisione;
        existing.NoteRevisione = updated.NoteRevisione;
        existing.UpdatedAt = DateTime.Now;
        existing.ModificatoDa = user;

        if (revisioneDopo && (revisioneCambiata || !revisionePrima))
            existing.RevisionatoDa = user;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var nc = await _db.NonConformita.FindAsync(id);
        if (nc == null) return false;
        _db.NonConformita.Remove(nc);
        await _db.SaveChangesAsync();
        return true;
    }

    private static bool HasRevisione(NonConformita nc)
        => (nc.DataRevisione.HasValue && nc.DataRevisione.Value.Year > 1900)
           || !string.IsNullOrWhiteSpace(nc.NoteRevisione);
}
