using System;
using System.Collections.Generic;
using System.Linq;

namespace FACTURA.Components.Data
{
    public class Factura
    {
        public int Id { get; set; }
        public string NombreFactura { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
<<<<<<< HEAD
        public string Cliente { get; set; } = "";
        public List<Articulo> Articulos { get; set; } = new();
=======
        public List<Articulo> Articulos { get; set; } = new(); // ✅ inicializada

        // ✅ Mantiene precisión decimal
>>>>>>> SAT
        public decimal Total => Articulos.Sum(a => a.Subtotal);
    }
}
