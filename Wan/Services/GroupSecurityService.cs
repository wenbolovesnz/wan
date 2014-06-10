using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.Business.Entities;
using FormBuilder.Business.Entities.Enums;

namespace Wan.Services
{
    public class GroupSecurityService
    {
        public bool IsUserGroupManager(User user, List<UserGroupRole> userGroupRoles )
        {
            var userFound = userGroupRoles.SingleOrDefault(m => m.UserId == user.Id && m.RoleId == (int) RoleTypes.GroupManager);

            return userFound != null;
        }
    }
}