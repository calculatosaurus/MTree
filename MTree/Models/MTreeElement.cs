using System;

namespace MTree.Models
{
	internal abstract class MTreeElement<T>
	{
		internal T Item { get; }
		internal double DistanceToParent { set; get; }
		internal double CoveringRadius { private set; get; }

		internal MTreeElement(T item, double distToParent, double coveringRadius)
        {
            Item = item;
            DistanceToParent = distToParent;
            CoveringRadius = coveringRadius;
        }

		internal void ExpandCoveringRadius(double value)
		{
			CoveringRadius = Math.Max(CoveringRadius, value);
		}

		internal abstract bool Contains(T item);
	}
}
