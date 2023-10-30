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
                    WriteUserInFile(f, PathResFile);
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
        private static void WriteEdgesInFile(List<VkApiUser> users)
        {
            char delimiter = csvHelper.delimiter;
            List<string> Edges = new List<string>();
            foreach (VkApiUser user in users)
            {
                Edges.Clear();
                foreach (VkApiUser friend in user.friends)
                    Edges.Add($"{user.me}{delimiter}{friend.me}");
                txtHelper.WriteFileLines(@"data\dataUser\Edges.txt", Edges, true);
            }
        }
        private static async Task RemovUnnecessaryEdges(string pathF, string pathFOF)
        {
            SenderMessage.sendMessage($"Чиатем файл {pathF}");
            List<VkApiUser> friends = await LoadUser(pathF);
            SenderMessage.sendMessage($"Фуууух, законили");
            SenderMessage.sendMessage($"Чиатем файл {pathFOF}");
            List<VkApiUser> friendsOfFriends = await LoadUser(pathFOF);
            SenderMessage.sendMessage($"Фуууух, законили");
            List<string> Edges = new List<string>();
            foreach (VkApiUser me in friends)
            {
                Edges.Clear();
                foreach(VkApiUser friend in me.friends)
                {
                    
                    foreach(VkApiUser FoF in friendsOfFriends)
                    {
                        string[] t = FoF.friends.Select(n => n.me).ToArray();
                        if (t.Contains(friend.me))
                        {
                            if (!Edges.Contains($"{friend.me},{FoF.me}") && !Edges.Contains($"{FoF.me},{friend.me}"))
                            {
                                Edges.Add($"{friend.me},{FoF.me}");
                                break;
                            }
                        }
                    }
                    if(Edges.Count > 0 && ( !Edges.Contains($"{me.me},{friend.me}") && !Edges.Contains($"{friend.me},{me.me}")))
                        Edges.Add($"{me.me},{friend.me}");
                }
                txtHelper.WriteFileLines(@"data\dataUser\Edges.txt", Edges, true);
            }
             
            SenderMessage.sendMessage("Все!!!");
        }
        
        private static List<string> GetUSerFromeCSV(string path)
        {
            List<string> ListUser = csvHelper.ReadCsvFile(path, "Ваш ID в VK");
            ListUser = ListUser.Select(item => CorrecterVkId(item)).ToList();
            return ListUser;
        }
        private static async Task Main(string[] args)
        {
            VkApi api = await Authorization();
            List<long> idVkUser = api.Users.Get(GetUSerFromeCSV(@"data\dataF.csv")).Select(n => n.Id).ToList();
            foreach (long nameUserVk in idVkUser)
                users.Add(new VkApiUser(nameUserVk.ToString()));


            await RemovUnnecessaryEdges($@"data\dataUser\lvl3.txt", $@"data\dataUser\lvl4.txt");

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

    }
}
