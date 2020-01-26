using System.Windows.Controls;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class UiItem : Canvas
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UiItem"/> class.
        /// UiItem is a base class for ui elements that can be in the canvas.
        /// Beams and all supports and graphics items are derived from this class.
        /// </summary>
        protected UiItem()
        {
        }

        private double _leftpos;

        private double _toppos;

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
