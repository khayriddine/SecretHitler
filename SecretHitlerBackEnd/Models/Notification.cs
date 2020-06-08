using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string sender { get; set; }
        public string content { get; set; }


        //public virtual List<User> Users { get; set; }
    }
}
