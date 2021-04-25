using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IronOcr;
using System.Drawing;

namespace OcrWebApp.Controllers
{
    public class FileUploadController : Controller
    {
        string filePath = "";

        public IActionResult Index()
        {
            ViewBag.Result = false;
            return View();
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file.Length > 0)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Upload/", file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            ViewBag.Result = true;
            //string calyObazek = CalyObrazek(filePath);
            string nazwisko = PobierzNazwisko(filePath);
            string ocenaMeczowa = PobierzOceneMeczowa(filePath);

            //ViewBag.res = ocenaMeczowa;
            ViewBag.res = nazwisko + Environment.NewLine + ocenaMeczowa;

            return View();
        }

        private string CalyObrazek(string file)
        {
            IronTesseract Ocr = new IronTesseract();
            OcrResult result;
            Ocr.Language = OcrLanguage.Polish;

            using (var Input = new OcrInput(file))
            {
                Input.MinimumDPI = 250;
                Input.TargetDPI = 300;

                Ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
                Ocr.Configuration.EngineMode = TesseractEngineMode.TesseractOnly;
                Ocr.Configuration.RenderSearchablePdfsAndHocr = true;
                Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.AutoOsd;
                Ocr.Configuration.TesseractVariables["tessedit_parallelize"] = false;

                result = Ocr.Read(Input);
            }
            return result.Text;
        }

        private string PobierzNazwisko(string file)
        {
            IronTesseract Ocr = new IronTesseract();
            OcrResult result;
            Ocr.Language = OcrLanguage.Polish;

            using (var Input = new OcrInput())
            {
                Rectangle contentArea = new Rectangle() { X = 260, Y = 425, Height = 40, Width = 150 };
                Input.AddImage(file, contentArea);
                ExportBitmapArea(file, contentArea, "Nazwisko");

                Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.SingleBlock;
                result = Ocr.Read(Input);
            }
            return result.Text;
        }

        private string PobierzOceneMeczowa(string file)
        {
            IronTesseract Ocr = new IronTesseract();
            OcrResult result;

            using (var Input = new OcrInput())
            {
                Rectangle contentArea = new Rectangle() { X = 1130, Y = 450, Height = 90, Width = 140 };
                Input.AddImage(file, contentArea);
                ExportBitmapArea(file, contentArea, "OcenaMeczowa");

                Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.SingleBlock;
                result = Ocr.Read(Input);
            }
            return result.Text;
        }

        private void ExportBitmapArea(string Inputfile, Rectangle contentArea, string OutputFile)
        {
            Bitmap original = new Bitmap(Inputfile);
            System.Drawing.Imaging.PixelFormat format = original.PixelFormat;
            Bitmap clone = original.Clone(contentArea, format);
            clone.Save(Path.Combine(Directory.GetCurrentDirectory(), @$"wwwroot/Upload/{OutputFile}.png"));
        }
    }
}