using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesnetMD.Classes.IO.Manifest
{
    public abstract class ManifestBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Global.ManifestType Type { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double Angle { get; set; }
    }
}
