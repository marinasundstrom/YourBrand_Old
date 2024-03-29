﻿using YourBrand.Accounting.Client;

using YourBrand.Invoicing.Client;

namespace YourBrand.Accountant.Services
{
    public class RefundService : IRefundService
    {
        private readonly IInvoicesClient _invoicesClient;
        private readonly IJournalEntriesClient _verificationsClient;
        private readonly ILogger<RefundService> _logger;

        public RefundService(IInvoicesClient invoicesClient, IJournalEntriesClient verificationsClient, ILogger<RefundService> logger)
        {
            _invoicesClient = invoicesClient;
            _verificationsClient = verificationsClient;
            _logger = logger;
        }

        public async Task CheckForRefund()
        {
            _logger.LogInformation("Querying for invoices");

            var results = await _invoicesClient.GetInvoicesAsync(0, 100, null, new [] { InvoiceStatus.Overpaid }, null);

            foreach(var invoice in results.Items)
            {
                if(invoice.Status == InvoiceStatus.Overpaid)
                {
                    _logger.LogDebug($"Handling overpaid invoice {invoice.Id}");

                    // TODO: Fix calculations with regards to VAT

                    var amountToRefund = invoice.Paid - invoice.Total;
                    var subTotal = amountToRefund / (1m + 0.25m);
                    var vat = amountToRefund - subTotal;

                    var journalEntryId = await _verificationsClient.CreateJournalEntryAsync(new CreateJournalEntry
                    {
                        Description = $"Betalade tillbaka för överbetalad faktura #{invoice.InvoiceNo}",
                        InvoiceNo = int.Parse(invoice.InvoiceNo),
                        Entries = new[]
                        {
                            new CreateEntry
                            {
                                AccountNo = 1510,
                                Description = string.Empty,
                                Credit = amountToRefund
                            },
                            new CreateEntry
                            {
                                AccountNo = 2610,
                                Description = string.Empty,
                                Debit = vat
                            },
                            new CreateEntry
                            {
                                AccountNo = 3001,
                                Description = string.Empty,
                                Debit = amountToRefund - vat
                            }
                        }
                    });

                    await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.PartiallyRepaid);
                }
            }
        }
    }
}

