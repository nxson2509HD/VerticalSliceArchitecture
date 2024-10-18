using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;
using VerticalSliceArchitecture.Infrastructure.DbContext;
using VerticalSliceArchitecture.Infrastructure.Helper;

namespace VerticalSliceArchitecture.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetUserByUserName(string userName)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }

}
