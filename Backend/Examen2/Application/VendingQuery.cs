using System.Collections.Generic;
using System.Linq;
using Domain.DTOs;
using Application.Interfaces;
using Domain;
using Infrastructure;

namespace Application
{
    public class VendingQuery : IVendingQuery
    {
        private readonly IVendingRepository _repository;

        public VendingQuery(IVendingRepository repository)
        {
            _repository = repository;
        }

        public List<BebidaDTO> ObtenerBebidas()
        {
            return _repository.ObtenerBebidas()
                .Select(b => new BebidaDTO
                {
                    Nombre = b.Nombre,
                    Precio = b.Precio,
                    Cantidad = b.Cantidad
                }).ToList();
        }

        public CompraResponseDTO ProcesarCompra(CompraRequestDTO request)
        {
            var bebida = ObtenerBebida(request.NombreBebida);
            if (bebida == null)
                return Error("Refresco no disponible.");

            if (!HayStockDisponible(bebida, request.Cantidad))
                return Error("No hay suficiente stock del refresco.");

            int totalCompra = CalcularTotal(bebida.Precio, request.Cantidad);
            int totalIngresado = CalcularDineroIngresado(request.DineroIngresado);

            if (totalIngresado < totalCompra)
                return Error("Dinero insuficiente para completar la compra.");

            int cambio = totalIngresado - totalCompra;

            var monedasDisponibles = _repository.ObtenerMonedas();
            var desgloseCambio = CalcularCambio(cambio, monedasDisponibles);

            if (desgloseCambio == null)
                return Error("No hay monedas suficientes para dar el cambio.");

            ActualizarStockYMonedas(bebida, request, desgloseCambio);

            return new CompraResponseDTO
            {
                Exito = true,
                Mensaje = "Compra realizada con éxito.",
                Cambio = cambio,
                DesgloseCambio = desgloseCambio
            };
        }

        private Bebida? ObtenerBebida(string nombre)
        {
            return _repository.ObtenerBebidas().FirstOrDefault(b => b.Nombre == nombre);
        }

        private bool HayStockDisponible(Bebida bebida, int cantidadSolicitada)
        {
            return bebida.Cantidad >= cantidadSolicitada;
        }

        private int CalcularTotal(int precioUnitario, int cantidad)
        {
            return precioUnitario * cantidad;
        }

        private int CalcularDineroIngresado(Dictionary<int, int> dinero)
        {
            return dinero.Sum(p => p.Key * p.Value);
        }

        private void ActualizarStockYMonedas(Bebida bebida, CompraRequestDTO request, Dictionary<int, int> cambio)
        {
            _repository.ActualizarBebida(bebida.Nombre, request.Cantidad);

            var monedasIngresadas = request.DineroIngresado
                .Select(p => new Moneda(p.Key, p.Value)).ToList();
            _repository.AgregarMonedasIngresadas(monedasIngresadas);

            var monedasCambio = cambio.Select(p => new Moneda(p.Key, p.Value)).ToList();
            _repository.ActualizarMonedas(monedasCambio);
        }

        private Dictionary<int, int>? CalcularCambio(int cambio, List<Moneda> monedasDisponibles)
        {
            var resultado = new Dictionary<int, int>();
            var ordenado = monedasDisponibles
                .Where(m => m.Cantidad > 0 && m.Valor <= cambio)
                .OrderByDescending(m => m.Valor)
                .ToList();

            foreach (var moneda in ordenado)
            {
                int cantidadNecesaria = cambio / moneda.Valor;
                int cantidadUsar = Math.Min(cantidadNecesaria, moneda.Cantidad);

                if (cantidadUsar > 0)
                {
                    resultado[moneda.Valor] = cantidadUsar;
                    cambio -= moneda.Valor * cantidadUsar;
                }
            }

            return cambio == 0 ? resultado : null;
        }

        private CompraResponseDTO Error(string mensaje)
        {
            return new CompraResponseDTO
            {
                Exito = false,
                Mensaje = mensaje,
                Cambio = 0,
                DesgloseCambio = new Dictionary<int, int>()
            };
        }
    }
}
