using System.Globalization;
using System.Xml.Linq;
using LaptopStore.Models;

namespace LaptopStore.Services;

public class XmlPromotionService
{
    private readonly string _filePath;

    public XmlPromotionService(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "Data", "promotions.xml");
    }

    public async Task<IReadOnlyList<Promotion>> GetAllAsync()
    {
        EnsureFileExists();
        await using var fileStream = File.OpenRead(_filePath);
        var document = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);

        return document.Root?.Elements("promotion")
            .Select(element => new Promotion
            {
                Id = (int?)element.Element("id") ?? 0,
                Title = (string?)element.Element("title") ?? string.Empty,
                Code = (string?)element.Element("code") ?? string.Empty,
                DiscountPercent = ParseDecimal((string?)element.Element("discountPercent")),
                IsActive = (bool?)element.Element("isActive") ?? false
            })
            .ToList() ?? new List<Promotion>();
    }

    public async Task AddAsync(Promotion promotion)
    {
        EnsureFileExists();
        await using var inputStream = File.OpenRead(_filePath);
        var document = await XDocument.LoadAsync(inputStream, LoadOptions.None, CancellationToken.None);
        var nextId = (document.Root?.Elements("promotion").Select(element => (int?)element.Element("id") ?? 0).DefaultIfEmpty().Max() ?? 0) + 1;
        promotion.Id = nextId;

        document.Root?.Add(new XElement("promotion",
            new XElement("id", nextId),
            new XElement("title", promotion.Title),
            new XElement("code", promotion.Code),
            new XElement("discountPercent", promotion.DiscountPercent.ToString(CultureInfo.InvariantCulture)),
            new XElement("isActive", promotion.IsActive)));

        await using var fileStream = File.Create(_filePath);
        await document.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            new XDocument(new XElement("promotions")).Save(_filePath);
        }
    }

    private static decimal ParseDecimal(string? value)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }
}
