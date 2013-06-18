using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WIO.Diagnostics;

namespace WIO.Imaging
{
    internal class ImageWriter
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public static bool Write(byte[] imageBytes, string outputFilename)
        {
            Logger.Debug("Creating memory stream from byte array of length {0}", imageBytes.Length);
            using (var stream = new MemoryStream(imageBytes)) //new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Logger.Debug("Creating image from stream");
                using (var image = Image.FromStream(stream))//, useEmbeddedColorManagement: true))
                {
                    Logger.Debug("Saving image {0} in Jpeg format", outputFilename);

                    try
                    {
                        image.Save(outputFilename, ImageFormat.Jpeg);
                        Logger.Info("Saved image {0}", outputFilename);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error saving {0}. Will attempt saving in chunks. Error was : {1}", outputFilename, ex.ToString());

                        try
                        {
                            SaveImageInChunks(imageBytes, outputFilename);
                        }
                        catch (Exception inner)
                        {
                            Logger.Error("Saving image in chunks failed: {0}", inner);
                        }
                    }
                }
            }
            
            return true;
        }

        private static void SaveImageInChunks(byte[] imageBytes, string outputFilename)
        {
            // this is to handle a large image where Image.Save croaked
            // one problemmatic example was: http://www.nationalconfidential.com/images/2012/01/jessica-alba-golden-globes-2012-7.jpg
            using (Stream source = new MemoryStream(imageBytes))
            using (Stream dest = File.Create(outputFilename))
            {
                var buffer = new byte[1024];
                int bytes;
                while ((bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dest.Write(buffer, 0, bytes);
                }
            }
        }
    }
}
