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
            String lang = Language.CurrentInputMethodLanguageTag;
            String output = "out.json";
            if (args.Length != 0)
            {
                foreach (String arg in args)
                {
                    if (arg == "-h")
                    {
                        Help();
                    }
                    else if (arg == "-l")
                    {
                        SupportedLanguages();          
                    }
                }
                imagePath = args[0];
                if (args.Length == 3)
                {
                    lang = args[1];
                    output = args[2];
                }else
                {
                    Console.WriteLine("Using input method language:" + lang);
                    Console.WriteLine("Using default output path:" + output);
                }
            }
            else
            {
                Console.WriteLine("Using default image path:" + imagePath);
                Console.WriteLine("Using input method language:" + lang);
                Console.WriteLine("Using default output path:" + output);
            }
            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    await Write(imagePath, lang, output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }              
                
            }
            else
            {
                Console.WriteLine("Bad path");
                Help();
            }
            //Console.ReadLine();
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

        private static void SupportedLanguages()
        {
            Console.WriteLine("Supported languages:");
            foreach (Language lang in OcrEngine.AvailableRecognizerLanguages)
            {
                Console.WriteLine(lang.LanguageTag);       
            }
            Environment.Exit(0);
        }
        private static void Help()
        {
            Console.WriteLine("Usage: WinRTOCR.exe imagePath language outputPath");
            Console.WriteLine("-h: Show this help");
            Console.WriteLine("-l: Show supported languages");
            Environment.Exit(0);
        }


    }
}
