using System.Collections.Generic;

namespace Domain.DTOs
{
    public class CompraResponseDTO
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int Cambio { get; set; }
        public Dictionary<int, int> DesgloseCambio { get; set; }
    }
}
