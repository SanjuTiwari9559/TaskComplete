using Microsoft.EntityFrameworkCore;
using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.APIServices
{
    public class CategoryApi : ICategoryApi
    {
        private readonly Cat_ProductDbContext cat_ProductDb;

        public CategoryApi(Cat_ProductDbContext cat_ProductDb)
        {
            this.cat_ProductDb = cat_ProductDb;
        }

        public async Task ActivateCategoryAsync(int id)
        {
            var category = await cat_ProductDb.Categories.FindAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            category.IsActive = true;
            await cat_ProductDb.SaveChangesAsync();
            await ActivateProductsByCategoryIdAsync(id);
        }

        public async Task AddCategoryAsync(Category addCategory)
        {
            if (addCategory == null) throw new ArgumentNullException(nameof(addCategory));

            await cat_ProductDb.Categories.AddAsync(addCategory);
            await cat_ProductDb.SaveChangesAsync();
        }

        public async Task<bool> DaleteCategoryAsyn(int id)
        {
            var category = await cat_ProductDb.Categories.FindAsync(id);
            if (category == null) return false;

            cat_ProductDb.Categories.Remove(category);
            await cat_ProductDb.SaveChangesAsync();
            return true;
        }

        public async Task DeactivateCategoryAsync(int id)
        {
            var category = await cat_ProductDb.Categories.FindAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            category.IsActive = false;
            await cat_ProductDb.SaveChangesAsync();
            await DeactivateProductsByCategoryIdAsync(id);

        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0) throw new ArgumentOutOfRangeException();

            return await cat_ProductDb.Categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var category = await cat_ProductDb.Categories.FindAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            return category;
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category Updatecategory)
        {
            if (Updatecategory == null) throw new ArgumentNullException(nameof(Updatecategory));

            var category = await cat_ProductDb.Categories.FindAsync(id);
            if (category == null) return false;

            category.Name = Updatecategory.Name;
            category.IsActive = Updatecategory.IsActive;

            await cat_ProductDb.SaveChangesAsync();
            return true;
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
    }
}

