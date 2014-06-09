using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class UserGroupRole
    {
        public int Id { get; set; }
 
        public int UserId { get; set; }
        public User User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }


    }
}
