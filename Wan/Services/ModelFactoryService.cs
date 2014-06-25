using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;
using Wan.Controllers.ApiControllers;

namespace Wan.Services
{
    public class ModelFactoryService
    {
        public ModelFactoryService()
        {
            
        }

        public GroupViewModel Create(Group group)
        {
            return new GroupViewModel()
                {
                    Description = group.Description,
                    CreatedDate = group.CreatedDate,
                    GroupName = group.GroupName,
                    CreatedById = group.CreatedById,
                    Id = group.Id,
                    GroupImage = group.GroupImage ?? "/Content/images/defaultgroup.png",
                    GroupManagers = group.UserGroupRoles.Where(ugr => ugr.RoleId == (int)RoleTypes.GroupManager).Select(ur => new UserViewModel()
                    {
                        Id = ur.UserId
                    }).ToList(),
                    Events = group.Events.Select(e => new EventViewModel()
                    {
                        Id = e.Id,
                        Description = e.Description,
                        EventDateTime = e.EventDateTime,
                        EventLocation = e.EventLocation,
                        Name = e.Name,
                        Group = new GroupViewModel() { Id = e.GroupId },
                        Users = e.Users.Select(eu => new UserViewModel()
                        {
                            UserName = eu.UserName,
                            Id = eu.Id,
                            ProfileImage = eu.ProfileImage
                        }).ToList()
                    }).OrderBy(e => e.EventDateTime).ToList(),
                    Users = group.Users.Select(u => new UserViewModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        AboutMe = u.AboutMe,
                        ProfileImage = u.ProfileImage,
                        IsGroupManager = group.UserGroupRoles.SingleOrDefault(ugr => ugr.UserId == u.Id && ugr.RoleId == (int)RoleTypes.GroupManager) != null
                    }).ToList()
                };
        }
    }
}