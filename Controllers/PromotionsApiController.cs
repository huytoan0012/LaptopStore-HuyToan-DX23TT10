using LaptopStore.Models;
using LaptopStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStore.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsApiController : ControllerBase
{
    private readonly XmlPromotionService _promotionService;

    public PromotionsApiController(XmlPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPromotions()
    {
        return Ok(await _promotionService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddPromotion([FromBody] Promotion promotion)
    {
        if (string.IsNullOrWhiteSpace(promotion.Title) || string.IsNullOrWhiteSpace(promotion.Code))
        {
            return BadRequest("Title và Code không được để trống.");
        }

        await _promotionService.AddAsync(promotion);
        return Created("/api/promotions", promotion);
    }
}
