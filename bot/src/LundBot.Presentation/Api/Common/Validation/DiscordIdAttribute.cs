using System.ComponentModel.DataAnnotations;
using LundBot.Application.Common.Validation;

namespace LundBot.Presentation.Api.Common.Validation
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class DiscordIdAttribute : ValidationAttribute
    {
        public DiscordIdAttribute()
            : base("The {0} field must be a valid Discord ID.") { }

        public override bool IsValid(object? value)
        {
            return value is null || value is ulong id && ValidationUtils.IsValidDiscordId(id);
        }
    }
}
