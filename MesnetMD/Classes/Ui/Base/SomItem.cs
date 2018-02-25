namespace MesnetMD.Classes.Ui.Base
{
    public class SomItem : UiItem
    {
        public SomItem()
        {
            ItemType = Global.ItemType.SomItem;
            Id = count++;
        }

        public string Name;

        public int Id;

        private static int count = 0;

        private double _angle;

        public double Angle
        {
            get { return _angle; }
            set { value = _angle; }
        }
    }
}
