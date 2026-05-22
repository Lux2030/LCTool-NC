using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCTool_NC.Models
{
    [Table("NonConformitaPartiInteressate")]
    public class NonConformitaPartiInteressate
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Valore { get; set; } = string.Empty;

        public int Ordine { get; set; }

        public bool FlagAttivo { get; set; } = true;
    }
}
