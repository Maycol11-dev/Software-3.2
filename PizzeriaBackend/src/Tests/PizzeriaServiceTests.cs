using Business.Services;
using Data.Repositories;
using Models;
using Moq;

namespace Tests;

public class PizzeriaServiceTests
{
    private readonly Mock<IPizzeriaRepository> _repositoryMock;
    private readonly PizzeriaService _service;

    public PizzeriaServiceTests()
    {
        _repositoryMock = new Mock<IPizzeriaRepository>();
        _service = new PizzeriaService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaPizzerias()
    {
        var pizzerias = new List<Pizzeria>
        {
            new() { IdPizzeria = 1, Nombre = "Pizzería Central" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pizzerias);

        var resultado = await _service.GetAllAsync();

        Assert.Single(resultado);
    }

    [Fact]
    public async Task CreateAsync_SinNombre_LanzaExcepcion()
    {
        var pizzeria = new Pizzeria { Nombre = "", Direccion = "Calle 1" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(pizzeria));
    }

    [Fact]
    public async Task CreateAsync_SinDireccion_LanzaExcepcion()
    {
        var pizzeria = new Pizzeria { Nombre = "Pizzería 1", Direccion = "" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(pizzeria));
    }

    [Fact]
    public async Task CreateAsync_Valido_InsertaPizzeria()
    {
        var pizzeria = new Pizzeria { Nombre = "Pizzería 1", Direccion = "Calle 1" };
        _repositoryMock.Setup(r => r.InsertAsync(pizzeria)).ReturnsAsync(1);

        var id = await _service.CreateAsync(pizzeria);

        Assert.Equal(1, id);
    }

    [Fact]
    public async Task DeleteAsync_LlamaRepository()
    {
        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}