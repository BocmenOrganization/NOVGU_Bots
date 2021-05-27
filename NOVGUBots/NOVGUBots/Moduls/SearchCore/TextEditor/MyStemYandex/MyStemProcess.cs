using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchCore.TextEditor.MyStemYandex
{
    public class MyStemProcess : IDisposable
    {
        private const string UrlDownloadProcess = "http://download.cdn.yandex.net/mystem/mystem-3.0-win7-32bit.zip";
        private readonly static Regex versionRegex = new(@"\d\.\d", RegexOptions.Compiled);
        private const string arguments = "-cds --format json";
        private readonly object LockObject = new();
        private readonly Process mystem;

        /// <summary>
        /// Только для Windows, путь ссылкой ведёт архив с 1 испольняемым файлом (.exe)
        /// </summary>
        public static MyStemProcess LoadProccesUrl(string url) => new(GetPathProcess(url));
        public static string GetPathProcess(string url)
        {
            string ver = versionRegex.Match(url).Value;
            string pathFolder = Path.Combine(Path.GetTempPath(), "MyStemYandex", ver);
            string pathProcess = Path.Combine(pathFolder, $"mystem.exe");

            if (!File.Exists(pathProcess))
            {
                if (!Directory.Exists(pathFolder))
                    Directory.CreateDirectory(pathFolder);

                string pathArchive = Path.Combine(pathFolder, $"mystem.zip");
                using WebClient client = new();
                client.DownloadFile(UrlDownloadProcess, pathArchive);
                ZipFile.ExtractToDirectory(pathArchive, pathFolder);
                File.Delete(pathArchive);
                string path = SearchFile(pathFolder);
                File.Move(path, pathProcess);
                string[] dirs = Directory.GetDirectories(pathFolder);
                foreach (var dir in dirs)
                    Directory.Delete(dir, true);

                static string SearchFile(string folderSearch)
                {
                    string resul = Directory.GetFiles(folderSearch).FirstOrDefault(x => x.Contains(".exe"));
                    if (resul == null)
                    {
                        string[] dirs = Directory.GetDirectories(folderSearch);
                        foreach (var dir in dirs)
                        {
                            resul = SearchFile(dir);
                            if (resul != null)
                                return resul;
                        }
                    }
                    return resul;
                }
            }
            return pathProcess;
        }

        public MyStemProcess(string pathProcess = null)
        {
            mystem = new Process();
            mystem.StartInfo.FileName = pathProcess ?? GetPathProcess(UrlDownloadProcess);
            mystem.StartInfo.UseShellExecute = false;
            mystem.StartInfo.RedirectStandardInput = true;
            mystem.StartInfo.RedirectStandardOutput = true;
            mystem.StartInfo.RedirectStandardError = true;
            mystem.StartInfo.StandardInputEncoding = Encoding.UTF8;
            mystem.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            mystem.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            mystem.StartInfo.CreateNoWindow = true;
            mystem.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            mystem.StartInfo.Arguments = arguments;
            mystem.Start();
        }

        private string GetJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            lock (LockObject)
            {
                mystem.StandardInput.BaseStream.Write(Encoding.UTF8.GetBytes($"{FilterText(text)}\n"));
                mystem.StandardInput.BaseStream.Flush();
                return mystem.StandardOutput.ReadLine();
            }
        }
        private static string FilterText(string text) => new(text.Where(c => !char.IsSurrogate(c)).ToArray());
        public string[] GetWordsAndSpace(string text)
        {
            string json = GetJson(text);
            if (json == null)
                return null;
            JArray array = JArray.Parse(json);
            return array.Select(x =>
            {
                var obj = x.ToObject<JObject>();
                if (obj.ContainsKey("analysis"))
                {
                    var analysisArray = obj.GetValue("analysis").ToObject<JArray>();
                    if (analysisArray.Count > 0)
                    {
                        var analysisObj = analysisArray[0].ToObject<JObject>();
                        if (analysisObj.ContainsKey("lex"))
                            return analysisObj.GetValue("lex").ToObject<string>();
                    }
                    return string.Empty;
                }
                if (obj.ContainsKey("text"))
                    return obj.GetValue("text").ToObject<string>();
                return string.Empty;
            }).SkipLast(1).ToArray();
        }
        public string[] GetWords(string text) => GetWordsAndSpace(text)?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        public string GetText(string text)
        {
            string[] words = GetWordsAndSpace(text);
            if (words == null)
                return null;
            return string.Join(string.Empty, words);
        }

        public void Dispose() => mystem?.Dispose();
    }
}