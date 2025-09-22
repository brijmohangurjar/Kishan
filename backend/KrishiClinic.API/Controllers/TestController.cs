using Microsoft.AspNetCore.Mvc;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> GetTest()
        {
            return Ok(new { 
                message = "API is working!", 
                timestamp = DateTime.UtcNow,
                status = "success" 
            });
        }

        [HttpGet("products")]
        public ActionResult<object> GetProductsTest()
        {
            var testProducts = new[]
            {
                new { id = 1, name = "Test Product 1", price = 100 },
                new { id = 2, name = "Test Product 2", price = 200 }
            };

            return Ok(new { 
                message = "Products endpoint is working!", 
                products = testProducts,
                count = testProducts.Length 
            });
        }
    }
}
