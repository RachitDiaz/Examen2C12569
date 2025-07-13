using NUnit.Framework;
using Application;
using Infrastructure;
using Domain;
using Domain.DTOs;
using System.Collections.Generic;
using Application.Interfaces;

namespace Examen2.Tests
{
    [TestFixture]
    public class VendingQueryTests
    {
        private IVendingRepository repository;
        private VendingQuery vendingQuery;

        [SetUp]
        public void SetUp()
        {
            repository = new VendingRepository(); 
            vendingQuery = new VendingQuery(repository);
        }

        [Test]
        public void ObtenerBebidas_DeberiaRetornarLista()
        {
            List<BebidaDTO> bebidas = vendingQuery.ObtenerBebidas();
            Assert.IsNotNull(bebidas);
            Assert.IsTrue(bebidas.Count > 0);
        }

        [Test]
        public void ObtenerEstadoCambio_DeberiaRetornarDiccionario()
        {
            Dictionary<int, int> estado = vendingQuery.ObtenerEstadoCambio();
            Assert.IsNotNull(estado);
            Assert.IsTrue(estado.Count > 0);
        }

        [Test]
        public void ProcesarCompra_CompraExitosa_DeberiaRetornarExitoTrue()
        {
            var request = new CompraRequestDTO
            {
                NombreBebida = "Pepsi",
                Cantidad = 1,
                DineroIngresado = new Dictionary<int, int>
                {
                    { 500, 2 } // ₡1000
                }
            };

            CompraResponseDTO resultado = vendingQuery.ProcesarCompra(request);
            Assert.IsTrue(resultado.Exito);
            Assert.AreEqual("Compra realizada con éxito.", resultado.Mensaje);
        }

        [Test]
        public void ProcesarCompra_BebidaNoExiste_DeberiaRetornarError()
        {
            var request = new CompraRequestDTO
            {
                NombreBebida = "Inexistente",
                Cantidad = 1,
                DineroIngresado = new Dictionary<int, int> { { 500, 2 } }
            };

            CompraResponseDTO resultado = vendingQuery.ProcesarCompra(request);
            Assert.IsFalse(resultado.Exito);
            Assert.AreEqual("Refresco no disponible.", resultado.Mensaje);
        }

        [Test]
        public void ProcesarCompra_StockInsuficiente_DeberiaRetornarError()
        {
            var request = new CompraRequestDTO
            {
                NombreBebida = "Pepsi",
                Cantidad = 999,
                DineroIngresado = new Dictionary<int, int> { { 1000, 1 } }
            };

            CompraResponseDTO resultado = vendingQuery.ProcesarCompra(request);
            Assert.IsFalse(resultado.Exito);
            Assert.AreEqual("No hay suficiente stock del refresco.", resultado.Mensaje);
        }

        [Test]
        public void ProcesarCompra_DineroInsuficiente_DeberiaRetornarError()
        {
            var request = new CompraRequestDTO
            {
                NombreBebida = "Pepsi",
                Cantidad = 1,
                DineroIngresado = new Dictionary<int, int> { { 100, 1 } }
            };

            CompraResponseDTO resultado = vendingQuery.ProcesarCompra(request);
            Assert.IsFalse(resultado.Exito);
            Assert.AreEqual("Dinero insuficiente para completar la compra.", resultado.Mensaje);
        }
    }
}
