using System.Collections.Generic;
using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;
        public DogRepository(IConfiguration config)
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
        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      SELECT Id, [Name], Breed, Notes, ImageUrl, OwnerId
                                      FROM Dog";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Dog> dogs = new List<Dog>();
                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),

                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null :
                                reader.GetString(reader.GetOrdinal("Notes")),

                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null :
                                reader.GetString(reader.GetOrdinal("ImageUrl")),

                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                        };
                     
                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }

        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      INSERT INTO Dog ([Name], Breed, Notes, ImageUrl, OwnerId)
                                      OUTPUT INSERTED.ID
                                      VALUES (@name, @breed, @notes, @imageUrl, @ownerId);";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    int id = (int)cmd.ExecuteScalar();
                    dog.Id = id;
                }
            }
        }
        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      UPDATE Dog
                                      SET
                                      [Name] = @name,
                                      Breed = @breed,
                                      Notes = @notes,
                                      ImageUrl = @imageUrl,
                                      OwnerId = @ownerId
                                      WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                      DELETE FROM Dog
                                      WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", dogId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
