using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Web.Models;
using Microsoft.EntityFrameworkCore;
using static Web.Models.DbTables;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SharedLibrary;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<IEnumerable<EmployeeReport>>> Login([FromBody] SharedLibrary.LoginModel model)
        {
            /*
            The ModelState.IsValid property is part of the ASP.NET MVC framework and is used to check
            whether the model passed to the action has passed validation. 
            */
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Check if username and password are located in database
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == model.UserName && e.Password == model.Password);

            if (employee == null)
            {
                // Authentication failed
                // Return a 401 Unauthorized response with JSON content
                return Unauthorized(new ContentResult
                {
                    Content = JsonConvert.SerializeObject("Invalid credentials."),
                    ContentType = "application/json",
                    StatusCode = 401
                });
            }

            // Generate JWT
            var token = GenerateJwtToken(employee);

            EmployeeReport employeeReport = MapEmployeeToEmployeeReport(employee);

            return Ok(new { Token = token, EmployeeReport = employeeReport });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the employee exists
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmpId == model.EmpId);

            if (employee == null)
            {
                return NotFound($"Employee with ID {model.EmpId} not found.");
            }

            // Check if the old password matches the current password
            if (employee.Password != model.OldPassword)
            {
                return BadRequest("Invalid old password.");
            }

            // Check the new password strength
            if (!IsStrongPassword(model.NewPassword))
            {
                return BadRequest("New password does not meet the required strength criteria.");
            }

            // Change the password
            employee.Password = model.NewPassword;
            employee.PassNotChanged = false;

            // Persist changes to the database
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        #region Helper methods
        private string GenerateJwtToken(Employee employee)
        {
            var issuer = "issuer";
            var audience = "audiance";
            var key = Encoding.ASCII.GetBytes("B+wNnFU7eT2Gu/D7jFB34exogIlMd8BwKasdfjalsdkfjalsdkfjalskdfjlsakdfjalskjfdasldkfjasdfasdfasdfasdfasdfasEEQKKjintc=");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.EmpId.ToString()),
                    new Claim(ClaimTypes.Name, employee.Username),
            }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        private EmployeeReport MapEmployeeToEmployeeReport(Employee employee)
        {
            EmployeeReport employeeReport = new EmployeeReport
            {
                EmpId = employee.EmpId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
                BirthDate = employee.BirthDate,
                HireDate = employee.HireDate,
                Address = employee.Address,
                City = employee.City,
                Region = employee.Region,
                PostalCode = employee.PostalCode,
                Country = employee.Country,
                Phone = employee.Phone,
                Role = employee.Role,
                Username = employee.Username,
                PassNotChanged = employee.PassNotChanged,
                PhotoName = employee.PhotoName
            };

            var manager = _context.Employees
                .Where(e => e.EmpId == employee.MgrId)
                .Select(e => new { e.FirstName, e.LastName })
                .FirstOrDefault();

            if (manager != null)
            {
                employeeReport.Manager = $"{manager.FirstName} {manager.LastName}";
            }
            else
            {
                employeeReport.Manager = "Unknown Manager";
            }

            return employeeReport;
        }
        private bool IsStrongPassword(string password)
        {
            // Minimum length of 8 characters
            if (password.Length < 8)
            {
                return false;
            }

            // At least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // At least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // At least one digit or special character
            if (!password.Any(char.IsDigit) && !password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}