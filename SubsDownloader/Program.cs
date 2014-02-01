namespace SubsDownloader
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Ionic.Zip;

    using NUnrar;
    using NUnrar.Archive;
    using NUnrar.Common;

    class Program
    {
        private static bool autoDownload;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            autoDownload = args.Length > 1 && args[1] == "-autoDownload";

            var media = File.Exists(args[0]) ? Path.GetFileNameWithoutExtension(args[0]) :
                args[0].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

            var targetFolder = Path.GetDirectoryName(args[0]);

            var tempFolder = ConfigurationManager.AppSettings["tempFolder"];
            tempFolder = tempFolder.Replace("{app}", Path.GetDirectoryName(Application.ExecutablePath)); 
            tempFolder = tempFolder.Replace("{target}", targetFolder);

            Console.WriteLine("Target folder: {0}", targetFolder);
            Console.WriteLine("Temp folder: {0}", tempFolder);

            var video = new Video(media);
            GetSubsForTorrent(video, media, targetFolder, tempFolder);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static Dictionary<string, string> cache = new Dictionary<string, string>();

        private static void GetSubsForTorrent(Video video, string videoName, string targetFolder, string tempFolder, bool searchInComments = false)
        {
            var subDivXManager = new SubDivXManager();
            var subs = subDivXManager.GetCandidateSubs(video, searchInComments, cache);

            if (subs.Any())
            {
                if (autoDownload)
                {
                    Console.WriteLine("Autodownload is enabled, downloading first sub.");
                    Download(video, videoName, targetFolder, tempFolder, subDivXManager, subs.First());
                }
                else
                {
                    Console.WriteLine("Showing list of matching subtitles, double-click to download");
                    var selectedSub = FrmShowSubs.SelectSub(subs);
                    if (selectedSub != null)
                    {
                        Download(video, videoName, targetFolder, tempFolder, subDivXManager, selectedSub);
                    }
                }
            }
            else
            {
                if (!searchInComments)
                {
                    Console.WriteLine("No subs were found. Searching for release group within comments.");
                    GetSubsForTorrent(video, videoName, targetFolder, tempFolder, true);
                }
                else
                {
                    Console.WriteLine("No subs were found. Do you want to specify search values? (Y/N)");
                    var key = Console.ReadKey();
                    Console.WriteLine();

                    if (key.KeyChar == 'Y' || key.KeyChar == 'y')
                    {
                        video = FrmVideoInfo.GetVideoInfo(video);
                        if (video != null)
                        {
                            GetSubsForTorrent(video, videoName, targetFolder, tempFolder);
                        }
                    }
                }
            }
        }

        private static void Download(
            Video video,
            string videoName,
            string targetFolder,
            string tempFolder,
            SubDivXManager subDivXManager,
            Sub selectedSub)
        {
            var tempRarFile = Path.Combine(tempFolder, "subtmp.rar");

            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }

            Console.WriteLine("Downloading sub to {0}", tempRarFile);

            subDivXManager.DownloadSub(tempRarFile, selectedSub.DownloadUrl);

            var rarFilesExtracted = new List<string>();

            var rarFiles = new List<string>();
            rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.rar", SearchOption.TopDirectoryOnly));
            rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.zip", SearchOption.TopDirectoryOnly));

            while (rarFiles.Any())
            {
                foreach (var rarFile in rarFiles)
                {
                    if (!RarArchive.IsRarFile(rarFile))
                    {
                        using (var zipFile = new ZipFile(rarFile))
                        {
                            zipFile.ExtractAll(tempFolder, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                    else
                    {
                        RarArchive.WriteToDirectory(rarFile, tempFolder, ExtractOptions.Overwrite);
                    }

                    rarFilesExtracted.Add(rarFile);
                }

                rarFiles.Clear();
                rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.rar", SearchOption.TopDirectoryOnly));
                rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.zip", SearchOption.TopDirectoryOnly));

                rarFiles.RemoveAll(item => rarFilesExtracted.Contains(item));
            }

            var subFiles = new List<string>();
            subFiles.AddRange(Directory.GetFiles(tempFolder, "*.sub", SearchOption.TopDirectoryOnly));
            subFiles.AddRange(Directory.GetFiles(tempFolder, "*.srt", SearchOption.TopDirectoryOnly));

            if (subFiles.Where(item => item.IndexOf(video.ReleaseGroup, StringComparison.OrdinalIgnoreCase) > -1).Any())
            {
                subFiles.RemoveAll(item => item.IndexOf(video.ReleaseGroup, StringComparison.OrdinalIgnoreCase) < 0);
            }
            else
            {
                Console.WriteLine("No subtitles matching ReleaseGroup found");
            }

            if (subFiles.Count == 1)
            {
                Console.WriteLine("Best match found, renaming to match video filename.");
            }
            else
            {
                Console.WriteLine("Mutiple subtitles found, leaving file names unmodified.");
            }

            foreach (var subFile in subFiles)
            {
                var destFile = string.Empty;
                if (subFiles.Count == 1)
                {
                    destFile = Path.Combine(targetFolder, Path.GetFileName(videoName + Path.GetExtension(subFile)));
                }
                else
                {
                    destFile = Path.Combine(targetFolder, Path.GetFileName(subFile));
                }

                File.Copy(subFile, destFile, true);
                Console.WriteLine("Copying sub {0} to {1}", Path.GetFileName(destFile), Path.GetDirectoryName(destFile));
            }
        }
    }
}
