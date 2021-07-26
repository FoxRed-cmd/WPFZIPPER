using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace WPFInterface
{
	static class ZipExt
	{
		public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, CompressionLevel compressionLevel, bool includeBaseDirectory)
		{
			var folders = new Stack<string>();

			folders.Push(sourceDirName);

			if (includeBaseDirectory)
			{
				DirectoryInfo info = new DirectoryInfo(sourceDirName);
				string rootDir = info.Name;
				do
				{
					var currentFolder = folders.Pop();

					foreach (var item in Directory.GetFiles(currentFolder))
					{
						archive.CreateEntryFromFile(item, rootDir + "\\" + item.Substring(sourceDirName.Length + 1), compressionLevel);
					}

					foreach (var item in Directory.GetDirectories(currentFolder))
					{
						folders.Push(item);
					}
				}
				while (folders.Count > 0);
			}
			else
			{
				do
				{
					var currentFolder = folders.Pop();

					foreach (var item in Directory.GetFiles(currentFolder))
					{
						archive.CreateEntryFromFile(item, item.Substring(sourceDirName.Length + 1), compressionLevel);
					}

					foreach (var item in Directory.GetDirectories(currentFolder))
					{
						folders.Push(item);
					}
				}
				while (folders.Count > 0);
			}

		}
	}
}
