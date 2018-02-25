using System;
using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Force : GraphicItem, IGraphic
    {
        public Force(PiecewisePoly forceppoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.Force;
            _beam = beam;
            _forceppoly = forceppoly;
            _length = _beam.Length;

            Draw(c);
        }

        private Beam _beam;

        private double _length;

        public PiecewisePoly _forceppoly;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        public PiecewisePoly ForcePpoly
        {
            get { return _forceppoly; }
            set { _forceppoly = value; }
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
