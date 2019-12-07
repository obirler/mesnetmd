using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class ConcentratedLoad : GraphicItem, IGraphic
    {
        public ConcentratedLoad(KeyValueCollection loads, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.ConcentratedLoad;

            _beam = beam;

            _loads = loads;

            _length = _beam.Length;

            _labellist = new List<TextBlock>();

            Draw(c);
        }

        private Beam _beam;

        /// <summary>
        /// The load list. List<KeyValuePair<xpos, loadmagnitude>>
        /// </summary>
        private KeyValueCollection _loads;

        private List<TextBlock> _labellist;

        private double _length;

        private double coeff;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
            }
        }

        public KeyValueCollection Loads
        {
            get { return _loads; }
        }

        public void Draw(int c)
        {
            coeff = c / Global.MaxConcLoad;
            Children.Clear();
            RemoveLabels();
            foreach (KeyValuePair<double, double> load in _loads)
            {
                DrawArrow(load.Key * 100, load.Value, coeff);
            }
        }

        /// <summary>
        /// Draws the arrow under the load spline.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void DrawArrow(double x, double y, double c)
        {
            var points = new PointCollection();
            var tbl = createtextblock();
            var polygon = new Polygon();

            if (y > 0)
            {
                if (c * y >= 15)
                {
                    points.Add(new Point(x - 2, c * y));
                    points.Add(new Point(x - 2, 10));
                    points.Add(new Point(x - 5, 10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, 10));
                    points.Add(new Point(x + 2, 10));
                    points.Add(new Point(x + 2, c * y));

                    tbl.Text = y + " kN";
                    _beam.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl, _beam.Angle);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, c * y + tbl.Height/2);
                }
                else
                {
                    points.Add(new Point(x - 2, 15));
                    points.Add(new Point(x - 2, 10));
                    points.Add(new Point(x - 5, 10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, 10));
                    points.Add(new Point(x + 2, 10));
                    points.Add(new Point(x + 2, 15));

                    tbl.Text = y + " kN";
                    _beam.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl, _beam.Angle);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, 15 + tbl.Height / 2);
                }

                _labellist.Add(tbl);

                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                Children.Add(polygon);
                Canvas.SetLeft(polygon, 0);
                Canvas.SetTop(polygon, 0);
            }
            else
            {
                if (c * y <= -15)
                {
                    points.Add(new Point(x - 2, c * y));
                    points.Add(new Point(x - 2, -10));
                    points.Add(new Point(x - 5, -10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, -10));
                    points.Add(new Point(x + 2, -10));
                    points.Add(new Point(x + 2, c * y));

                    tbl.Text = y + " kN";
                    _beam.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl, _beam.Angle);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, c * y - tbl.Height);
                }
                else
                {
                    points.Add(new Point(x - 2, -15));
                    points.Add(new Point(x - 2, -10));
                    points.Add(new Point(x - 5, -10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, -10));
                    points.Add(new Point(x + 2, -10));
                    points.Add(new Point(x + 2, -15));

                    tbl.Text = y + " kN";
                    _beam.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl, _beam.Angle);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -15 - tbl.Height);
                }

                _labellist.Add(tbl);

                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                Children.Add(polygon);
            }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            foreach (TextBlock label in _labellist)
            {
                label.Visibility = Visibility.Visible;
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;

            foreach (TextBlock label in _labellist)
            {
                label.Visibility = Visibility.Collapsed;
            }
        }

        public void RemoveLabels()
        {
            foreach (TextBlock label in _labellist)
            {
                _beam.Children.Remove(label);
            }
            _labellist.Clear();
        }
    }
}
