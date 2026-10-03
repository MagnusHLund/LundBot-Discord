namespace LundBot.Application.Common.Exceptions
{
    public sealed class CommandException : Exception
    {
        private readonly bool ShowMessageToUser;

        public CommandException(string message, bool showMessageToUser = false)
            : base(message)
        {
            ShowMessageToUser = showMessageToUser;
        }

        public string GetMessage()
        {
            if (ShowMessageToUser)
            {
                return Message;
            }

            return "An error occurred while processing your command. Please try again later.";
        }
    }
}
