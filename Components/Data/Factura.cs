using System;
using System.Collections.Generic;

namespace FACTURA.Components.Data
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Cliente { get; set; } = "";
        public List<Articulo> Articulos { get; set; } = new();
        public decimal Total => Articulos.Sum(a => a.Subtotal);
    }
}
