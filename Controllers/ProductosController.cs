using Microsoft.AspNetCore.Mvc;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;

    public ProductosController(IProductoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public ActionResult<List<Producto>> Get()
    {
        return Ok(_repo.ObtenerTodos());
    }

    [HttpGet("{id}")]
    public ActionResult<Producto> GetPorId(int id)
    {
        var producto = _repo.ObtenerPorId(id);
        if (producto == null)
            return NotFound("Producto no encontrado");

        return Ok(producto);
    }

    [HttpGet("buscar")]
    public ActionResult<List<Producto>> Buscar([FromQuery] string texto)
    {
        return Ok(_repo.Buscar(texto));
    }

    [HttpPost]
    public ActionResult Post(Producto producto)
    {
        int nuevoId = _repo.Insertar(producto);
        producto.Id = nuevoId;
        return CreatedAtAction(nameof(GetPorId), new { id = nuevoId }, producto);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Producto producto)
    {
        producto.Id = id;
        bool ok = _repo.Actualizar(producto);
        if (!ok)
            return NotFound("Producto no encontrado");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        bool ok = _repo.Eliminar(id);
        if (!ok)
            return NotFound("Producto no encontrado");

        return NoContent();
    }
}
