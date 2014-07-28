using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using Microsoft.Owin.Security.Provider;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
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
 
        public SponsorViewModel Get(int id)
        {
            var sponsor = _applicationUnit.SponsoRepository.Get(m => m.Id == id, null, "Group").First();

            var isCurrentUserGroupManager = sponsor.Group.UserGroupRoles.Select(m => m.UserId).Contains(WebSecurity.CurrentUserId);

            return new SponsorViewModel()
            {
                Id = sponsor.Id,
                Name = sponsor.Name,
                PhotoUrl = sponsor.PhotoUrl,
                IsManager = isCurrentUserGroupManager
            };
        }

         //GET api/sponsor/5 get all the sponsors in groupId 
        [HttpGet]
        public IList<SponsorViewModel> Get(string groupId)
        {
            var id = Int32.Parse(groupId);

            var sponsors = _applicationUnit.SponsoRepository.Get(m => m.GroupId == id);

            return sponsors.Select(s => new SponsorViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                PhotoUrl = s.PhotoUrl
            }).ToList();
        }

        // POST api/sponsor
        private SponsorViewModel Create(SponsorViewModel sponsorViewModel)
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
                sponsorViewModel.Id = newSponsor.Id;
            }

            return sponsorViewModel;
        }

        // PUT api/sponsor/5
        public SponsorViewModel Post(int id, [FromBody]SponsorViewModel sponsorViewModel)
        {
            if (id == 0)
            {
                return Create(sponsorViewModel);
            }

            var sponsor = _applicationUnit.SponsoRepository.GetByID(id);
            sponsor.Name = sponsorViewModel.Name;
            _applicationUnit.SponsoRepository.Update(sponsor);
            _applicationUnit.SaveChanges();

            return sponsorViewModel;
        }

        // DELETE api/sponsor/5
        public void Delete(int id)
        {
            var sponsor = _applicationUnit.SponsoRepository.GetByID(id);

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
            var blobStorage = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobStorage.GetContainerReference("productimages");

            var imagesContainer = blobStorage.GetContainerReference("productimages");
            var oldImageToDelete = imagesContainer.GetBlockBlobReference(sponsor.PhotoUrl);
            oldImageToDelete.DeleteIfExists();

            _applicationUnit.SponsoRepository.Delete(sponsor);
            _applicationUnit.SaveChanges();
        }        
    }

    public class SponsorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int GroupId { get; set; }
        public bool IsManager { get; set; }
    }
}
