using MySqlConnector;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default")!;
    }

    // Conexion
    private MySqlConnection CrearConexion()
    {
        return new MySqlConnection(_connectionString);
    }

    private static Producto Mapear(MySqlDataReader reader)
    {
        return new Producto
        {
            Id = reader.GetInt32("id"),
            Nombre = reader.GetString("nombre"),
            Descripcion = reader.GetString("descripcion"),
            Precio = reader.GetDecimal("precio"),
            Stock = reader.GetInt32("stock")
        };
    }

    // Consultas
    public List<Producto> ObtenerTodos()
    {
        var lista = new List<Producto>();
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand("SELECT id, nombre, descripcion, precio, stock FROM productos", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }
        return lista;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand(
            "SELECT id, nombre, descripcion, precio, stock FROM productos WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return Mapear(reader);
        }
        return null;
    }

    // Insert
    public int Insertar(Producto producto)
    {
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand(
            "INSERT INTO productos (nombre, descripcion, precio, stock) " +
            "VALUES (@nombre, @descripcion, @precio, @stock); SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
        cmd.Parameters.AddWithValue("@precio", producto.Precio);
        cmd.Parameters.AddWithValue("@stock", producto.Stock);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // Actualizar
    public bool Actualizar(Producto producto)
    {
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand(
            "UPDATE productos SET nombre = @nombre, descripcion = @descripcion, " +
            "precio = @precio, stock = @stock WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", producto.Id);
        cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
        cmd.Parameters.AddWithValue("@precio", producto.Precio);
        cmd.Parameters.AddWithValue("@stock", producto.Stock);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Eliminar
    public bool Eliminar(int id)
    {
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand("DELETE FROM productos WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Buscar
    public List<Producto> Buscar(string texto)
    {
        var lista = new List<Producto>();
        using var conn = CrearConexion();
        conn.Open();

        var cmd = new MySqlCommand(
            "SELECT id, nombre, descripcion, precio, stock FROM productos " +
            "WHERE nombre LIKE @texto OR descripcion LIKE @texto", conn);
        cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }
        return lista;
    }
}
