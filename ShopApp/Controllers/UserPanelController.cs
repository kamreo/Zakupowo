﻿using ShopApp.Models;
using ShopApp.ViewModels;
using ShopApp.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopApp.DAL;
using Microsoft.Ajax.Utilities;
using Antlr.Runtime.Tree;
using ShopApp.Utility;
using System.Diagnostics;
using System.IO;

namespace ShopApp.Controllers
{
    public class UserPanelController : Controller
    {
        private ShopContext db = new ShopContext();

        #region UserData 

        // VIEW WITH BASIC INFORMATION ABOUT USER
        public ActionResult Account()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User showUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            return View(showUser);
        }

        // VIEW WHERE USER CAN EDIT *BASIC* INFORMATION
        public ActionResult EditBasicInfo()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User showUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            return View(showUser);
        }

        [HttpPost]
        public ActionResult EditBasicInfo(FormCollection collection)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            if (editUser != null)
            {
                string changedFirstName = collection["FirstName"].Trim();
                string changedLastName = collection["LastName"].Trim();
                string changedEmail = collection["Email"].Trim();
                string changedPhoneNumber = collection["Phone"].Trim();


                if (changedFirstName != editUser.FirstName && changedFirstName != null)
                    editUser.FirstName = changedFirstName;

                if (changedLastName != editUser.LastName && changedLastName != null)
                    editUser.LastName = changedLastName;

                if (changedEmail != editUser.Email && changedEmail != null)
                    editUser.Email = changedEmail;

                if (changedPhoneNumber != editUser.Phone && changedPhoneNumber != null)
                    editUser.Phone = changedPhoneNumber;

                db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("EditBasicInfo", "UserPanel");
        }

        // VIEW WHERE USER CAN EDIT SHIPPING ADRESSES
        public ActionResult ShippingAdresses()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User showUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            return View(showUser);
        }

        [HttpPost]
        public ActionResult ShippingAdresses(FormCollection collection)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();
            ShippingAdress shippingAdress;

            if (editUser != null)
            {
                int adressNumber = int.Parse(collection["AdressNumber"].Trim());
                shippingAdress = editUser.ShippingAdresses.ToList()[adressNumber];

                string changedCountry = collection["Country"].Trim();
                string changedCity = collection["City"].Trim();
                string changedStreet = collection["Street"].Trim();
                string changedPremisesNumber = collection["PremisesNumber"].Trim();
                string changedPostalCode = collection["PostalCode"].Trim();

                if (changedCountry != shippingAdress.Country && changedCountry != null)
                    shippingAdress.Country = changedCountry;

                if (changedCity != shippingAdress.City && changedCity != null)
                    shippingAdress.City = changedCity;

                if (changedStreet != shippingAdress.Street && changedStreet != null)
                    shippingAdress.Street = changedStreet;

                if (changedPremisesNumber != shippingAdress.PremisesNumber && changedPremisesNumber != null)
                    shippingAdress.PremisesNumber = changedPremisesNumber;

                if (changedPostalCode != shippingAdress.PostalCode && changedPostalCode != null)
                    shippingAdress.PostalCode = changedPostalCode;


                editUser.ShippingAdresses.ToList()[adressNumber] = shippingAdress;

                db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ShippingAdresses", "UserPanel");
        }

        public ActionResult AddShippingAdress()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddShippingAdress(FormCollection collection)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            string country = collection["Country"];
            string city = collection["city"];
            string street = collection["street"];
            string premisesNumber = collection["PremisesNumber"];
            string postalCode = collection["PostalCode"];

            ShippingAdress adress = new ShippingAdress
            {
                Country = country,
                City = city,
                Street = street,
                PremisesNumber = premisesNumber,
                PostalCode = postalCode,
                User = editUser
            };

            //using (var dataBase = new ShopContext())
            //{

            //    dataBase.Entry(adress).State = System.Data.Entity.EntityState.Modified;
            //    dataBase.Entry(editUser).State = System.Data.Entity.EntityState.Modified;

            //    editUser.ShippingAdresses.Add(adress);
            //    dataBase.SaveChanges();


            //    dataBase.ShippingAdresses.Add(adress);
            //    dataBase.SaveChanges();
            //}

            db.ShippingAdresses.Add(adress);
            db.SaveChanges();

            editUser.ShippingAdresses.Add(adress);
            db.SaveChanges();

            return RedirectToAction("ShippingAdresses", "UserPanel");
        }

        public ActionResult DeleteShippingAdress(int? adressNumber)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (adressNumber == null)
                return RedirectToAction("ShippingAdresses", "UserPanel");


            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            List<ShippingAdress> lista = db.ShippingAdresses.Where(u => u.User.UserID == userID).ToList();

            ShippingAdress adressToRemove = db.ShippingAdresses.Where(u => u.User.UserID == userID).ToList()[(int)adressNumber];

            db.ShippingAdresses.Remove(adressToRemove);
            db.SaveChanges();


            return RedirectToAction("ShippingAdresses", "UserPanel");
        }


        // VIEW WHERE USER CAN EDIT PASSWORD
        public ActionResult EditPassword()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User showUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            return View(showUser);
        }

        [HttpPost]
        public ActionResult EditPassword(FormCollection collection)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            if (editUser != null)
            {
                string encryptedOldPassword = Cryptographing.Encrypt(collection["OldPassword"].Trim());
                string encryptedNewPassword = Cryptographing.Encrypt(collection["NewPassword"].Trim());
                string encryptedNewPasswordValidation = Cryptographing.Encrypt(collection["NewPasswordValidation"].Trim());

                if (encryptedOldPassword != editUser.EncryptedPassword && encryptedOldPassword != null)
                {
                    if (encryptedNewPassword.Equals(encryptedNewPasswordValidation))
                    {
                        editUser.EncryptedPassword = encryptedNewPassword;

                        db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Account", "UserPanel");
        }

        // VIEW WHERE USER CAN EDIT PASSWORD
        public ActionResult EditAvatar()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User showUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            return View(showUser);
        }

        [HttpPost]
        public ActionResult EditAvatar(HttpPostedFileBase file)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();

            string[] validExtensions = new string[] { "jpg", "png", "jpeg" };

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string folderLoadPath = @"../../App_Files/Images/UserAvatars/";
                    string folderSavePath = @"~/App_Files/Images/UserAvatars/";
                    string fileExtenstion = file.FileName.Substring(file.FileName.IndexOf('.') + 1);
                    string fileName = "Avatar_" + editUser.UserID + "." + fileExtenstion;

                    if (validExtensions.Contains(fileExtenstion))
                    {
                        string oldPath = string.Empty;
                        if (editUser.AvatarImage != null)
                            oldPath = editUser.AvatarImage.PathToFile;

                        string path = Path.Combine(Server.MapPath(folderSavePath), Path.GetFileName(fileName));

                        if (oldPath != path)
                        {
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        file.SaveAs(path);


                        AvatarImage newAvatar = new AvatarImage() { PathToFile = folderLoadPath + fileName, User = editUser };

                        db.Entry(newAvatar).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();

                        editUser.AvatarImage = newAvatar;
                        db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.Message = "File uploaded successfully";
                    }
                    else
                    {
                        throw new Exception("The file extension is invalid!");
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }


            return RedirectToAction("EditAvatar", "UserPanel");
        }

        #endregion

        public ActionResult AddOffer()
        {
            List<Category> categoryList = db.Categories.ToList();


            return View(categoryList);
        }

        [HttpPost]
        public ActionResult AddOffer(FormCollection collection)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userID = (int)Session["userId"];
            User editUser = db.Users.Where(u => u.UserID == userID).FirstOrDefault();
            var files = Request.Files;
            string[] validExtensions = new string[] { "jpg", "png", "jpeg" };

            Offer offer = new Offer
            {
                Title = collection["Name"],
                Description = collection["Description"],
                InStock = Convert.ToDouble(collection["Quantity"]),
                Price = Convert.ToDouble(collection["Price"]),
                //Category = collection["Category"],
                User = editUser
            };

            db.Entry(offer).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();

            if (offer == null)
                Debug.WriteLine("NULL OFERTY");

            List<OfferPicture> pictures = new List<OfferPicture>();

            if (files != null && files.Count > 0)
            {
                try
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var workFile = files[i];
                        string folderLoadPath = @"../../App_Files/Images/OfferPictures/";
                        string folderSavePath = @"~/App_Files/Images/OfferPictures/";
                        string fileExtension = workFile.FileName.Substring(workFile.FileName.IndexOf('.') + 1);
                        string fileName = "Offer_" + offer.OfferID + "_PictureNo_" + i + "." + fileExtension;

                        if (validExtensions.Contains(fileExtension))
                        {
                            string path = Path.Combine(Server.MapPath(folderSavePath), Path.GetFileName(fileName));

                            workFile.SaveAs(path);

                            OfferPicture offerPicture = new OfferPicture() { PathToFile = folderLoadPath + fileName, Offer = offer };
                            //AvatarImage newAvatar = new AvatarImage() { PathToFile = folderLoadPath + fileName, User = editUser };
                            pictures.Add(offerPicture);

                            //offer.listazdjec.add(offerImage);
                            //db.Entry(offer).State = System.Data.Entity.EntityState.Modified;

                            ViewBag.Message = "File uploaded successfully";
                        }
                        else
                        {
                            throw new Exception("The file extension is invalid!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            offer.OfferPictures = pictures;
            db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            editUser.Offers.Add(offer);
            db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            for (int i = 0; i < offer.OfferPictures.Count; i++)
            {
                Debug.WriteLine(offer.OfferPictures.ToList()[i].PathToFile);
            }

            ////Adds new offer in offer table
            //db.Offers.Add(offer);
            //db.SaveChanges();

            ////Adds the offer to offersList in User
            //editUser.Offers.Add(offer);
            //db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult OrderHistory()
        {
            return View();
        }

        public ActionResult Communicator()
        {
            return View();
        }
    }
}
