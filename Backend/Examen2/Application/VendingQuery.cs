using System.Collections.Generic;
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
            List<BebidaModel> bebidas = _repository.ObtenerBebidas();
            List<BebidaDTO> resultado = new List<BebidaDTO>();

            for (int i = 0; i < bebidas.Count; i++)
            {
                BebidaDTO dto = new BebidaDTO();
                dto.Nombre = bebidas[i].Nombre;
                dto.Precio = bebidas[i].Precio;
                dto.Cantidad = bebidas[i].Cantidad;
                resultado.Add(dto);
            }

            return resultado;
        }
        public Dictionary<int, int> ObtenerEstadoCambio()
        {
            return _repository.ObtenerEstadoCambio();
        }
        public CompraResponseDTO ProcesarCompra(CompraRequestDTO request)
        {
            BebidaModel bebida = null;
            List<BebidaModel> bebidas = _repository.ObtenerBebidas();
            for (int i = 0; i < bebidas.Count; i++)
            {
                if (bebidas[i].Nombre == request.NombreBebida)
                {
                    bebida = bebidas[i];
                    break;
                }
            }

            if (bebida == null)
            {
                return Error("Refresco no disponible.");
            }

            if (bebida.Cantidad < request.Cantidad)
            {
                return Error("No hay suficiente stock del refresco.");
            }

            int totalCompra = bebida.Precio * request.Cantidad;

            int totalIngresado = 0;
            foreach (KeyValuePair<int, int> item in request.DineroIngresado)
            {
                totalIngresado += item.Key * item.Value;
            }

            if (totalIngresado < totalCompra)
            {
                return Error("Dinero insuficiente para completar la compra.");
            }

            int cambio = totalIngresado - totalCompra;
            List<MonedaModel> monedasDisponibles = _repository.ObtenerMonedas();

            List<MonedaModel> monedasOrdenadas = new List<MonedaModel>();
            for (int i = monedasDisponibles.Count - 1; i >= 0; i--)
            {
                MonedaModel actual = monedasDisponibles[i];
                int j = 0;
                while (j < monedasOrdenadas.Count && actual.Valor < monedasOrdenadas[j].Valor)
                {
                    j++;
                }
                monedasOrdenadas.Insert(j, actual);
            }

            Dictionary<int, int> desglose = new Dictionary<int, int>();

            for (int i = 0; i < monedasOrdenadas.Count; i++)
            {
                MonedaModel moneda = monedasOrdenadas[i];

                if (moneda.Cantidad > 0 && moneda.Valor <= cambio)
                {
                    int cantidadNecesaria = cambio / moneda.Valor;
                    int cantidadUsar = moneda.Cantidad;

                    if (cantidadNecesaria < cantidadUsar)
                    {
                        cantidadUsar = cantidadNecesaria;
                    }

                    if (cantidadUsar > 0)
                    {
                        desglose[moneda.Valor] = cantidadUsar;
                        cambio -= cantidadUsar * moneda.Valor;
                    }
                }
            }

            if (cambio != 0)
            {
                return Error("No hay monedas suficientes para dar el cambio.");
            }

            _repository.ActualizarBebida(bebida.Nombre, request.Cantidad);

            List<MonedaModel> monedasIngresadas = new List<MonedaModel>();
            foreach (KeyValuePair<int, int> item in request.DineroIngresado)
            {
                MonedaModel m = new MonedaModel(item.Key, item.Value);
                monedasIngresadas.Add(m);
            }
            _repository.AgregarMonedasIngresadas(monedasIngresadas);

            List<MonedaModel> monedasVuelto = new List<MonedaModel>();
            foreach (KeyValuePair<int, int> item in desglose)
            {
                MonedaModel m = new MonedaModel(item.Key, item.Value);
                monedasVuelto.Add(m);
            }
            _repository.ActualizarMonedas(monedasVuelto);

            CompraResponseDTO respuesta = new CompraResponseDTO();
            respuesta.Exito = true;
            respuesta.Mensaje = "Compra realizada con éxito.";
            respuesta.Cambio = totalIngresado - totalCompra;
            respuesta.DesgloseCambio = desglose;

            return respuesta;
        }

        private CompraResponseDTO Error(string mensaje)
        {
            CompraResponseDTO respuesta = new CompraResponseDTO();
            respuesta.Exito = false;
            respuesta.Mensaje = mensaje;
            respuesta.Cambio = 0;
            respuesta.DesgloseCambio = new Dictionary<int, int>();
            return respuesta;
        }
    }
}
