using System;

namespace MTree.Models
{
	internal sealed class Entry<T> : MTreeObject<T>, IEquatable<Entry<T>>
	{
		internal Entry(T item, double distToParent)
			: base(item, distToParent, 0) { }

		public bool Equals(Entry<T> entry)
		{
			if (ReferenceEquals(this, entry)) return true;
			if (!Item.Equals(entry.Item)) return false;
			if (CoveringRadius != entry.CoveringRadius) return false;
			if (DistanceToParent != entry.DistanceToParent) return false;

			return true;
		}

		internal override bool Contains(T item)
		{
			return Item.Equals(item);
		}
	}
}
