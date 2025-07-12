namespace Domain
{
    public class BebidaModel
    {
        public string Nombre { get; set; }
        public int Precio { get; set; }
        public int Cantidad { get; set; }

        public BebidaModel(string nombre, int precio, int cantidad)
        {
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
        }
    }
}
