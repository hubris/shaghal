using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Shaghal
{
    public class DatReader
    {
        private bool _read = false;
        private string _fileName;
        private Dim3 _dim;
        private byte[] _data;
        private Vector3 _sliceThickness;

        public string Filename
        {
            get { return _fileName; }
            set 
            {
                _read = _fileName != value;
                _fileName = value; 
            }
        }

        public byte[] Data
        {
            get 
            {
                Read();
                return _data; 
            }
        }

        public Dim3 Dim
        {
            get 
            {
                Read();
                return _dim; 
            }
        }

        public Vector3 SliceThickness
        {
            get 
            {
                Read();
                return _sliceThickness; 
            }
        }

        public DatReader(string filename)
        {
            _fileName = filename;
        }

        private void Read()
        {
            if (_read)
                return;
            try 
            {                
                using (StreamReader sr = new StreamReader(_fileName)) 
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null) 
                    {
                        if(line.Contains("Resolution")) {
                            string[] split = line.Split(null);
                            _dim = new Dim3(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), Convert.ToInt32(split[3]));
                        }
                        if (line.Contains("SliceThickness"))
                        {
                            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                            string[] split = line.Split(null);
                            _sliceThickness = new Vector3(float.Parse(split[1], culture), float.Parse(split[2], culture), float.Parse(split[3], culture));
                        }
                    }
                }

                string rawFile = _fileName.Substring(0, _fileName.Length - 3) + "raw";
                FileStream fs = new FileStream(rawFile, FileMode.Open, FileAccess.Read);
                BinaryReader binReader = new BinaryReader(fs);
                _data = binReader.ReadBytes(_dim.Width * _dim.Height * _dim.Depth);
                binReader.Close();
                fs.Close();
                _read = true;
            }
            catch (Exception e) 
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }            
        }
    }
}
