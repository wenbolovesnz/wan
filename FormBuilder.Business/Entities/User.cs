using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using FormBuilder.Business.Entities.Enums;

namespace FormBuilder.Business.Entities
{
    public class User
    {
        private ICollection<Role> _roles;

        private ICollection<Group> _groups;

        public User()
        {
            _roles = new List<Role>();
            _groups = new List<Group>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public bool ForceChangePassword { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        public virtual ICollection<Group> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }
        
    }
}
