using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrepperV2
{
    class Filee
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Name { get; set; }
        public byte[] Buffers { get; set; }

        public string Md5 { get; set; }

        public Filee()
        {
            Start = -1;
            End = -1;
        }

    }
}
