using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WIO.Diagnostics;
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

        public void Fetch(string searchTerm, SearchOptions options = null)
        {
            Logger.Info("Inspecting output directory {0}", _outputPath);
            var dir = new DirectoryInfo(_outputPath);

            if (!dir.Exists) dir.Create();
            
            try
            {
                var searchOptions = options ?? AppSettings.Instance.Search.DefaultOptions;
                var results = RunSearches(searchTerm, searchOptions);

                Logger.Info("Image searches finished; {0} results", results.Count);
                DownloadImages(results, searchTerm, searchOptions);
            }
            catch (Exception ex)
            {
                Logger.Error("Error fetching images for term '{0}' : {1}", searchTerm, ex.ToString());
                throw;
            }
        }

        private List<ImageResult> RunSearches(string searchTerm, SearchOptions options, int take = 50)
        {
            var requests = options.Max/take;
            Logger.Info("Fetching images for term '{0}', options: {1}. Requests to make: {2}", searchTerm, options, requests);
            var results = new List<ImageResult>();

            for (var i = 0; i < requests; i++)
            {
                var skip = i * take;
                Logger.Info("Setting up search. Query: {0}, Options: {1}, Skip: {2}", searchTerm, options, skip);
                
                var query = _bingSearchContainer.Image(
                    Query: searchTerm,
                    Options: null,
                    Market: null,
                    Adult: options.Adult,
                    Latitude: null,
                    Longitude: null,
                    ImageFilters: options.Filters,
                    //ImageFilters:"Size:Height:768&Size:Width:1024", // how to combine multiple?
                    top: 50, // 50 is the max we can request in one shot
                    skip:skip);
                var currentResults = query.ToList();
                results.AddRange(currentResults);
            }

            return results;
        }

        private async void DownloadImages(IEnumerable<ImageResult> results, string searchTerm, SearchOptions options)
        {
            var sw = Stopwatch.StartNew();
            var filteredResults = results.Where(x => x.Width >= options.MinWidth && x.Height >= options.MinHeight).ToList();
            Logger.Info("Filtered result count: {0}", filteredResults.Count);
            var downloadCount = 0;

            //consider setting up a DS with source info so we don't get duplicate images (diff filename/id, same source)?

            var metadataMgr = new MetadataManager();
            foreach (var result in filteredResults)
            {
                var imageUrl = result.MediaUrl;
                var outputFilename = Path.Combine(_outputPath, string.Format("{0}.jpg", result.ID.ToString("N")));

                // don't redownload the image if it already exists locally, tho' the image could have changed
                if (!File.Exists(outputFilename))
                {
                    Logger.Debug("Image Url: {0}, destination: {1}", imageUrl, outputFilename);
                    await DownloadImage(imageUrl, outputFilename);
                    metadataMgr.Add(new Metadata
                        {
                            RemoteLocation = imageUrl,
                            LocalLocation = outputFilename,
                            Term = searchTerm
                        });
                    downloadCount++;
                }
                else 
                    Logger.Debug("File already exists locally, not redownloading {0}", imageUrl);
            }

            metadataMgr.Save();
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
                        Logger.Error( string.Format("Error deleting image '{0}': {1}", outputFilename, inner));
                    }
                }
            }
        }
    }
}
