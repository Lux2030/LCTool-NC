using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCTool_NC.Models;

[Table("NonConformitaStati")]
public class NonConformitaStati
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public bool FlagAttivo { get; set; } = true;
}