using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TTISDProject
{
    class FileWriteProvider : ISampleProvider
    {
        public ISampleProvider source;
        public WaveFileWriter writer;
        //bool dataToWrite
        public long Position { get; set; }

        public FileWriteProvider(ISampleProvider source, string fileName, WaveFormat format)
        {
            this.source = source;
            this.writer = new WaveFileWriter(fileName, format);
            //ThreadPool.QueueUserWorkItem(WriteThread);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            ThreadPool.QueueUserWorkItem((a) =>
            {
                writer.WriteSamples(buffer, offset, samplesRead);
            });
            return samplesRead;
        }

        /*private void WriteThread(object state)
        {
            while (!dataToWrite) ;

        }*/

        public WaveFormat WaveFormat { get { return this.source.WaveFormat; } }

        public void Dispose()
        {
            writer.Close();
            writer.Dispose();
        }
    }
}
