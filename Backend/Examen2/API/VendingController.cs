using Microsoft.AspNetCore.Mvc;
using Application;
using Domain;
using Domain.DTOs;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendingController : ControllerBase
    {
        private readonly IVendingQuery _vendingQuery;

        public VendingController(IVendingQuery vendingQuery)
        {
            _vendingQuery = vendingQuery;
        }

        [HttpGet("bebidas")]
        public ActionResult<List<BebidaDTO>> ObtenerBebidas()
        {
            var bebidas = _vendingQuery.ObtenerBebidas();
            return Ok(bebidas);
        }

        [HttpPost("comprar")]
        public ActionResult<CompraResponseDTO> Comprar([FromBody] CompraRequestDTO request)
        {
            var resultado = _vendingQuery.ProcesarCompra(request);
            return Ok(resultado);
        }
    }
}
