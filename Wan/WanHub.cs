using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Wan.Controllers.ApiControllers;
using Wan.Models;
using WebMatrix.WebData;

namespace Wan
{
    [HubName("joinmeHub")]
    public class WanHub : Hub
    {
        private IApplicationUnit _applicationUnit;


        public void NewGroup(GroupViewModel groupViewModel)
        {
            Clients.All.showNewGroup(groupViewModel);
        }

        public void JoinGroup(GroupViewModel groupViewModel)
        {
            Groups.Add(Context.ConnectionId, groupViewModel.GroupName);
            //Clients.Group(groupViewModel.GroupName).newGroupMememberArrived(new { userId = WebSecurity.CurrentUserId, userName = WebSecurity.CurrentUserName });
            Clients.All.newGroupMememberArrived(new { userId = WebSecurity.CurrentUserId, userName = WebSecurity.CurrentUserName, groupId = groupViewModel.Id });                    
        }

        public void JoinGroupChat(GroupViewModel groupViewModel)
        {
            Groups.Add(Context.ConnectionId, groupViewModel.GroupName);
            Clients.Group(groupViewModel.GroupName).newMemberJoinedChat(new { userId = WebSecurity.CurrentUserId, userName = WebSecurity.CurrentUserName });
        }

        public void NewGroupMemember(int userId, string userName, string groupName)
        {
            
        }

        public void LeaveGroup(GroupViewModel groupViewModel)
        {
            Groups.Remove(Context.ConnectionId, groupViewModel.GroupName);
        }

        public void SendGroupMessage(MessageData data)
        {
            Clients.Group(data.GroupName).newGroupMessage(data.SendName + ": " +data.Message);
        }

        public void SendPersonalMessage(JoinGroupRequestViewModel viewModel)
        {
            Clients.All.newPersonalMessage(viewModel);
        }
    }

    public class MessageData
    {
        public string GroupName { get; set; }
        public string Message { get; set; }
        public string SendName { get; set; }
    }
}