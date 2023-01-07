using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace WW_SYSTEM.Discord
{

		public readonly struct EmbedImage : IEquatable<EmbedImage>, IJsonSerializable
		{

		[SerializationConstructor]
		public EmbedImage(string Url)
		{
			this.url = Url;
		}

		public override bool Equals(object obj)
			{
				if (obj is EmbedImage)
				{
				EmbedImage other = (EmbedImage)obj;
					return this.Equals(other);
				}
				return false;
			}

			public bool Equals(EmbedImage other)
			{
				return this.url == other.url;
			}

			public readonly string url;

		public override int GetHashCode()
		{
			return this.url.GetHashCode() * 397;
		}


		public static bool operator ==(EmbedImage left, EmbedImage right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EmbedImage left, EmbedImage right)
		{
			return !left.Equals(right);
		}


	}



}
