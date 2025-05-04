
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Admin" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Role = "User" }
        };

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is required.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (_users.Any(u => u.Email == user.Email))
                {
                    return Conflict("A user with this email already exists.");
                }

                user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
                user.CreatedAt = DateTime.UtcNow;
                
                _users.Add(user);
                
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the user.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                if (updatedUser == null)
                {
                    return BadRequest("User data is required.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                if (_users.Any(u => u.Id != id && u.Email == updatedUser.Email))
                {
                    return Conflict("A user with this email already exists.");
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                _users.Remove(user);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
    }
}