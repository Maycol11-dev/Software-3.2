namespace Business.Services;

public interface ICocinaGateway
{
    Task<string> PrepararPedidoAsync(int pedidoId);
}