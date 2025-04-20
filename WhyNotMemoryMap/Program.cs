namespace WhyNotMemoryMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public static class StreamExtensions
    {
        public static unsafe T ReadStruct<T>(this Stream stream)
            where T : unmanaged
        {
            var size = sizeof(T);
            var bytesRead = 0;

#if NETCOREAPP2_1_OR_GREATER
            T result = default(T);
            do
            {
                Span<byte> buffer = new Span<byte>(((byte*)&result) + bytesRead, size - bytesRead);
                stream.Read(buffer);
            }
            while (bytesRead < size);

            return result;
#else
            var buffer = new byte[size];

            do
            {
                stream.Read(buffer, bytesRead, size - bytesRead);
            }
            while (bytesRead < size);

            fixed (byte* ptr = buffer)
            {
                return *(T*)ptr;
            }
#endif
        }

        public static unsafe void WriteStruct<T>(this Stream stream, T value)
            where T : unmanaged
        {
            var size = sizeof(T);
#if NETCOREAPP2_1_OR_GREATER
            var buffer = new Span<byte>(&value, size);
            stream.Write(buffer);
#else
            var buffer = new byte[size];

            byte* dataPtr = (byte*)&value;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = dataPtr[i];
            }

            stream.Write(buffer);
#endif
        }
    }
}
