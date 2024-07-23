using Microsoft.EntityFrameworkCore;
using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.APIServices
{
    public class ProductApi : IProductApi
    {

        private readonly Cat_ProductDbContext cat_ProductDb;

        public ProductApi(Cat_ProductDbContext cat_ProductDb)
        {

            this.cat_ProductDb = cat_ProductDb;
        }
        public async Task AddAsync(Product addproduct)
        {
            await cat_ProductDb.Products.AddAsync(addproduct);
            await cat_ProductDb.SaveChangesAsync();
        }


        public async Task<bool> DeleteAsync(int id)
        {
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
            var products = await cat_ProductDb.Products
                            .Include(p => p.Category)
                            .Where(x=>x.Category.IsActive==true)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await cat_ProductDb.Products.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Product updateProduct, int id)
        {
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

