using Business.Services;
using Data.Repositories;
using Models;
using Moq;

namespace Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaClientes()
    {
        var clientes = new List<Cliente>
        {
            new() { IdCliente = 1, Nombre = "Juan", Direccion = "Calle 1" },
            new() { IdCliente = 2, Nombre = "María", Direccion = "Calle 2" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(clientes);

        var resultado = await _service.GetAllAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetByIdAsync_RetornaCliente()
    {
        var cliente = new Cliente { IdCliente = 1, Nombre = "Juan", Direccion = "Calle 1" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);

        var resultado = await _service.GetByIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal("Juan", resultado.Nombre);
    }

    [Fact]
    public async Task GetByIdAsync_RetornaNull_SiNoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

        var resultado = await _service.GetByIdAsync(99);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task CreateAsync_Valido_InsertaCliente()
    {
        var cliente = new Cliente { Nombre = "Juan", Telefono = "1234", Direccion = "Calle 1" };
        _repositoryMock.Setup(r => r.InsertAsync(cliente)).ReturnsAsync(1);

        var id = await _service.CreateAsync(cliente);

        Assert.Equal(1, id);
        _repositoryMock.Verify(r => r.InsertAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SinNombre_LanzaExcepcion()
    {
        var cliente = new Cliente { Nombre = "", Direccion = "Calle 1" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(cliente));
    }

    [Fact]
    public async Task CreateAsync_SinDireccion_LanzaExcepcion()
    {
        var cliente = new Cliente { Nombre = "Juan", Direccion = "" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(cliente));
    }

    [Fact]
    public async Task UpdateAsync_Valido_ActualizaCliente()
    {
        var cliente = new Cliente { IdCliente = 1, Nombre = "Juan", Direccion = "Calle 1" };

        await _service.UpdateAsync(cliente);

        _repositoryMock.Verify(r => r.UpdateAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_LlamaRepository()
    {
        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}