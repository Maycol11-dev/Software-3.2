using Business.Services;
using Data.Repositories;
using Models;
using Moq;

namespace Tests;

public class CocinaServiceTests
{
    private readonly Mock<ICocinaRepository> _repositoryMock;
    private readonly CocinaService _service;

    public CocinaServiceTests()
    {
        _repositoryMock = new Mock<ICocinaRepository>();
        _service = new CocinaService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaCocinas()
    {
        var cocinas = new List<Cocina>
        {
            new() { IdCocina = 1, Nombre = "Cocina 1", Disponible = true }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(cocinas);

        var resultado = await _service.GetAllAsync();

        Assert.Single(resultado);
    }

    [Fact]
    public async Task StartPreparacionAsync_IncrementaContador()
    {
        var cocina = new Cocina { IdCocina = 1, Nombre = "Cocina 1", PedidosEnPreparacion = 0 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cocina);

        await _service.StartPreparacionAsync(1);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Cocina>(c => c.PedidosEnPreparacion == 1)), Times.Once);
    }

    [Fact]
    public async Task FinishPreparacionAsync_DecrementaContador()
    {
        var cocina = new Cocina { IdCocina = 1, Nombre = "Cocina 1", PedidosEnPreparacion = 2 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cocina);

        await _service.FinishPreparacionAsync(1);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Cocina>(c => c.PedidosEnPreparacion == 1)), Times.Once);
    }

    [Fact]
    public async Task FinishPreparacionAsync_SinPedidos_LanzaExcepcion()
    {
        var cocina = new Cocina { IdCocina = 1, Nombre = "Cocina 1", PedidosEnPreparacion = 0 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cocina);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.FinishPreparacionAsync(1));
    }

    [Fact]
    public async Task StartPreparacionAsync_CocinaNoExiste_LanzaExcepcion()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cocina?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.StartPreparacionAsync(99));
    }

    [Fact]
    public async Task CreateAsync_SinNombre_LanzaExcepcion()
    {
        var cocina = new Cocina { Nombre = "" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(cocina));
    }

    [Fact]
    public async Task CreateAsync_Valido_InsertaCocina()
    {
        var cocina = new Cocina { Nombre = "Cocina 1" };
        _repositoryMock.Setup(r => r.InsertAsync(cocina)).ReturnsAsync(1);

        var id = await _service.CreateAsync(cocina);

        Assert.Equal(1, id);
    }
}