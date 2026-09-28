using System.ComponentModel.DataAnnotations;

namespace GameVault.Web.Models;

public class NazivViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [Display(Name = "Naziv")]
    public string Naziv { get; set; } = string.Empty;
}
