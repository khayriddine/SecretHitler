using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Dummy
    {
        public int DummyId { get; set; }
        public string Name { get; set; }
        public int BagId { get; set; }
        public Bag Bag { get; set; }
    }

    public class Bag
    {
        public int BagId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Dummy> Dummies { get; set; }
    }
}
