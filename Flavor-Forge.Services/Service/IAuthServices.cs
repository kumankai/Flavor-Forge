/*
 * This service is for handling Registering, log in and log out related actions
 * Used for dependency injection
 */

using Flavor_Forge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flavor_Forge.Services.Service
{
    // Interface defining authentication services for user management
    public interface IAuthServices
    {
        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="user">The user object containing registration details.</param>
        /// <returns>A task representing the asynchronous operation, containing the registered user object if successful; otherwise, null.</returns>
        Task<User?> RegisterAsync(User user);

        /// <summary>
        /// Logs in a user asynchronously by verifying credentials.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the user object if login is successful; otherwise, null.</returns>
        Task<User?> LoginAsync(string username, string password);


        /// <summary>
        /// Logs out a user asynchronously by ending their session.
        /// </summary>
        /// <param name="userId">The ID of the user to log out.</param>
        /// <returns>A task representing the asynchronous operation, returning true if logout is successful; otherwise, false.</returns>
        Task<bool> LogoutAsync(int userId);

        /// <summary>
        /// Checks if a user with the specified username exists in the system.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>A task representing the asynchronous operation, returning true if the user exists; otherwise, false.</returns>
        Task<bool> UserExistsAsync(string username);

        /// <summary>
        /// Verifies if the specified password matches the stored password for a given user.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="userId">The ID of the user whose password needs to be checked.</param>
        /// <returns>A task representing the asynchronous operation, returning true if the password is correct; otherwise, false.</returns>
        Task<bool> CheckPassword(string password, int userId);
    }
}
