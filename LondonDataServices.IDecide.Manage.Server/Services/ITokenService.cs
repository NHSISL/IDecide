using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LondonDataServices.IDecide.Manage.Server.Services;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(HttpContext httpContext);
}