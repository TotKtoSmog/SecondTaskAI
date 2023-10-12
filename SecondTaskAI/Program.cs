using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace SecondTaskAI
{
    class Program
    {

        private static async Task Main(string[] args)
        {
            List<string> ListUser = csvHelper.ReadCsvFile(@"data\dataF.csv", "Ваш ID в VK");
            ListUser = ListUser.Select(item => CorrecterVkId(item)).ToList();
            VkApi api = await Authorization();
            Console.ReadKey();
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
