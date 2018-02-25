using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Stress : GraphicItem, IGraphic
    {
        public Stress(KeyValueCollection stresslist, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.Stress;
            _beam = beam;
            _stress = stresslist;
            _labellist = new List<TextBlock>();

            Draw(c);
        }

        private KeyValueCollection _stress;

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double coeff;

        private SolidColorBrush color = new SolidColorBrush(Colors.Green);

        private SolidColorBrush exceedcolor = new SolidColorBrush(Colors.Red);

        private List<TextBlock> _labellist;

        public double Calculate(double x)
        {
            return _stress.Calculate(x);
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
