using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WIO.Diagnostics;

namespace WIO.Imaging
{
    class ImageDownloader
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public static async Task DownloadImage(string url, string outputFilename)
        {
            //TODO: httpclient (web api client) and not WebClient
            using (var webClient = new WebClient())
            {
                try
                {
                    // consider throttling downloads:
                    //   http://www.codeproject.com/Articles/18243/Bandwidth-throttling OR
                    //   http://sharpbits.codeplex.com/

                    Logger.Debug("Downloading {0} to {1}", url, outputFilename);
                    var sw = Stopwatch.StartNew();
                    var imageBytes = await webClient.DownloadDataTaskAsync(url);
                    sw.Stop();
                    Logger.Debug("Downloaded {0} bytes in {1:00.0} second(s)", imageBytes.Length, sw.Elapsed.TotalSeconds);

                    Logger.Debug("Writing image bytes to disk");
                    var result = ImageWriter.Write(imageBytes, outputFilename);
                    Logger.Debug("Image write complete with result: {0}", result);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error downloading image '{0}' to '{1}'. Likely corrupt and will be deleted. Error: {2}", url, outputFilename, ex.ToString());
                    // file is likely corrupted
                    try
                    {
                        if (File.Exists(outputFilename))
                            File.Delete(outputFilename);
                    }
                    catch (Exception inner)
                    {
                        Logger.Error(string.Format("Error deleting image '{0}': {1}", outputFilename, inner));
                    }
                }
            }
        }
    }
}
