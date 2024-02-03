using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using Web.Services;

namespace Web.Controllers
{
    [Authorize]
    [Route("api/customer")]//[controller]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer(CustomerReport customer)
        {
            try
            {
                var result = await _customerService.UpdateCustomer(customer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpPut("create-customer")]
        public async Task<IActionResult> CreateCustomer(CustomerReport customer)
        {
            try
            {
                var result = await _customerService.CreateCustomer(customer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpGet("get-customers")]
        public async Task<IActionResult> GetListOfCustomers()
        {
            try
            {
                var customers = await _customerService.GetListOfCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }
    }
}
