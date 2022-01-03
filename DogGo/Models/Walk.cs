using System;
using System.Collections.Generic;

namespace DogGo.Models
{
    public class Walk
    {
        public int Id { get; set; }
        public Walker Walker { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public Dog Dog { get; set; }
        public object Owner { get; internal set; }
    }
}
