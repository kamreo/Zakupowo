﻿using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Security;
using System.Diagnostics;
using System.Web.Security;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Collections.Generic;
using ShopApp.DAL;
using ShopApp.Models;
using ShopApp.ModelsForm;
using ShopApp.Utility;
using ShopApp.ViewModels;
using ShopApp.ViewModels.User;
using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using System.Text;

namespace ShopApp.Controllers
{
	[Authorize]
	public class UserPanelController : Controller
	{
		private ShopContext db = new ShopContext();

		public ActionResult Account()
		{
			User showUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			return View(showUser);
		}

		[HttpPost]
		public ActionResult SendActivationEmail(string userLogin)
		{
			User user = db.Users.Where(u => u.Login == userLogin).FirstOrDefault();

			if (user != null)
			{
				Task.Run(() => EmailManager.SendEmailAsync(EmailManager.EmailType.Registration, user.FirstName, user.LastName, user.Email));
				return Json(new { email = user.Email });
			}
			else
			{
				return Json(false);
			}
		}

		#region Settings  

		public ActionResult Settings()
		{
			User showUser = db.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
			return View(showUser);
		}

		[HttpGet]
		public ActionResult Settings(int? settingPage) // 0 - Basic Data; 1 - Shipping Adresses; 2 - Password; 3 - Avatar
		{
			if (settingPage != null)
			{
				if (settingPage < 0 || settingPage > 3)
					ViewBag.Page = 0;
				else
				{
					ViewBag.Page = settingPage;
					if (settingPage == 2 && TempData["pswMessage"] != null)
					{
						ViewBag.PswMessage = TempData["pswMessage"].ToString();
					}
				}
			}
			else
			{
				ViewBag.Page = 0;
			}

			User showUser = db.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
			return View(showUser);
		}

		[HttpPost]
		public ActionResult EditBasicData(UserBasicData form)
		{
			User editUser = db.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();

			if (editUser != null)
			{
				string changedFirstName = form.FirstName.Trim();
				string changedLastName = form.LastName.Trim();
				string changedEmail = form.Email.Trim();
				string changedPhoneNumber = form.Phone.Trim();


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

			return Json("Pomyślnie zmienione podstawowe dane użytkownika!");
		}

		[HttpPost]
		public ActionResult AddNewShippingAdress(ShippingAdress newAdress)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();

			if (editUser != null && newAdress != null && newAdress.Country != null)
			{
				newAdress.User = editUser;
				editUser.ShippingAdresses.Add(newAdress);
				db.SaveChanges();

				return Json(new
				{
					newAdress.AdressID,
					newAdress.Country,
					newAdress.City,
					newAdress.Street,
					newAdress.PremisesNumber,
					newAdress.PostalCode
				});
			}
			return Json(false);
		}

		[HttpPost]
		public ActionResult EditShippingAdress(ShippingAdress form)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			ShippingAdress shippingAdress;
			var returnInfo = new StringBuilder("");

			if (editUser != null)
			{
				shippingAdress = editUser.ShippingAdresses.Where(a => a.AdressID == form.AdressID).FirstOrDefault();

				if (shippingAdress == null)
					return Json("Nie znaleziono podanego adresu!");

				if (form.Country != null && form.Country.Trim() != shippingAdress.Country)
					shippingAdress.Country = form.Country.Trim();
				else if (form.Country == null)
					returnInfo.AppendLine("Pole \"Country\" jest puste!");

				if (form.City != null && form.City.Trim() != shippingAdress.City)
					shippingAdress.City = form.City.Trim();
				else if (form.City == null)
					returnInfo.AppendLine("Pole \"City\" jest puste!");

				if (form.Street != null && form.Street.Trim() != shippingAdress.Street)
					shippingAdress.Street = form.Street.Trim();
				else if (form.Street == null)
					returnInfo.AppendLine("Pole \"Street\" jest puste!");

				if (form.PremisesNumber != null && form.PremisesNumber.Trim() != shippingAdress.PremisesNumber)
					shippingAdress.PremisesNumber = form.PremisesNumber.Trim();
				else if (form.PostalCode != null && form.PremisesNumber == null)
					shippingAdress.PremisesNumber = null;
				else if (form.PostalCode == null && form.PremisesNumber == null)
					returnInfo.AppendLine("Pole \"PremisesNumber\" nie może być puste gdy pole \"PostalCode\" również jest puste.");


				if (form.PostalCode != null && form.PostalCode.Trim() != shippingAdress.PostalCode)
					shippingAdress.PostalCode = form.PostalCode.Trim();
				else if (form.PostalCode == null && form.PremisesNumber != null)
					shippingAdress.PostalCode = null;
				else if (form.PremisesNumber == null && form.PostalCode == null)
					returnInfo.AppendLine("Pole \"PostalCode\" nie może być puste gdy pole \"PremisesNumber\" również jest puste.");


				if (returnInfo.Length < 5)
				{
					db.SaveChanges();
					returnInfo.AppendLine("Pomyślnie zmieniono adres wysyłki!");
				}
				//db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
			}
			else
				returnInfo.AppendLine("Nie znaleziono odpowiedniego użytkownika!");

			return Json(returnInfo.ToString());
		}

		[HttpPost]
		public ActionResult DeleteShippingAdress(int adressID)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();

			ShippingAdress adressToRemove = editUser.ShippingAdresses.Where(a => a.AdressID == adressID).FirstOrDefault();

			if (adressToRemove != null)
			{
				editUser.ShippingAdresses.Remove(adressToRemove);
				db.ShippingAdresses.Remove(adressToRemove);
				db.SaveChanges();
			}
			else
				return Json("Nie znaleziono takiego adresu!");

			var jsonAdresses = editUser.ShippingAdresses
				.Select(a => new
				{
					a.AdressID, // == AdressID = a.AdressID  
					a.Country,
					a.City,
					a.Street,
					a.PremisesNumber,
					a.PostalCode
				})
				.ToList();

			return Json(jsonAdresses);
		}

		[HttpPost]
		public ActionResult EditPassword(UserPasswordModel form)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();

			Debug.WriteLine("UserPanel");

			if (editUser != null)
			{
				string encryptedOldPassword = Cryptographing.Encrypt(form.OldPassword.Trim());
				string encryptedNewPassword = Cryptographing.Encrypt(form.NewPassword.Trim());
				string encryptedNewPasswordValidation = Cryptographing.Encrypt(form.NewPasswordConfirmation.Trim());

				// If written current password is the same as current password AND written current and new passwords are not NULLs
				if (encryptedOldPassword.Equals(editUser.EncryptedPassword) && encryptedOldPassword != null && encryptedNewPassword != null)
				{
					// If new password validates and is different from old one => CHANGE PASSWORD
					if (encryptedNewPassword.Equals(encryptedNewPasswordValidation) && !encryptedNewPassword.Equals(encryptedOldPassword))
					{
						ViewBag.ConfirmChanges = "Potwierdź link w wysłanym mail'u by zastosować zmianę hasła.";

						Task.Run(() => EmailManager.SendEmailAsync(EmailManager.EmailType.ChangePassword, editUser.FirstName, editUser.LastName, editUser.Email, encryptedNewPassword));
						return Json("Wysłano email z potwierdzeniem zmiany hasła!");
					}
				}
			}


			return Json("Something failed");
		}

		public ActionResult PasswordChange()
		{
			string userEmail = TempData["email"].ToString();
			TempData["pswMessage"] = "Hasło zostało pomyślnie zmienione!";

			User editUser = db.Users.Where(u => u.Email.Equals(userEmail)).FirstOrDefault();

			if (editUser != null)
			{
				string newPassword = TempData["encryptedNewPassword"].ToString();
				newPassword = newPassword.Replace(" ", "+");
				editUser.EncryptedPassword = newPassword;
				db.SaveChanges();

				return RedirectToAction("Settings", new { settingPage = 2 });
			}
			else
				return RedirectToAction("Index", "Home");
		}

		public ActionResult ConfirmPasswordChange(string email, string psw)
		{
			string userEmail = EmailManager.DecryptEmail(email);
			TempData["email"] = userEmail;
			TempData["encryptedNewPassword"] = psw;

			User editUser = db.Users.Where(u => u.Email.Equals(userEmail)).FirstOrDefault();

			if (editUser != null)
				return RedirectToAction("PasswordChange", "UserPanel");
			else
				return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<ActionResult> EditAvatar()
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			HttpPostedFileBase file = null;

			if (Request.Files.Count > 0)
			{
				file = Request.Files[0];
			}
			else
				return Json("There is no image to set as avatar!");


			var imageUrl = await FileManager.UploadAvatar(file, editUser.UserID);

			if (imageUrl != null)
			{
				if (editUser.AvatarImage == null)
				{
					AvatarImage newAvatar = new AvatarImage() { PathToFile = imageUrl, User = editUser };
					db.Entry(newAvatar).State = System.Data.Entity.EntityState.Added;
				}
				else
				{
					editUser.AvatarImage.PathToFile = imageUrl;
					db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
				}
				db.SaveChanges();
				return Json("Pomyślnie zmieniono avatar! \nZmiany mogą być widocznę dopiero po chwili.");
			}

			return Json("FileManager.UploadAvatar return null");
		}

		#endregion

		#region Offers  
		[HttpPost]
		public JsonResult SortOffers(string sortBy)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			List<Offer> offers = editUser.Offers.ToList();

			if (sortBy == null)
				return Json("SortBy is null!");

			switch (sortBy)
			{
				// SORTY BY TITLE
				case "Name-Asc":
					offers = offers.OrderBy(o => o.Title).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Name-Dsc":
					offers = offers.OrderByDescending(o => o.Title).ThenBy(o => o.CreationDate).ToList();
					break;

				//SORT BY STATUS
				case "Status-Asc":
					offers = offers.OrderByDescending(o => o.IsActive).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Status-Dsc":
					offers = offers.OrderBy(o => o.IsActive).ThenBy(o => o.CreationDate).ToList();
					break;

				//SORT BY DATE
				case "Date-Asc":
					offers = offers.OrderByDescending(o => o.CreationDate).ThenBy(o => o.Title).ToList();
					break;

				case "Date-Dsc":
					offers = offers.OrderBy(o => o.CreationDate).ThenBy(o => o.Title).ToList();
					break;

				// SORTY BY PRICE
				case "Price-Asc":
					offers = offers.OrderBy(o => o.Price).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Price-Dsc":
					offers = offers.OrderByDescending(o => o.Price).ThenBy(o => o.CreationDate).ToList();
					break;

				//SORT BY LEFT
				case "Left-Asc":
					offers = offers.OrderBy(o => o.InStockNow).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Left-Dsc":
					offers = offers.OrderByDescending(o => o.InStockNow).ThenBy(o => o.CreationDate).ToList();
					break;

				//SORT BY SOLD
				case "Sold-Asc":
					offers = offers.OrderBy(o => o.InStockOriginaly - o.InStockNow).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Sold-Dsc":
					offers = offers.OrderByDescending(o => o.InStockOriginaly - o.InStockNow).ThenBy(o => o.CreationDate).ToList();
					break;

				//SORT BY CATEGORY
				case "Category-Asc":
					offers = offers.OrderBy(o => o.Category.CategoryName).ThenBy(o => o.CreationDate).ToList();
					break;

				case "Category-Dsc":
					offers = offers.OrderByDescending(o => o.Category.CategoryName).ThenBy(o => o.CreationDate).ToList();
					break;

				default:
					offers = offers.OrderBy(o => o.OfferID).ToList();
					break;
			}

			var jsonOffers = offers
				.Select(o => new
				{
					OfferID = o.OfferID,     // Can be written as just "o.OfferID"
					Title = o.Title,         // Can be written as just "o.Title"
					Status = o.IsActive,
					CreationDate = o.CreationDate.ToShortDateString(),
					LongCreationDate = o.CreationDate.ToString(),
					Price = o.Price,         // Can be written as just "o.Price"
					Left = o.InStockNow,
					Sold = o.InStockOriginaly - o.InStockNow,
					Category = o.Category.CategoryName
				})
				.ToList();

			return Json(jsonOffers);
		}

		public ActionResult Offers(bool? userActivated, bool? success, int? offerID)
		{
			if (userActivated != null && !(bool)userActivated)
			{
				ViewBag.UserNotActivated = "Musisz aktywować swoje konto by móc wystawiać oferty!";
			}

			if (success != null && (bool)success)
			{
				ViewBag.Success = true;
			}

			if (success != null && !(bool)success)
			{
				ViewBag.Success = false;
				if (offerID != null)
				{
					Offer offerToRemove = db.Offers.Where(o => o.OfferID == offerID).FirstOrDefault();
					if (offerToRemove != null)
					{
						db.Offers.Remove(offerToRemove);
						ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
					}

				}
			}

			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			List<Offer> offers = editUser.Offers.ToList();

			offers = offers.OrderBy(o => o.OfferID).ToList();

			return View(offers);
		}

		public ActionResult AddOffer(bool? success)
		{
			User editUser = db.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();

			if (!editUser.IsActivated)
			{
				return RedirectToAction("Offers", "UserPanel", new { userActivated = false });
			}

			if (success != null && !(bool)success)
			{
				ViewBag.Success = "Oferta nie została utworzona.";
			}

			List<Category> categoryList = db.Categories.ToList();

			return View(categoryList);
		}

		[HttpPost]
		public async Task<ActionResult> AddOffer(FormCollection collection)
		{
			List<OfferPicture> pictures = new List<OfferPicture>();

			User editUser = db.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();

			int categoryID = int.Parse(collection["Category"]);
			Category offerCategory = db.Categories.Where(o => o.CategoryID == categoryID).FirstOrDefault();

			string priceWithDot = collection["Price"].Replace(',', '.');

			if (!double.TryParse(priceWithDot, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double priceDouble) && priceDouble > 0)
			{
				return RedirectToAction("AddOffer", "UserPanel", new { success = false });
			}

			if (!int.TryParse(collection["State"], out int offerStateInt))
			{
				return RedirectToAction("AddOffer", "UserPanel", new { success = false });
			}


			Offer offer = new Offer
			{
				Title = collection["Name"],
				Description = collection["Description"],
				InStockOriginaly = Convert.ToDouble(collection["Quantity"]),
				Price = priceDouble,
				OfferState = (OfferState)offerStateInt,
				Category = offerCategory,
				User = editUser,
				IsActive = true,
				CreationDate = DateTime.Now
			};
			offer.InStockNow = offer.InStockOriginaly;

			db.Offers.Add(offer);
			db.SaveChanges();

			HttpFileCollectionBase filesForOffer = null;
			if (TempData["offerImages"] != null)
				filesForOffer = (HttpFileCollectionBase)TempData["offerImages"];

			if (filesForOffer != null && filesForOffer.Count > 0 && offer != null)
			{
				var files = filesForOffer;

				if (files != null && files.Count > 0)
				{
					try
					{
						for (int i = 0; i < files.Count; i++)
						{
							var workFile = files[i];

							var fileUrl = await FileManager.UploadOfferImage(workFile, offer.OfferID, i);

							if (fileUrl != null)
							{
								OfferPicture offerPicture = new OfferPicture() { PathToFile = fileUrl, Offer = offer };
								pictures.Add(offerPicture);
							}
							else
							{
								return RedirectToAction("Offers", "UserPanel", new { success = false, offerID = offer.OfferID });
							}
						}
					}
					catch (Exception ex)
					{
						ViewBag.Error = "Wystąpił błąd: " + ex.Message.ToString();
						return RedirectToAction("Offers", "UserPanel", new { success = false, offerID = offer.OfferID });
					}
				}
				else
				{
					return RedirectToAction("Offers", "UserPanel", new { success = false, offerID = offer.OfferID });
				}

				if (ViewBag.Error == null)
				{
					offer.OfferPictures = pictures;
					db.SaveChanges();
				}
			}
			else
			{
				ViewBag.Error = "Wystąpiły problemy ze zdjęciami";
				return RedirectToAction("Offers", "UserPanel", new { success = false, offerID = offer.OfferID });
			}

			if (offer.OfferPictures.Count == 0)
			{
				return RedirectToAction("Offers", "UserPanel", new { success = false, offerID = offer.OfferID });
			}


			if (ViewBag.Error == null)
			{
				offer.Category.Offers.Add(offer);
				db.SaveChanges();

				editUser.Offers.Add(offer);
				db.SaveChanges();

				return RedirectToAction("Offers", "UserPanel", new { success = true });
			}
			else
			{
				return RedirectToAction("AddOffer", "UserPanel", new { success = false, offerID = offer.OfferID });
			}
		}

		// DO NOT REMOVE TASK<> -> IT MAKES METHOD "UploadOfferImages" RUNS BEFORE "AddOffer(FormCollection collection)"
		public async Task<JsonResult> UploadOfferImages()
		{
			TempData["offerImages"] = Request.Files;
			return Json("Moved files to AddOffer method.");
		}

		public ActionResult DeactivateOffer(int offerID)
		{
			bool result = true;
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			Offer offerToDeactivate = db.Offers.Where(o => o.OfferID == offerID).FirstOrDefault();

			if (offerToDeactivate == null)
				result = false;
			else
			{
				offerToDeactivate.IsActive = false;
				ConcurencyHandling.SaveChangesWithConcurencyHandling(db);

				if (offerToDeactivate.Bundle != null)
				{
					offerToDeactivate.Bundle.IsActive = false;
					ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
				}
			}

			return Json(result);
		}

		#endregion

		#region Bundles  
		[HttpPost]
		public JsonResult SortBundles(string sortBy)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			List<Bundle> userBundles = editUser.Bundles.ToList();

			if (sortBy == null)
				return Json("SortBy is null!");

			switch (sortBy)
			{
				// SORTY BY TITLE
				case "Name-Asc":
					userBundles = userBundles.OrderBy(o => o.Title).ToList();
					break;

				case "Name-Dsc":
					userBundles = userBundles.OrderByDescending(o => o.Title).ToList();
					break;

				// SORTY BY PRICE
				case "Price-Asc":
					userBundles = userBundles.OrderBy(o => o.BundlePrice).ToList();
					break;

				case "Price-Dsc":
					userBundles = userBundles.OrderByDescending(o => o.BundlePrice).ToList();
					break;

				//SORT BY STATUS
				case "Status-Asc":
					userBundles = userBundles.OrderByDescending(o => o.IsActive).ThenBy(o => o.Title).ToList();
					break;

				case "Status-Dsc":
					userBundles = userBundles.OrderBy(o => o.IsActive).ThenBy(o => o.Title).ToList();
					break;

				//SORT BY DATE
				case "Date-Asc":
					userBundles = userBundles.OrderByDescending(o => o.CreationDate).ToList();
					break;

				case "Date-Dsc":
					userBundles = userBundles.OrderBy(o => o.CreationDate).ToList();
					break;

				default:
					userBundles = userBundles.OrderBy(o => o.BundleID).ToList();
					break;
			}

			var jsonBundles = userBundles
				.Select(b => new
				{
					BundleID = b.BundleID,          // Can be written as just "b.BundleID"
					Title = b.Title,                // Can be written as just "b.Title"
					Status = b.IsActive,
					CreationDate = b.CreationDate.ToShortDateString(),
					LongCreationDate = b.CreationDate.ToString(),
					BundlePrice = b.BundlePrice,    // Can be written as just "b.BundlePrice"
				})
				.ToList();

			return Json(jsonBundles);
		}

		public ActionResult Bundles(bool? availableOffers, bool? addedBundle, int? bundleID)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();

			List<Bundle> userBundles = editUser.Bundles.ToList();

			if (availableOffers != null && availableOffers == false)
			{
				ViewBag.NoOffers = "Brak dostępnych ofert do stworzenia zestawu!";
			}

			if (addedBundle != null && (bool)addedBundle)
			{
				Debug.WriteLine("Dodano!");
				ViewBag.Success = "Zestaw stworzony pomyślnie!";
			}

			if (addedBundle != null && !(bool)addedBundle)
			{
				var newBundle = db.Bundles.Where(b => b.BundleID == bundleID).FirstOrDefault();
				if (newBundle != null)
				{
					db.Bundles.Remove(newBundle);
					db.SaveChanges();
					ViewBag.Success = "Zestaw stworzony pomyślnie!";

				}
			}

			userBundles = userBundles.OrderBy(o => o.BundleID).ToList();

			return View(userBundles);
		}

		public ActionResult AddBundle(bool? noPickedOffers)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			List<Offer> userOffers = editUser.Offers.Where(o => o.IsActive == true && o.InStockNow > 0 && o.Bundle == null).ToList();

			if (noPickedOffers != null && noPickedOffers == true)
			{
				ViewBag.NoPickedOffers = "Musisz wybrać przynajmniej jedną ofertę by stworzyć zestaw!";
			}


			if (userOffers.Count > 0)
				return View(userOffers);
			else
			{
				return RedirectToAction("Bundles", "UserPanel", new { availableOffers = false });
			}
		}

		[HttpPost]
		public async Task<ActionResult> AddBundle(FormCollection collection)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();

			double offersPriceSum = 0.0;
			double bundlePrice = 0.0;
			List<Offer> bundleOffers = new List<Offer>();

			if (collection["OffersInBundle"] == null)
			{
				return RedirectToAction("AddBundle", "UserPanel", new { noPickedOffers = true });
			}

			string[] offerIDs = collection["OffersInBundle"].Split(',');
			foreach (var offerID in offerIDs)
			{
				if (int.TryParse(offerID, out int ID))
				{
					Offer offer = db.Offers.Where(o => o.OfferID == ID).FirstOrDefault();
					bundleOffers.Add(offer);

					offersPriceSum += offer.Price;
				}
			}

			if (collection["RadioDiscount"] == "CurrencyDiscount")
			{
				string discountWithDot = collection["CurrencyDiscountValue"].Replace(',', '.');
				if (double.TryParse(discountWithDot, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double discount))
				{
					bundlePrice = offersPriceSum - discount;
					if (bundlePrice <= 0)
					{
						return RedirectToAction("Bundles", "UserPanel", new { addedBundle = false });
					}
				}
				else
				{
					ViewBag.Error = "Cant parse discount to double";
				}
			}
			else if (collection["RadioDiscount"] == "PercentDiscount")
			{
				if (int.TryParse(collection["PercentDiscountValue"], out int discount))
				{
					bundlePrice = Math.Round(offersPriceSum - offersPriceSum * discount / 100, 2);
				}
				else
				{
					ViewBag.Error = "Cant parse discount to int";
				}
			}
			else
			{
				bundlePrice = offersPriceSum;
			}

			Bundle newBundle = new Bundle()
			{
				Title = collection["BundleTitle"],
				Offers = bundleOffers,
				OffersPriceSum = offersPriceSum,
				BundlePrice = bundlePrice,
				CreationDate = DateTime.Now,
				IsActive = true,
				User = editUser
			};

			db.Bundles.Add(newBundle);
			db.SaveChanges();


			if (Request.Files.Count > 0)
			{
				var bundleFile = Request.Files[0];

				if (bundleFile != null)
				{
					try
					{
						var fileUrl = await FileManager.UploadBundlePicture(bundleFile, newBundle.BundleID);

						if (fileUrl != null)
						{
							BundlePicture bundlePicture = new BundlePicture() { PathToFile = fileUrl, Bundle = newBundle };
							newBundle.BundlePicture = bundlePicture;
							db.SaveChanges();
						}
						else
						{
							return RedirectToAction("Bundles", "UserPanel", new { addedBundle = false, bundleID = newBundle.BundleID });
						}
					}
					catch (Exception ex)
					{
						ViewBag.Error = "Wystąpił błąd: " + ex.Message.ToString();
						return RedirectToAction("Bundles", "UserPanel", new { addedBundle = false, bundleID = newBundle.BundleID });
					}
				}
			}

			if (ViewBag.Error == null)
			{
				editUser.Bundles.Add(newBundle);
				db.SaveChanges();
				return RedirectToAction("Bundles", "UserPanel", new { addedBundle = true });
			}
			else
			{
				return RedirectToAction("Bundles", "UserPanel", new { addedBundle = false, bundleID = newBundle.BundleID });
			}

		}

		public ActionResult DeactivateBundle(int bundleID)
		{
			bool result = true;

			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			Bundle bundleToDeactivate = db.Bundles.Where(b => b.BundleID == bundleID).FirstOrDefault();

			if (bundleToDeactivate == null)
				result = false;
			else
			{
				bundleToDeactivate.IsActive = false;
				ConcurencyHandling.SaveChangesWithConcurencyHandling(db);

				foreach (var offer in bundleToDeactivate.Offers)
				{
					offer.Bundle = null;
				}
				ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
			}

			return Json(result);
		}

		#endregion

		#region Communicator

		public ActionResult Communicator()
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			List<Message> lastMessages = new List<Message>();
			HashSet<User> uniqueUsers = new HashSet<User>();

			List<List<Message>> allMessages = new List<List<Message>>();

			var lastReceivedMessages = editUser.ReceivedMessages.OrderByDescending(m => m.SentTime).DistinctBy(m => m.Sender).ToList();
			var lastSentMessages = editUser.SentMessages.OrderByDescending(m => m.SentTime).DistinctBy(m => m.Receiver).ToList();

			foreach (var item in lastReceivedMessages)
			{
				uniqueUsers.Add(item.Sender);
			}

			foreach (var item in lastSentMessages)
			{
				uniqueUsers.Add(item.Receiver);
			}
			uniqueUsers.Remove(editUser);


			foreach (var user in uniqueUsers)
			{
				allMessages.Add(editUser.AllMesseges().Where(m => m.Receiver == user || m.Sender == user).OrderBy(m => m.SentTime).ToList());
			}

			foreach (var massageList in allMessages)
			{
				lastMessages.Add(massageList.Last());
			}

			ViewBag.AllMessages = allMessages;

			if (lastMessages == null || lastMessages.Count == 0)
				ViewBag.LackMessages = true;
			else
				lastMessages.Sort();

			return View(lastMessages);
		}

		[HttpPost]
		public ActionResult ReadMessages(int senderID)
		{
			User editUser = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).First();
			User sender = db.Users.Where(i => i.UserID == senderID).First();

			if (editUser != null && sender != null)
			{
				List<Message> unreadMessages = editUser.ReceivedMessages.Where(m => m.Sender == sender && m.IsRead == false).ToList();

				foreach (var msg in unreadMessages)
				{
					msg.IsRead = true;
					db.SaveChanges();
				}

				return Json(true);
			}

			return Json(false);
		}

		[HttpPost]
		public JsonResult UsersList(string userLogin) //RETURN 10 USERS
		{
			Debug.WriteLine("Szukam: " + userLogin);
			List<User> users = db.Users.Where(u => u.Login.Contains(userLogin) && u.Login != HttpContext.User.Identity.Name).Take(10).ToList();

			if (userLogin == null)
				return Json("userLogin is null!");

			Debug.WriteLine("users: " + users.Count);


			var jsonUsers = users
				.Select(u => new
				{
					u.Login,
					u.FirstName,
					// LastName = u.LastName,
					AvatarUrl = u.AvatarImage.PathToFile
				})
				.ToList();

			return Json(jsonUsers);
		}

		[HttpPost]
		public ActionResult GetUserIdFromName(string userLogin)
		{
			User user = db.Users.Where(u => u.Login == userLogin).FirstOrDefault();

			if (user != null && user.AvatarImage != null)
			{
				return Json(new { userID = user.UserID, userAvatarURL = user.AvatarImage.PathToFile });
			}
			else
			{
				return Json(false);
			}
		}

		#endregion

		#region Transaction
		public ActionResult Transactions()
		{
			var user = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();

			if (user != null)
			{
				var sold = user.SoldTransactions.OrderByDescending(t => t.CreationDate).ToList();
				var bought = user.BoughtTransactions.OrderByDescending(t => t.CreationDate).ToList();

				if (sold != null && bought != null)
				{
					OrderHistoryViewModel OHVM = new OrderHistoryViewModel()
					{
						Sold = sold,
						Bought = bought
					};
					if (OHVM != null)
						return View(OHVM);
					else
						return new HttpStatusCodeResult(500);
				}
				else
					return new HttpStatusCodeResult(500);
			}
			else
				return new HttpStatusCodeResult(404);
		}

		[HttpPost]
		public ActionResult ManageTransaction(int transactionID, bool isAccepted)
		{
			Transaction managedTransaction = db.Transactions.Where(t => t.TransactionID == transactionID).FirstOrDefault();
			User user = db.Users.Where(i => i.Login == HttpContext.User.Identity.Name).FirstOrDefault();
			StringBuilder returnInfo = new StringBuilder();

			if (managedTransaction != null
				&& user != null
				&& user.SoldTransactions.Contains(managedTransaction)
				&& managedTransaction.IsChosen == false)
			{
				managedTransaction.IsChosen = true;
				managedTransaction.IsAccepted = isAccepted;
				ConcurencyHandling.SaveChangesWithConcurencyHandling(db);

				if (isAccepted)
				{
					foreach (var bucketItem in managedTransaction.BucketItems)
					{
						if (bucketItem.Offer != null)
						{
							if (!DecrementProductFromOffer(bucketItem.Offer, bucketItem.Quantity))
							{
								returnInfo.AppendLine("Brak produktu: " + bucketItem.Offer.Title);
							}
						}
						else if (bucketItem.Bundle != null)
						{
							foreach (var offer in bucketItem.Bundle.Offers)
							{
								if (!DecrementProductFromOffer(offer, 1))
								{
									returnInfo.AppendLine("Brak produktu: " + offer.Title);
								}
							}
						}
					}
				}

				return Json(returnInfo.ToString());
			}

			return Json("ERROR");
		}

		private bool DecrementProductFromOffer(Offer offer, double quantity)
		{
			double restInSctock = offer.InStockNow - quantity;
			if (restInSctock > 1 && quantity <= offer.InStockNow)
			{
				offer.InStockNow -= quantity;
			}
			else if (restInSctock == 0)
			{
				offer.InStockNow = 0;
				offer.IsActive = false;

				if (offer.Bundle != null)
				{
					offer.Bundle.IsActive = false;
					foreach (Offer offerInBundle in offer.Bundle.Offers)
					{
						offerInBundle.IsActive = false;
					}
				}
			}
			else
			{
				return false;
			}
			ConcurencyHandling.SaveChangesWithConcurencyHandling(db);
			return true;
		}

		#endregion
	}
}
