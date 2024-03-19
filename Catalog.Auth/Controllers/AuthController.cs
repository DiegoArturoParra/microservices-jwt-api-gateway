using Catalog.Auth.Extensions;
using Catalog.Auth.Infrastructure.Repository;
using Catalog.Auth.Model;
using Catalog.Auth.Services;
using Catalog.Auth.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Auth.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuth _auth;
        private readonly IUnitOfWork uow;

        public AuthController(ILogger<AuthController> logger, IAuth auth, IUnitOfWork uow)
        {
            _logger = logger;
            _auth = auth;
            this.uow = uow;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var login = await _auth.Authenticate(model.Email, model.Password);
            if (login is null)
            {
                return BadRequest(new
                {
                    Succeeded = false,
                    Message = "User not found"
                });
            }
            return Ok(new
            {
                Result = login,
                Succeeded = true,
                Message = "User logged in successfully"
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            try
            {
                await uow.BeginTransactionAsync();

                var user = new User
                {
                    Email = model.Email,
                    Fullname = model.Fullname,
                    Password = model.Password.Hash(),

                };
                await uow.Repository<User>().Add(user);
                var user2 = new User
                {
                    Email = model.Email,
                    Fullname = model.Fullname + "otro",
                    Password = model.Password.Hash(),

                };
                await uow.Repository<User>().Add(user2);
                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return Ok(new
                {
                    Succeeded = true,
                    Message = "User created successfully"
                });
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                return BadRequest(new
                {
                    Succeeded = false,
                    Message = ex.Message
                });
            }



        }
    }
}