using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Content { get; set; }

        public User User { get; set; }
        [NotMapped]
        public List<User> To { get; set; }
    }
}
