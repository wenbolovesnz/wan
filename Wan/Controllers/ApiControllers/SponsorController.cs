using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using Microsoft.Owin.Security.Provider;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class SponsorController : ApiController
    {
        private IApplicationUnit _applicationUnit;

        public SponsorController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;
        }



         //GET api/sponsor/5 get all the sponsors in groupId 
        public IList<SponsorViewModel> Get(int id)
        {
            var sponsors = _applicationUnit.SponsoRepository.Get(m => m.GroupId == id);

            return sponsors.Select(s => new SponsorViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                PhotoUrl = s.PhotoUrl
            }).ToList();
        }

        // POST api/sponsor
        public void Post([FromBody]SponsorViewModel sponsorViewModel)
        {            
            var group =
                _applicationUnit.GroupRepository.Get(m => m.Id == sponsorViewModel.GroupId, null, "UserGroupRoles")
                    .First();

            if (group.UserGroupRoles.Select(m => m.UserId).Contains(WebSecurity.CurrentUserId))
            {
                var newSponsor = new Sponsor()
                {
                    Name = sponsorViewModel.Name,
                    GroupId = sponsorViewModel.GroupId
                };

                _applicationUnit.SponsoRepository.Insert(newSponsor);
                _applicationUnit.SaveChanges();               
            }
 
        }

        // PUT api/sponsor/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/sponsor/5
        public void Delete(int id)
        {
        }        
    }

    public class SponsorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int GroupId { get; set; }
    }
}
