using System;
using System.Collections.Generic;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NeighborhoodId { get; set; }
        public string ImageUrl { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Dog> Dogs { get; set; }
        List<Walk> Walks { get; set; }
        public TimeSpan TotalTime { get; set; }
    }
}