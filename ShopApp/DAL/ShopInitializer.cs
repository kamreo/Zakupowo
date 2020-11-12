﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopApp.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Configuration;
using ShopApp.DAL;

namespace ShopApp.DAL
{
    public class ShopInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ShopContext>
    {
        protected override void Seed(ShopContext context)
        {
            var usrs = new List<User>
            {
                new User{ Email="Mail@SeedUsr.com",
                    Login="LoginSeedUsr",
                    EncryptedPassword="PasswwordSeedUsr",
                    FirstName ="Imie",
                    LastName="Nazwisko",
                    Phone ="123456789"
                }
            };
            usrs.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            var Cat = new List<Category>
            {
                new Category{ CategoryName=CategoryEnum.Elektronika,
                    CategoryDescription="Wszystkie te takie z prądem"},
                new Category{ CategoryName=CategoryEnum.ModaIUroda,
                    CategoryDescription="Koszulka z napisem konstytucja"},
                new Category{ CategoryName=CategoryEnum.Meble,
                    CategoryDescription="Szafka prawie wisząca"},
                new Category{ CategoryName=CategoryEnum.SlawekPo2Piwach,
                    CategoryDescription="Byłby po 3 ale bohater oddał jedno koledze"}
            };
            Cat.ForEach(u => context.Categories.Add(u));
            context.SaveChanges();
        }

    }
}