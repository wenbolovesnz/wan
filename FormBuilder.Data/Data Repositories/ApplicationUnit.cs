using System;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;

namespace FormBuilder.Data
{
    public class ApplicationUnit: IApplicationUnit
    {
        private FormBuilderContext _context;

        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Role> _roleRepository;
        private IGenericRepository<Group> _groupRepository;
        private IGenericRepository<Event> _eventRepository;

        public ApplicationUnit(IGenericRepository<User> userRepository, 
                               IGenericRepository<Role> roleRepository,
                               IGenericRepository<Group> groupRepository,
                               IGenericRepository<Event> eventRepository,
                               FormBuilderContext formBuilderContext)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _groupRepository = groupRepository;
            _eventRepository = eventRepository;
            _context = formBuilderContext;
        }

        public IGenericRepository<Event> EventRepository
        {
            get { return this._eventRepository; }
        }

        public IGenericRepository<Group> GroupRepository
        {
            get { return this._groupRepository; }
        }

        public IGenericRepository<User> UserRepository
        {
            get { return this._userRepository; }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get { return this._roleRepository; }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
