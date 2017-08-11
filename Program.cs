using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ForMisha1_Bot_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("pak"); // Were used for checking the process
            bw_DoWork();
            //Console.WriteLine("pik");
            Console.WriteLine("--done--");
            Console.ReadKey();
        }

        static async void bw_DoWork()
        {
            //Console.WriteLine("pek");
            try
            {
                //Console.WriteLine("puk");
                
                // Set a variable to the My Documents path
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string key; // Here will be the Telegram token
                using (StreamReader inputFile = new StreamReader(mydocpath + @"\token.txt", true))
                {
                    key = inputFile.ReadToEnd();
                }
                                
                var Bot = new Telegram.Bot.TelegramBotClient(key); // API initialization
                await Bot.SetWebhookAsync(""); // Delete the old link

                bool stop = false; // Thing for 'clock ticking'
                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return;
                    var update = evu.Update;
                    var message = update.Message;
                    if (message == null) return;
                    // Open README for more information
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage) 
                    {
                        if (message.Text == "/hello")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Hi!", replyToMessageId: message.MessageId);
                        }

                        if (message.Text == "/token")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, key, replyToMessageId: message.MessageId);
                        }

                        if (message.Text == "/clock")
                        {
                            stop = false;
                            while (!stop)
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "tick", replyToMessageId: message.MessageId);
                                System.Threading.Thread.Sleep(60000);
                            }
                            
                        }

                        if (message.Text == "/stop")
                        {
                            stop = true;
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Ok", replyToMessageId: message.MessageId);
                        }

                        using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\Message.txt", true))
                        {
                            outputFile.WriteLine(message.Text);
                        }
                    }
                };

                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // In case of invalid Telegram token
            }
            


        }



    }
}
