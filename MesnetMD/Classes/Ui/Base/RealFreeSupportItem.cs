using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class RealFreeSupportItem : FreeSupportItem, IRealSupportItem
    {
        protected RealFreeSupportItem(Global.ObjectType type) : base(type)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Width = 26;
            Height = 16;
            rotateTransform = new RotateTransform();
            rotateTransform.CenterX = Width / 2;
            rotateTransform.CenterY = Height;
            rotateTransform.Angle = 0;
            RenderTransform = rotateTransform;
            createtriangle();
            createellipses();
            createcore();
            createcircle();
            createcirclecore();
            bindevents();
        }

        protected virtual void createtriangle()
        {

        }

        protected virtual void createellipses()
        {

        }

        protected virtual void createcore()
        {

        }

        private void createcircle()
        {
            _circle = new Ellipse();
            _circle.Width = 14;
            _circle.Height = 14;
            _circle.StrokeThickness = 3;
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            _circle.Margin = new Thickness(6, 9, 6, 0);
            Children.Add(_circle);
            CircleHide();
        }

        private void createcirclecore()
        {
            _circlecore = new Ellipse();
            _circlecore.Width = 14;
            _circlecore.Height = 14;
            _circlecore.Fill = new SolidColorBrush(Colors.Transparent);
            _circlecore.Margin = new Thickness(6, 9, 6, 0);
            Children.Add(_circlecore);
        }

        private void bindevents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.FreeSupportMouseDown;
            _core.MouseUp += mw.FreeSupportMouseUp;
            _circlecore.MouseDown += mw.CircleMouseDown;
        }

        protected RotateTransform rotateTransform;

        protected Polygon _triangle;

        protected Polygon _core;

        protected Ellipse _circlecore;

        public override void Select()
        {
            _triangle.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public override void UnSelect()
        {
            _triangle.Fill = new SolidColorBrush(Colors.Black);
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            _selected = false;
        }   

        /// <summary>
        /// Updates the position of the support according to the beam that is bounded.
        /// </summary>
        /// <param name="beam">The reference beam.</param>
        public void UpdatePosition(Beam beam)
        {
            foreach (Member member in Members)
            {
                if (Equals(member.Beam, beam))
                {
                    switch (member.Direction)
                    {
                        case Global.Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.LeftPoint.Y - Height);

                            beam.LeftSide = this;

                            break;

                        case Global.Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.RightPoint.Y - Height);

                            beam.RightSide = this;

                            break;
                    }
                    SetAngle(beam.Angle);
                }
            }
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetPosition(double x, double y)
        {
            var left = x - Width / 2;
            var right = y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MesnetMDDebug.WriteInformation("Position has been set : " + left + " : " + right);
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetPosition(Point point)
        {
            var left = point.X - Width / 2;
            var right = point.Y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MesnetMDDebug.WriteWarning("Position has been set : " + left + " : " + right);
        }

        public void SetAngle(double angle)
        {
            //rotateTransform.Angle = angle;
            //angle = angle;
        }
    }
}
