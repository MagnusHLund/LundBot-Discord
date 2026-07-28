using LundBot.Commands;

namespace LundBot.Exceptions
{
    public class CommandException : Exception
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<CommandException>();

        private readonly bool ShowMessageToUser;

        public CommandException(string message, bool showMessageToUser = false)
            : base(message)
        {
            ShowMessageToUser = showMessageToUser;
            _logger.Error(this, message);
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
