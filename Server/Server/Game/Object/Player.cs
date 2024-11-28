using System.Text;

namespace Server.Game
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string? Name { get; set; }

        public ClientSession? Session { get; set; } = null;
        public Channel? ChatChannel { get; set; } = null;
    }
}
