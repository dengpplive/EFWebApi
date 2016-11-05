using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// Set集合扩展
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    internal class Set<TElement>
    {
        private int[] _buckets;
        private readonly IEqualityComparer<TElement> _comparer;
        private int _count;
        private int _freeList;
        private Slot[] _slots;

        public Set()
            : this(null)
        {
        }

        public Set(IEqualityComparer<TElement> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TElement>.Default;
            }
            _comparer = comparer;
            _buckets = new int[7];
            _slots = new Slot[7];
            _freeList = -1;
        }

        public bool Add(TElement value)
        {
            return !Find(value, true);
        }

        public bool Contains(TElement value)
        {
            return Find(value, false);
        }

        private bool Find(TElement value, bool add)
        {
            var hashCode = InternalGetHashCode(value);
            for (var i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
            {
                if ((_slots[i].HashCode == hashCode) && _comparer.Equals(_slots[i].Value, value))
                {
                    return true;
                }
            }
            if (add)
            {
                int freeList;
                if (_freeList >= 0)
                {
                    freeList = _freeList;
                    _freeList = _slots[freeList].Next;
                }
                else
                {
                    if (_count == _slots.Length)
                    {
                        Resize();
                    }
                    freeList = _count;
                    _count++;
                }
                var index = hashCode % _buckets.Length;
                _slots[freeList].HashCode = hashCode;
                _slots[freeList].Value = value;
                _slots[freeList].Next = _buckets[index] - 1;
                _buckets[index] = freeList + 1;
            }
            return false;
        }

        internal int InternalGetHashCode(TElement value)
        {
            if (value != null)
            {
                return (_comparer.GetHashCode(value) & 0x7fffffff);
            }
            return 0;
        }

        public bool Remove(TElement value)
        {
            var hashCode = InternalGetHashCode(value);
            var index = hashCode % _buckets.Length;
            var num3 = -1;
            for (var i = _buckets[index] - 1; i >= 0; i = _slots[i].Next)
            {
                if ((_slots[i].HashCode == hashCode) && _comparer.Equals(_slots[i].Value, value))
                {
                    if (num3 < 0)
                    {
                        _buckets[index] = _slots[i].Next + 1;
                    }
                    else
                    {
                        _slots[num3].Next = _slots[i].Next;
                    }
                    _slots[i].HashCode = -1;
                    _slots[i].Value = default(TElement);
                    _slots[i].Next = _freeList;
                    _freeList = i;
                    return true;
                }
                num3 = i;
            }
            return false;
        }

        private void Resize()
        {
            var num = (_count * 2) + 1;
            var numArray = new int[num];
            var destinationArray = new Slot[num];
            Array.Copy(_slots, 0, destinationArray, 0, _count);
            for (var i = 0; i < _count; i++)
            {
                var index = destinationArray[i].HashCode % num;
                destinationArray[i].Next = numArray[index] - 1;
                numArray[index] = i + 1;
            }
            _buckets = numArray;
            _slots = destinationArray;
        }

        struct Slot
        {
            public int HashCode;
            public TElement Value;
            public int Next;
        }
    }
}
