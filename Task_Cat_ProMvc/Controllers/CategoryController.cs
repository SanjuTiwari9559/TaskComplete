

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using Task_Cat_ProMvc.Models;
using Task_Cat_ProMvc.Models.Data;
using Task_Cat_ProMvc.Models.viewModel;
using Task_Cat_ProMvc.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task_Cat_ProMvc.Controllers
{
    public class CategoryController : Controller
    {

        //Uri baseUrl = new Uri("https://localhost:7220/api/Category");
        private readonly ICategoryMvc _categoryService;
        private readonly HttpClient _client;
        private readonly string _apiBaseUrl;
        public bool _useApi;

        public Cat_ProductDbContext Cat_ProductDb { get; }

        //private readonly Cat_ProductDbContext cat_ProductDbContext;
        // private readonly bool _useApi=true ; 
        // private readonly bool _useMvc=true ;
        // private readonly bool apiaction;





        public CategoryController(ICategoryMvc category,Cat_ProductDbContext cat_ProductDb, HttpClient client, IConfiguration configuration)
        {
            this._categoryService = category;
            Cat_ProductDb = cat_ProductDb;
            this._client = client;
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");

        }

       


        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List(int pageNo = 1, int PageSize = 20)
        {
            //var APIorMVC= cat_ProductDbContext.CallApi.Where(x => x.IsActive == true);
            //  if(APIorMVC.Any())
            // {
            //var response = await client.GetAsync($"{baseUrl}?pageNo={pageNo}&pageSize={PageSize}");
            //if (response.IsSuccessStatusCode)
            //{
            //    var jsonData = await response.Content.ReadAsStringAsync();
            //    var newcategory = JsonConvert.DeserializeObject<List<Category>>(jsonData);
            //    return View(newcategory);
            //}

            _useApi =  await _categoryService.SetUseApi("ApiController");
            if (_useApi)
            {
                var response = await _client.GetAsync($"{_apiBaseUrl}/CategoryApi/List?pageNo={pageNo}&pageSize={PageSize}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var convert = JsonConvert.DeserializeObject<IEnumerable<Category>>(content);
                 return View(convert);
            }
            
            

                //var categories = await _categoryService.GetAllCategoriesAsync(pageNo, PageSize);
                return View(await _categoryService.GetAllCategoriesAsync(pageNo, PageSize));

            
        }
              

            
        


        //var categories = await newcategory.GetAllCategoriesAsync(pageNo, PageSize);
        //HttpResponseMessage responce = client.GetAsync(client.BaseAddress + "/Category/GetCategoriesAsync").Result;
        //if (responce.IsSuccessStatusCode)
        //{
        //    string Data = responce.Content.ReadAsStringAsync().Result;

        //    return View(JsonConvert.DeserializeObject<List<Product>>(Data));
        //}
        // return View(categories);





        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(addCategoryRequest addCategoryRequest)
        {
            if (!ModelState.IsValid) return View(addCategoryRequest);

            try
            {
                var category = new Category
                {
                    Name = addCategoryRequest.Name,
                    IsActive = addCategoryRequest.IsActive,
                };
                _useApi = await _categoryService.SetUseApi("ApiController");
                if (_useApi)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync($"{_apiBaseUrl}/CategoryApi/Add", content);
                    return RedirectToAction("List");
                }
                else
                {
                    await _categoryService.AddCategoryAsync(category);
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return View(ex.Message);
            }
        }

    
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            _useApi = await _categoryService.SetUseApi("ApiController");
            if (_useApi)
            {
                var response = await _client.GetAsync($"{_apiBaseUrl}/CategoryApi/Get/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                View( JsonConvert.DeserializeObject<Category>(content));
            }
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null) return NotFound();

                var editRequest = new editCategoryRequest
                {
                    Id = id,
                    IsActive = category.IsActive,
                    Name = category.Name,
                };
                return View(editRequest);
            }
            catch (Exception ex)
            {
               
                return View(ex.Message);
            }
        }
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(editCategoryRequest editCategoryRequest)
        {
            if (!ModelState.IsValid) return View(editCategoryRequest);

            try
            {
                var category = new Category
                {
                    Id = editCategoryRequest.Id,
                    Name = editCategoryRequest.Name,
                    IsActive = editCategoryRequest.IsActive,
                };
                _useApi = await _categoryService.SetUseApi("ApiController");
                if (_useApi)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    var response = await _client.PutAsync($"{_apiBaseUrl}/CategoryApi/Edit/{category.Id}", content);
                    RedirectToAction("List");
                }
                var result = await _categoryService.UpdateCategoryAsync(editCategoryRequest.Id, category);
                if (!result) return NotFound();

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                
                return View(ex.Message);
            }
        }



        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(DeleteCategory deleteViewModel)
        {
            _useApi = await _categoryService.SetUseApi("ApiController");

            if (_useApi)
            {
                var response = await _client.DeleteAsync($"{_apiBaseUrl}/CategoryApi/Delete/{deleteViewModel.Id}");
                return RedirectToAction("List");
            }
            try
            {
                var result = await _categoryService.DaleteCategoryAsyn(deleteViewModel.Id);
                if (!result) return NotFound();

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                
                return View(ex.Message);
            }
        }
    }
}
      
    

