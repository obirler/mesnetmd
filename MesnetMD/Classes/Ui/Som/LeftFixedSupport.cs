using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using static MesnetMD.Classes.Global;

namespace MesnetMD.Classes.Ui.Som
{
    public class LeftFixedSupport : SupportItem, ISomItem, ISupportItem, IFixedSupportItem
    {
        public LeftFixedSupport()
        {
            InitializeComponent();
            Name = "Left Fixed Support " + SupportId;
        }

        public LeftFixedSupport(Canvas canvas)
        {
            InitializeComponent();
            canvas.Children.Add(this);
            AddObject(this);
            Name = "Left Fixed Support " + SupportId;
            BindEvents();
        }

        private void InitializeComponent()
        {
            Width = 7;
            Height = 27;
            rotateTransform = new RotateTransform(7, 13, 0);
            LayoutTransform = rotateTransform;
            createpolygons();
            createcore();
        }

        private void createpolygons()
        {
            _p1 = new Polygon();
            _p1.Points.Add(new Point(7, 0));
            _p1.Points.Add(new Point(7, 27));
            _p1.Points.Add(new Point(5, 27));
            _p1.Points.Add(new Point(5, 0));
            _p1.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p1);

            _p2 = new Polygon();
            _p2.Points.Add(new Point(6, 0));
            _p2.Points.Add(new Point(0, 6));
            _p2.Points.Add(new Point(1, 7));
            _p2.Points.Add(new Point(6, 2.2));
            _p2.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p2);

            _p3 = new Polygon();
            _p3.Points.Add(new Point(6, 5));
            _p3.Points.Add(new Point(0, 11));
            _p3.Points.Add(new Point(1, 12));
            _p3.Points.Add(new Point(6, 7.2));
            _p3.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p3);

            _p4 = new Polygon();
            _p4.Points.Add(new Point(6, 10));
            _p4.Points.Add(new Point(0, 16));
            _p4.Points.Add(new Point(1, 17));
            _p4.Points.Add(new Point(6, 12.2));
            _p4.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p4);

            _p5 = new Polygon();
            _p5.Points.Add(new Point(6, 15));
            _p5.Points.Add(new Point(0, 21));
            _p5.Points.Add(new Point(1, 22));
            _p5.Points.Add(new Point(6, 17.2));
            _p5.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p5);

            _p6 = new Polygon();
            _p6.Points.Add(new Point(6, 20));
            _p6.Points.Add(new Point(0, 26));
            _p6.Points.Add(new Point(1, 27));
            _p6.Points.Add(new Point(6, 22.2));
            _p6.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p6);
        }

        private void createcore()
        {
            _core = new Polygon();
            _core.Points.Add(new Point(0, 0));
            _core.Points.Add(new Point(6, 0));
            _core.Points.Add(new Point(6, 27));
            _core.Points.Add(new Point(0, 27));
            _core.Points.Add(new Point(0, 0));
            _core.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_core);
        }

        private Polygon _p1;

        private Polygon _p2;

        private Polygon _p3;

        private Polygon _p4;

        private Polygon _p5;

        private Polygon _p6;

        private Polygon _core;

        public Member Member;

        private RotateTransform rotateTransform;

        public void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BasicSupportMouseDown;
            _core.MouseUp += mw.BasicSupportMouseUp;
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void UnSelect()
        {
            throw new NotImplementedException();
        }

        public void ResetSolution()
        {
            throw new NotImplementedException();
        }

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            throw new NotImplementedException();
        }

        public void UpdatePosition(Beam beam)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(Point point)
        {
            throw new NotImplementedException();
        }

        public void SetAngle(double angle)
        {
            throw new NotImplementedException();
        }

        public void AddBeam(Beam beam)
        {
            throw new NotImplementedException();
        }
    }
}
