﻿using BooKStore.Models.DTO;

namespace BooKStore.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {

        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegistrationModel model);
        Task<Status> ChangePasswordAsync(ChangePassword model, string username);
    }
}
