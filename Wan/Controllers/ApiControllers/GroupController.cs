using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
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
                    Users = m.Users.Select(u => new UserViewModel()
                        {
                            Id = u.Id,
                            UserName = u.UserName
                        }).ToList()
                }).ToList();

            return groupViewModels;
        }

        public GroupViewModel Post([FromBody] GroupViewModel groupViewModel)
        {
            var group = new Group();

            group.GroupName = groupViewModel.GroupName;
            group.Description = groupViewModel.Description;
            group.Users.Add(_applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId));
            group.CreatedDate = DateTime.Now;
            
            _applicationUnit.GroupRepository.Insert(group);
            _applicationUnit.SaveChanges();

            groupViewModel.Id = group.Id;

            groupViewModel.Users = group.Users.Select(m => new UserViewModel() {Id = m.Id, UserName = m.UserName}).ToList();
            groupViewModel.CreatedDate = group.CreatedDate;

            return groupViewModel;
        }

    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
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

    }
}
