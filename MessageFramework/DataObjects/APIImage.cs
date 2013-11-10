using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageFramework.DataObjects
{
    public class APIImage
    {
        public APIImage()
        {
        }

        public APIImage(byte[] bytes)
        {
            FileBytes = bytes;
        }

        public APIImage(string filename)
        {
            FileName = filename;
        }
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }

        public bool IsFile
        {
            get { return !string.IsNullOrEmpty(FileName); }
        }
    }
}
