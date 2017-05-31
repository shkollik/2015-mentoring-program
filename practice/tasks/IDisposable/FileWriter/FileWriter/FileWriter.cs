using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Convestudo.Unmanaged
{
    public class FileWriter : IFileWriter, IDisposable
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        // Pointer to an external unmanaged resource.
        private IntPtr _fileHandle;

        public FileWriter(string fileName)
        {
            _fileHandle = CreateFile(
                fileName,
                DesiredAccess.Write,
                ShareMode.None,
                IntPtr.Zero,
                CreationDisposition.CreateAlways,
                FlagsAndAttributes.Normal,
                IntPtr.Zero);

            //if the function "CreateFile" succeeds, the return value is an open handle to the specified file.
            //If the function fails, the return value is INVALID_HANDLE_VALUE 
            // and error will be thrown in "ThrowLastWin32Err" function
            ThrowLastWin32Err();
        }

        private void ThrowLastWin32Err()
        {
            if (_fileHandle.ToInt32() == -1)
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        public void Write(string str)
        {
            if (disposed)
            {
                throw new InvalidOperationException("file is already closed");
            }

            var bytes = GetBytes(str);
            uint bytesWritten = 0;
            WriteFile(_fileHandle, bytes, (uint)bytes.Length, ref bytesWritten, IntPtr.Zero);
        }

        public void WriteLine(string str)
        {
            Write(String.Format("{0}{1}", str, Environment.NewLine));
        }

        /// <summary>
        /// Creates file
        /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx"/>
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes, CreationDisposition dwCreationDisposition, FlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Writes data into a file
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="aBuffer"></param>
        /// <param name="cbToWrite"></param>
        /// <param name="cbThatWereWritten"></param>
        /// <param name="pOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(IntPtr hFile, Byte[] aBuffer, UInt32 cbToWrite, ref UInt32 cbThatWereWritten, IntPtr pOverlapped);

        [System.Runtime.InteropServices.DllImport("Kernel32", SetLastError = true)]
        private extern static bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Converts string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte[] GetBytes(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
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
                    //To clean up managed resources
                }

                //To clean up unmanaged resources
                if (_fileHandle != IntPtr.Zero)
                {
                    CloseHandle(_fileHandle);
                    _fileHandle = IntPtr.Zero;
                }

                // Note that disposing has been done.
                disposed = true;
            }
        }

        ~FileWriter()
        {
            Dispose(false);
        }

    }
}