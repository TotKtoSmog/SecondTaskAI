using System;

namespace SecondTaskAI.Service
{
    internal static class SenderMessage
    {
        static internal void SendMessage(string meassage, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(meassage);
        }
        static internal void SendErrorMessage(string meassage)
            => SendMessage(meassage, ConsoleColor.Red);
    }
}
