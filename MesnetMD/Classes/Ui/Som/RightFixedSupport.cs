using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using static MesnetMD.Classes.Global;

namespace MesnetMD.Classes.Ui.Som
{
    public class RightFixedSupport : RealFixedSupportItem
    {
        public RightFixedSupport() : base(ObjectType.RightFixedSupport)
        {
            Name = "Right Fixed Support " + SupportId;
        }

        public RightFixedSupport(Canvas canvas) : base(ObjectType.RightFixedSupport)
        {
            canvas.Children.Add(this);
            AddObject(this);
            Name = "Right Fixed Support " + SupportId;
        }

        protected override void createpolygons()
        {
            _p1 = new Polygon();
            _p1.Points.Add(new Point(0, 27));
            _p1.Points.Add(new Point(0, 0));
            _p1.Points.Add(new Point(2, 0));
            _p1.Points.Add(new Point(2, 27));
            _p1.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p1);

            _p2 = new Polygon();
            _p2.Points.Add(new Point(1, 27));
            _p2.Points.Add(new Point(7, 21));
            _p2.Points.Add(new Point(6, 20));
            _p2.Points.Add(new Point(1, 24.8));
            _p2.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p2);

            _p3 = new Polygon();
            _p3.Points.Add(new Point(1, 22));
            _p3.Points.Add(new Point(7, 16));
            _p3.Points.Add(new Point(6, 15));
            _p3.Points.Add(new Point(1, 19.8));
            _p3.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p3);

            _p4 = new Polygon();
            _p4.Points.Add(new Point(1, 17));
            _p4.Points.Add(new Point(7, 11));
            _p4.Points.Add(new Point(6, 10));
            _p4.Points.Add(new Point(1, 14.8));
            _p4.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p4);

            _p5 = new Polygon();
            _p5.Points.Add(new Point(1, 12));
            _p5.Points.Add(new Point(7, 6));
            _p5.Points.Add(new Point(6, 5));
            _p5.Points.Add(new Point(1, 9.8));
            _p5.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p5);

            _p6 = new Polygon();
            _p6.Points.Add(new Point(1, 7));
            _p6.Points.Add(new Point(7, 1));
            _p6.Points.Add(new Point(6, 0));
            _p6.Points.Add(new Point(1, 4.8));
            _p6.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_p6);
        }

        protected override void createcore()
        {
            _core = new Polygon();
            _core.Points.Add(new Point(1, 27));
            _core.Points.Add(new Point(7, 27));
            _core.Points.Add(new Point(7, 0));
            _core.Points.Add(new Point(1, 0));
            _core.Points.Add(new Point(1, 27));
            _core.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_core);
        }

        public override void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public override void UpdatePosition(Beam beam)
        {
            Canvas.SetLeft(this, beam.RightPoint.X);

            Canvas.SetTop(this, beam.RightPoint.Y - Height / 2);

            SetAngle(beam.Angle);
        }

        public override void AddBeam(Beam beam)
        {
            Canvas.SetLeft(this, beam.RightPoint.X);

            Canvas.SetTop(this, beam.RightPoint.Y - Height/2);

            Member = new Member(beam, Direction.Right);

            beam.RightSide = this;

            SetAngle(beam.Angle);
        }
    }
}
