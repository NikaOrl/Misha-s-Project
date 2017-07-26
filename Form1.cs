using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ForMisha1_Bot
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;

        public Form1()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //

            this.bw = new BackgroundWorker();
            this.bw.DoWork += bw_DoWork;
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String; // получаем ключ из аргументов
            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key); // инициализируем API
                await Bot.SetWebhookAsync(""); // Обязательно! убираем старую привязку к вебхуку для бота
                // Callback'и от кнопок
                /*Bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    var message = ev.CallbackQuery.Message;
                    if(ev.CallbackQuery.Data == "callback1") {
                        await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "You hav choosen " + ev.CallbackQuery.Data, true);
                    } else 
                    if (ev.CallbackQuery.Data == "callback2")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "тест", replyToMessageId: message.MessageId);
                        await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                    }
                };*/

                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; // в этом блоке нам келлбэки и инлайны не нужны
                    var update = evu.Update;
                    var message = update.Message;
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        if (message.Text == "/saysomething")
                        {
                            // в ответ на команду /saysomething выводим сообщение
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Hi!", replyToMessageId: message.MessageId);
                        }
                        // Set a variable to the My Documents path.
                        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        
                        // Write the string array to a file named "WriteLines.txt".
                        using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\Message.txt", true)) 
                        {
                            outputFile.WriteLine(message.Text);
                        }
                    }
                };

                // запускаем прием обновлений
                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // если ключ не подошел - пишем об этом в консоль отладки
            }

        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var text = @txtKey.Text; // получаем содержимое текстового поля txtKey в переменную text
            if (text != "" && this.bw.IsBusy != true)
            {
                this.bw.RunWorkerAsync(text); // передаем эту переменную в виде аргумента методу bw_DoWork
                BtnRun.Text = "...";
            }
        }
    }
}
