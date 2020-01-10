using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace TeleBot
{
    class Program
    {
        static TelegramBotClient bot;

        static void Main(string[] args)
        {
            connectToBot();

            bot.OnMessage += messageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void messageListener(object sender, MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            switch (e.Message.Type)
            {
                case MessageType.Text:
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Text}");
                    break;
                case MessageType.Photo:
                    var photo = e.Message.Photo.OrderByDescending(p => p.Width).FirstOrDefault();
                    if (photo != null)
                    {
                        download(photo.FileId);
                        bot.SendPhotoAsync(e.Message.Chat.Id, new InputOnlineFile(photo.FileId));
                    }
                    break;
                case MessageType.Audio:
                    download(e.Message.Audio.FileId);
                    bot.SendAudioAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.Audio.FileId));
                    break;
                case MessageType.Video:
                    download(e.Message.Video.FileId);
                    bot.SendVideoAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.Video.FileId));
                    break;
                case MessageType.Voice:
                    download(e.Message.Voice.FileId);
                    bot.SendVoiceAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.Voice.FileId));
                    break;
                case MessageType.Document:
                    download(e.Message.Document.FileId);
                    bot.SendDocumentAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.Document.FileId));
                    break;
                case MessageType.Sticker:
                    download(e.Message.Sticker.FileId);
                    bot.SendStickerAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.Sticker.FileId));
                    break;
                case MessageType.VideoNote:
                    download(e.Message.VideoNote.FileId);
                    bot.SendVideoNoteAsync(e.Message.Chat.Id, new InputOnlineFile(e.Message.VideoNote.FileId));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async void download(string fileId)
        {
            var file = await bot.GetFileAsync(fileId);
            using (FileStream fs = new FileStream(new FileInfo(file.FilePath).Name, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        private static void connectToBot()
        {
            string token = File.ReadAllText("token.txt");

            var proxy = new WebProxy
            {
                Address = new Uri("http://195.201.22.127:3128"),
                UseDefaultCredentials = false,
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy
            };

            HttpClient hc = new HttpClient(httpClientHandler);

            bot = new TelegramBotClient(token, hc);
        }
    }
}