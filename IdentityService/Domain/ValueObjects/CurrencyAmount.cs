namespace YourBrand.IdentityService.Domain.ValueObjects;

public record CurrencyAmount(
    decimal Currency,
    decimal Amount
) {
    public sealed override string ToString() => $"{Amount} {Currency}";
}