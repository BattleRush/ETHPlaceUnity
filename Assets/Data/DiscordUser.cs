using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Data
{
    public class DiscordUser
    {
        public short UserId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsBot { get; set; }
        public byte[] Image { get; set; }
    }
}
