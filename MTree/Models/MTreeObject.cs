using System;

namespace MTree.Models
{
	internal abstract class MTreeObject<T>
	{
		internal T Item { get; }
        internal double DistanceToParent { set; get; }
		private double _coveringRadius;
        
        internal MTreeObject(T item, double distToParent, double coveringRadius)
        {
            Item = item;
            DistanceToParent = distToParent;
            _coveringRadius = coveringRadius;
        }

        internal double CoveringRadius
		{
			get { return this._coveringRadius; }
		}

        internal void ResetCoveringRadius()
		{
			this._coveringRadius = 0;
		}

        internal void ExpandCoveringRadius(double value)
		{
			_coveringRadius = Math.Max(CoveringRadius, value);
		}

        internal abstract bool Contains(T item);
	}
}
