using Application.Common.Responses;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Response> RegisterAsync(UserSignupDto userSignupDto);

        Task<AuthenticationResponse> LoginAsync(UserLoginDto userLoginDto);
    }
}
