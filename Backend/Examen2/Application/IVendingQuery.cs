using System.Collections.Generic;
using Domain.DTOs;

namespace Application
{
    public interface IVendingQuery
    {
        List<BebidaDTO> ObtenerBebidas();
        CompraResponseDTO ProcesarCompra(CompraRequestDTO request);
    }
}
