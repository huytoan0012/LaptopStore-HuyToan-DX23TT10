using LaptopStore.Models;

namespace LaptopStore.Services;

public interface IProductAdoService
{
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
}