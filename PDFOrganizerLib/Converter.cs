using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PDFMosaic;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Reflection;
namespace PDFOrganizer
{
    public class Converter   
    {

        public Converter()
        {
    
         AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
    
        }

        public  void RemoveDuplicates(List<String> WithDups, ref List<String> WithoutDups)
        {
            // Get distinct elements and convert into a list again.
            WithoutDups = WithDups.Distinct().ToList();
        }

       
        public  Dictionary<String, int>  ExtractData(String FullFileName) {
            String line = null;
            int start = 0;
            int end = 0;
            String albaran = null;
            Dictionary<String, int> docs = new Dictionary<String, int>();//albaran docs
            PDFDocument document = new PDFDocument(FullFileName);

            for (int i = 0; i < document.Pages.Count; ++i)
            {
                using (StringReader sr = new StringReader(document.Pages[i].GetText()))
                {
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine().ToUpper();
                        if (line.Contains("ALBARÁN DE ENTREGA"))
                        {
                            start = line.IndexOf("Nº") + "Nº".Length;
                            end = line.IndexOf("FECHA");
                            albaran = line.Substring(start, end - start).Trim();
                            if (!docs.ContainsKey(albaran))
                            {
                                docs.Add(albaran, i);//nos guardamos
                            }
                            break;
                        }
                    }
                }
            }
            return docs;
        
        }

        public  void SavePDF(String FullFileName, Dictionary<String, int> docs, String DestinationPath)
        {
            PdfDocument outputDocument = null;
            PdfDocument inputDocument = PdfReader.Open(FullFileName, PdfDocumentOpenMode.Import);
            int count = inputDocument.PageCount;
            int end = 0;
            foreach (KeyValuePair<string, int> pair in docs)
            {
                outputDocument = new PdfDocument();//pdfsharp
                if (pair.Value >= 0 && pair.Value < count)
                {
                    end = pair.Value + 3;//las 3 siguientes
                    for (int copies = pair.Value; copies < end; copies++)//numeor de copias
                    {
                        outputDocument.AddPage(inputDocument.Pages[copies]);
                    }
                }
                //outputDocument.Save(DestinationPath + pair.Key + "_"+DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                outputDocument.Save(DestinationPath + pair.Key + ".pdf");
            }
        }


        public  void SplitPdf(List<String> Source, String DestinationPath)
        {
            
                List<String> SourceCleaned = new List<String>();
                RemoveDuplicates(Source, ref SourceCleaned);
                foreach (String FullFileName in SourceCleaned)
                {

                    SavePDF(FullFileName, ExtractData(FullFileName), DestinationPath);

                }
            

        }
    }
}