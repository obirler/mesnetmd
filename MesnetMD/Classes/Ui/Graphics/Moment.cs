using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Moment : GraphicItem, IGraphic
    {
        public Moment(PiecewisePoly momentppoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.Moment;
            _beam = beam;
            _momentppoly = momentppoly;
            Draw(c);
        }

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private PiecewisePoly _momentppoly;   

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        private double coeff;

        private SolidColorBrush color = new SolidColorBrush(Colors.Red);

        public PiecewisePoly MomentPpoly
        {
            get { return _momentppoly; }
            set { _momentppoly = value; }
        }

        public void Draw(int c)
        {
            if (starttext != null)
            {
                _beam.Children.Remove(starttext);
            }
            if (endtext != null)
            {
                _beam.Children.Remove(endtext);
            }
            if (mintext != null)
            {
                _beam.Children.Remove(mintext);
            }
            if (maxtext != null)
            {
                _beam.Children.Remove(maxtext);
            }
            Children.Clear();

            coeff = c / Global.MaxMoment;
            double calculated = 0;
            double value = 0;

            var leftpoints = new PointCollection();
            leftpoints.Add(new Point(0, -coeff * _momentppoly.Calculate(0)));
            leftpoints.Add(new Point(0, 0));
            var leftspline = new CardinalSplineShape(leftpoints);
            leftspline.Stroke = color;
            leftspline.StrokeThickness = 1;
            Children.Add(leftspline);

            Point lastpoint = new Point(0, 0);

            foreach (Poly poly in _momentppoly)
            {
                var points = new PointCollection();
                points.Clear();

                if (!poly.IsLinear())
                {
                    for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                    {
                        calculated = coeff * poly.Calculate(i / 100);
                        value = -calculated;
                        points.Add(new Point(i, value));
                    }
                }
                else
                {
                    calculated = coeff * poly.Calculate(poly.StartPoint);
                    value = -calculated;
                    points.Add(new Point(poly.StartPoint * 100, value));

                    calculated = coeff * poly.Calculate(poly.EndPoint);
                    value = -calculated;
                    points.Add(new Point(poly.EndPoint * 100, value));
                }

                lastpoint = points.Last();
                _spline = new CardinalSplineShape(points);
                _spline.Stroke = color;
                _spline.StrokeThickness = 1;
                _spline.MouseMove += _mw.momentmousemove;
                _spline.MouseEnter += _mw.mouseenter;
                _spline.MouseLeave += _mw.mouseleave;
                Children.Add(_spline);
            }

            var rightpoints = new PointCollection();
            var point1 = new Point(100 * _beam.Length, -coeff * _momentppoly.Calculate(_beam.Length));
            rightpoints.Add(point1);
            var point2 = new Point(100 * _beam.Length, 0);
            rightpoints.Add(point2);
            var rightspline = new CardinalSplineShape(rightpoints);
            rightspline.Stroke = color;
            rightspline.StrokeThickness = 1;
            Children.Add(rightspline);

            double max = _momentppoly.Max;
            double maxlocation = _momentppoly.MaxLocation;
            double min = _momentppoly.Min;
            double minlocation = _momentppoly.MinLocation;

            starttext = createtextblock();
            _beam.Children.Add(starttext);
            starttext.Text = System.Math.Round(_momentppoly.Calculate(0), 1) + " kNm";
            starttext.Foreground = color;
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            RotateAround(starttext, _beam.Angle);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = -coeff * _momentppoly.Calculate(0);
            if (calculated > 0)
            {
                Canvas.SetTop(starttext, calculated);
            }
            else
            {
                Canvas.SetTop(starttext, calculated - starttext.Height);
            }

            if (minlocation != 0 && minlocation != _beam.Length)
            {
                mintext = createtextblock();
                mintext.Text = System.Math.Round(min, 1) + " kNm";
                mintext.Foreground = color;
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext, _beam.Angle);

                _beam.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = -coeff * min;

                if (calculated > 0)
                {
                    Canvas.SetTop(mintext, calculated);
                }
                else
                {
                    Canvas.SetTop(mintext, calculated - mintext.Height);
                }

                var minpoints = new PointCollection();
                minpoints.Add(new Point(minlocation * 100, 0));
                minpoints.Add(new Point(minlocation * 100, calculated));
                var minspline = new CardinalSplineShape(minpoints);
                minspline.Stroke = color;
                Children.Add(minspline);
            }

            if (maxlocation != 0 && maxlocation != _beam.Length)
            {
                maxtext = createtextblock();
                maxtext.Text = System.Math.Round(max, 1) + " kNm";
                maxtext.Foreground = color;
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext, _beam.Angle);

                _beam.Children.Add(maxtext);

                Canvas.SetLeft(maxtext, maxlocation * 100 - maxtext.Width / 2);

                calculated = -coeff * max;

                if (calculated > 0)
                {
                    Canvas.SetTop(maxtext, calculated);
                }
                else
                {
                    Canvas.SetTop(maxtext, calculated - maxtext.Height);
                }

                var maxpoints = new PointCollection();
                maxpoints.Add(new Point(maxlocation * 100, 0));
                maxpoints.Add(new Point(maxlocation * 100, calculated));
                var maxspline = new CardinalSplineShape(maxpoints);
                maxspline.Stroke = color;
                Children.Add(maxspline);
            }

            endtext = createtextblock();
            _beam.Children.Add(endtext);
            endtext.Text = System.Math.Round(_momentppoly.Calculate(_beam.Length), 1) + " kNm";
            endtext.Foreground = color;
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext, _beam.Angle);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = -coeff * _momentppoly.Calculate(_beam.Length);
            if (calculated > 0)
            {
                Canvas.SetTop(endtext, calculated);
            }
            else
            {
                Canvas.SetTop(endtext, calculated - endtext.Height);
            }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            starttext.Visibility = Visibility.Visible;

            if (mintext != null)
            {
                mintext.Visibility = Visibility.Visible;
            }

            if (maxtext != null)
            {
                maxtext.Visibility = Visibility.Visible;
            }

            endtext.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;

            starttext.Visibility = Visibility.Collapsed;

            if (mintext != null)
            {
                mintext.Visibility = Visibility.Collapsed;
            }

            if (maxtext != null)
            {
                maxtext.Visibility = Visibility.Collapsed;
            }

            endtext.Visibility = Visibility.Collapsed;
        }

        public void RemoveLabels()
        {
            _beam.Children.Remove(starttext);
            _beam.Children.Remove(endtext);
            if (maxtext != null)
            {
                _beam.Children.Remove(maxtext);
            }
            if (mintext != null)
            {
                _beam.Children.Remove(mintext);
            }
        }
    }
}
