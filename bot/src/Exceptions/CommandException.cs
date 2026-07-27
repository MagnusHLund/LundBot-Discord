using LundBot.Commands;

namespace LundBot.Exceptions
{
    public class CommandException : Exception
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
            else
            {
                return BaseCommand.GENERIC_ERROR_MESSAGE;
            }
        }
    }
}
