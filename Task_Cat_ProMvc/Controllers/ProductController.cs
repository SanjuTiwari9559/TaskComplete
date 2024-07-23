using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Task_Cat_ProMvc.Models.Data;
using Task_Cat_ProMvc.Services;
using System.Threading.Tasks;
using System.Linq;
using Task_Cat_ProMvc.Models.viewModel;
using Task_Cat_ProMvc.APIServices;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Task_Cat_ProMvc.Controllers
{
    public class ProductController : Controller
    {


        private readonly IProductMvc _productService;
        private readonly ICategoryApi _categoryService;
        private readonly ILogger<ProductController> _logger;
        private readonly HttpClient _client;
        private readonly string _apiBaseUrl;
        public bool _useApi;



        public ProductController(IProductMvc productService, ICategoryApi categoryService, ILogger<ProductController> logger, HttpClient client, IConfiguration configuration)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            this._client = client;
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }
        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 10)
        {

            if (await _productService.SetUseApi("ApiController"))
            {
                var response = await _client.GetAsync($"{_apiBaseUrl}/ProductApi/List?pageNumber={pageNumber}&pageSize={pageSize}");
                //response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(content));
            }
            else
            {
                try
                {
                    var products = await _productService.GetAllAsync(pageNumber, pageSize);
                    return View(products);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching product list.");
                    return View("Error");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {

            try
            {
                if (await _productService.SetUseApi("ApiController"))
                {
                    //var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    var response = await _client.GetAsync($"{_apiBaseUrl}/CategoryApi/List?pageNumber={1}&pageSize={int.MaxValue}");
                    var content = await response.Content.ReadAsStringAsync();
                    var cat = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);
                    var addProductRequest = new AddProductRequest
                    {
                        Categories = cat.Select(c => new SelectListItem
                        {
                            Text = c.Name,
                            Value = c.Id.ToString()
                        }).ToList()
                    };
                    return View(addProductRequest);

                }

                //var categories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue);
                // var addProductRequest = new AddProductRequest
                //{
                //    Categories = categories.Select(c => new SelectListItem
                //    {
                //        Text = c.Name,
                //        Value = c.Id.ToString()
                //    }).ToList()
                //};


                else
                {
                    var categories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue);
                    var addProductRequest = new AddProductRequest
                    {
                        Categories = categories.Select(c => new SelectListItem
                        {
                            Text = c.Name,
                            Value = c.Id.ToString()
                        }).ToList()
                    };
                    return View(addProductRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories for add view.");
                return View("Error");
            }
        }
        [HttpPost]

        public async Task<IActionResult> Add(AddProductRequest addProduct)
        {
            //if (!ModelState.IsValid)
            //{
            //    try
            //    {
            //        var categories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue);
            //        addProduct.Categories = categories.Select(c => new SelectListItem
            //        {
            //            Text = c.Name,
            //            Value = c.Id.ToString()
            //        }).ToList();
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Error fetching categories for add view.");
            //        return View("Error");
            //    }
            //    return View(addProduct);
            //}

            try
            {
                var product = new Product
                {
                    Name = addProduct.Name,
                    IsActive = addProduct.IsActive,
                    CategoryId = addProduct.CategoryId
                };
                if (await _productService.SetUseApi("ApiController"))
                {
                    var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync($"{_apiBaseUrl}/ProductApi/Add", content);
                    return RedirectToAction("List");
                }
                else
                {


                    await _productService.AddAsync(product);
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product.");
                return View("Error");
            }
        }
    
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var categories = await _categoryService.GetAllCategoriesAsync(1, int.MaxValue);
                var editProduct = new EditProductRequest
                {
                    CategoryId = product.CategoryId,
                    Id = id,
                    IsActive = product.IsActive,
                    Name = product.Name,
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToList()
                };

                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(editProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product for edit view.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductRequest editProduct)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(editProduct);
            //}

            try
            {
                var product = new Product
                {
                    Id = editProduct.Id,
                    Name = editProduct.Name,
                    CategoryId = editProduct.CategoryId,
                    IsActive = editProduct.IsActive
                };

                var result = await _productService.UpdateAsync(editProduct.Id, product);
                if (result)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update product.");
                    return View(editProduct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(deleteProductRequest deleteProduct)
        {
            try

            {
                if ( await _productService.SetUseApi("ApiController"))
                {
                    var response = await _client.DeleteAsync($"{_apiBaseUrl}/ProductApi/Delete/{deleteProduct.Id}");
                    return RedirectToAction("List");
                }
                else
                {
                    var result = await _productService.DeleteAsync(deleteProduct.Id);
                    if (result)
                    {
                        return RedirectToAction("List");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to delete product.");
                        return RedirectToAction("List");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                return View("Error");
            }
        }
       
    }
}