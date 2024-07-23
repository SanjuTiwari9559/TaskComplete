using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Cat_ProMvc.APIServices;
using Task_Cat_ProMvc.Models.Data;
using Task_Cat_ProMvc.Models.viewModel;

namespace Task_Cat_ProMvc.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly ICategoryApi _categoryService;

        public CategoryApiController(ICategoryApi category)
        {
            this._categoryService = category;
        }

        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<Category>>> List(int pageNo = 1, int pageSize = 10)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(pageNo, pageSize);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null) return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult> Add( addCategoryRequest addCategoryRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var category = new Category
                {
                    Name = addCategoryRequest.Name,
                    IsActive = addCategoryRequest.IsActive,
                };
                await _categoryService.AddCategoryAsync(category);
                return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<ActionResult> Edit(int id,  editCategoryRequest editCategoryRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var category = new Category
                {
                    Id = editCategoryRequest.Id,
                    Name = editCategoryRequest.Name,
                    IsActive = editCategoryRequest.IsActive,
                };
                var result = await _categoryService.UpdateCategoryAsync(id, category);
                if (!result) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DaleteCategoryAsyn(id);
                if (!result) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Activate/{id}")]
        public async Task<ActionResult> Activate(int id)
        {
            try
            {
                await _categoryService.ActivateCategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Deactivate/{id}")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                await _categoryService.DeactivateCategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}

