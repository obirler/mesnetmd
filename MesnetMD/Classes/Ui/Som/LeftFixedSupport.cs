using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.IO.Manifest;
using MesnetMD.Classes.Ui.Base;
using static MesnetMD.Classes.Global;
using Member = MesnetMD.Classes.Tools.Member;

namespace MesnetMD.Classes.Ui.Som
{
    public class LeftFixedSupport : RealFixedSupportItem
    {
        public LeftFixedSupport()
        {           
            Name = "Left Fixed Support " + SupportId;
        }

        public LeftFixedSupport(Canvas canvas)
        {
            canvas.Children.Add(this);
            AddObject(this);
            Name = "Left Fixed Support " + SupportId;
        }

        public LeftFixedSupport(LeftFixedSupportManifest manifest) : base(manifest)
        {
        }

        protected override void createpolygons()
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

        protected override void createcore()
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

        public override void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public override void UpdatePosition(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - Width);

            Canvas.SetTop(this, beam.LeftPoint.Y - Height/2);

            SetAngle(beam.Angle);
        }
       
        public override void AddBeam(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - Width);

            Canvas.SetTop(this, beam.LeftPoint.Y - Height/2);

            Member = new Member(beam, Direction.Left);

            beam.LeftSide = this;

            SetAngle(beam.Angle);
        }
    }
}
