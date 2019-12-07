using System.Collections.Generic;

namespace MesnetMD.Classes.IO.Manifest
{
    public class BasicSupportManifest : ManifestBase
    {
        public int SupportId { get; set; }

        public List<Member> Members { get; set; }
    }
}
