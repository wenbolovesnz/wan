using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using Wan.Controllers.ApiControllers;

namespace Wan.Models
{
    public class JoinGroupRequestViewModel
    {
        public int Id { get; set; }
        public UserViewModel User { get; set; }
        public GroupViewModel Group { get; set; }
        public string Message { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? DecisionDate { get; set; }
        

        public string DeclineReason { get; set; }
        



    }
}