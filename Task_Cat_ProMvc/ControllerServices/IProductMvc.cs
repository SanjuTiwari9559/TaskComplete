using Task_Cat_ProMvc.Models.Data;

namespace Task_Cat_ProMvc.Services
{
    public interface IProductMvc
    {
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product  addproduct);
        Task<bool> UpdateAsync(int id,Product updateProduct);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetUseApi(string api);

    }
}
