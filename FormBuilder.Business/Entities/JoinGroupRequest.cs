using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class JoinGroupRequest
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime DecisionDate { get; set; }

        public int? DecisionUserId { get; set; }
        public User DecisionUser { get; set; }


    }
}
