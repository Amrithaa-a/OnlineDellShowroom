using OnlineDellShowroom.Error;
using OnlineDellShowroom.Models;
using OnlineDellShowroom.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineDellShowroom.Controllers
{
    public class AdminController : Controller
    {
        UsersignupRepository usersignuprepository = new UsersignupRepository();

        // GET: Admin
        /// <summary>
        /// Action method to display the admin index page.
        /// </summary>
        /// <returns>The view containing website statistics.</returns>
        public ActionResult AdminIndex()
        {
            try
            {
                StatisticsRepository statisticsRepository = new StatisticsRepository();
                WebsiteStatistics statistics = statisticsRepository.GetWebsiteStatistics();
                return View(statistics);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        // GET: User/Details
        /// <summary>
        /// Action method to display the user details.
        /// </summary>
        /// <returns>The view containing the list of user details.</returns>
        public ActionResult UserDetails()
        {
            try
            {
                UsersignupRepository usersignuprepository = new UsersignupRepository();
                var userList = usersignuprepository.GetDetails();
                return View(userList);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the user view with a specific userSignupId.
        /// </summary>
        /// <param name="userSignupId">The ID of the user signup.</param>
        /// <returns>The view containing the user signup details.</returns>
        public ActionResult View(int? userSignupId)
        {
            try
            {
                if (userSignupId == null)
                {
                    return RedirectToAction("UserDetails");
                }

                var userSignup = usersignuprepository.GetUserSignupById(userSignupId.Value);

                if (userSignup == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("UserDetails");
                }

                return View(userSignup);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        // GET: User/ViewById
        /// <summary>
        /// Action method to display the user view with a specific userSignupId.
        /// </summary>
        /// <param name="userSignupId">The ID of the user signup.</param>
        /// <returns>The "View" view containing the user signup details.</returns>
        public ActionResult ViewById(int userSignupId)
        {
            var userSignup = usersignuprepository.GetUserSignupById(userSignupId);
            return View("View", userSignup);
        }

        /// <summary>
        /// Action method to display the user signup details for deletion confirmation.
        /// </summary>
        /// <param name="id">The ID of the user signup to be deleted.</param>
        /// <returns>The view containing the user signup details.</returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var userSignup = usersignuprepository.GetUserSignupById(id);

                if (userSignup == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("UserDetails");
                }

                return View(userSignup);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to handle the user signup deletion confirmation.
        /// </summary>
        /// <param name="id">The ID of the user signup to be deleted.</param>
        /// <returns>Redirects to the UserDetails action if the user is deleted successfully, otherwise returns HttpNotFound().</returns>
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool isDeleted = usersignuprepository.DeleteUser(id);

                if (isDeleted)
                {
                    return RedirectToAction("UserDetails");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user.";
                    return RedirectToAction("UserDetails");
                }
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the view for adding a new laptop.
        /// </summary>
        /// <returns>The view for adding a new laptop with a new Laptop model.</returns>
        public ActionResult AddLaptop()
        {
            return View(new Laptop());
        }

        /// <summary>
        /// Action method to display the list of laptops.
        /// </summary>
        /// <returns>The view containing the list of laptops.</returns>
        [HttpGet]
        public ActionResult Productlist()
        {
            try
            {
                LaptopRepository laptopRepository = new LaptopRepository();
                List<Laptop> laptops = laptopRepository.LaptopDetails();
                return View(laptops);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to handle the form submission for adding a new laptop.
        /// </summary>
        /// <param name="laptop">The Laptop object containing the details of the new laptop.</param>
        /// <returns>
        /// If the model state is valid and the laptop is added successfully, 
        /// redirects to the ProductList action of the Admin controller.
        /// If there are validation errors or the laptop insertion fails, returns the AddLaptop view with appropriate error messages.
        /// </returns>
        [HttpPost]
        public ActionResult AddLaptop(Laptop laptop)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (laptop.ImageFile != null && laptop.ImageFile.ContentLength > 0)
                    {
                        if (!IsImageFile(laptop.ImageFile))
                        {
                            ModelState.AddModelError("ImageFile", "Please select an image file.");
                            return View(laptop);
                        }
                        // Generate a unique file name for the image to prevent overwriting
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(laptop.ImageFile.FileName);
                        string imagePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        // Save the uploaded image to the server
                        laptop.ImageFile.SaveAs(imagePath);
                        // Set the ImageUrl property to the saved image path
                        laptop.ImageUrl = "/Uploads/" + fileName;
                    }

                    LaptopRepository laptopRepository = new LaptopRepository();
                    bool result = laptopRepository.AddLaptopDetails(laptop);

                    if (result)
                    {
                        return RedirectToAction("ProductList", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to add laptop details. Please try again.");
                    }
                }
                return View(laptop);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Checks if the provided file is an image file based on its extension.
        /// </summary>
        /// <param name="file">The HttpPostedFileBase object representing the uploaded file.</param>
        /// <returns>True if the file is an image file (JPEG, JPG, PNG, or GIF), false otherwise.</returns>
        private bool IsImageFile(HttpPostedFileBase file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            return (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif");
        }

        /// <summary>
        /// Action method to display the edit laptop view for a specific laptop.
        /// </summary>
        /// <param name="id">The ID of the laptop to be edited.</param>
        /// <returns>
        /// If the laptop with the given ID is found, returns the view containing the laptop details for editing.
        /// If the laptop is not found, returns an HTTP Not Found error.
        /// </returns>
        public ActionResult EditLaptop(int id)
        {
            try
            {
                LaptopRepository laptopRepository = new LaptopRepository();
                var laptop = laptopRepository.GetLaptopById(id);
                if (laptop == null)
                {
                    return HttpNotFound();
                }
                var viewModel = new EditLaptopViewModel
                {
                    LaptopId = laptop.LaptopId,
                    LaptopName = laptop.LaptopName,
                    Description = laptop.Description,
                    Price = laptop.Price,

                    ExistingImageUrl = laptop.ImageUrl
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to handle the form submission for editing a laptop.
        /// </summary>
        /// <param name="laptop">The Laptop object containing the updated laptop details.</param>
        /// <returns>
        /// If the model state is valid and the laptop is updated successfully, redirects to the ProductList action of the Admin controller.
        /// If there are validation errors or the laptop update fails, returns the EditLaptop view with appropriate error messages.
        /// </returns>
        [HttpPost]
        public ActionResult EditLaptop(EditLaptopViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.NewImageFile != null && viewModel.NewImageFile.ContentLength > 0)
                    {
                        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewImageFile.FileName);
                        string newPath = Path.Combine(Server.MapPath("~/Uploads/"), newFileName);
                        viewModel.NewImageFile.SaveAs(newPath);
                        viewModel.ExistingImageUrl = "/Uploads/" + newFileName;
                    }

                    LaptopRepository laptopRepository = new LaptopRepository();
                    bool result = laptopRepository.UpdateLaptopDetails(viewModel);

                    if (result)
                    {
                        return RedirectToAction("ProductList");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update laptop details. Please try again.");
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the details of a laptop with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to view.</param>
        /// <returns>The view containing the details of the laptop.</returns>
        public ActionResult ViewLaptop(int id)
        {
            try
            {
                LaptopRepository laptopRepository = new LaptopRepository();
                var laptop = laptopRepository.GetLaptopById(id);
                if (laptop == null)
                {
                    return HttpNotFound();
                }

                return View(laptop);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the delete confirmation page for a laptop with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to delete.</param>
        /// <returns>The view containing the details of the laptop to be deleted.</returns>
        public ActionResult DeleteLaptop(int id)
        {
            try
            {
                LaptopRepository laptopRepository = new LaptopRepository();
                var laptop = laptopRepository.GetLaptopById(id);
                if (laptop == null)
                {
                    return HttpNotFound();
                }

                return View(laptop);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to handle the deletion of a laptop with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the laptop to delete.</param>
        /// <returns>Redirection to the ProductList action if the deletion is successful, otherwise HttpNotFound.</returns>
        [HttpPost]
        public ActionResult DeleteLaptopConfirmed(int id)
        {
            LaptopRepository laptopRepository = new LaptopRepository();
            bool result = laptopRepository.DeleteLaptop(id);

            if (result)
            {
                return RedirectToAction("ProductList");
            }
            else
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// Action method to display the view for adding a new admin user.
        /// </summary>
        /// <returns>The view for adding a new admin user.</returns>
        public ActionResult AddAdminUser()
        {
            return View(new AdminUser());
        }

        /// <summary>
        /// Action method to handle the submission of the form for adding a new admin user.
        /// </summary>
        /// <param name="adminUser">The AdminUser object containing the data submitted from the form.</param>
        /// <returns>
        /// If the form data is valid and the username doesn't already exist, redirects to AdminIndex action upon successful insertion.
        /// If the username already exists, returns to the AddAdminUser view with a validation error for the username field.
        /// If ModelState is invalid, returns to the AddAdminUser view with validation errors for the form fields.
        /// If insertion fails, returns to the AddAdminUser view with a general error message.
        /// </returns>
        [HttpPost]
        public ActionResult AddAdminUser(AdminUser adminUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LaptopRepository adminUserRepository = new LaptopRepository();

                    if (adminUserRepository.IsUsernameExists(adminUser.Username))
                    {
                        ModelState.AddModelError("Username", "This username already exists. Please choose a different one.");
                        return View(adminUser);
                    }

                    bool result = adminUserRepository.AddAdminUser(adminUser);

                    if (result)
                    {
                        return RedirectToAction("AdminIndex");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to add admin user. Please try again.");
                    }
                }

                return View(adminUser);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        private readonly StatisticsRepository statisticsRepository = new StatisticsRepository();

        /// <summary>
        /// Action method to display the admin page and fetch the website statistics.
        /// </summary>
        /// <returns>The view containing website statistics.</returns>
        public ActionResult AdminPage()
        {
            WebsiteStatistics websiteStatistics = statisticsRepository.GetWebsiteStatistics();
            return View(websiteStatistics);
        }

        private readonly UsersignupRepository usersignupRepository = new UsersignupRepository();

        /// <summary>
        /// Action method to display the new orders that are pending approval.
        /// </summary>
        /// <returns>The view containing the list of pending orders.</returns>
        [Authorize]
        public ActionResult NewOrders()
        {
            List<OrderView> pendingOrders = usersignupRepository.GetPendingOrders();
            return View(pendingOrders);
        }

        /// <summary>
        /// POST method to approve an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to be approved.</param>
        /// <returns>A JSON result indicating the success status of the order approval.</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ApproveOrder(int orderId)
        {
            bool result = usersignupRepository.ApproveOrder(orderId);
            return Json(new { success = result });
        }
    }
}
