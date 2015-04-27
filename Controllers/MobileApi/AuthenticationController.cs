using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Authenticate([FromBody]dynamic post)
        {
            if (post != null && post.username != null && post.password != null)
            {
                string username = post.username;
                string encryptedPassword = CalculateSHA1((string)post.password, Encoding.UTF8);
                
                using (var locationsDb = new LocationEntities())
                {
                    locationsDb.Database.Connection.Open();
                    var user = locationsDb.Users.FirstOrDefault(
                        u => u.Username == username && u.Password == encryptedPassword);

                    if (user != null)
                        return Ok(new UserDTO() { Id = user.Id, Name = user.Username });
                }
            }

            return Unauthorized();
        }

        private string CalculateSHA1(string text, Encoding enc)
        {
            byte[] buffer = enc.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
        }
    }
}
