using System;
using System.Linq;

namespace Svyaznoy.Core.Localization
{
    public struct TextEnum<TEnum>
        where TEnum : struct
    {
        public TEnum? Enum { get; set; }

        public string Text { get; set; }

        public int? IntValue
        {
            get { return Enum.HasValue ? (int?)Convert.ToInt32(Enum.Value) : null; }
        }

        public TextEnum(TEnum? @enum, string text)
            :this()
        {
            Enum = @enum;
            Text = text;
        }

        public override bool Equals(object obj)
        {
            if (obj is TextEnum<TEnum>)
            {
                return Equals(Enum, ((TextEnum<TEnum>)obj).Enum);
            }

            return Equals(this.Enum, obj);
        }

        public override int GetHashCode()
        {
            return Enum.HasValue ? Enum.Value.GetHashCode() : 0;
        }

        public static implicit operator TEnum?(TextEnum<TEnum> val)
        {
            return val.Enum;
        }

        public static implicit operator TEnum(TextEnum<TEnum> val)
        {
            return val.Enum.HasValue? val.Enum.Value: default(TEnum);
        }

        public static bool operator ==(TEnum left, TextEnum<TEnum> right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(TEnum left, TextEnum<TEnum> right)
        {
            return !right.Equals(left);
        }

        public static bool operator ==(TextEnum<TEnum> left,  TEnum right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextEnum<TEnum> left, TEnum right)
        {
            return !left.Equals(right);
        }
    }
}
