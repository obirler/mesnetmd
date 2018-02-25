using System.Windows.Controls;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class UiItem : Canvas
    {
        public UiItem()
        {           
        }

        private double _leftpos;

        private double _toppos;

        public Global.ObjectType Type;

        public Global.ItemType ItemType;

        public double LeftPos
        {
            get
            {
                _leftpos = Canvas.GetLeft(this);
                return _leftpos;
            }
            set
            {
                _leftpos = value;
                Canvas.SetLeft(this, _leftpos);
            }
        }

        public double TopPos
        {
            get
            {
                _toppos = Canvas.GetTop(this);
                return _toppos;
            }
            set
            {
                _toppos = value;
                Canvas.SetTop(this, _toppos);

            }
        }
    }
}
