using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class EventMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }

        public int  EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
