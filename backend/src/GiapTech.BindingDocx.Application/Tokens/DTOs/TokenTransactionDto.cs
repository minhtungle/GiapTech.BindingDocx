namespace GiapTech.BindingDocx.Application.Tokens.DTOs;

public record TokenTransactionDto(
    Guid Id,
    string Type,
    int Amount,
    string? Description,
    DateTime CreatedAt
);
