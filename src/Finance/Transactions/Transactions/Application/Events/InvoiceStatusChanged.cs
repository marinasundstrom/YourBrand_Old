using YourBrand.Transactions.Application.Common.Models;
using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Transactions.Hubs;
using YourBrand.Transactions.Application.Common.Interfaces;

namespace YourBrand.Transactions.Application.Events;

public class TransactionStatusChangedHandler : IDomainEventHandler<TransactionStatusChanged>
{
    private readonly ITransactionsContext _context;
    private readonly ITransactionsHubClient _transactionsHubClient;

    public TransactionStatusChangedHandler(ITransactionsContext context, ITransactionsHubClient transactionsHubClient)
    {
        _context = context;
        _transactionsHubClient = transactionsHubClient;
    }

    public async Task Handle(TransactionStatusChanged notification, CancellationToken cancellationToken)
    {
        var transaction = await _context
            .Transactions
            .FirstOrDefaultAsync(i => i.Id == notification.TransactionId);

        if(transaction is not null) 
        {
            await _transactionsHubClient.TransactionStatusUpdated(transaction.Id, transaction.Status);
        }
    }
}
