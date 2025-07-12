using Domain;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IVendingRepository
    {
        List<BebidaModel> ObtenerBebidas();
        List<MonedaModel> ObtenerMonedas();
        void ActualizarBebida(string nombre, int cantidadComprada);

        Dictionary<int, int> ObtenerEstadoCambio();
        void ActualizarMonedas(List<MonedaModel> cambioEntregado);
        void AgregarMonedasIngresadas(List<MonedaModel> ingreso);
    }
}
