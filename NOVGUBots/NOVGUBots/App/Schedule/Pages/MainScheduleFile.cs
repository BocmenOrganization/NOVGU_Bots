using BotsCore.Bots.Model;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule;
using BotsCore.Moduls.Tables.Services;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using static BotsCore.Bots.Model.ObjectDataMessageSend;

namespace NOVGUBots.App.Schedule.Pages
{
    public class MainScheduleFile : Page
    {
        private static readonly ModelMarkerTextData DownloadText = StaticData.Text_NoDataSchedule.GetElemNewId(13);
        private static readonly ModelMarkerTextData SatusDownloadFileInfo = DownloadText.GetElemNewId(14);
        private static readonly ModelMarkerStringlData ProgressBarChar = new ModelMarkerStringlData(CretePageSchedule.NameApp, CretePageSchedule.NameTableString, 19);
        private static readonly ModelMarkerStringlData ProgressBarCharEnd = ProgressBarChar.GetElemNewId(20);

        private Task taskMessage;
        private string Url;
        private bool StopLoad;
        private bool LoadPhotos;
        private bool LoadetPhoto;
        private string[] Photos;
        private Media[] medias;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventInMessage(ObjectDataMessageInBot inBot) => ResetLastMessenge(inBot);
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            if (!LoadetPhoto)
            {
                if (!LoadPhotos)
                {
                    taskMessage = Task.Run(() =>
                    {
                        LoadetPhoto = true;
                        Url = (DataNOVGU.GetScheduleUser(inBot) is TableScheduleStudents tableScheduleStudents) ? tableScheduleStudents.UrlSchedule : null;
                        if (Url != null && !StopLoad)
                        {
                            var task = Task.Run(() =>
                            {
                                Photos = GetUrlXlsToPNG(GetHttpContentFileUrl(Url));
                            });
                            string LoadText = string.Empty;
                            while (!task.IsCompleted && !StopLoad)
                            {
                                SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format(SatusDownloadFileInfo.GetText(inBot), $"{LoadText}{ProgressBarCharEnd}", $"[{DownloadText.GetText(inBot)}]({Url})") });
                                LoadText += ProgressBarChar;
                                System.Threading.Thread.Sleep(1000);
                            }
                            if (!StopLoad)
                            {
                                LoadPhotos = true;
                                medias = Photos?.Select(x => new Media(x, Media.MediaType.Photo)).ToArray();
                                ResetLastMessenge(inBot);
                            }
                        }
                        LoadetPhoto = true;
                    });
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Url))
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = $"[{DownloadText.GetText(inBot)}]({Url})", media = medias });
                else
                    SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = StaticData.Text_NoDataSchedule });
            }
        }
        public override void EventClose(ObjectDataMessageInBot inBot)
        {
            StopLoad = true;
            taskMessage?.Wait();
        }
        private static string[] GetUrlXlsToPNG(Stream file)
        {
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    HttpContent stringContent1 = new StreamContent(file);
                    stringContent1.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"0941 0931 0901 0902(2).xls\"");
                    stringContent1.Headers.Add("Content-Type", "application/vnd.ms-excel");
                    formData.Add(stringContent1);
                    formData.Add(new StringContent("png"), "targetformat");
                    formData.Add(new StringContent("86000"), "code");
                    formData.Add(new StringContent("local"), "filelocation");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://www.aconvert.com/");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
                    var r = JsonConvert.DeserializeObject(client.PostAsync("https://s6.aconvert.com/convert/convert-batch-win.php", formData).Result.Content.ReadAsStringAsync().Result);
                    if (r is JObject valuePairs)
                    {
                        var filename = valuePairs.GetValue("filename").Value<string>();
                        var num = int.Parse(valuePairs.GetValue("num").Value<string>());//https://s6.aconvert.com/convert/p3r68-cdx67/
                        return Enumerable.Range(1, num).Select(x => string.Format("https://s6.aconvert.com/convert/p3r68-cdx67/{0}-{1:000}.png", filename, x)).ToArray();
                    }
                }
            }
            return null;
        }
        private static Stream GetHttpContentFileUrl(string UrlFile)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
                return client.GetAsync(UrlFile).Result.Content.ReadAsStream();
            }
        }
    }
}
