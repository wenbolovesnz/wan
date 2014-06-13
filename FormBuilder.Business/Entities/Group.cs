using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class Group
    {
        public Group()
        {
            _users = new List<User>();
            _userGroupRoles = new List<UserGroupRole>();
        }

        private ICollection<User> _users;

        private ICollection<UserGroupRole> _userGroupRoles; 

        public virtual ICollection<User> Users
        {
            get { return _users; }
            set {_users = value; }
        }

        public virtual ICollection<UserGroupRole> UserGroupRoles
        {
            get { return _userGroupRoles; }
            set { _userGroupRoles = value; }
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }


        public string GroupImage { get; set; }
        public string ContentType { get; set; }



    }
}
