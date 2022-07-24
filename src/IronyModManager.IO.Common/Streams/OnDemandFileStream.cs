// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 07-24-2022
//
// Last Modified By : Mario
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="OnDemandFileStream.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.IO.Common.Streams
{
#nullable enable

    /// <summary>
    /// Class OnDemandFileStream.
    /// Implements the <see cref="Stream" />
    /// </summary>
    /// <seealso cref="Stream" />
    public class OnDemandFileStream : Stream
    {
        #region Fields

        /// <summary>
        /// The file
        /// </summary>
        private readonly string file;

        /// <summary>
        /// The file access
        /// </summary>
        private readonly FileAccess fileAccess;

        /// <summary>
        /// The file mode
        /// </summary>
        private readonly FileMode fileMode;

        /// <summary>
        /// The file share
        /// </summary>
        private readonly FileShare fileShare;

        /// <summary>
        /// Determines whether the filestream is really closed
        /// </summary>
        private bool? isClosed;

        /// <summary>
        /// The proxy
        /// </summary>
        private FileStream? proxy;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDemandFileStream" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <param name="fileShare">The file share.</param>
        public OnDemandFileStream(string file, FileMode fileMode, FileAccess fileAccess, FileShare fileShare) : base()
        {
            this.file = file;
            this.fileMode = fileMode;
            this.fileAccess = fileAccess;
            this.fileShare = fileShare;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
        public override bool CanRead => GetFileStream().CanRead;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
        public override bool CanSeek => GetFileStream().CanSeek;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
        public override bool CanWrite => GetFileStream().CanWrite;

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length.</value>
        public override long Length => GetFileStream().Length;

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value>The position.</value>
        public override long Position
        {
            get => GetFileStream().Position;
            set { GetFileStream().Position = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Closes the specified temporary close.
        /// </summary>
        /// <param name="tempClose">if set to <c>true</c> [temporary close].</param>
        public void Close(bool tempClose)
        {
            if (!isClosed.GetValueOrDefault())
            {
                isClosed = !tempClose;
            }
            proxy?.Close();
            proxy = null;
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close()
        {
            Close(false);
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            GetFileStream().Flush();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return GetFileStream().Read(buffer, offset, count);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return GetFileStream().Seek(offset, origin);
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            GetFileStream().SetLength(value);
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            GetFileStream().Write(buffer, offset, count);
        }

        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <returns>FileStream.</returns>
        /// <exception cref="System.NullReferenceException">FileStream has been disposed.</exception>
        private FileStream GetFileStream()
        {
            if (proxy == null && !isClosed.GetValueOrDefault())
            {
                proxy = new FileStream(file, fileMode, fileAccess, fileShare);
            }
            if (proxy == null)
            {
                throw new NullReferenceException("FileStream has been disposed.");
            }
            return proxy;
        }

        #endregion Methods
    }

#nullable disable
}
