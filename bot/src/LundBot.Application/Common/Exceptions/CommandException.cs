namespace LundBot.Application.Common.Exceptions
{
    public sealed class CommandException : Exception
    {
        private readonly bool _showMessageToUser;

        public CommandException(string message, bool showMessageToUser = false)
            : base(message)
        {
            _showMessageToUser = showMessageToUser;
        }

        public string GetMessage()
        {
            if (_showMessageToUser)
            {
                return Message;
            }

            return "An error occurred while processing your command. Please try again later.";
        }
    }
}
