using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace PaperlessREST.OcrWorker
{
    public class TesseractService
    {
        private readonly string _tessDataPath;

        public TesseractService()
        {
            _tessDataPath = DetectTessDataPath();
            Console.WriteLine($"[TesseractService] Using tessdata path: {_tessDataPath}");
        }

        public string ExtractTextFromImage(Stream imageStream, string language = "eng")
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default); // loads the tesseract engine
                using var img = Pix.LoadFromMemory(ReadStream(imageStream)); // converts the stream to pix (a tesseract image format)
                using var page = engine.Process(img);

                return page.GetText(); // returns text recognized by ocr
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"OCR failed: {ex.Message}", ex);
            }
        }

        private string DetectTessDataPath()
        {
            // possible locations depending on environment
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "tessdata"),        // deployed inside app
                Path.Combine(Directory.GetCurrentDirectory(), "tessdata"), // local dev
                "/usr/share/tesseract-ocr/4.00/tessdata",                  // common Linux install
                "/usr/share/tessdata",                                     // alternative Linux path
                "/app/tessdata",                                           // typical Docker mount
                //@"C:\Program Files\Tesseract-OCR\tessdata"
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                    return path;
            }

            throw new DirectoryNotFoundException(
                $"Could not find tessdata folder in any of the expected locations:\n{string.Join("\n", possiblePaths)}"
            );
        }

        private static byte[] ReadStream(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
