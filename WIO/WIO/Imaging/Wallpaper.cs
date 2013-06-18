using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;
using WIO.Diagnostics;

namespace WIO.Imaging
{
    // Modified from http://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
    internal static class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tiled,
            Centered,
            Stretched
        }

        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public static void Set(Uri remoteUri, Style style)
        {
           Set(new WebClient().OpenRead(remoteUri.ToString()), style);
        }

        public static void Set(Stream s, Style style)
        {
            Set(Image.FromStream(s), style);
        }

        public static void Set(string filename, Style style)
        {
            Set(Image.FromFile(filename), style);
        }

        private static readonly Dictionary<Style, int[]> StyleRegMap = new Dictionary<Style,int[]>
        {
            {Style.Stretched, new[] {2, 0}}, {Style.Centered, new[] {1, 0}}, {Style.Tiled, new[] {1, 1}},
        };

        public static void Set(Image image, Style style)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            if (File.Exists(tempPath)) File.Delete(tempPath);
            Logger.Troll("Saving image to {0}", tempPath);
            image.Save(tempPath, ImageFormat.Bmp);

            const string subKey = @"Control Panel\Desktop";
            Logger.Troll(@"Opening HKEY_CURRENT_USER\{0}", subKey);
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (null == key)
                throw new NullReferenceException(string.Format("Registry key {0} open result was null", subKey));

            Logger.Troll("Setting wallpaper style to {0}", StyleRegMap[style][0]);
            key.SetValue(@"WallpaperStyle", StyleRegMap[style][0]);

            Logger.Troll("Setting tile wallpaper to {0}", StyleRegMap[style][1]);
            key.SetValue(@"TileWallpaper", StyleRegMap[style][1]);
            
            Logger.Troll("Setting desktop wallpaper");
            var result = SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

            if (0 == result)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Set wallpaper failed!");
                sb.AppendFormat("Return result: {0}{1}", Marshal.GetLastWin32Error(), Environment.NewLine);
                sb.AppendFormat("Error message: {0}{1}", new Win32Exception(Marshal.GetLastWin32Error()).Message, Environment.NewLine);
                sb.AppendFormat("Filename: {0}", tempPath);
                Logger.Error(sb.ToString());
                return;
            }

            Logger.Troll("Set wallpaper was successful. Returned result: {0} for {1}", result, tempPath);
        }
    }
}
