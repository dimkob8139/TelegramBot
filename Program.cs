using ApiAiSDK;
using ApiAiSDK.Model;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class Program
    {
        public static TelegramBotClient Bot;
        public static ApiAi apiAi;
        public static void Main(string[] args)
        {
            //1499050142:AAHa7tohwfgKTWSHWOSjqsl9RXgEUD3jC6A
            Bot = new TelegramBotClient("1499050142:AAHa7tohwfgKTWSHWOSjqsl9RXgEUD3jC6A");
            AIConfiguration config = new AIConfiguration("AIzaSyCYA-hP9grKxjz16MMakirIRkIEpHuz6iI", SupportedLanguage.Russian);
            apiAi = new ApiAi(config);
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQueryReceived;


            var me = Bot.GetMeAsync().Result;

            Console.WriteLine(me.FirstName);
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();

        }

        private static async void Bot_OnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            try
            {
                string buttonText = e.CallbackQuery.Data;
                string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
                Console.WriteLine($"{name} нажал кнопочку {buttonText}");
                if (buttonText == "Картинка")
                {
                    await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://photos.google.com/photo/AF1QipMkjoVrNO21ikAMnqpsT-Gp7boGH21HattCVgvO");
                }
                if (buttonText == "Видео")
                {
                    await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=v8YmhbNf5Nw&t=2861s");
                }
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопочку {buttonText}");
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text) return;
            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} отправил сообщение :'{message.Text}'");

            switch (message.Text)
            {
                case "/start":
                    string text = "@Список команд:                  " +
                          "/start -запуск бота,                    " +
                          " /callback - вывод меню,                  " +
                          "/keyboard - вывод клавиатуры             ";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/callback":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Rozetka","https://rozetka.com.ua/"),
                             InlineKeyboardButton.WithUrl("Citrus","https://www.citrus.ua/")
                        },


                       new[]
                       {
                        InlineKeyboardButton.WithCallbackData("Картинка"),
                        InlineKeyboardButton.WithCallbackData("Видео")
                       }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню", replyMarkup: inlineKeyboard);



                    break;

                case "/keyboard":

                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Привет"),
                             new KeyboardButton("Как делишки?")
                        },
                        new[]
                        {
                            new KeyboardButton("Контакт") {RequestContact=true },
                            new KeyboardButton("Геолокация"){RequestLocation=true}
                        }
                    }
                        );
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboard);

                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                    {
                        answer = "Сорри, но я тебя не понял";
                    }
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }
        }
    }
}
