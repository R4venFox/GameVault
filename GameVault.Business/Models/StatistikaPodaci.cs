namespace GameVault.Business.Models;

public class StatistikaPodaci
{
    public int UkupnoIgara { get; set; }
    public int Planirane { get; set; }
    public int UToku { get; set; }
    public int Zavrsene { get; set; }
    public int Napustene { get; set; }
    public int Omiljene { get; set; }
    public long UkupnoSati { get; set; }
    public double? ProsecnaOcena { get; set; }
    public string? NajcesciZanr { get; set; }
    public string? NajcescaPlatforma { get; set; }
}
