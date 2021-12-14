﻿using System;

namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
        public string Breed { get; set; }

        public string Notes { get; set; }
        public string ImageUrl { get; set; }

        public static explicit operator Dog(DBNull v)
        {
            throw new NotImplementedException();
        }
    }
}
