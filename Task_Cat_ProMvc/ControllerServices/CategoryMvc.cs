using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.Services
{
    public class CategoryMvc : ICategoryMvc
    {
        private readonly Cat_ProductDbContext cat_ProductDb;
        private readonly HttpClient _client;
        private  bool _useApi;
        private readonly string _apiBaseUrl;

        public CategoryMvc(Cat_ProductDbContext cat_ProductDb, HttpClient client, IConfiguration configuration)
        {
            this.cat_ProductDb = cat_ProductDb;
            _client = client;
            //_useApi = configuration.GetValue<bool>("UseApi");
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        }

        //private void SetUseApi(string controller)
        //{
        //    var api = cat_ProductDb.ApiCallings.Where(a => a.ApiName == controller).FirstOrDefault();
        //    _useApi = api.IsActived;
        //}

        public async Task ActivateCategoryAsync(int id)
        {
           // SetUseApi("ApiController");


            //if (_useApi)
            //{
            //    var response = await _client.PutAsync($"{_apiBaseUrl}/CategoryApi/Activate/{id}", null);
            //    response.EnsureSuccessStatusCode();
            //}
            //else
            //{

                var category = await cat_ProductDb.Categories.FindAsync(id);
                if (category != null)
                {
                    category.IsActive = true;
                    await cat_ProductDb.SaveChangesAsync();
                    await ActivateProductsByCategoryIdAsync(id);

                }
            }
        

        public  async Task AddCategoryAsync(Category addCategory)
        {
            //SetUseApi("ApiController");

            //if (_useApi)
            //{
            //    var content = new StringContent(JsonConvert.SerializeObject(addCategory), Encoding.UTF8, "application/json");
            //    var response = await _client.PostAsync($"{_apiBaseUrl}/CategoryApi/Add", content);

            //}
            //else
            //{

                await cat_ProductDb.Categories.AddAsync(addCategory);
                await cat_ProductDb.SaveChangesAsync();
            }
        

        public async Task<bool> DaleteCategoryAsyn(int id)
        {
            //SetUseApi("ApiController");

            //if (_useApi)
            //{
            //    var response = await _client.DeleteAsync($"{_apiBaseUrl}/CategoryApi/Delete/{id}");
            //    return response.IsSuccessStatusCode;
            //}
            //else
            //{
                var category = await cat_ProductDb.Categories.FindAsync(id);
                if (category != null)
                {
                    cat_ProductDb.Categories.Remove(category);
                    await cat_ProductDb.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        

        public async Task DeactivateCategoryAsync(int id)
        {
            //SetUseApi("ApiController");
            //if (_useApi)
            //{
            //    var response = await _client.PutAsync($"{_apiBaseUrl}/CategoryApi/Deactivate/{id}", null);
            //    response.EnsureSuccessStatusCode();
            //}
            //else
            //{
                var category = await cat_ProductDb.Categories.FindAsync(id);
                if (category != null)
                {
                    category.IsActive = false;
                    await cat_ProductDb.SaveChangesAsync();
                    await DeactivateProductsByCategoryIdAsync(id);

                }
            }
        

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            //SetUseApi("ApiController");
            //if (_useApi)
            //{
            //    var response = await _client.GetAsync($"{_apiBaseUrl}/CategoryApi/List?pageNo={pageNumber}&pageSize={pageSize}");
            //    response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<IEnumerable<Category>>(content);
            //}
            //else
                return await cat_ProductDb.Categories.Skip((1-pageNumber)*pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            //SetUseApi("ApiController");

            //if (_useApi)
            //{
            //    var response = await _client.GetAsync($"{_apiBaseUrl}/CategoryApi/Get/{id}");
            //    response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<Category>(content);
            //}
            //else
                return await cat_ProductDb.Categories.FindAsync(id);
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category Updatecategory)
        {
            //SetUseApi("ApiController");
            //if (_useApi)
            //{
            //    var content = new StringContent(JsonConvert.SerializeObject(Updatecategory), Encoding.UTF8, "application/json");
            //    var response = await _client.PutAsync($"{_apiBaseUrl}/CategoryApi/Edit/{id}", content);
            //    return response.IsSuccessStatusCode;
            //}
            //else
            //{
                var category = await cat_ProductDb.Categories.FindAsync(id);
                if (category != null)
                {


                    category.Name = Updatecategory.Name;
                    category.IsActive = Updatecategory.IsActive;
                    //category.Products = Updatecategory.Products;

                    await cat_ProductDb.SaveChangesAsync();
                    return true;



                }
                return false;
            }
        
        private async Task ActivateProductsByCategoryIdAsync(int categoryId)
        {
            var products = await cat_ProductDb.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            foreach (var product in products)
            {
                product.IsActive = true;
                cat_ProductDb.Products.Update(product);
            }
            await cat_ProductDb.SaveChangesAsync();
        }
        private async Task DeactivateProductsByCategoryIdAsync(int categoryId)
        {
            var products = await cat_ProductDb.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            foreach (var product in products)
            {
                product.IsActive = false;
                cat_ProductDb.Products.Update(product);
            }
            await cat_ProductDb.SaveChangesAsync();
        }
        public async Task<bool> SetUseApi(string api)
        {
            var apiCalling = await cat_ProductDb.ApiCallings
                                            .Where(a => a.ApiName == api)
                                            .FirstOrDefaultAsync();
            if (apiCalling != null)
            {
                _useApi = apiCalling.IsActived; 
                return true;
            }
            return false;
        }


    }
    }

