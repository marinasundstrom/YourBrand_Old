﻿using System;

using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;

namespace YourBrand.Invoicing.Domain.Entities;

public class Invoice : Entity
{
    readonly List<InvoiceItem> _items = new List<InvoiceItem>();

    private Invoice() { }

    public Invoice(DateTime? date, InvoiceType type = InvoiceType.Invoice, InvoiceStatus status = InvoiceStatus.Draft, string currency = "SEK", string? note = null)
    {
        Id = Guid.NewGuid().ToString();

        IssueDate = date ?? DateTime.Now;
        Type = type;
        Status = status;
        Currency = currency;
        Note = note;

        AddDomainEvent(new InvoiceCreated(Id));
    }

    public string Id { get; set; }

    public string InvoiceNo { get; set; }

    public DateTime? IssueDate { get; set; }

    public void SetIssueDate(DateTime? date)
    {
        if (IssueDate != date)
        {
            IssueDate = date;
            AddDomainEvent(new InvoiceDateChanged(Id, IssueDate));
        }
    }

    public InvoiceType Type { get; private set; }

    public void SetType(InvoiceType type)
    {
        if (Type != type)
        {
            Type = type;
            AddDomainEvent(new InvoiceTypeChanged(Id, Type));
        }
    }

    public InvoiceStatus Status { get; private set; }

    public void SetStatus(InvoiceStatus status)
    {
        if (Status != status)
        {
            Status = status;
            AddDomainEvent(new InvoiceStatusChanged(Id, Status));
        }
    }

    public DateTime? DeliveryDate { get; private set; }

    public void SetDeliveryDate(DateTime deliveryDate)
    {
        if (DeliveryDate != deliveryDate)
        {
            DeliveryDate = deliveryDate;
            //AddDomainEvent(new InvoiceDueDateChanged(Id, DeliveryDate));
        }
    }

    public DateTime? DueDate { get; private set; }

    public void SetDueDate(DateTime dueDate)
    {
        if (DueDate != dueDate)
        {
            DueDate = dueDate;
            AddDomainEvent(new InvoiceDueDateChanged(Id, DueDate));
        }
    }

    public string? CustomerId { get; set; }

    public string Currency { get; private set; }

    public void SetCurrency(string currency)
    {
        if (Currency != currency)
        {
            Currency = currency;
            //AddDomainEvent(new InvoiceDueDateChanged(Id, DueDate));
        }
    }

    public string? Reference { get; private set; }

    public void SetReference(string? reference)
    {
        if (Reference != reference)
        {
            Reference = reference;
            AddDomainEvent(new InvoiceReferenceChanged(Id, Reference));
        }
    }

    public bool VatIncluded { get; set; }

    public decimal SubTotal { get; private set; }

    public double? VatRate { get; set; }

    public decimal Vat { get; set; }

    public decimal Discount { get; set; }

    public List<InvoiceVatAmount> VatAmounts { get; set; } = new List<InvoiceVatAmount>();

    public bool Rounding { get; set; }

    public decimal? Rounded { get; set; }

    public decimal Total { get; private set; }

    public decimal? Paid { get; private set; }

    public BillingDetails? BillingDetails { get; set; }

    public ShippingDetails? ShippingDetails { get; set; }

    public void SetPaid(decimal amount)
    {
        if (Paid != amount)
        {
            Paid = amount;
            AddDomainEvent(new InvoiceAmountPaidChanged(Id, Paid));
        }

        /*
        if(Paid == Total) 
        {
            SetStatus(InvoiceStatus.Paid);
        }
        else if(Paid < Total) 
        {
            SetStatus(InvoiceStatus.PartiallyPaid);
        }
        else if(Paid > Total) 
        {
            SetStatus(InvoiceStatus.Overpaid);
        }
        */
    }

    public string? Note { get; set; }

    public void SetNote(string note)
    {
        if (Note != note)
        {
            Note = note;
            AddDomainEvent(new InvoiceNoteChanged(Id, Note));
        }
    }

    public IReadOnlyList<InvoiceItem> Items => _items;

    public InvoiceItem AddItem(ProductType productType, string description, decimal unitPrice, string unit, double vatRate, double quantity)
    {
        var invoiceItem = new InvoiceItem(this, productType, description, unitPrice, unit, vatRate, quantity);
        _items.Add(invoiceItem);

        Update();

        return invoiceItem;
    }

    public void DeleteItem(InvoiceItem item)
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new Exception();
        }

        _items.Remove(item);

        Update();
    }

    public void Update()
    {
        UpdateVatAmounts();

        //VatRate = 0.25;
        Vat = Items.Sum(x => x.Vat.GetValueOrDefault());
        Total = Items.Sum(x => x.Total);
        SubTotal = Total - Vat;
        Discount = Items.Sum(x => x.Discount.GetValueOrDefault());

        Rounded = null;
        if (Rounding) 
        {
            Rounded = Math.Round(0m, MidpointRounding.AwayFromZero);
        }

        Total -= DomesticService?.RequestedAmount ?? 0;
    }

    private void UpdateVatAmounts()
    {
        VatAmounts.ForEach(x =>
        {
            x.Vat = 0;
            x.SubTotal = 0;
            x.Total = 0;
        });

        foreach (var item in Items)
        {
            item.Update();

            var vatAmount = VatAmounts.FirstOrDefault(x => x.VatRate == item.VatRate);
            if (vatAmount is null)
            {
                vatAmount = new InvoiceVatAmount()
                {
                    VatRate = item.VatRate.GetValueOrDefault(),
                    Name = $"{item.VatRate * 100}%"
                };

                VatAmounts.Add(vatAmount);
            }

            vatAmount.Vat += item.Vat.GetValueOrDefault();
            vatAmount.SubTotal += item.Total - item.Vat.GetValueOrDefault();
            vatAmount.Total += item.Total;
        }

        VatAmounts.ToList().ForEach(x =>
        {
            if (x.Vat == 0 && x.VatRate != 0)
            {
                VatAmounts.Remove(x);
            }
        });
    }

    public InvoiceDomesticService? DomesticService { get; set; }
}

public class InvoiceVatAmount
{
    public double VatRate { get; set; }

    public string Name { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public record InvoiceDomesticService(
    Domain.Entities.DomesticServiceKind Kind, 
    string Buyer,
    string Description,
    decimal RequestedAmount) {
    public PropertyDetails? PropertyDetails { get; set; }
};

public record PropertyDetails(PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);
