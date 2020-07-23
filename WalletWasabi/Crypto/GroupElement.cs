using NBitcoin.Secp256k1;
using System;
using System.Collections.Generic;
using System.Text;
using WalletWasabi.Helpers;

namespace WalletWasabi.Crypto
{
	public class GroupElement : IEquatable<GroupElement>
	{
		public GroupElement(GE groupElement)
		{
			if (groupElement.IsInfinity)
			{
				Ge = GE.Infinity;
			}
			else
			{
				Guard.True($"{nameof(groupElement)}.{nameof(groupElement.IsValidVariable)}", groupElement.IsValidVariable);
				Ge = groupElement;
			}
		}

		public GroupElement(GEJ groupElement)
			: this(groupElement.ToGroupElement())
		{
		}

		public static GroupElement Infinity { get; } = new GroupElement(GE.Infinity);

		private GE Ge { get; }

		public bool IsInfinity => Ge.IsInfinity;

		public override bool Equals(object obj) => Equals(obj as GroupElement);

		public bool Equals(GroupElement other) => this == other;

		public override int GetHashCode() => Ge.GetHashCode();

		public static bool operator ==(GroupElement a, GroupElement b)
		{
			if (a is null && b is null)
			{
				return true;
			}
			else if (a is null || b is null)
			{
				return false;
			}
			else if (a.IsInfinity && b.IsInfinity)
			{
				return true;
			}
			else
			{
				return a.IsInfinity == b.IsInfinity && a.Ge.x == b.Ge.x && a.Ge.y == b.Ge.y;
			}
		}

		public static bool operator !=(GroupElement a, GroupElement b) => !(a == b);
	}
}
