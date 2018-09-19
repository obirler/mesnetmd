using System.Windows;
using System.Windows.Controls;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class SomItem : UiItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SomItem"/> class.
        /// Beams and all types of supports are derived from this class
        /// </summary>
        protected SomItem()
        {
            ItemType = Global.ItemType.SomItem;
            Id = count++;
        }

        public string Name;

        public int Id;

        private static int count = 0;

        protected double _angle;

        public virtual void Move(Vector delta)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + delta.X);
            Canvas.SetTop(this, Canvas.GetTop(this) + delta.Y);
        }

        public double Angle
        {
            get { return _angle; }
            set { value = _angle; }
        }

    }
}
