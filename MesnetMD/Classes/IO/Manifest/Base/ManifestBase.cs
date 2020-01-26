namespace MesnetMD.Classes.IO.Manifest
{
    public abstract class ManifestBase
    {
        public ManifestBase()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double Angle { get; set; }
    }
}
