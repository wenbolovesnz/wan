using FormBuilder.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Data.Contracts
{
    public interface IApplicationUnit : IDisposable
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Group> GroupRepository { get; }
        IGenericRepository<Event>   EventRepository { get; }
        IGenericRepository<Sponsor> SponsoRepository { get; }
        IGenericRepository<JoinGroupRequest> JoinGroupRequestRepository { get; }
        IGenericRepository<GroupPhoto> GroupPhotoRepository { get; }
        IGenericRepository<PersonalMessage> PersonalMessageRepository{ get; } 
        void SaveChanges();
    }
}
