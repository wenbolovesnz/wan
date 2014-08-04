using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class PersonalMessage
    {
        public PersonalMessage()
        {
            
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsRead { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}
