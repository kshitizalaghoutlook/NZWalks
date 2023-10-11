using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using System.Runtime.CompilerServices;

namespace NZWalks.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public UserRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
           var user =  nZWalksDbContext.Users
                .FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.Password == password);

          

            if(user == null)
            {
                return null;
            }

            var userRoles = await nZWalksDbContext.User_Roles.Where(x => x.UserId == user.Id).ToListAsync();

            if (userRoles.Any())
            {
                user.Roles = new List<string>();
                foreach (var userRole in userRoles)
                {
                   var role = await nZWalksDbContext.Roles.FirstOrDefaultAsync(x=>x.Id == userRole.RoleId);
                    if(role!=null)
                    {
                        user.Roles.Add(role.Name);
                    }

                }
            }

            user.Password = null;
            return user;
        }
    }
}
