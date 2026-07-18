using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Responses
{
    public class AuthenticationResponse : Response
    {
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
