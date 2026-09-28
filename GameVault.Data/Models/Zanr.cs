using System.ComponentModel.DataAnnotations;

namespace GameVault.Data.Models;

public class Zanr
{
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = string.Empty;

    public ICollection<Igra> Igre { get; set; } = new List<Igra>();
}
