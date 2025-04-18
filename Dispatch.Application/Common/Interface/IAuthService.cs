﻿using Dispatch.Application.DTOs;
using Dispatch.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.Common.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    }
}
