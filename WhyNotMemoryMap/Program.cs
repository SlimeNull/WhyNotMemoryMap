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
            var buffer = new byte[size];

            var bytesRead = 0;

            do
            {
                stream.Read(buffer, bytesRead, size - bytesRead);
            }
            while (bytesRead < size);

            fixed (byte* ptr = buffer)
            {
                return *(T*)ptr;
            }
        }

        public static unsafe void WriteStruct<T>(this Stream stream, T value)
            where T : unmanaged
        {
            var size = sizeof(T);
            var buffer = new byte[size];

            byte* dataPtr = (byte*)&value;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = dataPtr[i];
            }

            stream.Write(buffer);
        }
    }
}
