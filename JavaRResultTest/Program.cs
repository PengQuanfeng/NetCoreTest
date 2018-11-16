using System;
using System.IO;

namespace JavaRResultTest
{
    public  class CustomFileStream:FileStream
    {
        public CustomFileStream(string filePath,FileMode fileMode,FileAccess fileAccess):
            base(filePath,fileMode,fileAccess)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"\test.txt";
            using (CustomFileStream fileStream = new CustomFileStream(fileName,FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    throw new FileNotFoundException();
                }
                catch (Exception)
                {

                    int test;
                }
            }
        }
    }
}
