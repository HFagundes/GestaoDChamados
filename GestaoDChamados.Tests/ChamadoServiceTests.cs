using System.Collections.Generic;
using Xunit;
using Moq;

namespace GestaoDChamados.Tests
{
    public class ChamadoServiceTests
    {
        [Fact]
        public void Listar_DeveRetornarListaVazia_QuandoNaoExistemChamados()
        {
            // Arrange
            var mockRepositorio = new Mock<IChamadoRepositorio>();
            mockRepositorio.Setup(repo => repo.Listar()).Returns(new List<Chamado>());
            var service = new ChamadoService(mockRepositorio.Object);

            // Act
            var resultado = service.Listar();

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public void Listar_DeveRetornarChamadosFiltradosPorStatus()
        {
            // Arrange
            var chamados = new List<Chamado>
            {
                new Chamado { Id = 1, Status = "Aberto" },
                new Chamado { Id = 2, Status = "Fechado" },
                new Chamado { Id = 3, Status = "Aberto" }
            };

            var mockRepositorio = new Mock<IChamadoRepositorio>();
            mockRepositorio.Setup(repo => repo.Listar()).Returns(chamados);
            var service = new ChamadoService(mockRepositorio.Object);

            // Act
            var resultado = service.ListarPorStatus("Aberto");

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, c => Assert.Equal("Aberto", c.Status));
        }
    }
}
