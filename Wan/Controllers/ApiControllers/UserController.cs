using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FormBuilder.Data.Contracts;

namespace Wan.Controllers.ApiControllers
{
    public class UserController : ApiController
    {
        private IApplicationUnit _applicationUnit;


        public UserController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;           
        }

        public Object Get()
        {

            var users = _applicationUnit.UserRepository.Get();

            return users;

        }

        public UserViewModel GetUser(int id)
        {

            var user = _applicationUnit.UserRepository.Get(u => u.Id == id, null, "Events,Groups").First();


            return new UserViewModel()
            {
                AboutMe = user.AboutMe,
                Id = user.Id,
                ProfileImage = user.ProfileImage,
                UserName = user.UserName,
                NickName = user.NickName,
                Groups = user.Groups.Select(m => new GroupViewModel()
                {
                    GroupName = m.GroupName,
                    GroupImage = m.GroupImage,
                    Id = m.Id
                }).ToList()
            };
        }

    }
}
