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
        }

        private ICollection<User> _users;

        public virtual ICollection<User> Users
        {
            get { return _users; }
            set {_users = value; }
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }

        

    }
}
