using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using Wan.Models;
using Wan.Services;
using WebMatrix.WebData;

namespace Wan.Controllers.ApiControllers
{
    public class JoinGroupRequestController : ApiController
    {
        private IApplicationUnit _applicationUnit;
        private ModelFactoryService _modelFactory;

        public JoinGroupRequestController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;
            _modelFactory = new ModelFactoryService();
        }

        [Authorize]
        public IList<JoinGroupRequestViewModel> Get()
        {
            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

            var groupsWithinManagerRole = currentUser.Groups.Where(m => m.UserGroupRoles.Select(ugr => ugr.UserId).Contains(currentUser.Id)).Select(g => g.Id);

            var joinGroupRequestViewModels =
                                _applicationUnit.JoinGroupRequestRepository.Get(
                    m => !m.IsProcessed & groupsWithinManagerRole.Contains(m.GroupId), null, "User").Select(jgr => _modelFactory.Create(jgr)).OrderByDescending(a => a.RequestedDate).ToList();
            return joinGroupRequestViewModels;
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

        [System.Web.Http.Authorize]
        [HttpPost]
        public JoinGroupRequestViewModel Update(int id, [FromBody] JoinGroupRequestViewModel joinGroupRequestViewModel)
        {

            var joinGroupRequest = _applicationUnit.JoinGroupRequestRepository.Get(m => m.Id == id, null, "User").First();
            var group = _applicationUnit.GroupRepository.GetByID(joinGroupRequest.GroupId);

            if (group.UserGroupRoles.Select(m => m.UserId).Contains(WebSecurity.CurrentUserId))
            {
                try
                {

                    joinGroupRequest.IsApproved = joinGroupRequestViewModel.IsApproved;
                    joinGroupRequest.IsProcessed = true;
                    joinGroupRequest.DecisionDate = System.DateTime.Now;
                    joinGroupRequest.DecisionUserId = WebSecurity.CurrentUserId;
                    joinGroupRequest.DeclineReason = joinGroupRequestViewModel.DeclineReason;

                    if (joinGroupRequest.IsApproved)
                    {
                        var user = _applicationUnit.UserRepository.GetByID(joinGroupRequest.UserId);

                        group.Users.Add(user);
                        _applicationUnit.GroupRepository.Update(group);
                    }

                    _applicationUnit.JoinGroupRequestRepository.Update(joinGroupRequest);
                    _applicationUnit.SaveChanges();
                }
                catch (Exception ex)
                {

                    //todo log
                    throw;
                }
    
            }
            
            return _modelFactory.Create(joinGroupRequest);

        }

    }
}
