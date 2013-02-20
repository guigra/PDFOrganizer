using PDFOrganizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace PDFOrganizerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            String DestinationPath = null;
            foreach (String filePath in args)
            {
                try
                {
                    FileInfo finfo = new FileInfo(filePath);
                    String newFolder = Path.GetFileNameWithoutExtension(finfo.FullName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    finfo.Directory.CreateSubdirectory(newFolder);
                    DestinationPath = finfo.Directory.FullName + "\\" + newFolder + "\\";

                    Converter converter = new Converter();
                    converter.SplitPdf(new List<String> { filePath }, DestinationPath);

                }
                catch
                {
                    try
                    {
                        Directory.Delete(DestinationPath);
                    }
                    catch
                    {

                    }
                }
            }
           
        }
    }
}
