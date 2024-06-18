using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleverInterviewCode.StorageService
{
    public interface IRfidStorageService
    {
        Task<IActionResult> SaveRfidTagAsync(string rfidTag);

        Task<bool> AuthenticateRfidTagAsync(string rfidTag);
    }
}
