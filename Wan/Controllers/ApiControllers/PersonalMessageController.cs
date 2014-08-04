using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBuilder.Data.Contracts;
using Wan.Services;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class PersonalMessageController : ApiController
    {
        private IApplicationUnit _applicationUnit;
        private ModelFactoryService _modelFactory;

        public PersonalMessageController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;
            _modelFactory = new ModelFactoryService();
        }

        // GET api/personalmessage
        [Authorize]
        public IList<PersonalMessageViewModel> Get()
        {
            var messages = _applicationUnit.PersonalMessageRepository.Get(m => m.UserId == WebSecurity.CurrentUserId && !m.IsRead);

            return messages.Select(m => new PersonalMessageViewModel()
            {
                Content = m.Content,
                CreatedByUserId = m.CreatedByUserId,
                CreatedDate = m.CreatedDate,
                Id = m.Id,
                IsRead = m.IsRead
            }).ToList();
        }

        // GET api/personalmessage/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/personalmessage
        public void Post([FromBody]string value)
        {
        }

        [Authorize]
        [HttpPost]
        public PersonalMessageViewModel Update(int id, [FromBody]PersonalMessageViewModel viewModel)
        {
            var pm = _applicationUnit.PersonalMessageRepository.GetByID(id);
            pm.IsRead = true;
            _applicationUnit.PersonalMessageRepository.Update(pm);
            _applicationUnit.SaveChanges();

            return viewModel;
        }

        // DELETE api/personalmessage/5
        public void Delete(int id)
        {
        }
    }

    public class PersonalMessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsRead { get; set; }
    }
}
