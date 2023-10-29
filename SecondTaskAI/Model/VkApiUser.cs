using System.Collections.Generic;
using VkNet.Model;

namespace SecondTaskAI.Model
{
    internal class VkApiUser
    {
        internal string me;
        internal List<VkApiUser> friends;
        internal VkApiUser(string id)
        {
            me = id;
            friends = new List<VkApiUser>();
        }
    }
}
