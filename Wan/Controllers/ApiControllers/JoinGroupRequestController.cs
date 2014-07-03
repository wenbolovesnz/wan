using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class JoinGroupRequestController : ApiController
    {
        private IApplicationUnit _applicationUnit;

        public JoinGroupRequestController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;
        }

        [System.Web.Http.Authorize]
        [HttpPost]
        public JoinGroupRequest Post([FromBody] JoinGroupRequest joinGroupRequest)
        {
            joinGroupRequest.RequestedDate = System.DateTime.Now;
            joinGroupRequest.IsApproved = false;
            joinGroupRequest.IsProcessed = false;
            joinGroupRequest.UserId = WebSecurity.CurrentUserId;

            try
            {
                _applicationUnit.JoinGroupRequestRepository.Insert(joinGroupRequest);
                _applicationUnit.SaveChanges();
            }
            catch (Exception ex)
            {
                
                //todo log
                throw;
            }

            return joinGroupRequest;

        }

    }
}
