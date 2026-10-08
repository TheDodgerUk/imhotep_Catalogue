
namespace Utils
{
    public static class BitMask
    {
        public static bool IsSet(int mask, int bit) => (mask & bit) != 0;
        public static bool IsSet<T>(T mask, T bit) where T : struct
        {
            return IsSet(Int(mask), Int(bit));
        }

        public static void Set(ref int mask, int bit) => mask = Set(mask, bit);
        public static int Set(int mask, int bit) => mask | bit;

        public static void Set<T>(ref T mask, T bit) where T : struct
        {
            mask = Set(mask, bit);
        }
        public static T Set<T>(T mask, T bit) where T : struct
        {
            return Cast<T>(Set(Int(mask), Int(bit)));
        }

        public static void Unset(ref int mask, int bit) => mask = Unset(mask, bit);
        public static int Unset(int mask, int bit) => mask & (~bit);
        public static void Unset<T>(ref T mask, T bit) where T : struct
        {
            mask = Unset(mask, bit);
        }
        public static T Unset<T>(T mask, T bit) where T : struct
        {
            return Cast<T>(Unset(Int(mask), Int(bit)));
        }

        public static void Toggle<T>(ref T mask, T bit) where T : struct
        {
            mask = Toggle(mask, bit);
        }
        public static T Toggle<T>(T mask, T bit) where T : struct
        {
            return Cast<T>(Toggle(Int(mask), Int(bit)));
        }
        public static int Toggle(ref int mask, int bit) => mask = Toggle(mask, bit);
        public static int Toggle(int mask, int bit)
        {
            bool state = IsSet(mask, bit);
            return Toggle(mask, bit, !state);
        }

        public static void Toggle<T>(ref T mask, T bit, bool state) where T : struct
        {
            mask = Toggle(mask, bit, state);
        }
        public static T Toggle<T>(T mask, T bit, bool state) where T : struct
        {
            return Cast<T>(Toggle(Int(mask), Int(bit), state));
        }
        public static void Toggle(ref int mask, int bit, bool state) => mask = Toggle(mask, bit, state);
        public static int Toggle(int mask, int bit, bool state)
        {
            if (state == true)
            {
                return Set(mask, bit);
            }
            else
            {
                return Unset(mask, bit);
            }
        }

        private static T Cast<T>(int value) where T : struct => (T)(object)value;
        private static int Int<T>(T value) where T : struct => (int)(object)value;
    }
}