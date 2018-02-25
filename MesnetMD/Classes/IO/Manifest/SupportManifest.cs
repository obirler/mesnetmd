using System.Collections.Generic;

namespace MesnetMD.Classes.IO.Manifest
{
    public class SupportManifest
    {
        public string Type { get; set; }

        public int Id { get; set; }

        public int SupportId { get; set; }

        public string Name { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double Angle { get; set; }

        public List<Member> Members { get; set; }
    }
}
