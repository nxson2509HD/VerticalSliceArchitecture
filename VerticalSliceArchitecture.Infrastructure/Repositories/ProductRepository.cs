using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;
using VerticalSliceArchitecture.Infrastructure.DbContext;
using VerticalSliceArchitecture.Infrastructure.Helper;

namespace VerticalSliceArchitecture.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<ProductModel?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        }
        public async Task<IEnumerable<ProductModel>> GetProductsDynamicFilter(string filter)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
        public async Task<int> Create(ProductModel entity)
        {
            _context.Products.Add(entity);

            await _context.SaveChangesAsync();

            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs", "product.json");

            var productDTO = _mapper.Map<ProductDto>(entity);

            await JsonFileHelper.AddItemToJsonFileAsync<ProductDto>(jsonFilePath, productDTO);

            return entity.ProductId;
        }

        public async Task Delete(ProductModel product)
        {
            _context.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
