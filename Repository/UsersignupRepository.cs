using OnlineDellShowroom.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OnlineDellShowroom.Repository
{
    public class UsersignupRepository
    {
        private SqlConnection connection;


        private void Connection()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();
            connection = new SqlConnection(connectionstring);
        }

        /// <summary>
        /// Sign up a new user.
        /// </summary>
        /// <param name="usersignup">UserSignup object containing user details</param>
        /// <returns>True if sign up is successful, false otherwise</returns>
        public bool SignUp(UserSignup usersignup)
        {
            try
            {
                Connection();
                SqlCommand sqlcommand = new SqlCommand("SPI_UserSignup", connection);
                sqlcommand.CommandType = CommandType.StoredProcedure;

                sqlcommand.Parameters.AddWithValue("@Firstname", usersignup.Firstname);
                sqlcommand.Parameters.AddWithValue("@Lastname", usersignup.Lastname);
                sqlcommand.Parameters.AddWithValue("@Dateofbirth", usersignup.Dateofbirth);
                sqlcommand.Parameters.AddWithValue("@Gender", usersignup.Gender);
                sqlcommand.Parameters.AddWithValue("@Mobilenumber", usersignup.Mobilenumber);
                sqlcommand.Parameters.AddWithValue("@Email", usersignup.Email);
                sqlcommand.Parameters.AddWithValue("@Address", usersignup.Address);
                sqlcommand.Parameters.AddWithValue("@State", usersignup.State);
                sqlcommand.Parameters.AddWithValue("@City", usersignup.City);
                sqlcommand.Parameters.AddWithValue("@Username", usersignup.Username);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(usersignup.Password);
                SqlParameter passwordParam = new SqlParameter("@Password", SqlDbType.VarBinary);
                passwordParam.Value = new SqlBinary(passwordBytes);
                sqlcommand.Parameters.Add(passwordParam);

                connection.Open();
                int i = sqlcommand.ExecuteNonQuery();

                if (i >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Method to fetch states from the database.
        /// </summary>
        /// <returns>List of SelectListItem containing state values and names</returns>
        public List<SelectListItem> GetStates()
        {
            List<SelectListItem> states = new List<SelectListItem>();

            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPS_GetStates", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    states.Add(new SelectListItem
                    {
                        Value = reader["StateId"].ToString(),
                        Text = reader["StateName"].ToString()
                    });
                }
            }
            finally
            {
                connection.Close();
            }

            return states;
        }

        /// <summary>
        /// Get cities by state.
        /// </summary>
        /// <param name="stateId">State ID</param>
        /// <returns>List of SelectListItem containing city values and names</returns>
        public List<SelectListItem> GetCitiesByState(int stateId)
        {
            List<SelectListItem> cities = new List<SelectListItem>();

            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPS_GetCitiesByState", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@StateId", stateId);

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    cities.Add(new SelectListItem
                    {
                        Value = reader["CityId"].ToString(),
                        Text = reader["CityName"].ToString()
                    });
                }

                return cities;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Check if a mobile number already exists.
        /// </summary>
        /// <param name="mobileNumber">Mobile number to check</param>
        /// <returns>True if the mobile number exists, false otherwise</returns>
        public bool IsMobileNumberExists(string mobileNumber)
        {
            Connection();

            try
            {
                using (SqlCommand command = new SqlCommand("SPS_MobileNumberExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Check if an email already exists.
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>True if the email exists, false otherwise</returns>
        public bool IsEmailExists(string email)
        {
            Connection();

            try
            {
                using (SqlCommand command = new SqlCommand("SPS_CheckEmailExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    // If the result is not null, it means the email already exists in the database
                    return result != null;
                }
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Check if a username already exists.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if the username exists, false otherwise</returns>
        public bool IsUsernameExists(string username)
        {
            Connection();

            try
            {
                using (SqlCommand command = new SqlCommand("SPS_UsernameExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get details of all user signups.
        /// </summary>
        /// <returns>List of UserSignup objects with user signup details</returns>
        public List<UserSignup> GetDetails()
        {
            Connection();

            try
            {
                List<UserSignup> usersignupList = new List<UserSignup>();
                SqlCommand sqlcommand = new SqlCommand("SPS_UserSignup", connection);
                sqlcommand.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader sqldatareader = sqlcommand.ExecuteReader();

                while (sqldatareader.Read())
                {
                    usersignupList.Add(new UserSignup
                    {
                        UserSignupId = Convert.ToInt32(sqldatareader["UserSignupId"]),
                        Firstname = sqldatareader["Firstname"].ToString(),
                        Lastname = sqldatareader["Lastname"].ToString(),
                        Dateofbirth = DateTime.Parse(sqldatareader["Dateofbirth"].ToString()),
                        Gender = sqldatareader["Gender"].ToString(),
                        Mobilenumber = sqldatareader["Mobilenumber"].ToString(),
                        Email = sqldatareader["Email"].ToString(),
                        Address = sqldatareader["Address"].ToString(),
                        State = sqldatareader["State"].ToString(),
                        City = sqldatareader["City"].ToString(),
                        Username = sqldatareader["Username"].ToString(),
                        Password = sqldatareader["Password"].ToString()
                    });
                }

                return usersignupList;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Authenticate user using user credentials.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>True if the user authentication is successful, false otherwise</returns>
        public bool AuthenticateUser(string username, string password)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPS_UserSignin", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);
                sqlCommand.Parameters.AddWithValue("@Password", password);

                connection.Open();
                int result = (int)sqlCommand.ExecuteScalar();
                return result == 1;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Authenticate admin using admin credentials.
        /// </summary>
        /// <param name="username">Admin's username</param>
        /// <param name="password">Admin's password</param>
        /// <returns>True if the admin authentication is successful, false otherwise</returns>
        public bool AuthenticateAdmin(string username, string password)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPS_AdminSignin", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);
                sqlCommand.Parameters.AddWithValue("@Password", password);

                connection.Open();
                int isAdmin = (int)sqlCommand.ExecuteScalar();
                return isAdmin == 1;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Sign in a user using user credentials.
        /// </summary>
        /// <param name="usersignin">UserSignin object with user's sign-in details</param>
        /// <returns>True if the user sign-in is successful, false otherwise</returns>
        public bool SignIn(UserSignin usersignin)
        {
            Connection();

            try
            {
                SqlCommand sqlcommand = new SqlCommand("SPI_UserSignin", connection);
                sqlcommand.CommandType = CommandType.StoredProcedure;

                sqlcommand.Parameters.AddWithValue("@Username", usersignin.Username);
                sqlcommand.Parameters.AddWithValue("@Password", usersignin.Password);

                connection.Open();
                int i = sqlcommand.ExecuteNonQuery();

                return i >= 1; 
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Retrieve user signup profile by user signup ID.
        /// </summary>
        /// <param name="userSignupId">ID of the user signup profile</param>
        /// <returns>UserSignup object with user signup profile details</returns>
        public UserSignup GetUserSignupById(int userSignupId)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPS_UserSignupById", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserSignupId", userSignupId);

                connection.Open();
                SqlDataReader sqldatareader = sqlCommand.ExecuteReader();
                UserSignup userSignup = null;

                if (sqldatareader.Read())
                {
                    userSignup = new UserSignup
                    {
                        UserSignupId = Convert.ToInt32(sqldatareader["UserSignupId"]),
                        Firstname = sqldatareader["Firstname"].ToString(),
                        Lastname = sqldatareader["Lastname"].ToString(),
                        Dateofbirth = DateTime.Parse(sqldatareader["Dateofbirth"].ToString()),
                        Gender = sqldatareader["Gender"].ToString(),
                        Mobilenumber = sqldatareader["Mobilenumber"].ToString(),
                        Email = sqldatareader["Email"].ToString(),
                        Address = sqldatareader["Address"].ToString(),
                        State = sqldatareader["State"].ToString(),
                        City = sqldatareader["City"].ToString(),
                        Username = sqldatareader["Username"].ToString(),
                        Password = sqldatareader["Password"].ToString()
                    };
                }

                return userSignup;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Delete a user by their user signup ID.
        /// </summary>
        /// <param name="userSignupId">ID of the user to delete</param>
        /// <returns>True if the user is successfully deleted, false otherwise</returns>
        public bool DeleteUser(int userSignupId)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPD_UserSignup", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UsersignupId", userSignupId);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Retrieve user profile by user signup ID.
        /// </summary>
        /// <param name="userSignupId">ID of the user signup profile</param>
        /// <returns>UserSignup object with user profile details</returns>
        public UserSignup GetUserProfileById(int userSignupId)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPS_UserSignupById", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserSignupId", userSignupId);

                connection.Open();
                SqlDataReader sqldatareader = sqlCommand.ExecuteReader();
                UserSignup userSignup = null;

                if (sqldatareader.Read())
                {
                    userSignup = new UserSignup
                    {
                        UserSignupId = Convert.ToInt32(sqldatareader["UserSignupId"]),
                        Firstname = sqldatareader["Firstname"].ToString(),
                        Lastname = sqldatareader["Lastname"].ToString(),
                        Dateofbirth = DateTime.Parse(sqldatareader["Dateofbirth"].ToString()),
                        Gender = sqldatareader["Gender"].ToString(),
                        Mobilenumber = sqldatareader["Mobilenumber"].ToString(),
                        Email = sqldatareader["Email"].ToString(),
                        Address = sqldatareader["Address"].ToString(),
                        State = sqldatareader["State"].ToString(),
                        City = sqldatareader["City"].ToString(),
                        Username = sqldatareader["Username"].ToString(),
                        Password = sqldatareader["Password"].ToString()
                    };
                }

                return userSignup;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Retrieve user details based on username.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>UserSignup object with user details</returns>
        public UserSignup GetUserSignupByUsername(string username)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPS_UserSignupByUsername", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);

                connection.Open();
                SqlDataReader sqldatareader = sqlCommand.ExecuteReader();
                UserSignup userSignup = null;

                if (sqldatareader.Read())
                {
                    userSignup = new UserSignup
                    {
                        UserSignupId = Convert.ToInt32(sqldatareader["UserSignupId"]),
                        Firstname = sqldatareader["Firstname"].ToString(),
                        Lastname = sqldatareader["Lastname"].ToString(),
                        Dateofbirth = DateTime.Parse(sqldatareader["Dateofbirth"].ToString()),
                        Gender = sqldatareader["Gender"].ToString(),
                        Mobilenumber = sqldatareader["Mobilenumber"].ToString(),
                        Email = sqldatareader["Email"].ToString(),
                        Address = sqldatareader["Address"].ToString(),
                        State = sqldatareader["State"].ToString(),
                        City = sqldatareader["City"].ToString(),
                        Username = sqldatareader["Username"].ToString(),
                        Password = sqldatareader["Password"].ToString()
                    };
                }

                return userSignup;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Update the user profile details.
        /// </summary>
        /// <param name="user">UserSignup object with updated details</param>
        /// <returns>True if the user profile is successfully updated, false otherwise</returns>
        public bool UpdateUserProfile(UserSignup user)
        {
            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPU_UserSignup", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserSignupId", user.UserSignupId);
                sqlCommand.Parameters.AddWithValue("@Firstname", user.Firstname);
                sqlCommand.Parameters.AddWithValue("@Lastname", user.Lastname);
                sqlCommand.Parameters.AddWithValue("@Dateofbirth", user.Dateofbirth);
                sqlCommand.Parameters.AddWithValue("@Gender", user.Gender);
                sqlCommand.Parameters.AddWithValue("@Mobilenumber", user.Mobilenumber);
                sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                sqlCommand.Parameters.AddWithValue("@Address", user.Address);
                sqlCommand.Parameters.AddWithValue("@State", user.State);
                sqlCommand.Parameters.AddWithValue("@City", user.City);
                sqlCommand.Parameters.AddWithValue("@Username", user.Username);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(user.Password);
                SqlParameter passwordParam = new SqlParameter("@Password", SqlDbType.VarBinary);
                passwordParam.Value = new SqlBinary(passwordBytes);
                sqlCommand.Parameters.Add(passwordParam);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                return rowsAffected > 0; // Return true if any rows were affected (update successful)
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Delete a user profile by their user signup ID.
        /// </summary>
        /// <param name="userSignupId">ID of the user signup profile to delete</param>
        /// <returns>True if the user profile is successfully deleted, false otherwise</returns>
        public bool DeleteUserProfile(int userSignupId)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPD_UserSignup", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@UserSignupId", userSignupId);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                // If rowsAffected > 0, the delete operation was successful
                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Add or update a cart item for a specific user.
        /// </summary>
        /// <param name="cartItem">CartItem to add or update</param>
        /// <param name="username">Username of the user</param>
        /// <returns>True if cart item is successfully added or updated, false otherwise</returns>
        public bool AddOrUpdateCartItem(CartItem cartItem, string username)
        {
            Connection();

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SPIU_CartItems", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@LaptopId", cartItem.LaptopId);
                sqlCommand.Parameters.AddWithValue("@LaptopName", cartItem.LaptopName);
                sqlCommand.Parameters.AddWithValue("@Price", cartItem.Price);
                sqlCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                sqlCommand.Parameters.AddWithValue("@Username", username);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }


        /// <summary>
        /// Add the method to remove a specific item from the cart
        /// </summary>
        /// <param name="laptopId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool RemoveSelectedCartItems(List<int> laptopIds, string username)
        {
            try
            {
                Connection();
                connection.Open();
                SqlCommand command = new SqlCommand("SPD_SelectedCartItems", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@LaptopIds", string.Join(",", laptopIds));

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }


        /// <summary>
        /// Clear the cart for a specific user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>True if cart is cleared successfully, false otherwise</returns>
        public bool ClearCart(string username)
        {
            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPD_ClearCart", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Username", username);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Retrieve cart items for a specific user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>List of cart items for the user</returns>
        public List<CartItem> GetCartItemsByUsername(string username)
        {
            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                Connection();
                using (connection)
                {
                    SqlCommand command = new SqlCommand("SPS_CartItemsByUsername", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cartItems.Add(new CartItem
                        {
                            LaptopId = Convert.ToInt32(reader["LaptopId"]),
                            LaptopName = reader["LaptopName"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            ImageUrl = reader["ImageUrl"].ToString()
                        });
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return cartItems;
        }

        /// <summary>
        /// Place an order for a specific user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="order">Order details</param>
        /// <returns>True if order is successfully placed, false otherwise</returns>
        public bool PlaceOrder(string username, OrderView order)
        {
            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPI_Order", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);
                sqlCommand.Parameters.AddWithValue("@Name", order.Name);
                sqlCommand.Parameters.AddWithValue("@Address", order.Address);
                sqlCommand.Parameters.AddWithValue("@ContactNumber", order.ContactNumber);
                sqlCommand.Parameters.AddWithValue("@Pincode", order.Pincode);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                // If the order was successfully placed, remove all items from the cart
                if (rowsAffected > 0)
                {
                    bool cartCleared = ClearCart(username);
                    return cartCleared;
                }

                return false; // Return false if the order was not placed
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get order details for a specific user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>Order details as an OrderView object</returns>
        public OrderView GetOrderDetailsByUsername(string username)
        {
            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPO_GetOrderDetailsByUsername", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);

                connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                OrderView order = null;

                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    order = new OrderView
                    {
                        Name = dataReader["Name"].ToString(),
                        Address = dataReader["Address"].ToString(),
                        ContactNumber = dataReader["ContactNumber"].ToString(),
                        Pincode = Convert.ToInt32(dataReader["Pincode"])
                    };
                }

                dataReader.Close();
                return order;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get order history for a specific user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>List of order details as OrderView objects</returns>
        public List<OrderView> GetOrderHistoryByUsername(string username)
        {
            List<OrderView> orders = new List<OrderView>();

            try
            {
                Connection();
                SqlCommand sqlCommand = new SqlCommand("SPS_OrderHistoryByUsername", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);

                connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    orders.Add(new OrderView
                    {
                        OrderId = Convert.ToInt32(dataReader["OrderId"]),
                        Username = dataReader["Username"].ToString(),
                        Name = dataReader["Name"].ToString(),
                        Address = dataReader["Address"].ToString(),
                        ContactNumber = dataReader["ContactNumber"].ToString(),
                        Pincode = Convert.ToInt32(dataReader["Pincode"]),
                        OrderDate = Convert.ToDateTime(dataReader["OrderDate"]),
                        OrderStatus = dataReader["OrderStatus"].ToString(),
                    });
                }

                dataReader.Close();
                return orders;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Cancel an order by its order ID.
        /// </summary>
        /// <param name="orderId">ID of the order to cancel</param>
        /// <returns>True if the order is successfully cancelled, false otherwise</returns>
        public bool CancelOrder(int orderId)
        {
            try
            {
                Connection();

                SqlCommand sqlCommand = new SqlCommand("SP_CancelOrder", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get a list of pending orders.
        /// </summary>
        /// <returns>List of pending orders as OrderView objects</returns>
        public List<OrderView> GetPendingOrders()
        {
            Connection();
            List<OrderView> pendingOrders = new List<OrderView>();

            try
            {
                SqlCommand command = new SqlCommand("SPS_PendingOrders", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderView order = new OrderView
                        {
                            OrderId = (int)reader["OrderId"],
                            Username = reader["Username"].ToString(),
                            OrderStatus = reader["OrderStatus"].ToString()
                        };
                        pendingOrders.Add(order);
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return pendingOrders;
        }

        /// <summary>
        /// Approve an order by its order ID.
        /// </summary>
        /// <param name="orderId">ID of the order to approve</param>
        /// <returns>True if the order is successfully approved, false otherwise</returns>
        public bool ApproveOrder(int orderId)
        {
            Connection();

            try
            {
                SqlCommand command = new SqlCommand("SP_ApproveOrder", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
