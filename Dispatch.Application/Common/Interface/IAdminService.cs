using Dispatch.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Application.Common.Interface
{
    public interface IAdminService
    {
        Task<AdminDashboardSummaryDTO> GetAdminDashboardSummaryAsync(Guid companyId);
    }
}
