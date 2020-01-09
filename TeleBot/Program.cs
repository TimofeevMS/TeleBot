using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TeleBot
{
    class Program
    {
        static TelegramBotClient bot;

        static void Main(string[] args)
        {
            string token = File.ReadAllText("token.txt");

            var proxy = new WebProxy
            {
                Address = new Uri("http://195.201.22.127:3128"),
                UseDefaultCredentials = false,
            };

            var httpClientHandler = new HttpClientHandler { Proxy = proxy };

            HttpClient hc = new HttpClient(httpClientHandler);

            bot = new TelegramBotClient(token, hc);

            //bot = new TelegramBotClient(token);

            var me = bot.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            bot.OnMessage += MessageListener;
            bot.OnUpdate += BotOnOnUpdate;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void BotOnOnUpdate(object sender, UpdateEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");
            
            switch (e.Message.Type)
            {
                case MessageType.Unknown:
                    break;
                case MessageType.Text:
                    break;
                case MessageType.Photo:
                    foreach (PhotoSize photoSiz in e.Message.Photo)
                    {
                        Console.WriteLine(photoSiz.FileId);
                        Console.WriteLine(photoSiz.FileSize);

                        DownLoad(photoSiz.FileId);
                    }
                    break;
                case MessageType.Audio:
                    break;
                case MessageType.Video:
                    break;
                case MessageType.Voice:
                    break;
                case MessageType.Document:
                    Console.WriteLine(e.Message.Document.FileId);
                    Console.WriteLine(e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileSize);

                    DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                    break;
                case MessageType.Sticker:
                    break;
                case MessageType.Location:
                    break;
                case MessageType.Contact:
                    break;
                case MessageType.Venue:
                    break;
                case MessageType.Game:
                    break;
                case MessageType.VideoNote:
                    break;
                case MessageType.Invoice:
                    break;
                case MessageType.SuccessfulPayment:
                    break;
                case MessageType.WebsiteConnected:
                    break;
                case MessageType.ChatMembersAdded:
                    break;
                case MessageType.ChatMemberLeft:
                    break;
                case MessageType.ChatTitleChanged:
                    break;
                case MessageType.ChatPhotoChanged:
                    break;
                case MessageType.MessagePinned:
                    break;
                case MessageType.ChatPhotoDeleted:
                    break;
                case MessageType.GroupCreated:
                    break;
                case MessageType.SupergroupCreated:
                    break;
                case MessageType.ChannelCreated:
                    break;
                case MessageType.MigratedToSupergroup:
                    break;
                case MessageType.MigratedFromGroup:
                    break;
                case MessageType.Animation:
                    break;
                case MessageType.Poll:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (e.Message.Text == null) return;

            var messageText = e.Message.Text;

            bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messageText}");
        }

        static async void DownLoad(string fileId)
        {
            var file = await bot.GetFileAsync(fileId);
            using (FileStream fs = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            using (FileStream fs = new FileStream("_" + path, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }
    }
}