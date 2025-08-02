using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace iBartender.Persistence.Utils
{
    public class DbExceptionHelper
    {
        public static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
        }
    }
}
