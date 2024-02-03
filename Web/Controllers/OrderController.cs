using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using SharedLibrary;
using Web.Services;
using System.Reflection.Metadata.Ecma335;

namespace Web.Controllers
{
    [Authorize]
    [Route("api/order")] //[controller]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly OrderService _editOrderService;

        public OrderController(OrderService editOrderService) //ApplicationContext context,
        {
            //_context = context;
            _editOrderService = editOrderService;
        }

        [HttpGet("get-orders")]
        public async Task<ActionResult<IEnumerable<OrderReport>>> GetOrders([FromQuery] OrderQueryModel orderQuery)
        {
            try
            {
                var orders = await _editOrderService.GetOrdersAsync(orderQuery);

                if (orders != null)
                {
                    return orders;
                }
                else
                {
                    return NotFound();
                }    
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpGet("get-products-for-dropdown")]
        public async Task<IActionResult> GetProductsForDropdownAsync()
        {
            try
            {
                var products = await _editOrderService.GetProductsForDropdownAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpGet("get-order-details")]
        public async Task<IActionResult> GetOrderDetails([FromQuery] OrderEditReport order)
        {
            try
            {
                var orderDetails = await _editOrderService.GetOrderDetailsAsync(order);
                return Ok(orderDetails);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpPut("update-order")]
        public async Task<IActionResult> UpdateOrder(OrderEditReport order)
        {
            try
            {
                var result = await _editOrderService.UpdateOrderAsync(order);

                if (result)
                {
                    return Ok("Order updated successfully");
                }
                else
                {
                    return NotFound("Order not found");
                }
            }
            //catch (InvalidOperationException ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpPut("create-order")]
        public async Task<IActionResult> CreateOrder(OrderEditReport order)
        {
            try
            {
                var result = await _editOrderService.CreateOrderAsync(order);

                if (result != 0)
                {
                    return Ok(result);
                }
                else 
                {
                    return BadRequest("Failed to create order. Check if query details are filled out correctly.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, String.Format("Internal Server Error! Details:{0}", ex.Message));
            }
        }

        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct([FromQuery] int orderId, [FromQuery] int productId)
        {
            try
            {
                var result = await _editOrderService.DeleteProductFromOrder(orderId, productId);

                if (result)
                {
                    return Ok("Part deleted successfully");
                }

                return NotFound("Part not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error!");
            }
        }

        [HttpDelete("delete-order")]
        public async Task<IActionResult> DeleteOrder([FromQuery] OrderEditReport order)
        {
            try
            {
                var result = await _editOrderService.DeleteOrderAsync(order);

                return result switch
                {
                    0 => Ok("Order deleted successfully"),
                    1 => NotFound("Order not found"),
                    2 => BadRequest("Order already shipped"),
                    _ => BadRequest(),
                };
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error!");
            }
        }
    }
}
