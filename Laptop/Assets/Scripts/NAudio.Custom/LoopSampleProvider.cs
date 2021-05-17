using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTISDProject
{
    class LoopSampleProvider : ISampleProvider
    {
        CachedSoundSampleProvider source;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopSampleProvider(CachedSoundSampleProvider source)
        {
            this.source = source;
            this.EnableLooping = true;
            this.Paused = false;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }
        public bool Paused { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public WaveFormat WaveFormat { get { return source.WaveFormat; } }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public long Position
        {
            get { return source.Position; }
            set { source.Position = value; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (Paused)
            {
                for (int i = offset; i < offset + count; i++)
                {
                    buffer[i] = 0;
                }
                return count;
            }
            else
            {
                int totalBytesRead = 0;
                while (totalBytesRead < count)
                {
                    int bytesRead = source.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        if (source.Position == 0 || !EnableLooping)
                        {
                            Console.WriteLine("Something wrong with the source");
                            break;
                        }
                        // loop
                        source.Position = 0;
                    }
                    totalBytesRead += bytesRead;
                }
                return totalBytesRead;
            }
        }
    }
}
