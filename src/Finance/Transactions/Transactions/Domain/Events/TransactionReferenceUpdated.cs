using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Domain.Events;

public record TransactionReferenceUpdated : DomainEvent
{
    public TransactionReferenceUpdated(string transactionId, string reference)
    {
        TransactionId = transactionId;
        Reference = reference;
    }

    public string TransactionId { get; }

    public string Reference { get; }
}