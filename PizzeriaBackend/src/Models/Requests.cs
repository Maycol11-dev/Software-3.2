namespace Models;

public record LoginRequest(string Correo, string Contrasena);

public record CrearPedidoRequest(int ClienteId, List<PedidoPizzaRequest> Pizzas);

public record PedidoPizzaRequest(int IdPizza, int Cantidad);

public record CambiarEstadoRequest(PedidoEstado Estado);
