using Organization.Domain.Abstractions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Organization.Application.Common.Validation
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string?> MustBeValueObject<T, TVO>(
           this IRuleBuilder<T, string?> rule)
           where TVO : IValueObject<TVO>
        {
            return rule.Must((obj, value, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(value)) return true;

                var (ok, error) = TVO.Validate(value);
                if (!ok)
                {
                    var msg = (error ?? $"Invalid value provided.").Replace("{PropertyName}", ctx.DisplayName);

                    ctx.MessageFormatter.AppendArgument("Msg", msg);
                }
                return ok;
            })
            .WithMessage("{Msg}");
        }

        public static IRuleBuilderOptions<T, TEnum> IsDefinedEnum<T, TEnum>(
            this IRuleBuilder<T, TEnum> rule)
            where TEnum : struct, Enum
        {
            return rule
                .Must((obj, val, ctx) =>
                {
                    var ok = Enum.IsDefined(typeof(TEnum), val);

                    if (!ok)
                    {
                        var msg = EnumAllowedMessage<TEnum>(ctx.DisplayName);
                        ctx.MessageFormatter.AppendArgument("Msg", msg);
                    }

                    return ok;
                })
                .WithMessage("{Msg}");
        }

        public static IRuleBuilderOptions<T, TEnum?> IsDefinedEnum<T, TEnum>(
            this IRuleBuilder<T, TEnum?> rule)
            where TEnum : struct, Enum
        {
            return rule
                    .Must((obj, val, ctx) =>
                    {
                        if (!val.HasValue)
                            return true;

                        var ok = Enum.IsDefined(typeof(TEnum), val);

                        if (!ok)
                        {
                            var msg = EnumAllowedMessage<TEnum>(ctx.DisplayName);

                            ctx.MessageFormatter.AppendArgument("Msg", msg);
                        }

                        return ok;
                    })
                    .WithMessage("{Msg}");
        }

        public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveNoDuplicates<T, TItem, TKey>(
            this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
            Func<TItem, TKey> keySelector)
        {
            return ruleBuilder.Must(list =>
                ValidationUtils.HasNoDuplicates(list, keySelector));
        }


        private static string GetEnumDisplayName(Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            if (member == null) return value.ToString();

            var display = member.GetCustomAttribute<DisplayAttribute>();
            if (display == null) return value.ToString();

            // DisplayAttribute supports resource-based names via ResourceType
            return display.GetName() ?? value.ToString();
        }

        private static string EnumAllowedList<TEnum>() where TEnum : Enum
        {
            var items = Enum.GetValues(typeof(TEnum))
                            .Cast<TEnum>()
                            .Select(e => $"{Convert.ToInt32(e)} = {GetEnumDisplayName(e as Enum)!}");
            return string.Join(", ", items);
        }

        private static string EnumAllowedMessage<TEnum>(string propertyName) where TEnum : Enum
            => $"{propertyName} must be one of: {EnumAllowedList<TEnum>()}.";



        public static IRuleBuilderOptions<T, string?> MustBeValidJson<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string errorMessage = "Invalid JSON format")
        {
            return ruleBuilder.Must(json =>
            {
                if (string.IsNullOrWhiteSpace(json))
                    return true;

                try
                {
                    JsonDocument.Parse(json);
                    return true;
                }
                catch
                {
                    return false;
                }
            }).WithMessage(errorMessage);
        }
    }
}
