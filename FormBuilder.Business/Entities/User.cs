using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ICollection<Event> _events;
        private ICollection<PersonalMessage> _personalMessages; 

        public User()
        {
            _roles = new List<Role>();
            _groups = new List<Group>();
            _events = new List<Event>();
            _personalMessages = new List<PersonalMessage>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public bool ForceChangePassword { get; set; }
        public DateTime CreatedDate { get; set; }

        public string NickName { get; set; }
        public string AboutMe { get; set; }
        public DateTime? DOB { get; set; }
        public string City { get; set; }

        public string ProfileImage { get; set; }
        public string ContentType { get; set; }

        public virtual ICollection<Event> Events
        {
            get { return _events; }
            set { _events = value; }
        }

        public virtual ICollection<PersonalMessage> PersonalMessages
        {
            get { return _personalMessages; }
            set { _personalMessages = value; }
        }

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
