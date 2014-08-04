using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;

namespace Wan.Services
{
    public static class PersonalMessageService
    {
        public static void CreateMessageFor(int userId, int currentUserId, string content, IApplicationUnit applicationUnit)
        {
            var newMessage = new PersonalMessage()
            {
                Content = content,
                CreatedByUserId = currentUserId,
                UserId = userId,
                CreatedDate = System.DateTime.Now,
                IsRead = false
            };

            applicationUnit.PersonalMessageRepository.Insert(newMessage);
        }
    }
}