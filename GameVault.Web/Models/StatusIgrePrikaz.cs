using GameVault.Data.Models;

namespace GameVault.Web.Models;

public static class StatusIgrePrikaz
{
    public static string Naziv(StatusIgre status) => status switch
    {
        StatusIgre.Planirana => "Planirana",
        StatusIgre.UToku => "U toku",
        StatusIgre.Zavrsena => "Završena",
        StatusIgre.Napustena => "Napuštena",
        _ => "Nepoznat status"
    };
}
