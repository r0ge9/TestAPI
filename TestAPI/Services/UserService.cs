
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text.Json;
using TestAPI.Model;
using TestAPI.Model.Entities;
using TestAPI.Model.Enums;
using TestAPI.Model.Parameters;
using TestAPI.Services.Interfaces;

namespace TestAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _appDbContext;
        public UserService(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
        }

        public async Task<User> AddRoleForUser(int id, RoleName name)
        {
            if(!_appDbContext.Users.Any(x=>x.Id==id)) 
            {
                return null;
            }
            var role=new Role {Name=name,UserId=id };
            _appDbContext.Roles.Add(role);
            await _appDbContext.SaveChangesAsync();
            return new User();
        }

        public async Task<User> CreateUser(User user)
        {
            if (_appDbContext.Users == null)
            {
                return null;
            }
            if (_appDbContext.Users.Any(x => x.Email.ToLower() == user.Email.ToLower()))
                return null;
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteUser(int id)
        {
            if (_appDbContext.Users == null)
            {
                return null;
            }
            if (!_appDbContext.Users.Any(x => x.Id == id))
                return null;
            var user = new User()
            {
                Id = id,
            };
            _appDbContext.Users.Remove(user);
            await _appDbContext.SaveChangesAsync();
            return new User();
        }

        public Task<User> GetUserById(int id)
        {
            return _appDbContext.Users.Include(x=>x.Roles).FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<User>> GetUsers(UserParameter userParameter)
        {
            if (_appDbContext.Users == null)
                return null;
            
            IQueryable<User> list=_appDbContext.Users.Skip((userParameter.PageNumber
                - 1) * userParameter.PageSize).Take(userParameter.PageSize);

            if (!string.IsNullOrEmpty( userParameter.nameFilter))
            {
                list = list.Where(x => x.Name.Contains(userParameter.nameFilter));
            }
            if(userParameter.minAgeFilter.HasValue&&userParameter.minAgeFilter>0)
            {
                list = list.Where(x => x.Age >= userParameter.minAgeFilter);
            }
            if (userParameter.maxAgeFilter.HasValue)
            {
                list = list.Where(x => x.Age <= userParameter.maxAgeFilter);
            }
            if (!string.IsNullOrEmpty(userParameter.emailFilter))
            {
                list = list.Where(x => x.Email.Contains(userParameter.emailFilter));
            }
            if (!string.IsNullOrEmpty(userParameter.roleNameFilter))
                list = list.Include(x => x.Roles.Where(r => r.Name.ToString().Contains(userParameter.roleNameFilter)));
            else
                list = list.Include(x => x.Roles);



            if (userParameter.isAsc)
            {
                if (userParameter.sortField.ToString() == "RoleName")
                {
                   
                    return list.Include(x => x.Roles).OrderBy(x => x.Name).ToListAsync();
                }
                    

                return list.OrderBy(x=>EF.Property<object>(x,userParameter.sortField.ToString()))
                    .ToListAsync();
            }
            if (userParameter.sortField.ToString() == "RoleName")
            {
                return list.Include(x => x.Roles).OrderByDescending(x => x.Name).ToListAsync();
                
            }
            return list.OrderByDescending(x => EF.Property<object>(x, userParameter.sortField.ToString()))
                    .ToListAsync();


        }

        public async Task<User> UpdateUser(int id, User user)
        {
            if(!string.IsNullOrEmpty(user.Email))
            if (_appDbContext.Users.Any(x => x.Email.ToLower() == user.Email.ToLower()))
                return null;
            var oldUser=_appDbContext.Users.FirstOrDefault(x=>x.Id==id);
            if (!string.IsNullOrEmpty(user.Name))
                oldUser.Name = user.Name;
            if(user.Age>0)
                oldUser.Age=user.Age;
            if(!string.IsNullOrEmpty(user.Email))
                oldUser.Email=user.Email;
            _appDbContext.Entry(oldUser).State = EntityState.Modified;

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return user;
        }
        private bool UserExists(int id)
        {
            return (_appDbContext.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
