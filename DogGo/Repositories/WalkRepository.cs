using System;
using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetWalksByWalkerId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.id as walkId, Date, Duration, d.Name as dogName, Walker.Name as walkerName
                                        FROM Walks w
                                        JOIN Dog d ON d.Id = w.DogId
                                        JOIN Walker on Walker.Id = w.WalkerId
                                        Where w.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walk> walks = new List<Walk>();
                        {
                            while (reader.Read())
                            {
                                Walk walk = new Walk
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("walkId")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    Dog = new Dog
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("dogName"))
                                    },
                                    Walker = new Walker
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("walkerName"))
                                    }
                                };
                                walks.Add(walk);
                            }
                            return walks;
                        }

                    }
                }
            }
        }





    }
}
