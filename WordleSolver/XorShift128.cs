
// (c) 2021 Kazuki KOHZUKI

using System;

namespace Wordle
{
    internal sealed class XorShift128
    {
        private const uint zero = 0x00000000;

        private uint t = 0x00000000; // 0000 0000 0000 0000 0000 0000 0000 0000
        private uint x = 0x0141FDAF; // 0000 0001 0100 0001 1111 1101 1010 1111
        private uint y = 0x21101999; // 0010 0001 0001 0000 0001 1001 1001 1001
        private uint z = 0x54655307; // 0101 0100 0110 0101 0101 0011 0000 0111
        private uint w = 0x2194BB93; // 0010 0001 1001 0100 1011 1011 1001 0011

        internal XorShift128(object o)
        {
            var bs = BitConverter.GetBytes(o.GetHashCode());
            this.x = BitConverter.ToUInt32(bs, 0);
            if (!SkipOver()) throw new Exception("The seed value must not be equal to zero.");
        } // ctor (object)

        private bool SkipOver()
        {
            if ((this.x | zero) == zero) return false;

            for (var i = 0; i < 40; i++) NextInt32(0, 1);

            return true;
        } // private bool SkipOver ()

        internal int NextInt32(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException($"{minValue} is greater than {maxValue}.");
            if (minValue == maxValue) return minValue;

            return (int)(NextUInt32() % (maxValue - minValue)) + minValue;
        } // internal int Next (int, int)

        internal uint NextUInt32()
        {
            this.t = this.x ^ (this.x << 11);
            this.x = this.y; this.y = this.z; this.z = this.w;
            this.w = (this.w ^ (this.w >> 19)) ^ (this.t ^ (this.t >> 8));
            return this.w;
        } // internal uint NextUInt32 ()
    } // internal sealed class XorShift128
} // namespace Wordle
