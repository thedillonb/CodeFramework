using CodeFramework.Core.Data;
using System.Threading.Tasks;

namespace CodeFramework.Core.Services
{
    public interface IAccountValidatorService
    {
        Task Validate(IAccount account);
    }
}

