using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.IO.Manifest;

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
            Id = IdCount++;
        }

        protected SomItem(ManifestBase manifest)
        {
            Id = manifest.Id;
            Name = manifest.Name;
        }

        public string Name;

        public static int IdCount = 0;

        protected double _angle;

        public int Id { get; }

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

        public virtual void ResetSolution()
        {
        }

        public virtual void Select()
        {
        }

        public virtual void UnSelect()
        {
        }
    }
}
