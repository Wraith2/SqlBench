
using System;
using System.Diagnostics;

namespace Wraith.Data.SqlClient
{

    public sealed class SimpleBitArray
    {
        private int _first;
        private readonly int _length;
        private readonly int[] _array;

        public SimpleBitArray(int length)
        {
            int arrayLength = GetInt32ArrayLengthFromBitLength(length) - 1;
            if (arrayLength > 0)
            {
                _array = new int[arrayLength];
            }
            _length = length;
        }

        public bool this[int index]
        {
            get
            {
                CheckIndex(index);
                int extraBits = 0;
                int value = GetLocationFromIndex(index, out extraBits);
                return (value & (1 << extraBits)) != 0;
            }
            set
            {
                CheckIndex(index);
                int extraBits = 0;
                ref int newValue = ref GetLocationFromIndex(index, out extraBits);
                if (value)
                {
                    newValue |= 1 << extraBits;
                }
                else
                {
                    newValue &= ~(1 << extraBits);
                }
            }
        }

        public int Length => _length;

        private void CheckIndex(int index)
        {
            if (index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        private ref int GetLocationFromIndex(int index, out int extraBits)
        {
            extraBits = 0;
            int elementIndex = Div32Rem(index, out extraBits);
            if (elementIndex==0)
            {
                return ref _first;
            }
            else
            {
                return ref _array[elementIndex - 1];
            }
        }


        private const int BitShiftPerInt32 = 5;

        private static int GetInt32ArrayLengthFromBitLength(int n)
        {
            Debug.Assert(n >= 0);
            return (int)((uint)(n - 1 + (1 << BitShiftPerInt32)) >> BitShiftPerInt32);
        }

        private static int Div32Rem(int number, out int remainder)
        {
            uint quotient = (uint)number / 32;
            remainder = number & (32 - 1);    // equivalent to number % 32, since 32 is a power of 2
            return (int)quotient;
        }
    }
}
