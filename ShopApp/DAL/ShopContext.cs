﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopApp.Models;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.AccessControl;

namespace ShopApp.DAL
{
    public interface IConcurrencyAwareEntity
    {
        byte[] RowVersion { get; set; }
    }
    public class ConcurrencyAwareEntityConvention : Convention
    {
        public ConcurrencyAwareEntityConvention()
        {
            this.Types<IConcurrencyAwareEntity>().Configure(
                config => config.Property(e => e.RowVersion).IsRowVersion()
               );
        }
    }
    public class ShopContext : DbContext
    {
        public ShopContext() : base("ZakupowoDatabase") { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Bundle> Bundles { get; set; }
        public virtual DbSet<BucketItem> BucketItems { get; set; }
        public virtual DbSet<Bucket> Buckets { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<ShippingAdress> ShippingAdresses { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Add(new ConcurrencyAwareEntityConvention());
        }

        public System.Data.Entity.DbSet<ShopApp.Models.AvatarImage> AvatarImages { get; set; }
    }
}