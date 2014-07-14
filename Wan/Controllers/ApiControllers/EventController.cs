using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;
using FormBuilder.Data.Contracts;
using Newtonsoft.Json;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class EventController : ApiController
    {
        private IApplicationUnit _applicationUnit;

        public EventController(IApplicationUnit applicationUnit )
        {
            _applicationUnit = applicationUnit;
        }

        public object Get()
        {
            return null;
        }

        [System.Web.Http.AllowAnonymous]
        public EventViewModel GetEvent(int id)
        {
            var eventModel = _applicationUnit.EventRepository.Get(m => m.Id == id, null, "Users,Group").First();

            return new EventViewModel()
            {
                Id = eventModel.Id,
                Description = eventModel.Description,
                EventDateTime = eventModel.EventDateTime,
                EventLocation = eventModel.EventLocation,
                Name = eventModel.Name,
                CreatedById = eventModel.CreatedByUserId,                
                Users = eventModel.Users.Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    ProfileImage = u.ProfileImage
                }).ToList(),
                Group = new GroupViewModel()
                {
                    Id = eventModel.GroupId,
                    GroupName = eventModel.Group.GroupName
                }
                
            };            
        }
        [System.Web.Http.Authorize]
        [HttpPost]
        public HttpResponseMessage Post(int eventId, [FromUri]int userId, [FromBody]EventViewModel eventViewModel)
        {
            try
            {
                var userToAdd = _applicationUnit.UserRepository.GetByID(userId);
                if (userToAdd == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not find user");
                }                     

                var eventToUpdate = _applicationUnit.EventRepository.GetByID(eventViewModel.Id);
                if (eventToUpdate == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not find Event");
                }

                eventToUpdate.Users.Add(userToAdd);
                _applicationUnit.EventRepository.Update(eventToUpdate);
                _applicationUnit.SaveChanges();                    
                return Request.CreateResponse(HttpStatusCode.OK);                                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }

    public class EventViewModel
    {
        public EventViewModel()
        {
            Users = new List<UserViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public GroupViewModel Group { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventLocation { get; set; }
        public string Description { get; set; }
        public IList<UserViewModel> Users { get; set; }
        public int CreatedById { get; set; }
    }
}
