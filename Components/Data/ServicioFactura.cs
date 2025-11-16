using FACTURA.Components.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

namespace FACTURA.Components.Data
{
    public class ServicioFacturas
    {
        private readonly string rutaDb = "facturas.db";

        public ServicioFacturas()
        {
            Inicializar();
        }

        private void Inicializar()
        {
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
            cx.Open();
            var cmd = cx.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS facturas(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombrefactura TEXT UNIQUE,
                    cliente TEXT,
                    fecha TEXT
                );
                CREATE TABLE IF NOT EXISTS articulos(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    factura_id INTEGER,
                    nombre TEXT,
                    cantidad INTEGER,
                    precio REAL
                );
            ";
            cmd.ExecuteNonQuery();
<<<<<<< HEAD
=======
        }

        public async Task<bool> ExisteNombreFactura(string nombre)
        {
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
            await cx.OpenAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM facturas WHERE nombrefactura = $n";
            cmd.Parameters.AddWithValue("$n", nombre);

            var result = await cmd.ExecuteScalarAsync();
            long count = result is long c ? c : Convert.ToInt64(result ?? 0);
            return count > 0;
        }

        public async Task GuardarFactura(Factura f)
        {
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
            await cx.OpenAsync();
            using var tx = cx.BeginTransaction();

            // Inserta la factura
            var cmd = cx.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "INSERT INTO facturas (nombrefactura, cliente, fecha) VALUES ($n, $c, $f)";
            cmd.Parameters.AddWithValue("$n", f.NombreFactura);
            cmd.Parameters.AddWithValue("$c", f.Cliente);
            cmd.Parameters.AddWithValue("$f", f.Fecha.ToString("s"));
            await cmd.ExecuteNonQueryAsync();

            // Obtén el ID insertado
            var result = await new SqliteCommand("SELECT last_insert_rowid()", cx, tx).ExecuteScalarAsync();
            long idFactura = result is long id ? id : Convert.ToInt64(result ?? 0);

            // Inserta los artículos
            foreach (var a in f.Articulos)
            {
                var cmdA = cx.CreateCommand();
                cmdA.Transaction = tx;
                cmdA.CommandText = "INSERT INTO articulos (factura_id, nombre, cantidad, precio) VALUES ($fid, $n, $cant, $p)";
                cmdA.Parameters.AddWithValue("$fid", idFactura);
                cmdA.Parameters.AddWithValue("$n", a.Nombre);
                cmdA.Parameters.AddWithValue("$cant", a.Cantidad);
                cmdA.Parameters.AddWithValue("$p", a.Precio);
                await cmdA.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
>>>>>>> SAT
        }

        public async Task<List<Factura>> ObtenerFacturas()
        {
            var lista = new List<Factura>();
<<<<<<< HEAD
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
=======
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
>>>>>>> SAT
            await cx.OpenAsync();
            var cmd = cx.CreateCommand();
<<<<<<< HEAD
            cmd.CommandText = "SELECT id, fecha, cliente FROM facturas ORDER BY id DESC";
=======
            cmd.CommandText = "SELECT id, nombrefactura, cliente, fecha FROM facturas";

>>>>>>> SAT
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                lista.Add(new Factura
                {
                    Id = rd.GetInt32(0),
<<<<<<< HEAD
                    Fecha = DateTime.Parse(rd.GetString(1)),
                    Cliente = rd.GetString(2)
                };
                f.Articulos = await ObtenerArticulos(f.Id);
                lista.Add(f);
            }
            return lista;
        }

        public async Task<Factura?> ObtenerFacturaPorId(int id)
        {
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();
            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, fecha, cliente FROM facturas WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                var f = new Factura
                {
                    Id = rd.GetInt32(0),
                    Fecha = DateTime.Parse(rd.GetString(1)),
                    Cliente = rd.GetString(2)
                };
                f.Articulos = await ObtenerArticulos(f.Id);
                return f;
            }
            return null;
        }

        private async Task<List<Articulo>> ObtenerArticulos(int facturaId)
        {
            var lista = new List<Articulo>();
            using var cx = new SqliteConnection($"Data Source={RutaDb}");
            await cx.OpenAsync();
            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, nombre, cantidad, precio FROM articulos WHERE facturaId = $id";
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
=======
                    NombreFactura = rd.GetString(1),
                    Cliente = rd.GetString(2),
                    Fecha = DateTime.Parse(rd.GetString(3))
>>>>>>> SAT
                });
            }
            return lista;
        }

<<<<<<< HEAD
        public async Task CrearFactura(Factura f)
=======
        public async Task<Factura?> ObtenerFacturaPorId(int id)
>>>>>>> SAT
        {
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
            await cx.OpenAsync();
<<<<<<< HEAD
            var cmd = cx.CreateCommand();
            cmd.CommandText = "INSERT INTO facturas(fecha, cliente) VALUES($fecha, $cliente); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$fecha", f.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$cliente", f.Cliente);
            var id = (long)await cmd.ExecuteScalarAsync();
            f.Id = (int)id;
            foreach (var a in f.Articulos)
                await AgregarArticulo(f.Id, a);
        }

        public async Task ActualizarFacturaCabecera(Factura f)
=======

            Factura? f = null;
            var cmd = cx.CreateCommand();
            cmd.CommandText = "SELECT id, nombrefactura, cliente, fecha FROM facturas WHERE id=$id";
            cmd.Parameters.AddWithValue("$id", id);

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                f = new Factura
                {
                    Id = rd.GetInt32(0),
                    NombreFactura = rd.GetString(1),
                    Cliente = rd.GetString(2),
                    Fecha = DateTime.Parse(rd.GetString(3))
                };
            }

            if (f != null)
            {
                var cmdArt = cx.CreateCommand();
                cmdArt.CommandText = "SELECT nombre, cantidad, precio FROM articulos WHERE factura_id=$id";
                cmdArt.Parameters.AddWithValue("$id", id);

                using var rd2 = await cmdArt.ExecuteReaderAsync();
                while (await rd2.ReadAsync())
                {
                    f.Articulos.Add(new Articulo
                    {
                        Nombre = rd2.GetString(0),
                        Cantidad = rd2.GetInt32(1),
                        Precio = (decimal)rd2.GetDouble(2) // ✅ conversión explícita
                    });
                }
            }


            return f;
        }

        public async Task EliminarFactura(int id)
>>>>>>> SAT
        {
            using var cx = new SqliteConnection($"Data Source={rutaDb}");
            await cx.OpenAsync();
<<<<<<< HEAD
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
            cmd.CommandText = "INSERT INTO articulos(facturaId, nombre, cantidad, precio) VALUES($facturaId,$nombre,$cantidad,$precio)";
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
            var cmd1 = cx.CreateCommand();
            cmd1.CommandText = "DELETE FROM articulos WHERE facturaId=$id";
            cmd1.Parameters.AddWithValue("$id", id);
            await cmd1.ExecuteNonQueryAsync();
            var cmd2 = cx.CreateCommand();
            cmd2.CommandText = "DELETE FROM facturas WHERE id=$id";
            cmd2.Parameters.AddWithValue("$id", id);
            await cmd2.ExecuteNonQueryAsync();
        }
=======

            var cmdA = cx.CreateCommand();
            cmdA.CommandText = "DELETE FROM articulos WHERE factura_id=$id";
            cmdA.Parameters.AddWithValue("$id", id);
            await cmdA.ExecuteNonQueryAsync();

            var cmd = cx.CreateCommand();
            cmd.CommandText = "DELETE FROM facturas WHERE id=$id";
            cmd.Parameters.AddWithValue("$id", id);
            await cmd.ExecuteNonQueryAsync();
        }
>>>>>>> SAT
    }
}
