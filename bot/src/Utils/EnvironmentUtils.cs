namespace LundBot.Utils
{
    public static class EnvironmentUtils
    {
        private static string _environment = "";

        public static string GetEnvironment()
        {
            if (string.IsNullOrEmpty(_environment))
            {
                _environment =
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            }

            return _environment;
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
