namespace Domain
{
    public class MonedaModel
    {
        public int Valor { get; set; }
        public int Cantidad { get; set; }

        public MonedaModel(int valor, int cantidad)
        {
            Valor = valor;
            Cantidad = cantidad;
        }
    }
}
