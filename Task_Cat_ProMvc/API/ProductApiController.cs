using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Cat_ProMvc.APIServices;
using Task_Cat_ProMvc.Models.Data;
using Task_Cat_ProMvc.Models.viewModel;

namespace Task_Cat_ProMvc.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductApi _productService;
        private readonly ICategoryApi _categoryService;
        private readonly ILogger<ProductApiController> _logger;

        public ProductApiController(IProductApi productApi, ICategoryApi categoryApi, ILogger<ProductApiController> logger)
        {



            this._productService = productApi;
            this._categoryService = categoryApi;
            this._logger = logger;
        }
        [HttpGet("List")]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetAllAsync(pageNumber, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product list.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddProductRequest addProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = new Product
                {
                    Name = addProduct.Name,
                    IsActive = addProduct.IsActive,
                    CategoryId = addProduct.CategoryId
                };

                await _productService.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("GetById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("Edit/{id}")]

        [HttpPut]

        public async Task<IActionResult> Edit(int id, [FromBody] EditProductRequest editProduct)
        {
            _logger.LogInformation($"Received PUT request to edit product with ID {id}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = new Product
                {
                    Id = editProduct.Id,
                    Name = editProduct.Name,
                    CategoryId = editProduct.CategoryId,
                    IsActive = editProduct.IsActive
                };

                var result = await _productService.UpdateAsync(product, id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                return StatusCode(500, "Internal server error");
            }
        }
        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Received DELETE request to remove product with ID {id}");

            try
            {
                var result = await _productService.DeleteAsync(id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}


        



