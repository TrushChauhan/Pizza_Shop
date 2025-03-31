using Microsoft.AspNetCore.Http;

namespace Service.Interfaces;

public interface IFileService
{
    Task<string> SaveProfileImageAsync(IFormFile file);
    Task<string> SaveItemImageAsync(IFormFile file);
}
