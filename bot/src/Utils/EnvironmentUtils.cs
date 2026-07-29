namespace LundBot.Utils
{
    public static class EnvironmentUtils
    {
        private static string _environment = "";

        public static string GetEnvironment()
        {
            return _environment ??=
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        }

        public static bool IsProduction()
        {
            return GetEnvironment().Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDevelopment()
        {
            return GetEnvironment().Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}
