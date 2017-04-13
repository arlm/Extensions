using System;

namespace CLR.Extensions.Portable
{
    public static class FloatingPointExtensions
    {
        public static bool NearlyEquals(this double a, double b, double epsilon = 0.00001d)
        {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }

            if (a == 0 || b == 0 || diff < double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }

            return diff / (absA + absB) < epsilon;
        }

        public static bool NearlyEquals(this float a, float b, float epsilon = 0.00001f)
        {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }

            if (a == 0 || b == 0 || diff < float.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }

            return diff / (absA + absB) < epsilon;
        }

        public static bool NearlyEqual2sComplement(this float a, float b, int maxDeltaBits)
        {
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);

            if (aInt < 0)
            {
                // Int32.MinValue = 0x80000000
                aInt = int.MinValue - aInt;
            }

            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);

            if (bInt < 0)
            {
                bInt = int.MinValue - bInt;
            }

            var intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (1 << maxDeltaBits);
        }

        public static bool NearlyEqual2sComplement(this double a, double b, int maxDeltaBits)
        {
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(a), 0);

            if (aInt < 0)
            {
                // Int32.MinValue = 0x80000000
                aInt = int.MinValue - aInt;
            }

            var bInt = BitConverter.ToInt64(BitConverter.GetBytes(b), 0);

            if (bInt < 0)
            {
                bInt = int.MinValue - bInt;
            }

            var intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (1 << maxDeltaBits);
        }

        public static bool NearlyEqualFast2sComplement(this float a, float b, int maxDeltaBits)
        {
            var aInt = a.ToInt32Bits();

            if (aInt < 0)
            {
                aInt = int.MinValue - aInt;
            }

            var bInt = b.ToInt32Bits();

            if (bInt < 0)
            {
                bInt = int.MinValue - bInt;
            }

            var intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (1 << maxDeltaBits);
        }

        public static bool NearlyEqualFast2sComplement(this double a, double b, int maxDeltaBits)
        {
            var aInt = a.ToInt64Bits();

            if (aInt < 0)
            {
                aInt = int.MinValue - aInt;
            }

            var bInt = b.ToInt64Bits();

            if (bInt < 0)
            {
                bInt = int.MinValue - bInt;
            }

            var intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (1L << maxDeltaBits);
        }

        private static unsafe int ToInt32Bits(this float value)
        {
            return *((int*)&value);
        }

        private static unsafe long ToInt64Bits(this double value)
        {
            return *((int*)&value);
        }
    }
}
