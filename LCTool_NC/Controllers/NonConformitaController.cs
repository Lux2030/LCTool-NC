using LCTool_NC.Data;
using LCTool_NC.Models;
using LCTool_NC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LCTool_NC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NonConformitaController : ControllerBase
{
    private readonly NonConformitaService _service;
    private readonly CurrentUserService _currentUser;
    private readonly AppDbContext _db;

    public NonConformitaController(
        NonConformitaService service,
        CurrentUserService currentUser,
        AppDbContext db)
    {
        _service = service;
        _currentUser = currentUser;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var nc = await _service.GetByIdAsync(id);
        return nc == null ? NotFound() : Ok(nc);
    }

    [HttpGet("prossimo-numero")]
    public async Task<IActionResult> ProssimoNumero()
        => Ok(new { numero = await _service.GetProssimoNumeroAsync() });

    [HttpGet("rilevata-da")]
    public async Task<IActionResult> GetRilevataDa()
    {
        var valori = await _db.NonConformitaRilevataDa
            .Where(r => r.FlagAttivo)
            .OrderBy(r => r.Ordine)
            .Select(r => new { r.Id, r.Valore })
            .ToListAsync();
        return Ok(valori);
    }

    [HttpGet("stati")]
    public async Task<IActionResult> GetStati()
    {
        var valori = await _db.NonConformitaStati
            .Where(s => s.FlagAttivo)
            .OrderBy(s => s.Id)
            .Select(s => new { s.Id, s.Nome })
            .ToListAsync();
        return Ok(valori);
    }

    [HttpGet("parti-interessate")]
    public async Task<IActionResult> GetPartiInteressate()
    {
        var valori = await _db.NonConformitaPartiInteressate
            .Where(p => p.FlagAttivo)
            .OrderBy(p => p.Ordine)
            .Select(p => new { p.Id, p.Valore })
            .ToListAsync();
        return Ok(valori);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NonConformita nc)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(nc);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT standard (per ambienti senza WebDAV)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] NonConformita nc)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, nc);
        return updated == null ? NotFound() : Ok(updated);
    }

    // POST alternativo per fix errore 405 su IIS/IIS Express con WebDAV attivo
    [HttpPost("{id:int}/update")]
    public async Task<IActionResult> UpdateViaPost(int id, [FromBody] NonConformita nc)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, nc);
        return updated == null ? NotFound() : Ok(updated);
    }

    // DELETE standard
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // POST alternativo per fix errore 405 su IIS/IIS Express con WebDAV attivo
    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> DeleteViaPost(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // ── DATA RETENTION ──────────────────────────────────────────────────────
    // Elimina le NC chiuse più vecchie di N giorni (configurabile in appsettings)
    [HttpPost("data-retention")]
    public async Task<IActionResult> EseguiDataRetention()
    {
        var giorni = _db.Database.GetDbConnection().ConnectionString.Contains("LCToolDb") ? 3650 : 3650;
        var config = HttpContext.RequestServices.GetService<IConfiguration>();
        if (int.TryParse(config?["DataRetention:GiorniConservazione"], out var g) && g > 0)
            giorni = g;

        var soglia = DateTime.Now.AddDays(-giorni);
        var daEliminare = await _db.NonConformita
            .Where(nc => nc.Stato == "Chiusa" && nc.DataEffettivaChiusura.HasValue
                      && nc.DataEffettivaChiusura.Value < soglia)
            .ToListAsync();

        _db.NonConformita.RemoveRange(daEliminare);
        await _db.SaveChangesAsync();

        return Ok(new { eliminate = daEliminare.Count, soglia = soglia.ToString("dd/MM/yyyy") });
    }

    [HttpGet("data-retention/anteprima")]
    public async Task<IActionResult> AnteprimaDataRetention()
    {
        var config = HttpContext.RequestServices.GetService<IConfiguration>();
        var giorni = 3650;
        if (int.TryParse(config?["DataRetention:GiorniConservazione"], out var g) && g > 0)
            giorni = g;

        var soglia = DateTime.Now.AddDays(-giorni);
        var count = await _db.NonConformita
            .Where(nc => nc.Stato == "Chiusa" && nc.DataEffettivaChiusura.HasValue
                      && nc.DataEffettivaChiusura.Value < soglia)
            .CountAsync();

        return Ok(new { count, giorni, soglia = soglia.ToString("dd/MM/yyyy") });
    }
}
