using System;
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
            string msg = "";
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
                            //await Bot.SendTextMessageAsync(message.Chat.Id, "Hi!", replyToMessageId: message.MessageId);
                            await Bot.SendTextMessageAsync(440075367, "Hi!");
                        }

                        if (message.Text == "/listen")
                        {
                            //await Bot.SendTextMessageAsync(message.Chat.Id, "Hi!", replyToMessageId: message.MessageId);
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Done");
                            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\followers.txt", true))
                            {
                                outputFile.WriteLine(message.Chat.Id);
                            }
                        }

                        /*if (message.Text == "/token")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, key, replyToMessageId: message.MessageId);
                        }*/

                        /*if (message.Text == "/clock") //clock = listen
                        {
                            int min = 0;
                            stop = false;
                            while (!stop)
                            {
                                //msg = string from Message(?).txt
                                await Bot.SendTextMessageAsync(message.Chat.Id, "tick", replyToMessageId: message.MessageId); //tick = "/"+msg+/" + min
                                System.Threading.Thread.Sleep(60000);
                                min++;
                            }
                            
                        }*/

                        /*if (message.Text == "/stopclock")
                        {
                            stop = true;
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Ok", replyToMessageId: message.MessageId);
                        }*/

                        if (message.Text == "/stop")
                        {
                            msg = "";
                            stop = true;
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Ok", replyToMessageId: message.MessageId);
                            //!!!!File.Delete("d:\\messages.txt"); //удаление файла 
                        }

                        if (message.Text[0] != '/')
                        {                           
                            if (msg == "")
                            {
                                using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\messages.txt", true))
                                {
                                    outputFile.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " said \"" + message.Text + "\"" + message.Date);
                                }
                                stop = false;
                                while (!stop)
                                {
                                    using (StreamReader inputFile = new StreamReader(mydocpath + @"\messages.txt", true))
                                    {
                                        msg = inputFile.ReadToEnd();
                                    }

                                    using (StreamReader FollowList = new StreamReader(mydocpath + @"\followers.txt", true))
                                    {
                                        while(!FollowList.EndOfStream)
                                        {
                                            long TheID = Convert.ToInt64(FollowList.ReadLine());
                                            if (TheID != message.Chat.Id) await Bot.SendTextMessageAsync(TheID, msg);
                                        }
                                    }
                                    System.Threading.Thread.Sleep(60000);
                                }
                            }
                            else
                            {
                                using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\messages.txt", true))
                                {
                                    outputFile.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " said \"" + message.Text + "\" " + message.Date);
                                }
                            }
                        }

                        using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\logs.txt", true))
                        {
                            outputFile.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " said \"" + message.Text + "\" " + message.Date);
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
