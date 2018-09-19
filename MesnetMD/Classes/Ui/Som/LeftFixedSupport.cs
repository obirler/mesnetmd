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
        public LeftFixedSupport() : base(ObjectType.LeftFixedSupport)
        {
            InitializeComponent();
            Name = "Left Fixed Support " + SupportId;
        }

        public LeftFixedSupport(Canvas canvas) : base(ObjectType.LeftFixedSupport)
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
            rotateTransform = new RotateTransform();
            rotateTransform.CenterX = Width;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = 0;
            RenderTransform = rotateTransform;
            createpolygons();
            createcore();
        }

        private void createpolygons()
        {
            _p1 = new Polygon();
            _p1.Points.Add(new Point(7, 27));
            _p1.Points.Add(new Point(7, 0));
            _p1.Points.Add(new Point(5, 0));
            _p1.Points.Add(new Point(5, 27));
            _p1.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p1);

            _p2 = new Polygon();
            _p2.Points.Add(new Point(6, 27));
            _p2.Points.Add(new Point(0, 21));
            _p2.Points.Add(new Point(1, 20));
            _p2.Points.Add(new Point(6, 24.8));
            _p2.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p2);

            _p3 = new Polygon();
            _p3.Points.Add(new Point(6, 22));
            _p3.Points.Add(new Point(0, 16));
            _p3.Points.Add(new Point(1, 15));
            _p3.Points.Add(new Point(6, 19.8));
            _p3.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p3);

            _p4 = new Polygon();
            _p4.Points.Add(new Point(6, 17));
            _p4.Points.Add(new Point(0, 11));
            _p4.Points.Add(new Point(1, 10));
            _p4.Points.Add(new Point(6, 14.8));
            _p4.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p4);

            _p5 = new Polygon();
            _p5.Points.Add(new Point(6, 12));
            _p5.Points.Add(new Point(0, 6));
            _p5.Points.Add(new Point(1, 5));
            _p5.Points.Add(new Point(6, 9.8));
            _p5.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p5);

            _p6 = new Polygon();
            _p6.Points.Add(new Point(6, 7));
            _p6.Points.Add(new Point(0, 1));
            _p6.Points.Add(new Point(1, 0));
            _p6.Points.Add(new Point(6, 4.8));
            _p6.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p6);
        }

        private void createcore()
        {
            _core = new Polygon();
            _core.Points.Add(new Point(0, 27));
            _core.Points.Add(new Point(6, 27));
            _core.Points.Add(new Point(6, 0));
            _core.Points.Add(new Point(0, 0));
            _core.Points.Add(new Point(0, 27));
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
            _p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p3.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p4.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p5.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p6.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            _p1.Fill = new SolidColorBrush(Colors.Black);
            _p2.Fill = new SolidColorBrush(Colors.Black);
            _p3.Fill = new SolidColorBrush(Colors.Black);
            _p4.Fill = new SolidColorBrush(Colors.Black);
            _p5.Fill = new SolidColorBrush(Colors.Black);
            _p6.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        public void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);

            Canvas.SetLeft(this, leftpos);

            Canvas.SetTop(this, toppos);
        }

        public void UpdatePosition(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - Width);

            Canvas.SetTop(this, beam.LeftPoint.Y - Height/2);

            SetAngle(beam.Angle);
        }

        public void SetPosition(double x, double y)
        {
            var left = x - Width / 2;
            var right = y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MyDebug.WriteInformation("Position has been set : " + left + " : " + right);
        }

        public void SetPosition(Point point)
        {
            SetPosition(point.X, point.Y);
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
            _angle = angle;
        }

        public void AddBeam(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - Width);

            Canvas.SetTop(this, beam.LeftPoint.Y - Height/2);

            Member = new Member(beam, Direction.Left);

            beam.LeftSide = this;

            SetAngle(beam.Angle);
        }
    }
}
