using LaptopStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStore.Controllers;

[ApiController]
[Route("api/ado/products")]
public class AdoProductsController : ControllerBase
{
    private readonly IProductAdoService _productAdoService;

    public AdoProductsController(IProductAdoService productAdoService)
    {
        _productAdoService = productAdoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var products = await _productAdoService.GetActiveProductsAsync(cancellationToken);
        return Ok(products);
    }
}