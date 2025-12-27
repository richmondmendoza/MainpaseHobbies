using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Extensions
{
    public static class ByteExtensions
    {
        public static string GetImageExtension(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return null;

            // JPG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                return ".jpg";

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50 &&
                bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49 &&
                bytes[2] == 0x46)
                return ".gif";

            // BMP
            if (bytes[0] == 0x42 && bytes[1] == 0x4D)
                return ".bmp";

            return null;
        }
    }
}