using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.APIServices
{
    public interface ICategoryApi
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize);
        Task<Category> GetByIdAsync(int id);
        Task AddCategoryAsync(Category addCategory);
        Task<bool> UpdateCategoryAsync(int id, Category Updatecategory);
        Task<bool> DaleteCategoryAsyn(int id);
        Task DeactivateCategoryAsync(int id);
        Task ActivateCategoryAsync(int id);
    }
}
