using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        protected void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BasicSupportMouseDown;
            _core.MouseUp += mw.BasicSupportMouseUp;
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

        protected RotateTransform rotateTransform;

        protected Polygon _triangle;

        protected Polygon _core;

        public void Select()
        {
            _triangle.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            _triangle.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        public void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public virtual void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);

            Canvas.SetLeft(this, leftpos);

            Canvas.SetTop(this, toppos);
        }

        /// <summary>
        /// Updates the position of the support according to the beam that is bounded.
        /// </summary>
        /// <param name="beam">The reference beam.</param>
        public void UpdatePosition(Beam beam)
        {
            foreach (Member member in Members)
            {
                if (member.Beam == beam)
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
            rotateTransform.Angle = angle;
            _angle = angle;
        }
    }
}
