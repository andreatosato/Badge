using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badge.Web.Models.Register
{
    public class RegisterViewModel
    {
        public IFormFile AvatarImage { get; set; }
    }

}
