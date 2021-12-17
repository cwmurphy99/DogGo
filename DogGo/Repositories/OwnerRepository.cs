using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
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

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Owner.Id as ownerId, Owner.Name as ownerName, n.Name as neighborhoodName, Address, Email, Phone
                        FROM Owner
                        join Neighborhood n on n.Id = Owner.NeighborhoodId";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new List<Owner>();
                        while (reader.Read())
                        {
                            Owner walker = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("ownerName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Neighborhood = new Neighborhood
                                {
                                    Name = reader.GetString(reader.GetOrdinal("neighborhoodName"))
                                },
                                Phone = reader.GetString(reader.GetOrdinal("Phone"))
                            };

                            owners.Add(walker);
                        }

                        return owners;
                    }
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            Owner owner = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.*, d.Id DogId, d.Name DogName, d.Breed, d.Notes, d.ImageUrl, o.Id as ownerId, o.Name as ownerName, o.NeighborhoodId as neighborhoodId, n.Name as neighborhoodName, Address, Email, Phone
                        FROM Owner o
                        LEFT JOIN Dog d ON d.OwnerId = o.Id
                        join Neighborhood n ON n.Id = o.NeighborhoodId
                        WHERE o.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            if (owner == null)
                            {
                                owner = new Owner
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                    Name = reader.GetString(reader.GetOrdinal("ownerName")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    NeighborhoodId = reader.GetInt32(reader.GetOrdinal("neighborhoodId")),
                                    Neighborhood = new Neighborhood
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("neighborhoodName"))
                                    },
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    Dogs = new List<Dog>()
                                };

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("DogId")))
                            {
                                owner.Dogs.Add(new Dog
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),
                                    Breed = reader.GetString(reader.GetOrdinal("Breed"))
                                });
                            }
                        }
                        return owner;
                    }
                }
            }
        }

        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                // Instead of filtering by id, use an Owner email to find the Owner
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Owner owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                            };

                            return owner;
                        }

                        return null;
                    }
                }
            }
        }

        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    // When this executes, it returns the new id created by the database
                    int id = (int)cmd.ExecuteScalar();
                    // we add that id to our owner object
                    owner.Id = id;
                }
            }
        }

        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.Id);
                    // we do not need data back from the database, so we use ExecuteNonQuery
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                // In a delete, we filter by id to make sure we only delete one.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);
                    // we don't need data returned, so we use ExecuteNonQuery here
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
    }
}