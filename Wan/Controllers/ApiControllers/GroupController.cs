using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;
using FormBuilder.Data.Contracts;
using Microsoft.Ajax.Utilities;
using Wan.Services;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class GroupController : ApiController
    {
        private IApplicationUnit _applicationUnit;

        public GroupController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;           
        }

        public List<GroupViewModel> Get()
        {
            var groups = _applicationUnit.GroupRepository.Get().ToList();

            var groupViewModels = groups.Select(m => new GroupViewModel()
                {
                    Description = m.Description,
                    CreatedDate = m.CreatedDate,
                    GroupName = m.GroupName,
                    Id = m.Id,
                    GroupImage = m.GroupImage != null ? Convert.ToBase64String(m.GroupImage): null,
                    Users = m.Users.Select(u => new UserViewModel()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            IsGroupManager = m.UserGroupRoles.SingleOrDefault(ugr => ugr.UserId == u.Id && ugr.RoleId == (int)RoleTypes.GroupManager) != null                            
                        }).ToList()
                }).ToList();

            return groupViewModels;
        }

        [System.Web.Http.Authorize]
        public GroupViewModel Create([FromBody] GroupViewModel groupViewModel)
        {
            var group = new Group();

            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
            group.GroupName = groupViewModel.GroupName;
            group.Description = groupViewModel.Description;
            group.Users.Add(currentUser);
            group.CreatedDate = DateTime.Now;

            var userGroupRole = new UserGroupRole();
            userGroupRole.User = currentUser;
            userGroupRole.Group = group;
            userGroupRole.Role =
                _applicationUnit.RoleRepository.Get(m => m.RoleId == (int)RoleTypes.GroupManager).First();
            group.UserGroupRoles.Add(userGroupRole);
            
            _applicationUnit.GroupRepository.Insert(group);
            _applicationUnit.SaveChanges();

            groupViewModel.Id = group.Id;

            groupViewModel.Users = group.Users.Select(m => new UserViewModel() {Id = m.Id, UserName = m.UserName}).ToList();
            groupViewModel.CreatedDate = group.CreatedDate;

            return groupViewModel;
        }

        [System.Web.Http.Authorize]
        public GroupViewModel UpdateGroup([FromBody] GroupViewModel groupViewModel)
        {

            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

            var group = _applicationUnit.GroupRepository.GetByID(groupViewModel.Id);
            if (!group.Users.Contains(currentUser))
            {
                group.Users.Add(currentUser);
            }

            _applicationUnit.GroupRepository.Update(group);
            _applicationUnit.SaveChanges();    

            return groupViewModel;
        }

        public object UploadImage()
        {
            var request = HttpContext.Current.Request;

            try
            {
                var uploadedFile = request.Files["uploadProfilePic"];
                var xFileName = request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;
                int groupId;
                int.TryParse(request.Form["groupId"], out groupId);
                var group = _applicationUnit.GroupRepository.GetByID(groupId);
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);


                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && request.ContentLength < 550000 && groupSecService.IsUserGroupManager(currentUser, group.UserGroupRoles.ToList()) )
                {
                    var contentType = request.ContentType;
                    var contentLength = request.ContentLength;

                    group.GroupImage = null;
                    group.GroupImage = new byte[contentLength];
                    group.ContentType = contentType;
                    inputStream.Read(group.GroupImage, 0, contentLength);

                    _applicationUnit.GroupRepository.Update(group);
                    _applicationUnit.SaveChanges();
                    return new { succeeded = true, imageFile = Convert.ToBase64String(group.GroupImage) };
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return new { succeeded = false };
            }

            return new { succeeded = false };
        }

    }




    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsGroupManager { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            Users = new List<UserViewModel>();
        }

        public ICollection<UserViewModel> Users { get; set; }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string GroupImage { get; set; }

    }
}
