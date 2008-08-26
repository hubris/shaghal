using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VolumeRendering
{
    class DatReader
    {
        private string _fileName;
        private Dim3 _dim;
        private byte[] _data;

        public string Filename
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public Dim3 Dim
        {
            get { return _dim; }
        }

        public DatReader(string filename)
        {
            _fileName = filename;
        }

        public void Read()
        {            
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
                    }
                }

                string rawFile = _fileName.Substring(0, _fileName.Length - 3) + "raw";
                FileStream fs = new FileStream(rawFile, FileMode.Open, FileAccess.Read);
                BinaryReader binReader = new BinaryReader(fs);
                _data = binReader.ReadBytes(_dim.Width * _dim.Height * _dim.Depth);
                binReader.Close();
                fs.Close();
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
