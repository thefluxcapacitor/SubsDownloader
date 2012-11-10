namespace SubsDownloader
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.IO;

    using Ionic.Zip;

    using NUnrar;
    using NUnrar.Archive;
    using NUnrar.Common;

    class Program
    {
        static void Main(string[] args)
        {
            //TODO: add verbose mode with app.config setting

            if (args.Length != 1)
            {
                return;
            }

            var media = File.Exists(args[0]) ? Path.GetFileNameWithoutExtension(args[0]) :
                args[0].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

            GetSubsForTorrent(media, Path.GetDirectoryName(args[0]));

            Console.WriteLine("Press any key to exit...");
            // TODO: support -noui switch. If switch is present, exit without prompting to press a key
            Console.ReadKey();
        }

        private static void GetSubsForTorrent(string videoName, string targetFolder)
        {
            var video = new Video(videoName);

            var subDivXManager = new SubDivXManager();
            var subs = subDivXManager.GetCandidateSubs(video);

            if (subs.Any())
            {
                // TODO: support -noui switch. If switch is present don't show the form and automatically download the sub with more downloads

                var selectedSub = FrmShowSubs.SelectSub(subs);
                if (selectedSub != null)
                {
                    // TODO: support specifiying the temp folder (through cmdline or app.config)

                    var tempFolder = Path.Combine(targetFolder, "tmp");
                    var tempRarFile = Path.Combine(tempFolder, "subtmp.rar");

                    Directory.Delete(tempFolder, true);

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
                            
                            //File.Delete(rarFile); << TODO: fails. bug in RarArchive
                        }

                        rarFiles.Clear();
                        rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.rar", SearchOption.TopDirectoryOnly));
                        rarFiles.AddRange(Directory.GetFiles(tempFolder, "*.zip", SearchOption.TopDirectoryOnly));

                        rarFiles.RemoveAll(item => rarFilesExtracted.Contains(item));
                    }

                    var subFiles = new List<string>();
                    subFiles.AddRange(Directory.GetFiles(tempFolder, "*.sub", SearchOption.TopDirectoryOnly));
                    subFiles.AddRange(Directory.GetFiles(tempFolder, "*.srt", SearchOption.TopDirectoryOnly));

                    if (subFiles.Where(item => item.Contains(video.ReleaseGroup)).Any())
                    {
                        subFiles.RemoveAll(item => !item.Contains(video.ReleaseGroup));
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
            else
            {
                Console.WriteLine("No subs were found");
            }
        }
    }
}
