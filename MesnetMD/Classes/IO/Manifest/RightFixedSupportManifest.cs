namespace MesnetMD.Classes.IO.Manifest
{
    public class RightFixedSupportManifest
    {
        public int Id { get; set; }

        public int SupportId { get; set; }

        public string Name { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double Angle { get; set; }

        public Member Member { get; set; }
    }
}
