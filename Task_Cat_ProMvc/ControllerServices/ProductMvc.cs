using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.Services
{

    public class ProductMvc : IProductMvc
    {
        private readonly Cat_ProductDbContext cat_ProductDb;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private  bool _useApi;

        public ProductMvc(Cat_ProductDbContext cat_ProductDb, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.cat_ProductDb = cat_ProductDb;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiBaseUrl"];
            //_useApi = bool.Parse(configuration["UseApi"]);
        }
        public async Task AddAsync(Product addproduct)
        {
            //string controller = "ApiController";
            //var api = cat_ProductDb.ApiCallings.Where(a => a.ApiName == controller).FirstOrDefault();
            //_useApi = api.IsActived;

            //if (_useApi)
            //{
            //    var content = new StringContent(JsonConvert.SerializeObject(addproduct), Encoding.UTF8, "application/json");
            //    var response = await _httpClient.PostAsync($"{_apiBaseUrl}/ProductApi/Add", content);
            //    response.EnsureSuccessStatusCode();
            //}
            //else
            
                await cat_ProductDb.Products.AddAsync(addproduct);
                await cat_ProductDb.SaveChangesAsync();
            }
        

        public async Task<bool> DeleteAsync(int id)
        {
            //string controller = "ApiController";
            //var api = cat_ProductDb.ApiCallings.Where(a => a.ApiName == controller).FirstOrDefault();
            //_useApi = api.IsActived; 

            //if (_useApi)
            //{
            //    var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/ProductApi/Delete/{id}");
            //    return response.IsSuccessStatusCode;
            //}
            //else
            //{
                var Products = await cat_ProductDb.Products.FindAsync(id);
                if (Products != null)
                {
                    cat_ProductDb.Products.Remove(Products);
                    await cat_ProductDb.SaveChangesAsync();
                    return true;
                }
                return false;

            }
        

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            //string controller = "ApiController";
            //var api = cat_ProductDb.ApiCallings.Where(a => a.ApiName == controller).FirstOrDefault();
            //_useApi = api.IsActived;

            //if (_useApi)
            //{
            //    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/ProductApi/List?pageNumber={pageNumber}&pageSize={pageSize}");
            //    //response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<IEnumerable<Product>>(content);
            //}

            return await cat_ProductDb.Products
                            .Include(p => p.Category)
                            .Where(p => p.Category.IsActive == true)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            //string controller = "ApiController";
            //var api = cat_ProductDb.ApiCallings.Where(a => a.ApiName == controller).FirstOrDefault();
            //_useApi = api.IsActived;


            //if (_useApi)
            //{
            //    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/ProductApi/GetById/{id}");
            //    //response.EnsureSuccessStatusCode();
            //    var content = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<Product>(content);
            //}
            return await cat_ProductDb.Products.FindAsync(id);
        }

        public  async Task<bool> SetUseApi(string api)
        {

            var apiCalling = await cat_ProductDb.ApiCallings
                                            .Where(a => a.ApiName == api)
                                            .FirstOrDefaultAsync();
            if (apiCalling != null)
            {
                _useApi = apiCalling.IsActived;
                return _useApi;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(int id,Product updateProduct )
        {
            //if (_useApi)
            //{
            //    var content = new StringContent(JsonConvert.SerializeObject(updateProduct), Encoding.UTF8, "application/json");

            //    try
            //    {
            //        var response = await _httpClient.PutAsync($"{_apiBaseUrl}/ProductApi/Edit/{id}", content);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //_logger.LogError(ex, "Exception occurred while updating product.");
            //        return false;
            //    }
            //} else
                //{
                    var products = await cat_ProductDb.Products.FindAsync(id);
                    if (products != null)
                    {
                        products.Name = updateProduct.Name;
                        products.Id = id;
                        products.CategoryId = updateProduct.CategoryId;
                        products.IsActive = updateProduct.IsActive;
                        await cat_ProductDb.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }

        
    }
    
