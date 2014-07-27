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
            _sponsors = new List<Sponsor>();
            _users = new List<User>();
            _eventMessages = new List<EventMessage>();
        }

        private ICollection<Sponsor> _sponsors;

        public virtual ICollection<Sponsor> Sponsors
        {
            get { return _sponsors; }
            set { _sponsors = value; }
        }

        private ICollection<User> _users;

        public virtual ICollection<User> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        private ICollection<EventMessage> _eventMessages;

        public virtual ICollection<EventMessage> EventMessages
        {
            get { return _eventMessages; }
            set { _eventMessages = value; }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventLocation { get; set; }
        public string Description { get; set; }
        public int CreatedByUserId { get; set; }

    }
}
