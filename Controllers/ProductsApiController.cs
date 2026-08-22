using LaptopStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaptopStore.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .Select(product => new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.StockQuantity,
                product.ImageUrl,
                product.Specs,
                product.BrandId
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(item => item.Id == id && item.IsActive)
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.Price,
                item.Description,
                item.StockQuantity,
                item.ImageUrl,
                item.Specs,
                item.BrandId
            })
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }
}
