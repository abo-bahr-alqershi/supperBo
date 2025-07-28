namespace YemenBooking.Application.DTOs.PropertySearch;

/// <summary>
/// Aliases for backward-compatibility with MobileApp codebase.
/// </summary>
public static class CompatibilityDtoAliases { }

public partial class PropertyTypeFilterDto
{
    public int Count
    {
        get => PropertiesCount;
        set => PropertiesCount = value;
    }
}

public partial class AmenityFilterDto
{
    public int Count
    {
        get => PropertiesCount;
        set => PropertiesCount = value;
    }
}

public partial class PriceRangeDto
{
    public decimal Min
    {
        get => MinPrice;
        set => MinPrice = value;
    }

    public decimal Max
    {
        get => MaxPrice;
        set => MaxPrice = value;
    }

    public string? Currency { get; set; }
}