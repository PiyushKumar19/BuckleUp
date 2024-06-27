using BuckleUp.DTOs;
using BuckleUp.Models;

namespace BuckleUp.InterfaceAndService
{
    public interface IAuthService
    {
        public Task<User> Register(UserDto request);
        public Task<string> Login(UserDto request);
    }
}
