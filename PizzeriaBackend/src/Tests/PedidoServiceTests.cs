using Business.Services;
using Data.Repositories;
using Models;
using Moq;

namespace Tests;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<IPizzaRepository> _pizzaRepoMock;
    private readonly Mock<ICocinaGateway> _cocinaMock;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _pedidoRepoMock = new Mock<IPedidoRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _pizzaRepoMock = new Mock<IPizzaRepository>();
        _cocinaMock = new Mock<ICocinaGateway>();
        _service = new PedidoService(_pedidoRepoMock.Object, _clienteRepoMock.Object, _pizzaRepoMock.Object, _cocinaMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaPedidos()
    {
        var pedidos = new List<Pedido>
        {
            new() { IdPedido = 1, IdCliente = 1, Estado = PedidoEstado.EsperaDeConfirmacion },
            new() { IdPedido = 2, IdCliente = 2, Estado = PedidoEstado.EnPreparacion }
        };

        _pedidoRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pedidos);

        var resultado = await _service.GetAllAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetByIdAsync_RetornaPedido()
    {
        var pedido = new Pedido { IdPedido = 1, IdCliente = 1, Estado = PedidoEstado.EsperaDeConfirmacion };

        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        var resultado = await _service.GetByIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal(PedidoEstado.EsperaDeConfirmacion, resultado.Estado);
    }

    [Fact]
    public async Task CreateAsync_SinPizzas_LanzaExcepcion()
    {
        _clienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Cliente { IdCliente = 1, Nombre = "Juan" });

        var pizzas = new List<PedidoPizza>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(1, pizzas));
    }

    [Fact]
    public async Task CreateAsync_ClienteNoExiste_LanzaExcepcion()
    {
        _clienteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

        var pizzas = new List<PedidoPizza> { new() { IdPizza = 1, Cantidad = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(99, pizzas));
    }

    [Fact]
    public async Task CreateAsync_CantidadCero_LanzaExcepcion()
    {
        _clienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Cliente { IdCliente = 1, Nombre = "Juan" });
        _pizzaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Pizza { IdPizza = 1, Nombre = "Muzz" });

        var pizzas = new List<PedidoPizza> { new() { IdPizza = 1, Cantidad = 0 } };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(1, pizzas));
    }

    [Fact]
    public async Task CreateAsync_PizzaNoExiste_LanzaExcepcion()
    {
        _clienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Cliente { IdCliente = 1, Nombre = "Juan" });
        _pizzaRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pizza?)null);

        var pizzas = new List<PedidoPizza> { new() { IdPizza = 99, Cantidad = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(1, pizzas));
    }

    [Fact]
    public async Task CreateAsync_Valido_InsertaPedido()
    {
        var cliente = new Cliente { IdCliente = 1, Nombre = "Juan", Direccion = "Calle 1" };
        var pizza = new Pizza { IdPizza = 1, Nombre = "Muzzarella", Precio = 1000 };
        _clienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        _pizzaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pizza);
        _pedidoRepoMock.Setup(r => r.InsertAsync(It.IsAny<Pedido>())).ReturnsAsync(1);

        var pizzas = new List<PedidoPizza> { new() { IdPizza = 1, Cantidad = 2 } };

        var id = await _service.CreateAsync(1, pizzas);

        Assert.Equal(1, id);
        _pedidoRepoMock.Verify(r => r.InsertAsync(It.Is<Pedido>(p =>
            p.IdCliente == 1 &&
            p.Estado == PedidoEstado.EsperaDeConfirmacion &&
            p.Pizzas.Count == 1
        )), Times.Once);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EsperaAEnPreparacion_Exito()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);
        _cocinaMock.Setup(c => c.PrepararPedidoAsync(1)).ReturnsAsync("LISTO 1");

        await _service.ChangeEstadoAsync(1, PedidoEstado.EnPreparacion);

        _cocinaMock.Verify(c => c.PrepararPedidoAsync(1), Times.Once);
        _pedidoRepoMock.Verify(r => r.UpdateEstadoAsync(1, PedidoEstado.EnPreparacion), Times.Once);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EnPreparacion_DelegaEnCocina()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);
        _cocinaMock.Setup(c => c.PrepararPedidoAsync(1)).ReturnsAsync("LISTO 1");

        await _service.ChangeEstadoAsync(1, PedidoEstado.EnPreparacion);

        _cocinaMock.Verify(c => c.PrepararPedidoAsync(1), Times.Once);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EnPreparacion_CocinaNoConfirma_LanzaExcepcion()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);
        _cocinaMock.Setup(c => c.PrepararPedidoAsync(1)).ReturnsAsync("ERROR horno apagado");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(1, PedidoEstado.EnPreparacion));
        _pedidoRepoMock.Verify(r => r.UpdateEstadoAsync(It.IsAny<int>(), It.IsAny<PedidoEstado>()), Times.Never);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EnPreparacion_CocinaCaida_LanzaExcepcion()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);
        _cocinaMock.Setup(c => c.PrepararPedidoAsync(1)).ThrowsAsync(new InvalidOperationException("La cocina no está disponible"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(1, PedidoEstado.EnPreparacion));
        _pedidoRepoMock.Verify(r => r.UpdateEstadoAsync(It.IsAny<int>(), It.IsAny<PedidoEstado>()), Times.Never);
    }

    [Fact]
    public async Task ChangeEstadoAsync_TransicionQueNoIniciaPreparacion_NoLlamaCocina()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EnPreparacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await _service.ChangeEstadoAsync(1, PedidoEstado.EnViaje);

        _cocinaMock.Verify(c => c.PrepararPedidoAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EnPreparacionAEnViaje_Exito()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EnPreparacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await _service.ChangeEstadoAsync(1, PedidoEstado.EnViaje);

        _pedidoRepoMock.Verify(r => r.UpdateEstadoAsync(1, PedidoEstado.EnViaje), Times.Once);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EnViajeAEntregado_Exito()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EnViaje };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await _service.ChangeEstadoAsync(1, PedidoEstado.Entregado);

        _pedidoRepoMock.Verify(r => r.UpdateEstadoAsync(1, PedidoEstado.Entregado), Times.Once);
    }

    [Fact]
    public async Task ChangeEstadoAsync_EsperaAEnViaje_LanzaExcepcion()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(1, PedidoEstado.EnViaje));
    }

    [Fact]
    public async Task ChangeEstadoAsync_EsperaAEntregado_LanzaExcepcion()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.EsperaDeConfirmacion };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(1, PedidoEstado.Entregado));
    }

    [Fact]
    public async Task ChangeEstadoAsync_EntregadoAEnPreparacion_LanzaExcepcion()
    {
        var pedido = new Pedido { IdPedido = 1, Estado = PedidoEstado.Entregado };
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pedido);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(1, PedidoEstado.EnPreparacion));
    }

    [Fact]
    public async Task ChangeEstadoAsync_PedidoNoExiste_LanzaExcepcion()
    {
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pedido?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeEstadoAsync(99, PedidoEstado.EnPreparacion));
    }

    [Fact]
    public async Task DeleteAsync_LlamaRepository()
    {
        await _service.DeleteAsync(1);

        _pedidoRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}