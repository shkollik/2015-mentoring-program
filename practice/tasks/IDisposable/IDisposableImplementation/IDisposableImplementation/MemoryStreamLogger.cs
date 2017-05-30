using System;
using System.IO;

namespace NetMentoring
{
    class MemoryStreamLogger : IDisposable
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        private FileStream memoryStream;
        private StreamWriter streamWriter;
        public MemoryStreamLogger()
        {
            memoryStream = new FileStream(@"\log.txt", FileMode.OpenOrCreate);
            streamWriter = new StreamWriter(memoryStream);
        }

        public void Log(string message)
        {
            if (streamWriter == null) return;

            streamWriter.Write(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // free managed resources
                    if (streamWriter != null)
                    {
                        streamWriter.Dispose();
                        streamWriter = null;
                    }
                }

                //free native resources if there are any

                // Note that disposing has been done.
                disposed = true;
            }
        }

        ~MemoryStreamLogger()
        {
            Dispose(false);
        }

    }
}