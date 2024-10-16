﻿using Pagos.Domain.Enums;

namespace Pagos.Domain.Entity;

public class Order
{
    public string orderId { get; set; }
    public decimal amount { get; set; }
    public string status { get; set; }
    public string method { get; set; }
    public List<Fee> fees { get; set; }
    public List<Product> products { get; set; }
    public DateTime createdDate { get; set; }
    public string createdBy { get; set; }
}
