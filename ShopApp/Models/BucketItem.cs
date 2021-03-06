﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ShopApp.DAL;
namespace ShopApp.Models
{
    public class BucketItem : IConcurrencyAwareEntity
    {
        public int BucketItemID { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public bool IsChosen { get; set; }
        public virtual Offer Offer { get; set; }
        [JsonIgnore]
        public virtual Bundle Bundle { get; set; }
        [JsonIgnore]
        public virtual Bucket Bucket { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Order { get; set; }
        [JsonIgnore]
        public virtual ICollection<Transaction> Transaction { get; set; }
        [JsonIgnore]
        public byte[] RowVersion { get; set; }
    }
}