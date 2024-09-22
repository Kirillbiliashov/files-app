using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.Http
{
    public class LoginUserBody
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}