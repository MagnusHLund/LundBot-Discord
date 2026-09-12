using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LundBot.Infrastructure.Utils
{
    public static class DatabaseUtils
    {
        public static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is MySqlException { Number: 1062 };
        }
    }
}
