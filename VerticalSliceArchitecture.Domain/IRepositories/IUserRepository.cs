using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Domain.Models;

namespace VerticalSliceArchitecture.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserName(string userName);
    }

}
