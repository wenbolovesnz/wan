using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class Sponsor
    {
        public Sponsor()
        {
            _events = new List<Event>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        private ICollection<Event> _events;

        public virtual ICollection<Event> Events
        {
            get { return _events; }
            set { _events = value; }
        }
    }
}
