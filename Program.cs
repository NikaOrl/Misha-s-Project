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
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MishasBot";

                string key; // Here will be the Telegram token
                using (StreamReader inputFile = new StreamReader(mydocpath + @"\token.txt", true))
                {
                    key = inputFile.ReadToEnd();
                }

                var Bot = new Telegram.Bot.TelegramBotClient(key); // API initialization
                await Bot.SetWebhookAsync(""); // Delete the old link

                bool stop = false; 

                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return;
                    var update = evu.Update;
                    var message = update.Message;
                    if (message == null) return;

                    // Open README for more information
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage) 
                    {
                        if (message.Text[0] == '/')
                        {
                            switch (message.Text)
                            {
                                case "/hello":
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "Hi!");
                                    break;

                                case "/follow":
                                    bool haveID = false;
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "Done");
                                    try
                                    {
                                        using (StreamReader FollowList = new StreamReader(mydocpath + @"\followers.txt", true))
                                        {
                                            while (!FollowList.EndOfStream)
                                            {
                                                long TheID = Convert.ToInt64(FollowList.ReadLine());
                                                if (TheID == message.Chat.Id) haveID = true;
                                            }
                                        }
                                    }
                                    catch (Exception) { } // followers.txt can't be found
                                    if (!haveID)
                                    {
                                        using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\followers.txt", true))
                                        {
                                            outputFile.WriteLine(message.Chat.Id);
                                        }
                                    }
                                    break;

                                case "/unfollow":
                                    string[] readText = File.ReadAllLines(mydocpath + @"\followers.txt");
                                    using (StreamWriter file = new StreamWriter(mydocpath + @"\followers.txt", false))
                                    {
                                        for (int i = 0; i < readText.Length; i++)
                                        {
                                            if (Convert.ToInt64(readText[i]) != message.Chat.Id)
                                                file.WriteLine(readText[i]);
                                        }
                                    }
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "Ok");
                                    break;
                            }
                        }
                        else
                        {
                            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\messages.txt", true))
                            {
                                outputFile.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " said: \"" + message.Text + "\" " + message.Date);
                            }
                            try
                            {
                                string[] readLine = File.ReadAllLines(mydocpath + @"\followers.txt");

                                if (readLine[0] != "")
                                {
                                    stop = false;
                                    if (msg == "") // Checking if it's the first message
                                    {
                                        while (!stop)
                                        {
                                            using (StreamReader inputFile = new StreamReader(mydocpath + @"\messages.txt", true))
                                            {
                                                msg = inputFile.ReadToEnd();
                                            }
                                            try
                                            {
                                                using (StreamReader FollowList = new StreamReader(mydocpath + @"\followers.txt", true))
                                                {
                                                    while (!FollowList.EndOfStream)
                                                    {
                                                        long TheID = Convert.ToInt64(FollowList.ReadLine());
                                                        //if (TheID != message.Chat.Id) //We tried not to show the user their own messages. Faled.
                                                        await Bot.SendTextMessageAsync(TheID, msg);
                                                    }
                                                }
                                            }
                                            catch (Exception) { } // followers.txt can't be found
                                            System.Threading.Thread.Sleep(60000);
                                        }
                                    }
                                }
                                else
                                {
                                    stop = true;
                                }
                            }
                            catch (Exception) { }
                        }

                        // Logs              
                        using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\logs.txt", true))
                        {
                            outputFile.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " said \"" + message.Text + "\" " + message.Date);
                        }   
                    }
                };

                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex) // In case of invalid Telegram token
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
