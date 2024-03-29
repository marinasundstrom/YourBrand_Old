﻿using MassTransit;

using MediatR;

using YourBrand.Transactions.Domain;

namespace YourBrand.Transactions.Application.Commands;

public record PostTransactions(IEnumerable<TransactionDto> Transactions) : IRequest
{
    public class Handler : IRequestHandler<PostTransactions>
    {
        private readonly ITransactionsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(ITransactionsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(PostTransactions request, CancellationToken cancellationToken)
        {
            foreach (var transaction in request.Transactions)
            {
                _context.Transactions.Add(new Domain.Entities.Transaction(
                    transaction.Id,
                    transaction.Date ?? DateTime.Now,
                    transaction.Status,
                    transaction.From,
                    transaction.Reference,
                    transaction.Currency,
                    transaction.Amount));
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new Contracts.IncomingTransactionBatch(request.Transactions.Select(t => new Contracts.Transaction(t.Id, t.Date.GetValueOrDefault(), (Contracts.TransactionStatus)t.Status, t.From, t.Reference, t.Currency, t.Amount))));

        }
    }
}