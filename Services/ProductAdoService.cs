using LaptopStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LaptopStore.Services;

public class ProductAdoService : IProductAdoService
{
    private readonly IConfiguration _configuration;

    public ProductAdoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = new List<Product>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Chưa cấu hình DefaultConnection.");

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(@"
            SELECT Id, Name, Price, Description, StockQuantity, ImageUrl, Specs,
                   CreatedDate, IsActive, BrandId
            FROM Products
            WHERE IsActive = @isActive
            ORDER BY CreatedDate DESC", connection);

        command.Parameters.Add("@isActive", SqlDbType.Bit).Value = true;
        await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                Specs = reader.IsDBNull(reader.GetOrdinal("Specs")) ? null : reader.GetString(reader.GetOrdinal("Specs")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                BrandId = reader.GetInt32(reader.GetOrdinal("BrandId"))
            });
        }

        return products;
    }
}