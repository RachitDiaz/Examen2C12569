using Domain;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IVendingRepository
    {
        List<Bebida> ObtenerBebidas();
        List<Moneda> ObtenerMonedas();
        void ActualizarBebida(string nombre, int cantidadComprada);
        void ActualizarMonedas(List<Moneda> cambioEntregado);
        void AgregarMonedasIngresadas(List<Moneda> ingreso);
    }
}
