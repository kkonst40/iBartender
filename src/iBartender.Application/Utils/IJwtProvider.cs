using iBartender.Core.Models;

namespace iBartender.Application.Utils
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}