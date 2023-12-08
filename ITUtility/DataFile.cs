using System;
using System.Collections.Generic;
using System.Text;

namespace ITUtility
{

    public class DataFile
    {
        private String _FileName;
        private String _ContentType;
        private Byte[] _FileData;

        public DataFile()
        {
            _FileName = null;
            _ContentType = null;
            _FileData = null;
        }

        public DataFile(string filePath) : this(new System.IO.FileInfo(filePath)) { }

        public DataFile(System.IO.FileInfo fileInfo)
        {
            _FileName = fileInfo.Name;
            _ContentType = ITUtility.MimeTypes.GetContentType(fileInfo.Name);
            _FileData = System.IO.File.ReadAllBytes(fileInfo.FullName);
        }

        public String FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public String ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        public Byte[] FileData
        {
            get { return _FileData; }
            set { _FileData = value; }
        }
    }
}

