using GameVault.Business.Models;
using GameVault.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameVault.Web.Models;

public class IgraListaViewModel
{
    public IgraPretraga Pretraga { get; set; } = new();
    [BindNever, ValidateNever]
    public List<Igra> Igre { get; set; } = new();
    [BindNever, ValidateNever]
    public List<SelectListItem> Zanrovi { get; set; } = new();
    [BindNever, ValidateNever]
    public List<SelectListItem> Platforme { get; set; } = new();
}
