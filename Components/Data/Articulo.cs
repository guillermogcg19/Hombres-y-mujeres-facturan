public class Articulo
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Precio { get; set; } // ✅ decimal
    public decimal Subtotal => Cantidad * Precio; // ✅ también decimal
}
