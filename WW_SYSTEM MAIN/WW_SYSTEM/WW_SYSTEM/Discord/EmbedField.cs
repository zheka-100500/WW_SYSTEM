using System;
using Utf8Json;

namespace WW_SYSTEM.Discord
{
	public readonly struct EmbedField : IEquatable<EmbedField>, IJsonSerializable
	{
		[SerializationConstructor]
		public EmbedField(string name, string value, bool inline)
		{
			this.name = name;
			this.value = value;
			this.inline = inline;
		}

		public bool Equals(EmbedField other)
		{
			return this.name == other.name && this.value == other.value && this.inline == other.inline;
		}

		public override bool Equals(object obj)
		{
			if (obj is EmbedField)
			{
				EmbedField other = (EmbedField)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((this.name != null) ? this.name.GetHashCode() : 0) * 397 ^ ((this.value != null) ? this.value.GetHashCode() : 0)) * 397 ^ this.inline.GetHashCode();
		}

		public static bool operator ==(EmbedField left, EmbedField right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EmbedField left, EmbedField right)
		{
			return !left.Equals(right);
		}

		public readonly string name;

		public readonly string value;

		public readonly bool inline;
	}
}
