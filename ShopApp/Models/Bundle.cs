﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace ShopApp.Models
{
    public class Bundle
    {
        [Key]
        public int BundleID { get; set; }
        [Required]
        public double BundlePriceSum { get; set; }
        [Required]
        public string Title { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual User User { get; set; }
    }
}