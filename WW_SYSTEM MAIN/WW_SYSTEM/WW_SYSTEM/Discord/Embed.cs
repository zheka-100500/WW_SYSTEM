using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace WW_SYSTEM.Discord
{
	public readonly struct Embed : IEquatable<Embed>, IJsonSerializable
	{
		
		[SerializationConstructor]
		public Embed(string title, string type, string description, int color, EmbedField[] fields, EmbedImage image, EmbedImage thumbnail)
		{ 
			this.title = title;
			this.type = type;
			this.description = description;
			this.color = color;
			this.fields = fields;
			this.image = image;
			this.thumbnail = thumbnail;
		}


		public bool Equals(Embed other)
		{
			return this.title == other.title && this.type == other.type && this.description == other.description && this.color == other.color && object.Equals(this.fields, other.fields);
		}

	
		public override bool Equals(object obj)
		{
			if (obj is Embed)
			{
				Embed other = (Embed)obj;
				return this.Equals(other);
			}
			return false;
		}


		public override int GetHashCode()
		{
			return (((((this.title != null) ? this.title.GetHashCode() : 0) * 397 ^ ((this.type != null) ? this.type.GetHashCode() : 0)) * 397 ^ ((this.description != null) ? this.description.GetHashCode() : 0)) * 397 ^ this.color) * 397 ^ ((this.fields != null) ? this.fields.GetHashCode() : 0);
		}

		
		public static bool operator ==(Embed left, Embed right)
		{
			return left.Equals(right);
		}


		public static bool operator !=(Embed left, Embed right)
		{
			return !left.Equals(right);
		}

		
		public readonly string title;

		public readonly string type;

		public readonly EmbedImage image;

		public readonly EmbedImage thumbnail;


		public readonly string description;

	
		public readonly int color;


		public readonly EmbedField[] fields;
	}
}
