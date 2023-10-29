using SecondTaskAI.Model;
using SecondTaskAI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
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
            VkApi api = await Authorization();

            users = await LoadUser($@"data\dataUser\lvl3.txt");
            SenderMessage.sendMessage($"{users.Count}");
            getDataUserInFile(ref users, $@"data\dataUser\lvl5.txt", api);

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
        private static void joinFile()
        {
            txtHelper.JoinFile(paths);
            sendMessageConsole("Объединение файлов окончено");
        }
    }
}
