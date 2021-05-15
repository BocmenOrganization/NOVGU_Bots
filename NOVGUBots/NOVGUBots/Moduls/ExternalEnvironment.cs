using BotsCore.Moduls;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Timers;

namespace NOVGUBots.Moduls
{
    public static class ExternalEnvironment
    {
        private const string Token_openweathermap = "759726268b72dc806df14e82adb2d218";
        private const uint KoofFaringate = 273;
        // ==================================================================================== Данные освещённости времени суток
        public static (TimeSpan start, TimeSpan end) blueHourMorning { get; private set; }
        public static (TimeSpan start, TimeSpan end) goldenHourMorning { get; private set; }
        public static (TimeSpan start, TimeSpan end) goldenHourNight { get; private set; }
        public static (TimeSpan start, TimeSpan end) blueHourNight { get; private set; }
        // ==================================================================================== Данные о погоде
        /// <summary>
        /// Текущая температура, в °C
        /// </summary>
        public static double Temp { get; private set; }
        /// <summary>
        /// Ощущаемая температура, в °C
        /// </summary>
        public static double TempFeelsLike { get; private set; }
        /// <summary>
        /// Минимальная температура, в °C
        /// </summary>
        public static double TempMin { get; private set; }
        /// <summary>
        /// Максимальная температура, в °C
        /// </summary>
        public static double TempMax { get; private set; }
        /// <summary>
        /// Скорость ветра, в м/с
        /// </summary>
        public static double WindSpeed { get; private set; }
        /// <summary>
        /// Облачность, значения от 0 до 1
        /// </summary>
        public static float Clouds { get; private set; }
        /// <summary>
        /// Давление, в мм. рт. ст.
        /// </summary>
        public static uint Pressure { get; private set; }
        /// <summary>
        /// Влажность, значения от 0 до 1
        /// </summary>
        public static float Humidity { get; private set; }


        private static readonly Timer timerUpdateSunPosition;
        private static readonly Timer timerUpdateWeather;

        static ExternalEnvironment()
        {
            timerUpdateSunPosition = new();
            timerUpdateSunPosition.Elapsed += (o, e) => LoadInfoDay();
            timerUpdateSunPosition.AutoReset = false;
            LoadInfoDay();
            timerUpdateWeather = new();
            timerUpdateWeather.Elapsed += (o, e) => LoadWeather();
            timerUpdateWeather.AutoReset = true;
            timerUpdateWeather.Interval = 3600000;
            LoadWeather();
        }

        private static void LoadInfoDay()
        {
            HtmlDocument document = new();
            document.LoadHtml(new WebClient().DownloadString("https://voshod-solnca.ru/sun/%D0%B2%D0%B5%D0%BB%D0%B8%D0%BA%D0%B8%D0%B9_%D0%BD%D0%BE%D0%B2%D0%B3%D0%BE%D1%80%D0%BE%D0%B4"));

            blueHourMorning = GetDates("blueHourMorning");
            goldenHourMorning = GetDates("goldenHourMorning");
            goldenHourNight = GetDates("goldenHourNight");
            blueHourNight = GetDates("blueHourNight");
            timerUpdateSunPosition.Interval = GetTimerPeriod();
            timerUpdateSunPosition.Start();
            (TimeSpan start, TimeSpan end) GetDates(string id)
            {
                TimeSpan[] dates = document.DocumentNode.SelectSingleNode($"//input[@id='{id}'][@value]").Attributes["value"].Value?.Split('—')?.Select(x => TimeSpan.Parse(x))?.ToArray();
                return (dates.First(), dates.Last());
            }
        }
        private static void LoadWeather()
        {
            try
            {
                dynamic objJson = JObject.Parse((new WebClient()).DownloadString($"http://api.openweathermap.org/data/2.5/weather?lat=58,5213&lon=31,271&appid={Token_openweathermap}"));
                Temp = objJson.main.temp - KoofFaringate;
                TempFeelsLike = objJson.main.feels_like - KoofFaringate;
                TempMin = objJson.main.temp_min - KoofFaringate;
                TempMax = objJson.main.temp_max - KoofFaringate;
                Pressure = objJson.main.pressure * 0.750063755419211;
                Humidity = objJson.main.humidity / 100f;
                WindSpeed = objJson.wind.speed;
                Clouds = objJson.clouds.all / 100f;
            }
            catch(Exception e) { EchoLog.Print($"Не удалось получить данные о погоде: {e.Message}", EchoLog.PrivilegeLog.Warning); }
        }
        private static double GetTimerPeriod() => (DateTime.Now.Date.AddHours(3).AddDays(1) - DateTime.Now).TotalMilliseconds;
    }
}