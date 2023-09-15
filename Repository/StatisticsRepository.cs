using OnlineDellShowroom.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace OnlineDellShowroom.Repository
{
    public class StatisticsRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ToString();

        /// <summary>
        /// Retrieve website statistics from the database.
        /// </summary>
        /// <returns>WebsiteStatistics object containing statistics</returns>
        public WebsiteStatistics GetWebsiteStatistics()
        {
            WebsiteStatistics statistics = new WebsiteStatistics();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SPS_WebsiteStatistics", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                statistics.TotalUsers = (int)reader["TotalUsers"];
                                statistics.TotalProducts = (int)reader["TotalProducts"];
                                statistics.TotalOrders = (int)reader["TotalOrders"];
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return statistics;
        }
    }
}