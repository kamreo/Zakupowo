﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Testowanie.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
    }
}