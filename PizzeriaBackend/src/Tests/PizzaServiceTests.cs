using Business.Services;
using Data.Repositories;
using Models;
using Moq;

namespace Tests;

public class PizzaServiceTests
{
    private readonly Mock<IPizzaRepository> _repositoryMock;
    private readonly PizzaService _service;

    public PizzaServiceTests()
    {
        _repositoryMock = new Mock<IPizzaRepository>();
        _service = new PizzaService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaPizzas()
    {
        var pizzas = new List<Pizza>
        {
            new() { IdPizza = 1, Nombre = "Muzzarella", Precio = 1000 },
            new() { IdPizza = 2, Nombre = "Fugazzeta", Precio = 1200 }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pizzas);

        var resultado = await _service.GetAllAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetByIdAsync_RetornaPizza()
    {
        var pizza = new Pizza { IdPizza = 1, Nombre = "Muzzarella", Precio = 1000 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pizza);

        var resultado = await _service.GetByIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal("Muzzarella", resultado.Nombre);
    }

    [Fact]
    public async Task CreateAsync_PrecioCero_LanzaExcepcion()
    {
        var pizza = new Pizza { Nombre = "Muzzarella", Precio = 0 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(pizza));
    }

    [Fact]
    public async Task CreateAsync_PrecioNegativo_LanzaExcepcion()
    {
        var pizza = new Pizza { Nombre = "Muzzarella", Precio = -100 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(pizza));
    }

    [Fact]
    public async Task CreateAsync_SinNombre_LanzaExcepcion()
    {
        var pizza = new Pizza { Nombre = "", Precio = 1000 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(pizza));
    }

    [Fact]
    public async Task CreateAsync_Valido_InsertaPizza()
    {
        var pizza = new Pizza { Nombre = "Muzzarella", Precio = 1000 };
        _repositoryMock.Setup(r => r.InsertAsync(pizza)).ReturnsAsync(1);

        var id = await _service.CreateAsync(pizza);

        Assert.Equal(1, id);
        _repositoryMock.Verify(r => r.InsertAsync(pizza), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Valido_ActualizaPizza()
    {
        var pizza = new Pizza { IdPizza = 1, Nombre = "Muzzarella", Precio = 1000 };

        await _service.UpdateAsync(pizza);

        _repositoryMock.Verify(r => r.UpdateAsync(pizza), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_LlamaRepository()
    {
        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}