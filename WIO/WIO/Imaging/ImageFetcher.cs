using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WIO.Diagnostics;
using WIO.Jobs;
using WIO.Settings;

namespace WIO.Imaging
{
    internal class ImageFetcher
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();
        private readonly BingSearchContainer _bingSearchContainer;

        public ImageFetcher(string outputPath)
        {
            _outputPath = outputPath;
            Logger.Info("Creating Image Search client");

            _bingSearchContainer = new BingSearchContainer(new Uri(AppSettings.Instance.Search.ImageSearchUrl))
            {
                IgnoreMissingProperties = true,
                Timeout = AppSettings.Instance.Search.Timeout,
                Credentials = new NetworkCredential(AppSettings.Instance.Search.Username,
                                                    AppSettings.Instance.Search.ApiKey)
            };
        }

        private readonly string _outputPath;

        public void Fetch(string searchTerm, int maxResults = 64)
        {
            Logger.Info("Fetching images for term '{0}', max results of {1}", searchTerm, maxResults);

            Logger.Info("Inspecting output directory {0}", _outputPath);
            var dir = new DirectoryInfo(_outputPath);

            if (!dir.Exists) dir.Create();
            
            try
            {
                const string imageFilters = "Size:Large";
                Logger.Info("Setting up search. Query: {0}, Image filters: {1}", searchTerm, imageFilters);

                var query = _bingSearchContainer.Image(
                    Query: searchTerm,
                    Options: null,
                    Market: null,
                    Adult: null,
                    Latitude: null,
                    Longitude: null,
                    ImageFilters: "Size:Large",
                    top: 100);

                Logger.Info("Performing image search using ", _bingSearchContainer.BaseUri);
                var results = query.ToList();
                Logger.Info("Image search finished; {0} results", results.Count);
                DownloadImages(results);
            }
            catch (Exception ex)
            {
                Logger.Error("Error fetching images for term '{0}' : {1}", searchTerm, ex.ToString());
                throw;
            }
        }

        private async void DownloadImages(IEnumerable<ImageResult> results)
        {
            var sw = Stopwatch.StartNew();
            //TODO: remove hardcoded min dimensions
            var filteredResults = results.Where(x => x.Width >= 1024 && x.Height >= 768).ToList();
            Logger.Info("Filtered result count: {0}", filteredResults.Count);
            var downloadCount = 0;

            //TODO: setup RavenDB database or other DS with source info so we don't get duplicate info?

            foreach (var result in filteredResults)
            {
                var imageUrl = result.MediaUrl;
                var outputFilename = Path.Combine(_outputPath, string.Format("{0}.jpg", result.ID.ToString("N")));

                // don't redownload the image if it already exists locally, tho' the image could have changed
                if (!File.Exists(outputFilename))
                {
                    Logger.Debug("Image Url: {0}, destination: {1}", imageUrl, outputFilename);
                    // http://stackoverflow.com/questions/4054263/how-does-c-sharp-5-0s-async-await-feature-differ-from-the-tpl
                    await DownloadImage(imageUrl, outputFilename);
                    downloadCount++;
                }
                else 
                    Logger.Debug("File already exists locally, not redownloading {0}", imageUrl);
            }

            sw.Stop();
            Logger.Info("Saved {0} images in {1:000.0} seconds", downloadCount, sw.Elapsed.TotalSeconds);
        }

        private static async Task DownloadImage(string url, string outputFilename)
        {
            //TODO: httpclient (web api client) and not WebClient
            using (var webClient = new WebClient())
            {
                try
                {
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
                        Logger.Error( string.Format("Error deleting image '{0}': {1}", outputFilename, inner));
                    }
                }
            }
        }
    }
}
