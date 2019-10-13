using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using PM_Utils;

namespace BotDiscord_IMSO
{
    static partial class Program
    {
        private static string log_fname = "log.txt";
        private static ulong  log_file_number = 0;
        private static bool file_used = true;

        private static Task BotLog(LogMessage message)
        {
            string text = "";
            if (message.Exception != null)
            {
                text = $"Erreur [{message.Severity}] : " + message.Exception + "\n";
                PConsole.PrintLn(text, ConsoleColor.DarkRed);
                WriteFileLog(text);
            }
            else
            {
                text = $"Log [{message.Severity}] : " + message.Message + "\n";
                PConsole.PrintLn(text, ConsoleColor.DarkGreen);
                WriteFileLog(text);
            }

            return Task.CompletedTask;
        }

        private static void WriteError(string msg, string code, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "XXXX", [CallerFilePath] string file = "XXXX")
        {
            string text = $"ERREUR [{code}] (ligne : {line} || membre : {member} || fichier : {file})\n\tmessage >> {msg}";
            PConsole.WErrorParsing(text);
            WriteFileLog(text);
        }

        private static void WriteLog(string msg)
        {
            string text = "LOG > " + msg + "\n";
            PConsole.PrintLn(text, ConsoleColor.Cyan);
            WriteFileLog(text);

        }

        private static void WriteLoopLog(string msg)
        {
            string text = "BOUCLE:log > " + msg + "\n";
            PConsole.PrintLn(text, ConsoleColor.Green);
            WriteFileLog(text);

        }

        private static void WriteFileLog(string message)
        {
            Task.Run(() => WriteFileLogAsync(message));
        }

        private static async Task WriteFileLogAsync(string message)
        {
            while (file_used)
            {
                await Task.Delay(5);
            }

            file_used = true;
                
            if (!File.Exists(log_fname))
            {
                File.Create(log_fname);
                File.AppendAllText(log_fname, GetIntroFileText());
            }

            if(log_file_number == 0)
                File.AppendAllText(log_fname, GetIntroFileText());


            File.AppendAllText(log_fname,++log_file_number + " ==>> " + DateTime.Now.ToString() + " ==>> " + message.Trim('\n') + "\n");
        }

        private static string GetIntroFileText() => "\n\n\n\n\n------------------------------------\n" + DateTime.Now.ToLongDateString() + "\n\n";
    }
}
