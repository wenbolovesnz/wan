using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;
using FormBuilder.Data.Contracts;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Wan.Services;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class GroupController : ApiController
    {
        private IApplicationUnit _applicationUnit;
        private ModelFactoryService _modelFactoryService;

        public GroupController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;    
            _modelFactoryService = new ModelFactoryService();
        }

        public List<GroupViewModel> Get()
        {
            var groups = _applicationUnit.GroupRepository.Get().ToList();
            var groupViewModels = groups.Select(m => _modelFactoryService.Create(m)).ToList();
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
            group.CreatedById = currentUser.Id;

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
        [System.Web.Http.HttpPost]
        public GroupViewModel Post(int id, [FromBody] GroupViewModel groupViewModel)
        {
            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

            var group = _applicationUnit.GroupRepository.GetByID(groupViewModel.Id);
            var isCurrentUserManager = group.UserGroupRoles.SingleOrDefault(
                m => m.UserId == currentUser.Id && m.RoleId == (int) RoleTypes.GroupManager) != null;


            if (!group.Users.Contains(currentUser))
            {
                group.Users.Add(currentUser);
            }

            if (isCurrentUserManager)
            {
                group.GroupName = groupViewModel.GroupName;
                group.Description = groupViewModel.Description;

                this.updateGroupEvents(group, groupViewModel);
                this.updateGroupManagers(group, groupViewModel);
            }

            if (group.Users.Count > groupViewModel.Users.Count && isCurrentUserManager)
            {
                var userIdsToRemove = group.Users.Select(m => m.Id).Except(groupViewModel.Users.Select(m => m.Id)).ToList();
                foreach (var i in userIdsToRemove)
                {
                    var userToRemove = group.Users.Single(m => m.Id == i);
                    group.Users.Remove(userToRemove);
                }
            }

            _applicationUnit.GroupRepository.Update(group);
            _applicationUnit.SaveChanges();    

            return _modelFactoryService.Create(group);
        }

        private void updateGroupEvents(Group group, GroupViewModel groupViewModel)
        {
            var eventToCreate = groupViewModel.Events.SingleOrDefault(m => m.Id == 0);
            if (eventToCreate != null)
            {
                var newEvent = new Event();
                newEvent.Description = eventToCreate.Description;
                newEvent.EventDateTime = eventToCreate.EventDateTime;
                newEvent.EventLocation = eventToCreate.EventLocation;
                newEvent.GroupId = group.Id;
                newEvent.Name = eventToCreate.Name;
                group.Events.Add(newEvent);               
            }

            if (group.Events.Count < groupViewModel.Events.Count)
            {
                var eventIdsToRemove = group.Events.Select(m => m.Id).Except(groupViewModel.Events.Select(m => m.Id)).ToList();
                foreach (var i in eventIdsToRemove)
                {
                    var eventToRemove = group.Events.Single(m => m.Id == i);
                    group.Events.Remove(eventToRemove);
                }
            }         
        }

        private void updateGroupManagers(Group group, GroupViewModel groupViewModel)
        {
            if (group.UserGroupRoles.Count < groupViewModel.GroupManagers.Count)
            {
                var userIdsToAddToManagers =
                    groupViewModel.GroupManagers.Select(m => m.Id)
                                  .Except(group.UserGroupRoles.Select(m => m.UserId))
                                  .ToList();

                foreach (var userId in userIdsToAddToManagers)
                {
                    var newManagerConfig = new UserGroupRole();
                    newManagerConfig.UserId = userId;
                    newManagerConfig.GroupId = groupViewModel.Id;
                    newManagerConfig.RoleId = (int) RoleTypes.GroupManager;
                    group.UserGroupRoles.Add(newManagerConfig);
                }
            }

            if (group.UserGroupRoles.Count > groupViewModel.GroupManagers.Count)
            {
                var userIdsToRemove =
                            group.UserGroupRoles.Select(m => m.UserId)
                                          .Except(groupViewModel.GroupManagers.Select(m => m.Id))
                                          .ToList();

                foreach (var i in userIdsToRemove)
                {
                    var managerToRemove = group.UserGroupRoles.Single(m => m.UserId == i);
                    group.UserGroupRoles.Remove(managerToRemove);
                }

            }
        }

        [System.Web.Http.Authorize]
        public GroupViewModel CreateEvent([FromBody] EventViewModel eventViewModel)
        {
            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
            var group = _applicationUnit.GroupRepository.GetByID(eventViewModel.Group.Id);

            if (
                group.UserGroupRoles.SingleOrDefault(
                    m => m.UserId == currentUser.Id && m.RoleId == (int)RoleTypes.GroupManager) != null)
            {


            }


            return null;
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
                var contentType = request.ContentType;

                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && request.ContentLength < 550000 && groupSecService.IsUserGroupManager(currentUser, group.UserGroupRoles.ToList()) )
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);


                    var urlstring = blob.Uri.ToString();

                    if (group.GroupImage != null)
                    {
                        var imagesContainer = blobStorage.GetContainerReference("productimages");
                        var oldImageToDelete = imagesContainer.GetBlockBlobReference(group.GroupImage);
                        oldImageToDelete.DeleteIfExists();                        
                    }     

                    group.GroupImage = urlstring;
                    currentUser.ContentType = contentType;

                    _applicationUnit.GroupRepository.Update(group);
                    _applicationUnit.SaveChanges();

                    return new { succeeded = true, imageFile = urlstring };
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
        public string ProfileImage { get; set; }
        public string AboutMe { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            Users = new List<UserViewModel>();
            Events = new List<EventViewModel>();
        }

        public ICollection<UserViewModel> GroupManagers { get; set; } 
        public ICollection<UserViewModel> Users { get; set; }
        public ICollection<EventViewModel> Events { get; set; }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public string Description { get; set; }
        public string GroupImage { get; set; }

    }
}
