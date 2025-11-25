using System.Runtime.CompilerServices;

namespace Multiplexer8to1
{
    public static class MuxLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Process(byte dataInput, byte selector)
        {
            return (byte)((dataInput >> (selector & 7)) & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SolveFromFrontend(
            bool d0, bool d1, bool d2, bool d3, bool d4, bool d5, bool d6, bool d7,
            bool s0, bool s1, bool s2)
        {
            byte packedData = (byte)(
                (d0 ? 1 : 0) |
                (d1 ? 2 : 0) |
                (d2 ? 4 : 0) |
                (d3 ? 8 : 0) |
                (d4 ? 16 : 0) |
                (d5 ? 32 : 0) |
                (d6 ? 64 : 0) |
                (d7 ? 128 : 0)
            );

            byte packedSelector = (byte)(
                (s0 ? 1 : 0) |
                (s1 ? 2 : 0) |
                (s2 ? 4 : 0)
            );

            return Process(packedData, packedSelector);
        }
    }
}
