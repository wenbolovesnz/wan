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

    }
}
