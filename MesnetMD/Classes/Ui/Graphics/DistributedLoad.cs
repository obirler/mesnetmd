using System;
using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class DistributedLoad : GraphicItem, IGraphic
    {
        public DistributedLoad(PiecewisePoly ppoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.DistibutedLoad;
            _beam = beam;
            _loadppoly = ppoly;
            _length = beam.Length;

            Draw(c);
        }

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double _length;

        private PiecewisePoly _loadppoly;

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

        public PiecewisePoly LoadPpoly
        {
            get { return _loadppoly; }
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
