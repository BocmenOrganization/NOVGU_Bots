using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class User
    {
        public string Name;
        public uint Id;
        public string IdString;
        public string Email;
        public string UrlPage => $"http://people.novsu.ru/profiles/html/profileView.do?userid={IdString}&lang=ru_ru";
        public string PageFiles => $"https://www.novsu.ru/doc/study/{IdString}";
        private static CookieContainer cookiesPars;
        private static readonly Regex rg = new Regex("userid=([^&$\r\n]*)", RegexOptions.Compiled);

        public User(string Name, string url)
        {
            this.Name = Name;
            Id = uint.Parse(Path.GetFileName(url));
            IdString = GetId(GetUrlUserPage(Id));
            Email = GetEmail(UrlPage);
        }
        private static string GetId(string url)
        {
            MatchCollection matchedAuthors = rg.Matches(url);
            if (matchedAuthors != null && matchedAuthors.Count > 0)
                return matchedAuthors.Last().Groups[1].Value;
            return null;
        }
        private static string GetEmail(string url)
        {
            HtmlDocument htmlDocument = GetHtmlDocumentPageUser(url);
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//span[@id='npe_instance_3453_npe_content']//table[@class='viewtablewhite']//table[2]//a[@href]");
            return htmlNodes?.FirstOrDefault(x => x.Attributes["href"].Value.Contains("mailto"))?.InnerText;
        }
        private static HtmlDocument GetHtmlDocumentPageUser(string url)
        {
            if (cookiesPars == null)
            {
                var cookiesContener = new CookieContainer();
                cookiesContener.Add(GetResponse(url).Cookies);
                if (cookiesPars == null)
                    cookiesPars = cookiesContener;
            }
            var r = GetResponse(url);
            using Stream stream = r.GetResponseStream();
            using StreamReader streamReader = new StreamReader(stream);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(streamReader.ReadToEnd());
            if (htmlDocument.DocumentNode.SelectSingleNode("//span[@id='npe_instance_3453_npe_content']") != default)
                return htmlDocument;
            else
            {
                cookiesPars = null;
                return GetHtmlDocumentPageUser(url);
            }
        }
        private static HttpWebResponse GetResponse(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookiesPars == default ? new CookieContainer() : cookiesPars;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
        private static string GetUrlUserPage(uint id)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"https://www.novsu.ru/person/{id}/");
            try
            {
                req.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Response.Headers.AllKeys.Contains("Location"))
                {
                    return e.Response.Headers["Location"];
                }
            }
            return null;
        }
        public override string ToString() => Name;
    }
}