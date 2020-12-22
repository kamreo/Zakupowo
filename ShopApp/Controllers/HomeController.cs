﻿using ShopApp.DAL;
using ShopApp.Models;
using ShopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using ShopApp.Utility;

namespace ShopApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private ShopContext db = new ShopContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Kat(int KatID = 1)//We come here from
        {
            //Filters logic
            var user = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            OffersAndBundles offersAndBundles = new OffersAndBundles();

            var Offers = db.Offers.Where(i => i.Category.CategoryID == KatID && i.IsActive).ToList();

            if (Offers != null)
            {
                var OffersFiltered = Offers
                    .OrderByDescending(i => i.CreationDate)
                    .Take(20)
                    .ToList();
                offersAndBundles.Offers = OffersFiltered;
                if (user != null)
                {
                    var FavouriteOffers = user.FavouriteOffer.Where(i => i.Offer != null).Select(i => i.Offer).ToList();
                    offersAndBundles.FavouriteOffersIDs = FavouriteOffers
                        .Where(i => i.IsActive && OffersFiltered.Contains(i))
                        .Select(i => i.OfferID);
                    if (user.Bucket.BucketItems != null)
                    {
                        offersAndBundles.InBucketOffersIDs = user.Bucket.BucketItems.Where(i => i.Offer != null)
                            .Select(i => i.Offer.OfferID).ToList();
                    }
                }
            }
            else
                ViewBag.Message = "Brak ofert dla podanych filtrów";
            var Bundles = db.Bundles.Where(i => i.Offers.Where(x => x.Category.CategoryID == KatID).Any() && i.IsActive).ToList();

            if (Bundles != null)
            {
                var BundlesFiltered = Bundles.OrderByDescending(i => i.CreationDate)
                                    .Take(20)
                                    .ToList();
                offersAndBundles.Bundles = BundlesFiltered;
                if (user != null)
                {
                    var FavouriteBundles = user.FavouriteOffer.Where(i => i.Bundle != null).Select(i => i.Bundle).ToList();
                    if (FavouriteBundles != null)
                    {
                        offersAndBundles.FavouriteBundlesIDs = FavouriteBundles
                            .Where(i => i.IsActive && BundlesFiltered.Contains(i))
                            .Select(i => i.BundleID);
                        if (user.Bucket.BucketItems != null)
                            offersAndBundles.InBucketBundlesIDs = user.Bucket.BucketItems.Where(i => i.Bundle != null)
                                .Select(i => i.Bundle.BundleID).ToList();
                    }
                }
            }
            else
                ViewBag.Message = "Brak zestawów dla podanych filtrów";
            return View(offersAndBundles);
        }
        [HttpPost]
        public ActionResult Kat(FormCollection collection, int KatID = 1)//We come here from
        {
            //Filters logic
            var user = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            OffersAndBundles offersAndBundles = new OffersAndBundles();

            int OffersAmount = db.Offers.Where(i => i.IsActive && i.Bundle == null).Count();
            int BundlesAmount = db.Bundles.Where(i => i.IsActive).Count();

            offersAndBundles.MaxPage = OffersAmount > BundlesAmount ? (OffersAmount / 20) + 1 : (BundlesAmount / 20) + 1;

            int Page = int.TryParse(collection["page"], out int page) && page > 0 ? page : 1;

            string firstPriceFilterSection = "";
            string secondPriceFilterSection = "";

            if (collection["price"] != null && collection["price"].Contains('-') && (collection["price"].Split('-').Count() == 2))
            {
                firstPriceFilterSection = collection["price"].Split('-')[0];
                secondPriceFilterSection = collection["price"].Split('-')[1];
            }

            int startingPriceFilter = int.TryParse(firstPriceFilterSection, out int startingPrice) ? startingPrice : 1;
            int endingPriceFilter = int.TryParse(secondPriceFilterSection, out int endingPrice) ? endingPrice : 100;


            var Offers = db.Offers.Where(i => i.Category.CategoryID == KatID && i.IsActive).ToList();

            if (Offers != null)
            {
                var OffersFiltered = Offers
                    .Where(i => i.Price > startingPrice && i.Price < endingPrice)
                    .OrderByDescending(i => i.CreationDate)
                    .Skip(20 * (Page - 1))
                    .Take(20)
                    .ToList();
                offersAndBundles.Offers = OffersFiltered;
                if (user != null)
                {
                    var FavouriteOffers = user.FavouriteOffer.Where(i => i.Offer != null).Select(i => i.Offer).ToList();
                    offersAndBundles.FavouriteOffersIDs = FavouriteOffers
                        .Where(i => i.IsActive && OffersFiltered.Contains(i))
                        .Select(i => i.OfferID);
                    if (user.Bucket.BucketItems != null)
                    {
                        offersAndBundles.InBucketOffersIDs = user.Bucket.BucketItems.Where(i => i.Offer != null)
                            .Select(i => i.Offer.OfferID).ToList();
                    }
                }
            }
            else
                ViewBag.Message = "Brak ofert dla podanych filtrów";
            var Bundles = db.Bundles.Where(i => i.Offers.Where(x => x.Category.CategoryID == KatID).Any() && i.IsActive).ToList();
            var BundlesFiltered = Bundles.Where(i => i.BundlePrice > startingPrice && i.BundlePrice < endingPrice)
                                    .OrderByDescending(i => i.CreationDate)
                                    .Skip(20 * (page - 1))
                                    .Take(20)
                                    .ToList();
            if (BundlesFiltered != null)
            {
                offersAndBundles.Bundles = BundlesFiltered;
                if (user != null)
                {
                    var FavouriteBundles = user.FavouriteOffer.Where(i => i.Bundle != null).Select(i => i.Bundle).ToList();
                    if (FavouriteBundles != null)
                    {
                        offersAndBundles.FavouriteBundlesIDs = FavouriteBundles
                            .Where(i => i.IsActive && BundlesFiltered.Contains(i))
                            .Select(i => i.BundleID);
                        if (user.Bucket.BucketItems != null)
                            offersAndBundles.InBucketBundlesIDs = user.Bucket.BucketItems.Where(i => i.Bundle != null)
                                .Select(i => i.Bundle.BundleID).ToList();
                    }
                }
            }
            else
                ViewBag.Message = "Brak zestawów dla podanych filtrów";
            return View(offersAndBundles);
        }

        #region FavouriteOfferManagement
        [HttpPost]
        public async Task<ActionResult> Fav(string type, int id)
        {
            List<string> errors = new List<string>(); // You might want to return an error if something wrong happened

            User User = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();

            if ((type == "Offer" || type == "Bundle") && User != null)
            {
                Favourite Fv = new Favourite();
                if (type == "Offer")
                {
                    Fv.Offer = db.Offers.Where(i => i.OfferID == id).FirstOrDefault();
                    var OfferList = User.FavouriteOffer.Where(i => i.Offer != null).ToList();
                    if (Fv.Offer != null && !OfferList.Where(i => i.Offer.OfferID == Fv.Offer.OfferID).Any())
                    {
                        Fv.User = User;
                        db.Favourites.Add(Fv);
                        Fv.Offer.FavouriteOffer.Add(Fv);
                        Fv.User.FavouriteOffer.Add(Fv);
                        ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
                    }
                }
                else
                {
                    Fv.Bundle = db.Bundles.Where(i => i.BundleID == id).FirstOrDefault();
                    var BundleList = User.FavouriteOffer.Where(i => i.Bundle != null).ToList();
                    if (Fv.Bundle != null && !BundleList.Where(i => i.Bundle.BundleID == Fv.Bundle.BundleID).Any())
                    {
                        Fv.User = User;
                        db.Favourites.Add(Fv);
                        Fv.Bundle.Favourites.Add(Fv);
                        Fv.User.FavouriteOffer.Add(Fv);
                        ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
                    }
                }
            }

            return Json(errors, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> UnFav(string type, int id)
        {
            List<string> errors = new List<string>(); // You might want to return an error if something wrong happened

            User User = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();

            if ((type == "Offer" || type == "Bundle") && User != null)
            {
                if (type == "Offer")
                {
                    Favourite Fv = User.FavouriteOffer.Where(i => i.Offer != null && i.Offer.OfferID == id).FirstOrDefault();

                    if (Fv != null)
                    {
                        User.FavouriteOffer.Remove(Fv);
                        var offer = db.Offers.Where(i => i.OfferID == id).First();
                        offer.FavouriteOffer.Remove(Fv);
                        db.Favourites.Remove(Fv);
                        ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
                    }
                }
                else
                {
                    Favourite Fv = User.FavouriteOffer.Where(i => i.Bundle != null && i.Bundle.BundleID == id).FirstOrDefault();
                    if (Fv != null)
                    {
                        User.FavouriteOffer.Remove(Fv);
                        var offer = db.Bundles.Where(i => i.BundleID == id).First();
                        offer.Favourites.Remove(Fv);
                        db.Favourites.Remove(Fv);
                        ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
                    }
                }
            }

            return Json(errors, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}