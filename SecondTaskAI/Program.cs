using SecondTaskAI.Model;
using SecondTaskAI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
        static List<string> paths = new List<string>();
        static List<VkApiUser> users = new List<VkApiUser>();
        static IDictionary<string, string> parameters = new Dictionary<string, string>();
        private static void getDataUserInFile(ref List<VkApiUser> users, string PathResFile, VkApi api)
        {
            string param = "";
            int i = -1;
            int counter = 0;
            List<string> list = new List<string>();
            foreach (var u in users)
            {
                i = -1;
                counter = 0;
                param = "";

                while (param != "" || i < u.friends.Count - 1)
                {
                    i++;
                    SenderMessage.sendMessage($"{i + 1}) Пользователь {u.friends[i].me}");
                    param += $"{u.friends[i].me},";
                    if (i % 12 == 0 && i != 0)
                    {

                        int id = param.Split(',').Count() - 1;
                        var result = GetFriendsID(ref param, api);

                        for (int j = 0; j < id; j++)
                        {
                            foreach (var item in result[j].ToListOf<string>(n => n))
                                u.friends[counter].friends.Add(new VkApiUser(item));
                            counter++;
                        }
                        Thread.Sleep(310);
                    }
                    if (i >= u.friends.Count - 1)
                    {
                        int id = param.Split(',').Count() - 1;
                        if (id > 0)
                        {
                            var result = GetFriendsID(ref param, api);
                            for (int j = 0; j < id; j++)
                            {
                                foreach (var item in result[j].ToListOf<string>(n => n))
                                    u.friends[counter].friends.Add(new VkApiUser(item));
                                counter++;
                            }
                        }
                        Thread.Sleep(310);
                    }
                }
                foreach (var f in u.friends)
                {

                    WriteUserInFile(f, PathResFile);
                    /*txtHelper.WriteFileLines($@"data\dataUser\lvl4.txt", new List<string>() { "#" + f.me }, true);
                    list.Clear();
                    foreach (var ff in f.friends)
                        list.Add(ff.me);
                    txtHelper.WriteFileLines($@"data\dataUser\lvl4.txt", list, true);*/
                }


            }
        }
        private static async Task<List<VkApiUser>> LoadUser(string PathUserFile)
        {
            List<string> list = await txtHelper.ReadFileLinesAsync(PathUserFile);
            List<VkApiUser> users = new List<VkApiUser>();
            int i = -1;
            foreach (var item in list)
            {
                if (item.First() == '#')
                {
                    users.Add(new VkApiUser(item.Replace("#", "")));
                    i++;
                    continue;
                }
                else
                    users[i].friends.Add(new VkApiUser(item));
            }
            return users;
        }
        private static void WriteUserInFile(VkApiUser user, string Path)
        {
            List<string> list = new List<string>();
            txtHelper.WriteFileLines(Path, new List<string>() { "#" + user.me }, true);
            list.Clear();
            foreach (var f in user.friends)
                list.Add(f.me);
            txtHelper.WriteFileLines(Path, list, true);
        }
        private static async Task Main(string[] args)
        {
            /*
            string param = "";
            int i = -1;
            int counter = 0;
            List<string> list = new List<string>();*/
            VkApi api = await Authorization();
            /*
            if (!txtHelper.FileExists($@"data\dataUser\lvl1.txt") || !txtHelper.FileExists($@"data\dataUser\lvl2.txt"))
            {
                foreach (var vUser in api.Users.Get(csvHelper.ReadCsvFile(@"data\dataF.csv", "Ваш ID в VK")))
                {
                    users.Add(new VkApiUser(vUser.Id.ToString()));
                }
                
                while (param != "" || i < users.Count - 1)
                {
                    i++;
                    SenderMessage.sendMessage($"{i + 1}) Пользователь {users[i].me}");
                    param += $"{users[i].me},";
                    if (i % 12 == 0 && i != 0)
                    {

                        int id = param.Split(',').Count() - 1;
                        var result = GetFriendsID(ref param, api);
                        for (int j = 0; j < id; j++)
                        {
                            foreach (var u in result[j].ToListOf<string>(n => n))
                                users[counter].friends.Add(new VkApiUser(u));
                            counter++;
                        }
                        Thread.Sleep(310);
                    }
                    if (i >= users.Count - 1)
                    {
                        int id = param.Split(',').Count() - 1;
                        var result = GetFriendsID(ref param, api);
                        for (int j = 0; j < id; j++)
                        {
                            foreach (var u in result[j].ToListOf<string>(n => n))
                                users[counter].friends.Add(new VkApiUser(u));
                            counter++;
                        }
                        Thread.Sleep(310);
                    }

                }
                foreach (var u in users)
                    list.Add(u.me);
                txtHelper.WriteFileLines($@"data\dataUser\lvl1.txt", list);
                foreach (var u in users)
                {
                    txtHelper.WriteFileLines($@"data\dataUser\lvl2.txt", new List<string>() { "#" + u.me }, true);
                    list.Clear();
                    foreach (var f in u.friends)
                        list.Add(f.me);
                    txtHelper.WriteFileLines($@"data\dataUser\lvl2.txt", list, true);
                }
            }
            else
            {
                list = await txtHelper.ReadFileLinesAsync($@"data\dataUser\lvl1.txt");
                foreach (var user in list)
                    users.Add(new VkApiUser(user));
                list.Clear();
                list = await txtHelper.ReadFileLinesAsync($@"data\dataUser\lvl2.txt");
                foreach (var user in list)
                {
                    if(user.First() == '#')
                    {
                        i++;
                        continue;
                    }  
                    users[i].friends.Add(new VkApiUser(user));
                }
                Console.WriteLine();
            }

            users.Clear();
            list = await txtHelper.ReadFileLinesAsync($@"data\dataUser\lvl3.txt");
            i = -1;
            foreach (var item in list)
            {
                if (item.First() == '#')
                {
                    users.Add(new VkApiUser(item.Replace("#", "")));
                    i++;
                    continue;
                }
                else
                    users[i].friends.Add(new VkApiUser(item));
            }*/
           


            users = await LoadUser($@"data\dataUser\lvl3.txt");
            SenderMessage.sendMessage($"{users.Count}");
            getDataUserInFile(ref users, $@"data\dataUser\lvl5.txt", api);

            /*
            SenderMessage.sendMessage($"Читаем друзей");
            if (!txtHelper.FileExists($@"data\dataUser\lvl1.txt"))
            {
                foreach (var u in users)
                {
                    i = -1;
                    counter = 0;
                    param = "";

                    while (param != "" || i < u.friends.Count - 1)
                    {
                        i++;
                        SenderMessage.sendMessage($"{i + 1}) Пользователь {u.friends[i].me}");
                        param += $"{u.friends[i].me},";
                        if (i % 12 == 0 && i != 0)
                        {

                            int id = param.Split(',').Count() - 1;
                            var result = GetFriendsID(ref param, api);

                            for (int j = 0; j < id; j++)
                            {
                                foreach (var item in result[j].ToListOf<string>(n => n))
                                    u.friends[counter].friends.Add(new VkApiUser(item));
                                counter++;
                            }
                            Thread.Sleep(310);
                        }
                        if (i >= u.friends.Count - 1)
                        {
                            int id = param.Split(',').Count() - 1;
                            if (id > 0)
                            {
                                var result = GetFriendsID(ref param, api);
                                for (int j = 0; j < id; j++)
                                {
                                    foreach (var item in result[j].ToListOf<string>(n => n))
                                        u.friends[counter].friends.Add(new VkApiUser(item));
                                    counter++;
                                }
                            }
                            Thread.Sleep(310);
                        }
                    }
                    foreach (var f in u.friends)
                    {
                        txtHelper.WriteFileLines($@"data\dataUser\lvl4.txt", new List<string>() { "#" + f.me }, true);
                        list.Clear();
                        foreach (var ff in f.friends)
                            list.Add(ff.me);
                        txtHelper.WriteFileLines($@"data\dataUser\lvl4.txt", list, true);
                    }


                }
            }
            else
            {
                users.Clear();
                list = await txtHelper.ReadFileLinesAsync($@"data\dataUser\lvl3.txt");
                i = -1;
                foreach (var item in list)
                {
                    if(item.First() == '#')
                    {
                        users.Add(new VkApiUser(item.Replace("#", "")));
                        i++;
                        continue;
                    }
                    else 
                        users[i].friends.Add(new VkApiUser(item));
                }
            }
            */
            Console.ReadKey();
        }

        private static VkResponse GetFriendsID(ref string ids, VkApi api)
        {
            ids = ids.Remove(ids.Length - 1);
            parameters["userId"] = ids;
            var result = api.Call("execute.getFriendsById", new VkParameters(parameters));
            ids = "";
            return result;
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
        private static async Task<VkCollection<User>> GetVkFriends(VkApi api, long idVk, int MaxCountFriends = 20000)
        {
            VkCollection<User> friend = null;
            try
            {
                friend = await api.Friends.GetAsync(new FriendsGetParams { UserId = idVk, Count = MaxCountFriends });
            }
            catch(Exception e)
            {
                SenderMessage.sendErrorMessage($"Возникла ошибка {e.Message}");
            }
            return friend;
        }
        private static async void checkIdVk(VkApi api, string id)
        {
            sendMessageConsole($"Начало работы с id/псевдонимом {id}",ConsoleColor.Green);
            var uservk = api.Users.Get(new List<string>() { id });
            Thread.Sleep(510);
            if (uservk.Count() > 0)
            {
                long idvk = uservk[0].Id;
                string path = $@"data\dataUser\id{idvk}.txt";
                paths.Add(path);
                if (txtHelper.FileExists(path))
                {
                    sendMessageConsole($"Файл с пользователем id {id} уже существует", ConsoleColor.Green);
                    return;
                }
                sendMessageConsole($"Запись в файл: {path}");
                VkCollection<User> friend = await GetVkFriends(api, idvk);
                if (friend != null)
                {
                    sendMessageConsole($"У польователя {id}, числовой id = {idvk}, количество друзей {friend.Count}");
                    List<string> myFriend = friend.Select(n => $"{idvk}:{n.Id}").ToList();
                    txtHelper.WriteFileLines(path, myFriend);
                    int i = 1;
                    foreach (User user in friend)
                    {
                        sendMessageConsole($"\t {i++}) начало работы с другом id {user.Id}");
                        VkCollection<User> f = await GetVkFriends(api, user.Id);
                        if (f != null)
                        {
                            sendMessageConsole($"\t количестов друзей {f.Count}");
                            myFriend = f.Select(n => $"{user.Id}:{n.Id}").ToList();
                            txtHelper.WriteFileLines(path, myFriend, true);
                        }
                        Thread.Sleep(510);
                    }
                }
            }
            else
                sendMessageConsole($"Пользователь по id {id} не найден", ConsoleColor.Red);
            sendMessageConsole($"Закончина работа с пользователем {id}",ConsoleColor.Green);  
        }
        private static void joinFile()
        {
            txtHelper.JoinFile(paths);
            sendMessageConsole("Объединение файлов окончено");
        }
    }
}
