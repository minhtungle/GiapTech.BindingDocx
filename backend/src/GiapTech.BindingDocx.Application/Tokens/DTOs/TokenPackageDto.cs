namespace GiapTech.BindingDocx.Application.Tokens.DTOs;

public record TokenPackageDto(
    Guid Id,
    string Name,
    int TokenAmount,
    decimal PricePerToken,
    decimal TotalPrice,
    bool IsActive,
    int SortOrder
);
