using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;

namespace SecondTaskAI
{
    class Program
    {
        static string message = "";
        private static async Task Main(string[] args)
        {
            Thread PrinterMessage = new Thread(PrintMessage);
            List<string> ListUser = csvHelper.ReadCsvFile(@"data\dataF.csv", "Ваш ID в VK");
            ListUser = ListUser.Select(item => CorrecterVkId(item)).ToList();
            VkApi api = await Authorization();
            PrinterMessage.Start();
            //List<long> idVkUser = api.Users.Get(ListUser).Select(n => n.Id).ToList();
            //checkIdVk(api, "id_totktosmog");
            foreach (string userId in ListUser)
            {
                checkIdVk(api, userId);
            }
            sendMessageConsole("exit");


            Console.ReadKey();
            
        }

        private static void PrintMessage()
        {
            string temp = "";
            while (message != "exit")
            {
                if (message == "" || message == temp) continue;
                else
                {
                    temp = message;
                    Console.WriteLine(message);
                }
            }
        }
        private static void sendMessageConsole(string mes, ConsoleColor color = ConsoleColor.White )
        {
            Console.ForegroundColor = color;
            message = mes;
        }
        private static string CorrecterVkId(string item) => item.Trim('@').Replace("https://vk.com/", "");
        private static async Task<VkApi> Authorization()
        {
            string AccessToken = await txtHelper.ReadFileAsync(Authorize.getAuthorizeDataPath());
            VkApi api = new VkApi();
            api.Authorize(new ApiAuthParams()
            {
                AccessToken = AccessToken,
                Settings = Settings.All
            });
            return api;
        }
        private static VkCollection<User> GetVkFriends(VkApi api, long idVk, int MaxCountFriends = 20000)
        {
            VkCollection<User> friend = null;
            try
            {
                friend = api.Friends.Get(new FriendsGetParams { UserId = idVk, Count = MaxCountFriends });
            }
            catch(Exception e)
            {
                sendMessageConsole($"\t Возникла ошибка {e.Message}", ConsoleColor.Red);
            }
            return friend;
        }
        private static void checkIdVk(VkApi api, string id)
        {
            sendMessageConsole($"Начало работы с id/псевдонимом {id}",ConsoleColor.Green);
            var uservk = api.Users.Get(new List<string>() { id });
            if (uservk.Count() > 0)
            {
                long idvk = uservk[0].Id;
                string path = $@"data\dataUser\{id}.txt";
                if (txtHelper.FileExists(path))
                {
                    sendMessageConsole($"Файл с пользователем id {id} уже существует", ConsoleColor.Green);
                    Thread.Sleep(1000);
                    return;
                }
                sendMessageConsole($"Запись в файл: {path}");
                VkCollection<User> friend = GetVkFriends(api, idvk);
                if (friend != null)
                {
                    sendMessageConsole($"У польователя {id}, числовой id = {idvk}, количество друзей {friend.Count}");
                    List<string> myFriend = friend.Select(n => $"{idvk}:{n.Id}").ToList();
                    txtHelper.WriteFileLinesAsync(path, myFriend);
                    int i = 1;
                    foreach (User user in friend)
                    {
                        sendMessageConsole($"\t {i++}) начало работы с другом id {user.Id}");
                        VkCollection<User> f = GetVkFriends(api, user.Id);
                        if (f != null)
                        {
                            sendMessageConsole($"\t количестов друзей {f.Count}");
                            myFriend = f.Select(n => $"{user.Id}:{n.Id}").ToList();
                            txtHelper.WriteFileLinesAsync(path, myFriend, true);
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
            else
                sendMessageConsole($"Пользователь по id {id} не найден", ConsoleColor.Red);
            Thread.Sleep(1000);
            sendMessageConsole($"Закончина работа с пользователем {id}",ConsoleColor.Green);  
        }
    }
}
