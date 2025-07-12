using System.Collections.Generic;
using System.Linq;
using Domain;
using Application.Interfaces;

namespace Infrastructure
{
    public class VendingRepository : IVendingRepository
    {
        private static List<Bebida> bebidas = new()
        {
            new Bebida("Coca Cola", 800, 10),
            new Bebida("Pepsi", 750, 8),
            new Bebida("Fanta", 950, 10),
            new Bebida("Sprite", 975, 15),
        };

        private static List<Moneda> monedas = new()
        {
            new Moneda(1000, 0),
            new Moneda(500, 20),
            new Moneda(100, 30),
            new Moneda(50, 50),
            new Moneda(25, 25),
        };

        public List<Bebida> ObtenerBebidas() => bebidas;

        public List<Moneda> ObtenerMonedas() => monedas;

        public void ActualizarBebida(string nombre, int cantidadComprada)
        {
            var bebida = bebidas.FirstOrDefault(b => b.Nombre == nombre);
            if (bebida != null)
                bebida.Cantidad -= cantidadComprada;
        }

        public void ActualizarMonedas(List<Moneda> cambioEntregado)
        {
            foreach (var m in cambioEntregado)
            {
                var moneda = monedas.FirstOrDefault(x => x.Valor == m.Valor);
                if (moneda != null)
                    moneda.Cantidad -= m.Cantidad;
            }
        }

        public void AgregarMonedasIngresadas(List<Moneda> ingreso)
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
