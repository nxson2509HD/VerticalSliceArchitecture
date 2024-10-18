using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Domain.Models;

namespace VerticalSliceArchitecture.Domain.IRepositories
{
    public interface IProductMongoDbRepository
    {
        Task<ProductMongodb?> GetByIdAsync(int id);
        Task<int> CreateAsync(ProductMongodb product);
        Task DeleteAsync(int id);
        Task UpdateAsync(ProductMongodb product);
    }

}
