using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class RealFixedSupportItem : SupportItem, IRealSupportItem
    {
        protected RealFixedSupportItem(Global.ObjectType type) : base(type)
        {
            InitializeComponent();
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
            bindevents();
        }

        protected virtual void createpolygons()
        {            
        }

        protected virtual void createcore()
        {           
        }

        private void bindevents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.FixedSupportMouseDown;
            _core.MouseUp += mw.FixedSupportMouseUp;
        }

        protected Polygon _p1;

        protected Polygon _p2;

        protected Polygon _p3;

        protected Polygon _p4;

        protected Polygon _p5;

        protected Polygon _p6;

        protected Polygon _core;

        public Member Member;

        protected RotateTransform rotateTransform;

        public override void Select()
        {
            _p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p3.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p4.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p5.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _p6.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public override void UnSelect()
        {
            _p1.Fill = new SolidColorBrush(Colors.Black);
            _p2.Fill = new SolidColorBrush(Colors.Black);
            _p3.Fill = new SolidColorBrush(Colors.Black);
            _p4.Fill = new SolidColorBrush(Colors.Black);
            _p5.Fill = new SolidColorBrush(Colors.Black);
            _p6.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        public override void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public virtual void UpdatePosition(Beam beam)
        {
        }

        public void SetPosition(double x, double y)
        {
            var left = x - Width / 2;
            var right = y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MesnetMDDebug.WriteInformation("Position has been set : " + left + " : " + right);
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

        public virtual void AddBeam(Beam beam)
        {

        }
    }
}
