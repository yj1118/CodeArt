using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Drawing;
using System.Drawing.Imaging;

using CodeArt.Drawing.Imaging;

using System.IO;

namespace CodeArt.Media
{

    public class BitmapStream : Stream
    {
        private long _length = 0;
        private long _frameCount = 0;

        private byte[] _bitmapBuffer;
        private byte[] _alignedBitmapBuffer;
        private int _bitmapBufferPosition = 0;
        private int _width;
        private int _height;
        private int _stride;
        private int _alignedStride;
        private PixelFormat _pixelFormat;

        private List<Bitmap> _bitmaps;

        public Bitmap[] GetBitmaps()
        {
            return _bitmaps.ToArray();
        }


        public BitmapStream()
        {
        }

        public BitmapStream(int width, int height)
        {
            Init(width, height);
        }

        public void Init(int width, int height)
        {
            _bitmaps = new List<Bitmap>();
            this._width = width;
            this._height = height;
            this._pixelFormat = PixelFormat.Format24bppRgb;
            CalcStride(width, _pixelFormat);

            _bitmapBuffer = new byte[height * _stride];
            if (_stride != _alignedStride)
                _alignedBitmapBuffer = new byte[height * _alignedStride];
        }

        private void CalcStride(int width, PixelFormat pxFormat)
        {
            int bitsPerPixel = Image.GetPixelFormatSize(pxFormat);
            //Number of bits used to store the image data per line (only the valid data)
            _stride = width * bitsPerPixel / 8;
            //4 bytes for every int32 (32 bits)
            var align = _stride % 4;
            _alignedStride = _stride + (align == 0 ? 0 : 4 - align); // ((validBitsPerLine + 31) / 32) * 4;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get
            {
                return _length;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new InvalidOperationException("Bitmap stream is not initialized");

            while (count > 0)
            {
                var copyLength = _bitmapBuffer.Length - _bitmapBufferPosition;
                if (count < copyLength)
                    copyLength = count;
                Buffer.BlockCopy(buffer, offset, _bitmapBuffer, _bitmapBufferPosition, copyLength);
                _bitmapBufferPosition += copyLength;
                _length += copyLength;
                offset += copyLength;
                count -= copyLength;

                if (_bitmapBufferPosition == _bitmapBuffer.Length)
                {
                    // frame buffer is full
                    var bitmapDataBuffer = _bitmapBuffer;
                    if (_stride != _alignedStride)
                    {
                        // lets copy scan lines one-by-one and add windows bitmap 4-bytes alignment
                        for (int h = 0; h < _height; h++)
                        {
                            Buffer.BlockCopy(_bitmapBuffer, h * _stride, _alignedBitmapBuffer, h * _alignedStride, _stride);
                        }
                        bitmapDataBuffer = _alignedBitmapBuffer;
                    }

                    GCHandle handle = GCHandle.Alloc(bitmapDataBuffer, GCHandleType.Pinned);
                    try
                    {
                        var bmp = new Bitmap(_width, _height, _alignedStride, _pixelFormat, handle.AddrOfPinnedObject());
                        _bitmaps.Add(bmp.Copy());
                        bmp.Dispose();
                    }
                    finally
                    {
                        handle.Free();
                    }
                    _frameCount++;
                    _bitmapBufferPosition = 0;
                }
            }
        }
    }
}
