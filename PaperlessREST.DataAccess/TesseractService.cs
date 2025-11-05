using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using Tesseract;



namespace PaperlessREST.DataAccess
{
    public class TesseractService : ITesseractService
    {
        private readonly string _tessDataPath;

        public TesseractService()
        {
            // Path to tessdata — adjust if running inside Docker
            _tessDataPath = DetectTessDataPath();
            Console.WriteLine($"[TesseractService] Using tessdata path: {_tessDataPath}");
        }

        public string ExtractTextFromImage(Stream imageStream, string language = "eng")
        {
            try
            {
                // Load Tesseract engine
                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);

                // Convert Stream → Pix (Tesseract image format)
                using var img = Pix.LoadFromMemory(ReadStream(imageStream));

                // Process image
                using var page = engine.Process(img);

                // Get recognized text
                return page.GetText();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"OCR failed: {ex.Message}", ex);
            }
        }

        private string DetectTessDataPath()
        {
            // Possible locations depending on environment
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "tessdata"), // deployed inside app
                Path.Combine(Directory.GetCurrentDirectory(), "tessdata"), // local dev
                "/usr/share/tesseract-ocr/4.00/tessdata",  // common Linux install
                "/usr/share/tessdata",                     // alternative Linux path
                "/app/tessdata"                            // typical Docker mount
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
