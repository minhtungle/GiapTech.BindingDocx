namespace GiapTech.BindingDocx.Application.Admin.Tokens.DTOs;

public record AdminTokenTransactionDto(
    Guid Id,
    Guid UserId,
    string Username,
    string Type,
    int Amount,
    string? Description,
    DateTime CreatedAt
);
