using System;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace SkdAdminModel
{
    [Serializable]
    public class BindFeedBack
    {
        string _courseName;
        string _userName;
        string _userAccount;
        private string _vender;
        string _description;
        private string _imagePath;
        private string _createTime;
        Image _image;

        public string CourseName
        {
            get
            {
                return _courseName;
            }

            set
            {
                _courseName = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        public string UserAccount
        {
            get
            {
                return _userAccount;
            }

            set
            {
                _userAccount = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public Image Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }

            set
            {
                _imagePath = value;
            }
        }

        public string Vender
        {
            get
            {
                return _vender;
            }

            set
            {
                _vender = value;
            }
        }

        public string CreateTime
        {
            get
            {
                return _createTime;
            }

            set
            {
                _createTime = value;
            }
        }

        public static byte[] SerializeObject(object pObj)
        {
            if (pObj == null)
                return null;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, pObj);
            memoryStream.Position = 0;
            byte[] read = new byte[memoryStream.Length];
            memoryStream.Read(read, 0, read.Length);
            memoryStream.Close();
            return read;
        }
        public object DeserializeObject(byte[] pBytes)
        {
            object newOjb = null;
            if (pBytes == null)
            {
                return newOjb;
            }
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(pBytes);
            memoryStream.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            newOjb = formatter.Deserialize(memoryStream);
            memoryStream.Close();
            return newOjb;
        }
    }
}
