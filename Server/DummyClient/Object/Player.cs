using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string? Name { get; set; }

        public ServerSession? Session { get; set; } = null;
    }
}
