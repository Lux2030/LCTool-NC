using System.ComponentModel.DataAnnotations;

namespace LCTool_NC.Models;

public class NonConformita
{
    public int Id { get; set; }


    public int Numero { get; set; }


    [MaxLength(20)]
    public string Stato { get; set; } = "Aperta";

    [MaxLength(500)]
    public string? IsoNormaRiferimento { get; set; }

    [MaxLength(500)]
    public string? ItemRiferimento { get; set; }

    [MaxLength(100)]
    public string? RilevataDa { get; set; }

    public DateTime? DataApertura { get; set; }
    public DateTime? DataPrevistaChiusura { get; set; }
    public DateTime? DataEffettivaChiusura { get; set; }


    public string? Descrizione { get; set; }
    public string? Causa { get; set; }
    public string? Risoluzione { get; set; }
    public string? AzioniCorrettive { get; set; }

    [MaxLength(1000)]
    public string? DocSgqModificati { get; set; }


    public string? Owner { get; set; }

    [MaxLength(200)]
    public string? DipartimentoCoinvolto { get; set; }

    public string? PartiInteressate { get; set; }

    // Nuovi campi: Cliente e Sedi
    [MaxLength(1000)]
    public string? Cliente { get; set; }

    [MaxLength(500)]
    public string? Sedi { get; set; }

    public DateTime? DataRevisione { get; set; }


    public string? NoteRevisione { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }


    [MaxLength(100)]
    public string? CreatoDa { get; set; }


    [MaxLength(100)]
    public string? ModificatoDa { get; set; }


    [MaxLength(100)]
    public string? RevisionatoDa { get; set; }
}
