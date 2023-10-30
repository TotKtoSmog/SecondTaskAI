using System;

namespace SecondTaskAI.Service
{
    internal static class SenderMessage
    {
        static internal void sendMessage(string meassage, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(meassage);
        }
        static internal void sendErrorMessage(string meassage)
            => sendMessage(meassage, ConsoleColor.Red);
    }
}
