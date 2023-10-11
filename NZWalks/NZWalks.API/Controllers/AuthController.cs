using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller

    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenHandler tokenHandler;

        public AuthController(IUserRepository userRepository, ITokenHandler tokenHandler)
        {
            this._userRepository = userRepository;
            this.tokenHandler = tokenHandler;
        }

        public IUserRepository UserRepository { get; }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(Models.DTO.LoginRequest loginRequest)
        {
            //Validate the incoming request
            if(! await ValidateUserAsync(loginRequest))
            {
                return BadRequest(ModelState);
            }
            //Check if user is authenticated

           var user = await  _userRepository.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

            if (user !=null)
            {
                //Generate a JWT Token
             var token =  await tokenHandler.CreateTokenAsync(user);
                return Ok(token);
            }

            return BadRequest("USername or PAssword is incorrect");
        }

        #region Private Methods

        private async Task<bool> ValidateUserAsync(Models.DTO.LoginRequest loginRequest)
        {

            if (loginRequest == null)
            {
                
                    ModelState.AddModelError(nameof(loginRequest), $"Add Region Data is required.");
                    return false;
                

            }
            if (string.IsNullOrWhiteSpace(loginRequest.Username))
            {
                ModelState.AddModelError(nameof(loginRequest.Username), $"{nameof(loginRequest.Username)} cannot be null or empty or white space.");

            }

            if (string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                ModelState.AddModelError(nameof(loginRequest.Password), $"{nameof(loginRequest.Password)} cannot be null or empty or white space.");

            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
