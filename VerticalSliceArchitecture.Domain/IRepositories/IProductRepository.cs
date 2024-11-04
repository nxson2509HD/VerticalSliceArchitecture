using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Models;

namespace VerticalSliceArchitecture.Domain.IRepositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetAllAsync();
        Task<IEnumerable<ProductModel>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<ProductModel?> GetByIdAsync(int id);
        Task<IEnumerable<ProductModel>> GetProductsDynamicFilter(string filter);
        Task<int> Create(ProductModel product);
        Task Delete(ProductModel product);
        Task SaveChanges();
    }
}
