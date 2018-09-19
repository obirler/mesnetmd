using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class DistributedLoad : GraphicItem, IGraphic
    {
        public DistributedLoad(PiecewisePoly ppoly, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.DistibutedLoad;
            _beam = beam;
            _loadppoly = ppoly;
            _length = beam.Length;

            Draw(c);            
        }

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double _length;

        private PiecewisePoly _loadppoly;

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        private double coeff;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
            }
        }

        public PiecewisePoly LoadPpoly
        {
            get { return _loadppoly; }
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

            coeff = c / Global.MaxDistLoad;

            double calculated = 0;

            double value = 0;

            foreach (Poly poly in _loadppoly)
            {
                var points = new PointCollection();
                points.Clear();

                double diff = (poly.EndPoint - poly.StartPoint) * 100;

                int arrownumber = 0;

                arrownumber = Convert.ToInt32(System.Math.Round(diff / 10, 0));

                //draw spline
                if (!poly.IsLinear())
                {
                    for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                    {
                        calculated = coeff * poly.Calculate(i / 100);
                        points.Add(new Point(i, calculated));
                    }
                }
                else
                {
                    //if the poly is linear, we only need two points to represent it
                    calculated = coeff * poly.Calculate(poly.StartPoint);
                    points.Add(new Point(poly.StartPoint * 100, calculated));

                    calculated = coeff * poly.Calculate(poly.EndPoint);
                    points.Add(new Point(poly.EndPoint * 100, calculated));
                }

                //draw arrows
                for (int i = 0; i <= arrownumber; i++)
                {
                    double tobecalc = poly.StartPoint * 100 + diff * i / arrownumber;
                    calculated = coeff * poly.Calculate(tobecalc / 100);
                    if (calculated >= 5)
                    {
                        drawarrow(tobecalc, calculated);
                    }
                    else if (calculated <= -5)
                    {
                        drawnegativearrow(tobecalc, calculated);
                    }
                }

                _spline = new CardinalSplineShape(points);
                _spline.Stroke = new SolidColorBrush(Colors.Black);
                _spline.StrokeThickness = 1;
                _spline.MouseMove += _mw.distloadmousemove;
                _spline.MouseEnter += _mw.mouseenter;
                _spline.MouseLeave += _mw.mouseleave;
                Children.Add(_spline);
            }

            double max = _loadppoly.Max;
            double maxlocation = _loadppoly.MaxLocation;
            double min = _loadppoly.Min;
            double minlocation = _loadppoly.MinLocation;

            starttext = createtextblock();
            _beam.Children.Add(starttext);
            starttext.Text = System.Math.Round(_loadppoly.Calculate(0), 1) + " kN/m";
            starttext.Foreground = new SolidColorBrush(Colors.Black);
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            RotateAround(starttext, _beam.Angle);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = coeff * _loadppoly.Calculate(0);
            if (calculated > 0)
            {
                Canvas.SetTop(starttext, calculated);
            }
            else
            {
                Canvas.SetTop(starttext, calculated - starttext.Height);
            }

            if (minlocation != 0 && minlocation != _length)
            {
                mintext = createtextblock();
                mintext.Text = System.Math.Round(min, 1) + " kNm";
                mintext.Foreground = new SolidColorBrush(Colors.Black);
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext, _beam.Angle);

                _beam.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = coeff * min;
                if (calculated > 0)
                {
                    Canvas.SetTop(mintext, calculated);
                }
                else
                {
                    Canvas.SetTop(mintext, calculated - mintext.Height);
                }
            }

            if (maxlocation != 0 && maxlocation != _length)
            {
                maxtext = createtextblock();
                maxtext.Text = System.Math.Round(max, 1) + " kNm";
                maxtext.Foreground = new SolidColorBrush(Colors.Black);
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext, _beam.Angle);

                _beam.Children.Add(maxtext);

                Canvas.SetLeft(maxtext, maxlocation * 100 - maxtext.Width / 2);

                calculated = coeff * max;

                if (calculated > 0)
                {
                    Canvas.SetTop(maxtext, calculated);
                }
                else
                {
                    Canvas.SetTop(maxtext, calculated - maxtext.Height);
                }
            }

            endtext = createtextblock();
            _beam.Children.Add(endtext);
            endtext.Text = System.Math.Round(_loadppoly.Calculate(_beam.Length), 1) + " kN/m";
            endtext.Foreground = new SolidColorBrush(Colors.Black);
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext, _beam.Angle);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = coeff * _loadppoly.Calculate(_beam.Length);
            if (calculated > 0)
            {
                Canvas.SetTop(endtext, calculated);
            }
            else
            {
                Canvas.SetTop(endtext, calculated - endtext.Height);
            }
        }

        /// <summary>
        /// Draws the arrow under the load spline.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void drawarrow(double x, double y)
        {
            var points = new PointCollection();
            points.Add(new Point(x - 0.5, y));
            points.Add(new Point(x - 0.5, 5));
            points.Add(new Point(x - 1.7, 5));
            points.Add(new Point(x, 0));
            points.Add(new Point(x + 1.7, 5));
            points.Add(new Point(x + 0.5, 5));
            points.Add(new Point(x + 0.5, y));
            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(polygon);
        }

        /// <summary>
        /// Draws the arrow when the load spline is negative.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void drawnegativearrow(double x, double y)
        {
            var points = new PointCollection();
            points.Add(new Point(x - 0.5, y));
            points.Add(new Point(x - 0.5, -5));
            points.Add(new Point(x - 1.7, -5));
            points.Add(new Point(x, 0));
            points.Add(new Point(x + 1.7, -5));
            points.Add(new Point(x + 0.5, -5));
            points.Add(new Point(x + 0.5, y));
            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(polygon);
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
            if (starttext != null)
            {
                if (_beam.Children.Contains(starttext))
                {
                    _beam.Children.Remove(starttext);
                }
            }

            if (endtext != null)
            {
                if (_beam.Children.Contains(endtext))
                {
                    _beam.Children.Remove(endtext);
                }
            }

            if (mintext != null)
            {
                if (_beam.Children.Contains(mintext))
                {
                    _beam.Children.Remove(mintext);
                }
            }

            if (maxtext != null)
            {
                if (_beam.Children.Contains(maxtext))
                {
                    _beam.Children.Remove(maxtext);
                }
            }
        }
    }
}
