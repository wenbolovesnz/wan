using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;

namespace Wan.Controllers.ApiControllers
{
    public class EventController : ApiController
    {
        private IApplicationUnit _applicationUnit;

        public EventController(IApplicationUnit applicationUnit )
        {
            _applicationUnit = applicationUnit;
        }

        public object Get()
        {
            return null;
        }

        public Event GetEvent(int id)
        {
            return new Event();
        }
    }
}
