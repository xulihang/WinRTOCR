using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text.Json;

namespace WinRTOCR
{
    class Program
    {
        static async Task Main(string[] args)
        {
            String imagePath="image.jpg";
            String lang = "zh-Hans";
            String output = "out.json";
            if (args.Length != 0)
            {
                imagePath = args[0];
                if (args.Length == 3)
                {
                    lang = args[1];
                    output = args[2];
                }
            }
            if (System.IO.File.Exists(imagePath))
            {
                await Write(imagePath, lang, output);
            }
            else
            {
                Console.WriteLine("Bad path");
            }
            //Print(imagePath,lang);
            
        }

        private static async Task<OcrResult> GetResult(string imagePath, string lang)
        {
            StorageFile storageFile;
            var path = Path.GetFullPath(imagePath);
            storageFile = await StorageFile.GetFileFromPathAsync(path);
            IRandomAccessStream randomAccessStream = await storageFile.OpenReadAsync();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
            SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            Language language = new Language(lang);
            OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(language);
            OcrResult ocrResult = await ocrEngine.RecognizeAsync(bitmap);
            Console.Write(ocrResult.Text);
            return ocrResult;
        }

        private static async Task<Object> Print(string imagePath, string lang)
        {
            OcrResult ocrResult = await GetResult(imagePath, lang);
            Console.WriteLine(ocrResult.Text);
            return "";
        }

        private static async Task<Object> Write(string imagePath, string lang, string output)
        {
            OcrResult ocrResult = await GetResult(imagePath, lang);
            string jsonString;
            jsonString = JsonSerializer.Serialize(ocrResult);
            StreamWriter streamWriter= new StreamWriter(output);
            streamWriter.Write(jsonString);
            streamWriter.Dispose();
            return "";
        }
    }
}
