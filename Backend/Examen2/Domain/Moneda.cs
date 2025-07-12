namespace Domain
{
    public class Moneda
    {
        public int Valor { get; set; }
        public int Cantidad { get; set; }

        public Moneda(int valor, int cantidad)
        {
            Valor = valor;
            Cantidad = cantidad;
        }
    }
}
