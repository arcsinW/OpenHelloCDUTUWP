﻿using HelloCDUT.Models;
using HelloCDUT.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCDUT.Services
{
    public interface IHelloCDUTService
    {
        /// <summary>
        /// Gets the configuration data.
        /// </summary>
        /// <returns>The configuration data.</returns>
        //Task<Config> GetConfig();

        Task SignInAsync(AccountAndPassword accountPassword);

        Task RestoreSignInStatusAsync();

        Task SignOutAsync();

        Task UploadAvatar();


    }
}
