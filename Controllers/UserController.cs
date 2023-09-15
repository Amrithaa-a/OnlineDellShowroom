using OnlineDellShowroom.Error;
using OnlineDellShowroom.Models;
using OnlineDellShowroom.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using static OnlineDellShowroom.Repository.UsersignupRepository;

namespace OnlineDellShowroom.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// Action method to display the list of laptops for the index page.
        /// </summary>
        /// <returns>The view containing the list of laptops.</returns>
        public ActionResult Index()
        {
            LaptopRepository laptopRepository = new LaptopRepository();
            List<Laptop> laptops = laptopRepository.LaptopDetails();
            return View(laptops);
        }

        /// <summary>
        /// Action method to display the admin index page.
        /// </summary>
        /// <returns>The view for the admin index page.</returns>
        public ActionResult AdminIndex()
        {
            return View();
        }

        /// <summary>
        /// Action method to display the user signin page.
        /// </summary>
        /// <returns>The view for the user signin page.</returns>
        public ActionResult UserSignin()
        {
            return View();
        }

        /// <summary>
        /// Action method to display the user signup page with states and cities dropdown lists.
        /// </summary>
        /// <returns>The view for the user signup page with dropdown lists.</returns>
        public ActionResult UserSignup()
        {
            UserSignup userSignup = new UserSignup();
            userSignup.States = usersignupRepository.GetStates();
            userSignup.Cities = new List<SelectListItem>(); 
            return View(userSignup);
        }

        /// <summary>
        /// Action method to handle the user signup form submission.
        /// </summary>
        /// <param name="userSignup">The UserSignup object containing the user's signup data.</param>
        /// <returns>
        /// - If signup is successful, redirects to the sign-in page with a success message.
        /// - If signup fails, returns the UserSignup view with validation errors and populated dropdown lists.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserSignup(UserSignup userSignup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (usersignupRepository.IsUsernameExists(userSignup.Username))
                    {
                        ModelState.AddModelError("Username", "This username already exists. Please choose a different one.");
                    }

                    if (usersignupRepository.IsMobileNumberExists(userSignup.Mobilenumber))
                    {
                        ModelState.AddModelError("MobileNumber", "Mobile number already exists.");
                        return View("UserSignup", userSignup);
                    }
                    if (usersignupRepository.IsEmailExists(userSignup.Email))
                    {
                        ModelState.AddModelError("Email", "Email address already exists. Please use a different email.");
                        return View("UserSignup", userSignup);
                    }
                    bool result = usersignupRepository.SignUp(userSignup);
                    if (result)
                    {
                        TempData["SignUpSuccess"] = "User account created successfully. Please sign in with your credentials.";
                        return RedirectToAction("UserSignin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create user. Please try again.");
                    }
                }
                userSignup.States = usersignupRepository.GetStates();
                userSignup.Cities = usersignupRepository.GetCitiesByState(userSignup.StateId);
                return View(userSignup);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to fetch cities based on the selected stateId.
        /// </summary>
        /// <param name="stateId">The selected StateId.</param>
        /// <returns>The cities as JSON data.</returns>
        public ActionResult GetCitiesByState(int stateId)
        {
            List<SelectListItem> cities = usersignupRepository.GetCitiesByState(stateId);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action method to display the view for deleting a user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user to be deleted.</param>
        /// <returns>The view for deleting a user.</returns>
        public ActionResult Delete(int id)
        {
            return View();
        }

        /// <summary>
        /// Action method to handle the user sign-in form submission.
        /// </summary>
        /// <param name="usersignin">The UserSignin object containing the user's sign-in data.</param>
        /// <returns>
        /// - If authentication is successful, redirects to the user home page or admin home page based on user role.
        /// - If authentication fails, returns the UserSignin view with an error message.
        /// </returns>
        [HttpPost]
        public ActionResult UserSignin(UserSignin usersignin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsersignupRepository repository = new UsersignupRepository();
                    bool isUser = repository.AuthenticateUser(usersignin.Username, usersignin.Password);
                    bool isAdmin = repository.AuthenticateAdmin(usersignin.Username, usersignin.Password);

                    if (isUser || isAdmin)
                    {
                        FormsAuthentication.SetAuthCookie(usersignin.Username, false);

                        if (isAdmin)
                        {
                            return RedirectToAction("AdminIndex", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["SigninErrorMsg"] = "Invalid username or password";
                        return View("UserSignin", usersignin);
                    }
                }
                return View("UserSignin", usersignin);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        private UsersignupRepository usersignupRepository = new UsersignupRepository();

        /// <summary>
        /// Action method to display the user profile of the currently logged-in user.
        /// </summary>
        /// <returns>The view containing the user's profile details.</returns>
        [Authorize]
        public ActionResult UserProfile()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string currentUsername = User.Identity.Name;
                    UsersignupRepository usersignupRepository = new UsersignupRepository();
                    UserSignup user = usersignupRepository.GetUserSignupByUsername(currentUsername);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    return View(user);
                }
                else
                {
                    return RedirectToAction("UserSignin", "User");
                }
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the EditProfile page for the currently logged-in user.
        /// </summary>
        /// <returns>The view containing the user's profile details for editing.</returns>
        [Authorize]
        public ActionResult EditProfile()
        {
            try
            {
                string currentUsername = User.Identity.Name;
                UsersignupRepository usersignupRepository = new UsersignupRepository();
                UserSignup userSignup = usersignupRepository.GetUserSignupByUsername(currentUsername);
                userSignup.States = usersignupRepository.GetStates();
                userSignup.Cities = new List<SelectListItem>();

                if (userSignup == null)
                {
                    return HttpNotFound();
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
        /// Action method to handle the form submission for editing the user's profile.
        /// </summary>
        /// <param name="updatedUser">The UserSignup object containing the updated user profile details.</param>
        /// <returns>
        /// - If the update is successful, redirects to the user profile page.
        /// - If the update fails or ModelState is invalid, returns the EditProfile view with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserSignup updatedUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsersignupRepository usersignupRepository = new UsersignupRepository();
                    bool result = usersignupRepository.UpdateUserProfile(updatedUser);

                    if (result)
                    {
                        return RedirectToAction("UserProfile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update user profile. Please try again.");
                    }
                }
                return View(updatedUser);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to handle the form submission for saving the user's profile.
        /// </summary>
        /// <param name="user">The UserSignup object containing the updated user profile details.</param>
        /// <returns>
        /// - If the update is successful, redirects to the user profile page.
        /// - If the update fails or ModelState is invalid, returns the EditProfile view with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProfile(UserSignup user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsersignupRepository usersignupRepository = new UsersignupRepository();
                    bool result = usersignupRepository.UpdateUserProfile(user);

                    if (result)
                    {
                        return RedirectToAction("UserProfile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update user profile. Please try again.");
                    }
                }
                return View("EditProfile", user);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the DeleteProfile page for the currently logged-in user.
        /// </summary>
        /// <returns>The view containing the user's profile details for deletion.</returns>
        public ActionResult DeleteProfile()
        {
            string currentUsername = User.Identity.Name;
            UsersignupRepository usersignupRepository = new UsersignupRepository();
            UserSignup userSignup = usersignupRepository.GetUserSignupByUsername(currentUsername);

            if (userSignup == null)
            {
                return HttpNotFound();
            }

            return View(userSignup);
        }

        /// <summary>
        /// Action method to handle the form submission for confirming the deletion of the user's profile.
        /// </summary>
        /// <returns>
        /// - If the deletion is successful, logs the user out and redirects to the login page.
        /// - If the deletion fails or the user profile is not found, returns an error view.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProfileConfirmed()
        {
            string currentUsername = User.Identity.Name;
            UsersignupRepository usersignupRepository = new UsersignupRepository();
            UserSignup userSignup = usersignupRepository.GetUserSignupByUsername(currentUsername);

            if (userSignup == null)
            {
                return HttpNotFound();
            }
              bool result = usersignupRepository.DeleteUserProfile(userSignup.UserSignupId);

            if (result)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("UserSignin", "User");
            }
            else
            {
                return View("Error"); 
            }
        }

        /// <summary>
        /// Action method to handle the logout request.
        /// </summary>
        /// <returns>Redirects the user to the login page after signing out.</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("UserSignin", "User");
        }

        /// <summary>
        /// Action method to display the user's cart.
        /// </summary>
        /// <returns>The view containing the user's cart items and total price.</returns>
        [Authorize]
        public ActionResult ViewCart()
        {
            try
            {
                string username = User.Identity.Name;
                List<CartItem> cartItems = usersignupRepository.GetCartItemsByUsername(username);
                decimal totalPrice = cartItems.Sum(item => item.Subtotal);

                ViewBag.CartItems = cartItems;
                ViewBag.SelectedTotalPrice = totalPrice;

                Session["SelectedTotalPrice"] = totalPrice; 

                return View(cartItems);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }




        /// <summary>
        /// Action method to add an item to the user's cart.
        /// </summary>
        /// <param name="laptopId">The ID of the laptop to add to the cart.</param>
        /// <param name="quantity">The quantity of the laptop to add to the cart.</param>
        /// <returns>Redirects to the index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int laptopId, int quantity)
        {
            try
            {
                LaptopRepository laptopRepository = new LaptopRepository();
                Laptop laptopData = laptopRepository.GetLaptopById(laptopId);

                if (laptopData != null)
                {
                    string currentUsername = User.Identity.Name;
                    CartItem cartItem = new CartItem
                    {
                        LaptopId = laptopId,
                        LaptopName = laptopData.LaptopName,
                        Price = laptopData.Price,
                        Quantity = quantity,
                    };

                    bool result = usersignupRepository.AddOrUpdateCartItem(cartItem, currentUsername);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to update the quantity of an item in the user's cart.
        /// </summary>
        /// <param name="laptopId">The ID of the laptop to update in the cart.</param>
        /// <param name="quantity">The new quantity of the laptop in the cart.</param>
        /// <returns>Redirects to the cart view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCartItemQuantity(int laptopId, int quantity)
        {
            try
            {
                string currentUsername = User.Identity.Name;
                CartItem cartItem = new CartItem
                {
                    LaptopId = laptopId,
                    Quantity = quantity
                };

                bool result = usersignupRepository.AddOrUpdateCartItem(cartItem, currentUsername);
                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }



        /// <summary>
        /// Action method to clear the user's cart.
        /// </summary>
        /// <returns>Redirects to the cart view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            try
            {
                string currentUsername = User.Identity.Name;
                bool result = usersignupRepository.ClearCart(currentUsername);

                if (result)
                {
                    return RedirectToAction("ViewCart");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while clearing the cart. Please try again later.";
                    return RedirectToAction("ViewCart"); 
                }
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }
        /// <summary>
        /// Action method to remove selected items from the cart page
        /// </summary>
        /// <param name="laptopIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveSelectedCartItems(List<int> laptopIds)
        {
            try
            {
                bool result = usersignupRepository.RemoveSelectedCartItems(laptopIds, User.Identity.Name);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// Action method to display the order form.
        /// </summary>
        /// <returns>The view containing the order form.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult OrderForm()
        {
            try
            {
                decimal selectedTotalPrice = 0;

                if (Session["SelectedTotalPrice"] != null)
                {
                    selectedTotalPrice = (decimal)Session["SelectedTotalPrice"];
                }

                OrderView order = new OrderView();
                order.SelectedTotalPrice = selectedTotalPrice;

                if (Session["SelectedItems"] != null)
                {
                    List<CartItem> selectedItems = (List<CartItem>)Session["SelectedItems"];
                    order.SelectedItems = selectedItems;
                }

                return View(order);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error"); 
            }
        }


        /// <summary>
        /// Action method to place an order when the user submits the order form.
        /// </summary>
        /// <param name="order">The order details submitted by the user.</param>
        /// <returns>Redirects to the order confirmation page on successful order placement, or returns to the order form with validation errors.</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(OrderView order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string username = User.Identity.Name;
                    bool result = usersignupRepository.PlaceOrder(username, order);

                    if (result)
                    {
                        TempData["OrderSuccess"] = "Order placed successfully!";
                        TempData["OrderDetails"] = order;
                        return RedirectToAction("OrderConfirmation");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to place the order. Please try again.");
                    }
                }
                return View("OrderForm", order);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error"); 
            }
        }




        // Define a class to represent selected items data
        public class SelectedItem
        {
            public int laptopId { get; set; }
            public string laptopName { get; set; }
            public string itemPrice { get; set; }
        }
        /// <summary>
        /// Action method to display the order confirmation page.
        /// </summary>
        /// <returns>The view containing the order details for confirmation.</returns>
        [Authorize]
        public ActionResult OrderConfirmation()
        {
            try
            {
                OrderView order = TempData["OrderDetails"] as OrderView;

                if (order == null)
                {
                    throw new Exception("Order details are missing.");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to display the user's order history.
        /// </summary>
        /// <returns>The view containing the user's order history.</returns>
        [Authorize]
        public ActionResult OrderHistory()
        {
            try
            {
                string username = User.Identity.Name;
                List<OrderView> orders = usersignupRepository.GetOrderHistoryByUsername(username);
                return View(orders);
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Action method to cancel an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to cancel.</param>
        /// <returns>A JSON response indicating success or failure of the order cancellation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int orderId)
        {
            try
            {
                bool result = usersignupRepository.CancelOrder(orderId);

                if (result)
                {
                    TempData["OrderCancelled"] = "Order has been cancelled successfully!";
                }
                else
                {
                    TempData["OrderCancellationFailed"] = "Failed to cancel the order. Please try again.";
                }
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                ErrHandler.WriteError(ex.Message);
                return Json(new { success = false });
            }
        }

    }
}

