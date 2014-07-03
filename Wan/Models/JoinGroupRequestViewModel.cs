using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wan.Models
{
    public class JoinGroupRequestViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}