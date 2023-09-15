using OnlineDellShowroom.Error;
using OnlineDellShowroom.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace OnlineDellShowroom.Repository
{
    public class LaptopRepository
    {
        private readonly string connectionString;

        public LaptopRepository()
        {
            connectionString = WebConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();
        }

        /// <summary>
        /// Retrieve laptop details from the database.
        /// </summary>
        /// <returns>List of Laptop objects containing details</returns>
        public List<Laptop> LaptopDetails()
        {
            List<Laptop> laptops = new List<Laptop>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPS_LaptopDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                laptops.Add(new Laptop
                                {
                                    LaptopId = Convert.ToInt32(reader["LaptopId"]),
                                    LaptopName = reader["LaptopName"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    ImageUrl = reader["ImageUrl"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"])
                                });
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return laptops;
        }


        /// <summary>
        /// Add laptop details to the database.
        /// </summary>
        /// <param name="laptop">Laptop object containing details to be added</param>
        /// <returns>True if the details were successfully added, false otherwise</returns>
        public bool AddLaptopDetails(Laptop laptop)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPI_LaptopDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LaptopName", laptop.LaptopName);
                        command.Parameters.AddWithValue("@Description", laptop.Description);
                        command.Parameters.AddWithValue("@ImageUrl", laptop.ImageUrl);
                        command.Parameters.AddWithValue("@Price", laptop.Price);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Get laptop details by ID.
        /// </summary>
        /// <param name="laptopId">ID of the laptop</param>
        /// <returns>Laptop object if found, null otherwise</returns>
        public Laptop GetLaptopById(int laptopId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPS_LaptopById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LaptopId", laptopId);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Laptop
                                {
                                    LaptopId = Convert.ToInt32(reader["LaptopId"]),
                                    LaptopName = reader["LaptopName"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    ImageUrl = reader["ImageUrl"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"])
                                };
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return null;
        }

        /// <summary>
        /// Update laptop details.
        /// </summary>
        /// <param name="laptop">Laptop object containing updated details</param>
        /// <returns>True if the laptop details are successfully updated, false otherwise</returns>
        public bool UpdateLaptopDetails(EditLaptopViewModel viewModel)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPU_LaptopDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LaptopId", viewModel.LaptopId);
                        command.Parameters.AddWithValue("@LaptopName", viewModel.LaptopName);
                        command.Parameters.AddWithValue("@Description", viewModel.Description);
                        command.Parameters.AddWithValue("@NewImageUrl", viewModel.ExistingImageUrl); 
                        command.Parameters.AddWithValue("@Price", viewModel.Price);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }



        /// <summary>
        /// Delete a laptop by its ID.
        /// </summary>
        /// <param name="laptopId">ID of the laptop to be deleted</param>
        /// <returns>True if the laptop is successfully deleted, false otherwise</returns>
        public bool DeleteLaptop(int laptopId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPD_LaptopDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LaptopId", laptopId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Add an admin user.
        /// </summary>
        /// <param name="adminUser">AdminUser object containing admin user details</param>
        /// <returns>True if the admin user is successfully added, false otherwise</returns>
        public bool AddAdminUser(AdminUser adminUser)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPI_AdminSignin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", adminUser.Username);

                        // Convert the plain text password to varbinary
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(adminUser.Password);
                        SqlParameter passwordParam = new SqlParameter("@Password", SqlDbType.VarBinary);
                        passwordParam.Value = new SqlBinary(passwordBytes);
                        command.Parameters.Add(passwordParam);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Check if a username already exists.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if the username exists, false otherwise</returns>
        public bool IsUsernameExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SPS_Adminuserexists", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);

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
        }

        private List<Laptop> cartItems = new List<Laptop>();

        /// <summary>
        /// Get the list of items in the cart.
        /// </summary>
        /// <returns>List of laptops in the cart</returns>
        public List<Laptop> GetCartItems()
        {
            return cartItems;
        }


        /// <summary>
        /// Add a laptop to the cart based on its ID.
        /// </summary>
        /// <param name="laptopId">ID of the laptop to be added to the cart</param>
        public void AddToCart(int laptopId)
        {
            Laptop laptop = LaptopDetails().Find(l => l.LaptopId == laptopId);
            if (laptop != null)
            {
                Laptop cartItem = cartItems.Find(ci => ci.LaptopId == laptop.LaptopId);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    laptop.Quantity = 1;
                    cartItems.Add(laptop);
                }
            }

            Console.WriteLine("Cart items after adding an item:");
            foreach (var item in cartItems)
            {
                Console.WriteLine($"{item.LaptopName} - {item.Quantity}");
            }
        }


        /// <summary>
        /// Method to retrieve cart items from the session
        /// </summary>
        /// <returns></returns>
        private List<Laptop> GetCartItemsFromSession()
        {
            var cartItems = HttpContext.Current.Session["CartItems"] as List<Laptop>;
            if (cartItems == null)
            {
                cartItems = new List<Laptop>();
                HttpContext.Current.Session["CartItems"] = cartItems; 
            }
            Console.WriteLine("Cart items from session:");
            foreach (var item in cartItems)
            {
                Console.WriteLine($"{item.LaptopName} - {item.Quantity}");
            }

            return cartItems;
        }
        /// <summary>
        /// Update the quantity of a specific item in the cart based on laptop ID.
        /// </summary>
        /// <param name="laptopId">ID of the laptop to be updated</param>
        /// <param name="quantity">New quantity for the laptop</param>
        public void UpdateCartItemQuantity(int laptopId, int quantity)
        {
            Laptop cartItem = cartItems.Find(ci => ci.LaptopId == laptopId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }
        }

        /// <summary>
        /// Save cart items to the session
        /// </summary>
        /// <param name="cartItems"></param>
        public void SaveCartItemsToSession(List<Laptop> cartItems)
        {
            HttpContext.Current.Session["CartItems"] = cartItems;
        }

        /// <summary>
        /// Remove a specific item from the cart based on laptop ID.
        /// </summary>
        /// <param name="laptopId">ID of the laptop to be removed</param>
        public void RemoveFromCart(int laptopId)
        {
            cartItems.RemoveAll(ci => ci.LaptopId == laptopId);
        }


        /// <summary>
        /// Clear the items in the cart.
        /// </summary>
        public void ClearCart()
        {
            cartItems.Clear();
        }

    }
}
