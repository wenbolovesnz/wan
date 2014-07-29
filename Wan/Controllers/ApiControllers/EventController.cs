using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
            var eventModel = _applicationUnit.EventRepository.Get(m => m.Id == id, null, "Users,Group,EventMessages,Sponsors").First();

            return new EventViewModel()
            {
                Id = eventModel.Id,
                Description = eventModel.Description,
                EventDateTime = eventModel.EventDateTime,
                EventLocation = eventModel.EventLocation,
                Name = eventModel.Name,
                CreatedById = eventModel.CreatedByUserId,                
                Sponsors = eventModel.Sponsors.Select(s => new SponsorViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PhotoUrl = s.PhotoUrl
                }).ToList(),
                Users = eventModel.Users.Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    ProfileImage = u.ProfileImage
                }).ToList(),
                Group = new GroupViewModel()
                {
                    Id = eventModel.GroupId,
                    GroupName = eventModel.Group.GroupName,
                    Users = eventModel.Group.Users.Select( gu => new UserViewModel()
                    {
                        Id = gu.Id,
                        UserName = gu.UserName,
                        ProfileImage = gu.ProfileImage
                    }).ToList(),

                    GroupManagers = eventModel.Group.UserGroupRoles.Select(gm => new UserViewModel()
                    {
                        Id = gm.Id
                    }).ToList()
                },
                EventMessages = eventModel.EventMessages.Select(e => new EventMessageViewModel()
                {
                    Id = e.Id,
                    User = new UserViewModel()
                    {
                        UserName = e.User.UserName,
                        Id = e.User.Id,
                        ProfileImage = e.User.ProfileImage
                    },
                    CreatedDate = e.CreatedDate,
                    Message = e.Message                    
                }).ToList()                
            };            
        }

        [System.Web.Http.Authorize]
        [HttpPost]
        public HttpResponseMessage Post(int id, [FromBody]EventViewModel eventViewModel)
        {
            try
            {
                var currentUserId = WebSecurity.CurrentUserId;

                var eventToUpdate = _applicationUnit.EventRepository.Get(e => e.Id == id, null, "Group, Users, EventMessages").First();
                if (eventToUpdate == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not find Event");
                }

                if (eventToUpdate.Group.UserGroupRoles.Select(m => m.Id).Contains(currentUserId))
                {
                    eventToUpdate.Name = eventViewModel.Name;
                    eventToUpdate.Description = eventViewModel.Description;
                    eventToUpdate.EventLocation = eventViewModel.EventLocation;
                    eventToUpdate.EventDateTime = eventViewModel.EventDateTime;

                    eventToUpdate.Sponsors.Clear();

                    foreach (var sponsorToAdd in eventViewModel.Sponsors)
                    {
                        var itemToAdd = _applicationUnit.SponsoRepository.GetByID(sponsorToAdd.Id);
                        eventToUpdate.Sponsors.Add(itemToAdd);
                    }

                }

                if (eventToUpdate.Group.Users.Select(u => u.Id).Contains(currentUserId))
                {
                    if (eventToUpdate.EventMessages.Count < eventViewModel.EventMessages.Count)
                    {
                        var messageToAdd = eventViewModel.EventMessages.SingleOrDefault(m => m.Id == 0);
                        if (messageToAdd != null)
                        {
                            var newMessageToAdd = new EventMessage()
                            {
                                EventId = eventToUpdate.Id,
                                CreatedDate = System.DateTime.Now,
                                Message = messageToAdd.Message,
                                UserId = currentUserId
                            };

                            eventToUpdate.EventMessages.Add(newMessageToAdd);
                        }
                    }
                }
                
                _applicationUnit.EventRepository.Update(eventToUpdate);
                _applicationUnit.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [System.Web.Http.Authorize]
        [HttpPost]
        public HttpResponseMessage Post(int eventId, [FromUri]int? userId, [FromBody]EventViewModel eventViewModel)
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
                if (!eventToUpdate.Users.Contains(userToAdd))
                {
                    eventToUpdate.Users.Add(userToAdd);    
                }
                
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
            EventMessages = new List<EventMessageViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public GroupViewModel Group { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventLocation { get; set; }
        public string Description { get; set; }
        public IList<UserViewModel> Users { get; set; }
        public IList<EventMessageViewModel> EventMessages { get; set; }
        public IList<SponsorViewModel> Sponsors { get; set; }
        public int CreatedById { get; set; }
    }

    public class EventMessageViewModel
    {
        public EventMessageViewModel()
        {
           
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserViewModel User { get; set; }

    }
}
