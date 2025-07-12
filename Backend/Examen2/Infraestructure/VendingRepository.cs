using System.Collections.Generic;
using System.Linq;
using Domain;
using Application.Interfaces;

namespace Infrastructure
{
    public class VendingRepository : IVendingRepository
    {
        private static List<BebidaModel> bebidas = new()
        {
            new BebidaModel("Coca Cola", 800, 10),
            new BebidaModel("Pepsi", 750, 8),
            new BebidaModel("Fanta", 950, 10),
            new BebidaModel("Sprite", 975, 15),
        };

        private static List<MonedaModel> monedas = new()
        {
            new MonedaModel(1000, 0),
            new MonedaModel(500, 20),
            new MonedaModel(100, 30),
            new MonedaModel(50, 50),
            new MonedaModel(25, 25),
        };

        public List<BebidaModel> ObtenerBebidas() => bebidas;

        public List<MonedaModel> ObtenerMonedas() => monedas;

        public void ActualizarBebida(string nombre, int cantidadComprada)
        {
            var bebida = bebidas.FirstOrDefault(b => b.Nombre == nombre);
            if (bebida != null)
                bebida.Cantidad -= cantidadComprada;
        }

        public void ActualizarMonedas(List<MonedaModel> cambioEntregado)
        {
            foreach (var m in cambioEntregado)
            {
                var moneda = monedas.FirstOrDefault(x => x.Valor == m.Valor);
                if (moneda != null)
                    moneda.Cantidad -= m.Cantidad;
            }
        }

        public void AgregarMonedasIngresadas(List<MonedaModel> ingreso)
        {
            foreach (var m in ingreso)
            {
                var moneda = monedas.FirstOrDefault(x => x.Valor == m.Valor);
                if (moneda != null)
                    moneda.Cantidad += m.Cantidad;
            }
        }
    }
}
