using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers.DataAccesssApi
{
    public class UsersController : ApiController
    {
        [HttpGet]
        public IEnumerable<UserDTO> GetAll()
        {
            using (var locationsEntity = new LocationEntities())
            {
                return (from user in locationsEntity.Users
                       select new UserDTO()
                       {
                           Id = user.Id,
                           Name = user.Username
                       }).ToList();
            }
        }
    }
}
