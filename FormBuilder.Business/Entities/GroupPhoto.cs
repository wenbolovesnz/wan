using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Business.Entities
{
    public class GroupPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
