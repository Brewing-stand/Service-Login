using Refit;
using Service_Login.Models;
using System.Threading.Tasks;

namespace Service_Login.Services;

    public interface IUserServiceApi
    {
        // Define the POST method for checking or creating a user
        [Post("/api/User")] 
        Task<User> CheckOrCreateUserAsync([Body] GitUserData gitUserData);
    }

