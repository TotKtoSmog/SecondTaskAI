using System;
using System.Threading.Tasks;

namespace SecondTaskAI.Service
{
    internal static class SenderMessage
    {
        static internal void SendMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }
        static internal void SendErrorMessage(string message)
            => SendMessage(message, ConsoleColor.Red);

        static internal async Task SendErrorMessageAsync(string message) 
            => await SendMessageAsync(message, ConsoleColor.Red);

        static internal async Task SendMessageAsync(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            await Console.Out.WriteLineAsync(message);
        }
    }
}
