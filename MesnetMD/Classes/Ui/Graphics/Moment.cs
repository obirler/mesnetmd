using System;
using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Moment : GraphicItem, IGraphic
    {
        public Moment(PiecewisePoly momentppoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.Moment;
            _beam = beam;
            _momentppoly = momentppoly;
            Draw(c);
        }

        private Beam _beam;

        private double _length;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private PiecewisePoly _momentppoly;

        public PiecewisePoly MomentPpoly
        {
            get { return _momentppoly; }
            set { _momentppoly = value; }
        }

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        private double coeff;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
            }
        }
        public void Draw(int c)
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void RemoveLabels()
        {
            throw new NotImplementedException();
        }
    }
}
