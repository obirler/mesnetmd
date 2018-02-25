using System;
using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Inertia : GraphicItem, IGraphic
    {
        public Inertia(PiecewisePoly inertiappoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.Inertia;
            _beam = beam;
            _inertiappoly = inertiappoly;
            _length = _beam.Length;
            _max = _inertiappoly.Max;

            Draw(c);
        }

        private double _max;

        private double _length;

        private Beam _beam;

        public PiecewisePoly _inertiappoly;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        public PiecewisePoly InertiaPpoly
        {
            get { return _inertiappoly; }
            set { _inertiappoly = value; }
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
