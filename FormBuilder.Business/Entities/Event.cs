using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class Event
    {
        public Event()
        {
            _users = new List<User>();
        }

        private ICollection<User> _users;

        public virtual ICollection<User> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventLocation { get; set; }
        public string Description { get; set; }

    }
}
