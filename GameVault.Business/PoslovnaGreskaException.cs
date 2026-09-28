namespace GameVault.Business;

public class PoslovnaGreskaException : Exception
{
    public PoslovnaGreskaException(string message) : base(message)
    {
    }
}
