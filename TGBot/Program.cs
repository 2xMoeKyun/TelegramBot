using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Net;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;

namespace TGBot
{
    class Program
    {
        static TelegramBotClient bot = new TelegramBotClient("5674243047:AAFPXyiFnCTqjl4oHhMCuPyyhAZUhXTaqFk");

        static void Main(string[] args)
        {


            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }


        public static List<string> GetSearch(bool showHTML = false)
        {
            HtmlDocument doc = new HtmlDocument();


            string page = GetSitePage("https://hh.ru/vacancies/programmist?hhtmFromLabel=rainbow_profession&page=1&hhtmFrom=main");
            if (showHTML)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(page);
                Console.ResetColor();
            }
            doc.LoadHtml(page);

            List<string> images = new List<string>();
            List<string> links = new List<string>();

            try
            {
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a "))
                {
                    foreach (var item in link.Attributes)
                    {
                        if (item.Name == "href" && item.Value.Contains("http") && item.Value.Contains("vacancy"))
                        {
                            links.Add(item.Value);
                        }
                        //if (item.Name == "class" && item.Value == "vacancy-serp-item-body__main-info")
                        //{
                        //    foreach (var source in link.SelectNodes("//a "))
                        //    {
                        //        foreach (var x in source.Attributes)
                        //        {

                        //        }
                        //    }

                        //}
                    }

                }
            }
            catch (System.NullReferenceException)
            {
                links.Add("https://hh.ru/analytics_source/vacancy/70701163?query=программист&requestId=1666548644855a3e4883e7763e91531c&totalVacancies=15312&position=0&from=vacancy_search_catalog&hhtmFrom=vacancy_search_catalog&source=vacancies");
            }
            return links;
        }

        public static string GetSitePage(string url)
        {

            WebClient client = new WebClient();

            string htmlCode = client.DownloadString(url);

            return htmlCode;
        }


        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать");
                    return;
                }
                if (message.Text.ToLower() == "/start hh")
                {
                    var a = GetSearch();
                    await botClient.SendTextMessageAsync(message.Chat.Id, a[(new Random()).Next(a.Count + 1)]);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Не верная команда");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}
