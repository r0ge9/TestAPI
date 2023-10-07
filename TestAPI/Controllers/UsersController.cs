using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.RegularExpressions;
using TestAPI.Model.Entities;
using TestAPI.Model.Enums;
using TestAPI.Model.Parameters;
using TestAPI.Services;
using TestAPI.Services.Interfaces;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private Regex regex;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            regex = new Regex(@"\w\@gmail\.com");
        }

        /// <summary>
        ///     Get list of all users (Pagination, filters, sort included, but not required)
        /// </summary>
        /// <param name="userParameter">Parameters of selection</param>
        /// <remarks>If you dont want to set selection parameters (like sort, filtering
        /// ,pagination) leave it empty.
        /// Encode of sorting fields: 0-name,1-age,2-email,3-name of role</remarks>
        /// <returns>Returns List of Users</returns>
        /// <response code="404">If database or user not found</response>
        // GET: api/Users
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers([FromQuery] UserParameter userParameter)
        {
            var result= await _userService.GetUsers(userParameter);
          if (result == null)
          {
              return NotFound();
          }
            Log.Information("All users was found successfull: {@res}",result);
            return Ok(result);
        }


        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">ID of necessary user</param>
        /// <returns>Returns User</returns>
        /// <response code="400">Wrong value of ID</response>
        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!CheckId(id))
                return BadRequest();
            var result = await _userService.GetUserById(id);
          if (result == null)
          {
              return NotFound();
          }

            Log.Information("User {@user} was successfull found", result);
            return this.Ok(result);
        }

        /// <summary>
        /// Update necessary user by ID
        /// </summary>
        /// <param name="id">ID of User</param>
        /// <param name="name">Name of user</param>
        /// <param name="age">Age of user</param>
        /// <param name="email">Email of user (allow only gmail.com)</param>
        /// <remarks>If you dont want to update all parameters, leave them empty</remarks>
        /// <returns>Status of operation</returns>
        /// <response code="400">Wrong value of id or user not found</response>
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(int id, string? name,int age,string? email)
        {
            if(!CheckId(id))
                return BadRequest();
            var user = new User();
            user.Id = id;
            if(!string.IsNullOrEmpty(name))
                user.Name = name;
            if(age>0) 
                user.Age = age;
            if (!string.IsNullOrEmpty(email))
                if (regex.IsMatch(email))
                    user.Email = email;
                else
                    return Problem("Email is wrong (Allow only gmail.com)");
            
            if(await _userService.UpdateUser(id, user)==null)
                return BadRequest();
            Log.Information("Update for user {@user} was successfull", id);
            return Ok();
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="name">Name of user</param>
        /// <param name="age">Age of user</param>
        /// <param name="email">Email of user (allow only gmail.com)</param>
        /// <returns>Status of operation</returns>
        /// <response code="400">Wrond data</response>
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> CreateUser(string? name,int age,string? email)
        {
            if (!string.IsNullOrEmpty(name) && age > 0 && !string.IsNullOrEmpty(email))
            {
                
                if (!regex.IsMatch(email))
                    return Problem("Wrong email (Allow only gmail.com)");
                var result = await _userService.CreateUser(new User
                {
                    Name= name,
                    Age=age,
                    Email=email
                });

                if (result == null)
                {
                    return Problem("Entity set 'AppDbContext.Users'  is null.");
                }

                Log.Information("Create user was successfull");
                return Ok();
            }
            return BadRequest();
        }

        /// <summary>
        /// Add new role for user by id
        /// </summary>
        /// <param name="id">ID of necessary user</param>
        /// <param name="name">Name of role</param>
        /// <remarks>Encode of role names:
        /// 0-user,1-admin,2-support,3-Super admin</remarks>
        /// <returns>Status of operation</returns>
        /// <response code="400">Wrong value of id or role name</response>
        [HttpPost("{id}/{name}")]
       [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddRole(int id, RoleName name)
        {
            if(!CheckId(id))
                return BadRequest();
            
            var result=await _userService.AddRoleForUser(id, name);
            if (result == null)
                return NotFound();
            Log.Information("New role {@role} for user {@user} was successfull added", name,id);
            return Ok();
        }

        /// <summary>
        /// Delete user by ID
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>Status of operation</returns>
        /// <response code="400">Wrond value of id</response>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if(!CheckId(id))
                return BadRequest();
            var result = await _userService.DeleteUser(id);
            if (result == null)
            {
                return NotFound();
            }

            Log.Information("Delete user {@user} was successfull", id);
            return this.Ok();
        }

        private bool CheckId(int id)
        {
            if(id <= 0)
                return false;
            return true;
        }
    }
}
