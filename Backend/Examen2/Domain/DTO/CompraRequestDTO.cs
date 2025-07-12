using System.Collections.Generic;

namespace Domain.DTOs
{
    public class CompraRequestDTO
    {
        public string NombreBebida { get; set; }
        public int Cantidad { get; set; }
        public Dictionary<int, int> DineroIngresado { get; set; } 
    }
}
