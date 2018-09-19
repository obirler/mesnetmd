using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesnetMD.Classes.IO.Manifest
{
    public class SlidingSupportManifest : ManifestBase
    {
        public int SupportId { get; set; }

        public List<Member> Members { get; set; }
    }
}
