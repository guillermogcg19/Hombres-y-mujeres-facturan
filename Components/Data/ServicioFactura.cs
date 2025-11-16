using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FACTURA.Components.Data
{
    public class ServicioFacturas
    {
        private string RutaDb => Path.Combine(AppContext.BaseDirectory, "facturas.db");

        public ServicioFacturas()
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            cx.Open();

            var cmd = cx.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS facturas(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    fecha TEXT,
                    cliente TEXT
                );
                CREATE TABLE IF NOT EXISTS articulos(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    facturaId INTEGER,
                    nombre TEXT,
                    cantidad INTEGER DEFAULT 1,
                    precio REAL
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public async Task<List<Factura>> ObtenerFacturas()
        {
            var lista = new List<Factura>();

            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, fecha, cliente FROM facturas ORDER BY id DESC";

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                int id = rd.GetInt32(0);
                lista.Add(new Factura
                {
                    Id = id,
                    Fecha = DateTime.Parse(rd.GetString(1)),
                    Cliente = rd.GetString(2),
                    Articulos = await ObtenerArticulos(id)
                });
            }
            return lista;
        }

        public async Task<Factura?> ObtenerFacturaPorId(int id)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, fecha, cliente FROM facturas WHERE id=$id";
            cmd.Parameters.AddWithValue("$id", id);

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                return new Factura
                {
                    Id = rd.GetInt32(0),
                    Fecha = DateTime.Parse(rd.GetString(1)),
                    Cliente = rd.GetString(2),
                    Articulos = await ObtenerArticulos(id)
                };
            }
            return null;
        }

        private async Task<List<Articulo>> ObtenerArticulos(int facturaId)
        {
            var lista = new List<Articulo>();

            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, nombre, cantidad, precio FROM articulos WHERE facturaId=$id";
            cmd.Parameters.AddWithValue("$id", facturaId);

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                lista.Add(new Articulo
                {
                    Id = rd.GetInt32(0),
                    FacturaId = facturaId,
                    Nombre = rd.GetString(1),
                    Cantidad = rd.GetInt32(2),
                    Precio = (decimal)rd.GetDouble(3)
                });
            }
            return lista;
        }

        public async Task CrearFactura(Factura f)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "INSERT INTO facturas(fecha, cliente) VALUES($fecha, $cliente); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$fecha", f.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$cliente", f.Cliente);

            var result = await cmd.ExecuteScalarAsync();
            f.Id = Convert.ToInt32(result);

            foreach (var a in f.Articulos)
                await AgregarArticulo(f.Id, a);
        }

        public async Task ActualizarFacturaCabecera(Factura f)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "UPDATE facturas SET fecha=$fecha, cliente=$cliente WHERE id=$id";
            cmd.Parameters.AddWithValue("$fecha", f.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$cliente", f.Cliente);
            cmd.Parameters.AddWithValue("$id", f.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarArticulo(Articulo a)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "UPDATE articulos SET nombre=$nombre, cantidad=$cantidad, precio=$precio WHERE id=$id";
            cmd.Parameters.AddWithValue("$nombre", a.Nombre);
            cmd.Parameters.AddWithValue("$cantidad", a.Cantidad);
            cmd.Parameters.AddWithValue("$precio", a.Precio);
            cmd.Parameters.AddWithValue("$id", a.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarArticulo(int articuloId)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "DELETE FROM articulos WHERE id=$id";
            cmd.Parameters.AddWithValue("$id", articuloId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AgregarArticulo(int facturaId, Articulo a)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "INSERT INTO articulos(facturaId, nombre, cantidad, precio) VALUES($facturaId, $nombre, $cantidad, $precio)";
            cmd.Parameters.AddWithValue("$facturaId", facturaId);
            cmd.Parameters.AddWithValue("$nombre", a.Nombre);
            cmd.Parameters.AddWithValue("$cantidad", a.Cantidad);
            cmd.Parameters.AddWithValue("$precio", a.Precio);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarFactura(int id)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var c1 = cx.CreateCommand();
            c1.CommandText = "DELETE FROM articulos WHERE facturaId=$id";
            c1.Parameters.AddWithValue("$id", id);
            await c1.ExecuteNonQueryAsync();

            var c2 = cx.CreateCommand();
            c2.CommandText = "DELETE FROM facturas WHERE id=$id";
            c2.Parameters.AddWithValue("$id", id);
            await c2.ExecuteNonQueryAsync();
        }

        public async Task<List<int>> ObtenerAñosDisponibles()
        {
            var lista = new List<int>();

            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT strftime('%Y', fecha) FROM facturas ORDER BY 1 DESC";

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
                lista.Add(int.Parse(rd.GetString(0)));

            return lista;
        }

        public async Task<List<Factura>> ObtenerFacturasPorMes(int año, int mes)
        {
            var lista = new List<Factura>();

            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = @"
                SELECT id, fecha, cliente 
                FROM facturas
                WHERE strftime('%Y', fecha) = $año
                AND strftime('%m', fecha) = $mes
                ORDER BY fecha ASC";

            cmd.Parameters.AddWithValue("$año", año.ToString());
            cmd.Parameters.AddWithValue("$mes", mes.ToString("00"));

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                int idfact = rd.GetInt32(0);
                lista.Add(new Factura
                {
                    Id = idfact,
                    Fecha = DateTime.Parse(rd.GetString(1)),
                    Cliente = rd.GetString(2),
                    Articulos = await ObtenerArticulos(idfact)
                });
            }

            return lista;
        }

        public async Task<decimal> ObtenerTotalPorMes(int año, int mes)
        {
            var facturas = await ObtenerFacturasPorMes(año, mes);
            decimal total = 0;

            foreach (var f in facturas)
                total += f.Total;

            return total;
        }
    }
}
