using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class Stress : GraphicItem, IGraphic
    {
        public Stress(KeyValueCollection stresslist, Beam beam, int c = 200)
        {
            _beam = beam;
            _stress = stresslist;
            _labellist = new List<TextBlock>();
            Draw(c);
        }

        private KeyValueCollection _stress;

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double coeff;

        private SolidColorBrush color = new SolidColorBrush(Colors.Green);

        private SolidColorBrush exceedcolor = new SolidColorBrush(Colors.Red);

        private List<TextBlock> _labellist;

        public void Draw(int c)
        {
            clearlabellist();

            Children.Clear();

            coeff = c / Global.MaxStress;
            bool red = false;
            var points = new PointCollection();
            for (int i = 0; i < _stress.Count; i++)
            {
                var point = new Point(_stress[i].Key * 100, _stress[i].Value * coeff);
                points.Add(point);
                if (i == 0)
                {
                    if (System.Math.Abs(_stress[0].Value) >= _beam.MaxAllowableStress)
                    {
                        red = true;
                    }
                    else
                    {
                        red = false;
                    }
                }
                else
                {
                    if (red)
                    {
                        if (System.Math.Abs(_stress[i].Value) < _beam.MaxAllowableStress)
                        {
                            addspline(points, red);
                            red = false;
                            points = new PointCollection();
                        }
                    }
                    else
                    {
                        if (System.Math.Abs(_stress[i].Value) >= _beam.MaxAllowableStress)
                        {
                            addspline(points, red);
                            red = true;
                            points = new PointCollection();
                        }
                    }
                }
            }

            if (points.Count > 0)
            {
                addspline(points, red);
            }

            if (_stress[0].Value != 0)
            {
                var leftline = new PointCollection();
                leftline.Add(new Point(0, 0));
                leftline.Add(new Point(0, _stress[0].Value * coeff));

                var leftspline = new CardinalSplineShape(leftline);
                if (System.Math.Abs(_stress[0].Value) >= _beam.MaxAllowableStress)
                {
                    leftspline.Stroke = exceedcolor;
                }
                else
                {
                    leftspline.Stroke = color;
                }
                leftspline.StrokeThickness = 1;
                Children.Add(leftspline);

                var lefttext = createtextblock();
                _labellist.Add(lefttext);
                _beam.Children.Add(lefttext);
                lefttext.Text = System.Math.Round(_stress[0].Value, 1) + " MPa";
                if (System.Math.Abs(_stress[0].Value) >= _beam.MaxAllowableStress)
                {
                    lefttext.Foreground = exceedcolor;
                }
                else
                {
                    lefttext.Foreground = color;
                }
                MinSize(lefttext);
                lefttext.TextAlignment = TextAlignment.Center;
                RotateAround(lefttext, _beam.Angle);
                if (_stress[0].Value > 0)
                {
                    Canvas.SetTop(lefttext, _stress[0].Value * coeff + lefttext.Height);
                }
                else
                {
                    Canvas.SetTop(lefttext, _stress[0].Value * coeff - lefttext.Height);
                }

                Canvas.SetLeft(lefttext, -lefttext.Width / 2);
            }

            if (_stress[_stress.Count - 1].Value != 0)
            {
                var rightline = new PointCollection();
                rightline.Add(new Point(_beam.Length * 100, 0));
                rightline.Add(new Point(_beam.Length * 100, _stress[_stress.Count - 1].Value * coeff));

                var rightspline = new CardinalSplineShape(rightline);
                if (System.Math.Abs(_stress[_stress.Count - 1].Value) >= _beam.MaxAllowableStress)
                {
                    rightspline.Stroke = exceedcolor;
                }
                else
                {
                    rightspline.Stroke = color;
                }
                rightspline.StrokeThickness = 1;
                Children.Add(rightspline);

                var righttext = createtextblock();
                _labellist.Add(righttext);
                _beam.Children.Add(righttext);
                righttext.Text = System.Math.Round(_stress[_stress.Count - 1].Value, 1) + " MPa";
                if (System.Math.Abs(_stress[_stress.Count - 1].Value) >= _beam.MaxAllowableStress)
                {
                    righttext.Foreground = exceedcolor;
                }
                else
                {
                    righttext.Foreground = color;
                }
                MinSize(righttext);
                righttext.TextAlignment = TextAlignment.Center;
                RotateAround(righttext, _beam.Angle);
                if (_stress[_stress.Count - 1].Value > 0)
                {
                    Canvas.SetTop(righttext, _stress[_stress.Count - 1].Value * coeff + righttext.Height);
                }
                else
                {
                    Canvas.SetTop(righttext, _stress[_stress.Count - 1].Value * coeff - righttext.Height);
                }

                Canvas.SetLeft(righttext, _beam.Length * 100 - righttext.Width / 2);
            }

            if (_stress.YMaxPosition != 0 && _stress.YMaxPosition != _beam.Length)
            {
                var maxtext = createtextblock();
                _labellist.Add(maxtext);
                _beam.Children.Add(maxtext);
                maxtext.Text = System.Math.Round(_stress.YMax, 1) + " MPa";
                if (System.Math.Abs(_stress.YMax) >= _beam.MaxAllowableStress)
                {
                    maxtext.Foreground = exceedcolor;
                }
                else
                {
                    maxtext.Foreground = color;
                }
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext, _beam.Angle);
                Canvas.SetTop(maxtext, _stress.YMax * coeff);
                Canvas.SetLeft(maxtext, _stress.YMaxPosition * 100 - maxtext.Width / 2);

                var maxline = new PointCollection();
                maxline.Add(new Point(_stress.YMaxPosition * 100, 0));
                maxline.Add(new Point(_stress.YMaxPosition * 100, _stress.YMax * coeff));

                var maxspline = new CardinalSplineShape(maxline);
                if (System.Math.Abs(_stress.YMax) >= _beam.MaxAllowableStress)
                {
                    maxspline.Stroke = exceedcolor;
                }
                else
                {
                    maxspline.Stroke = color;
                }
                maxspline.StrokeThickness = 1;
                Children.Add(maxspline);
            }

            if (_stress.YMinPosition != 0 && _stress.YMinPosition != _beam.Length)
            {
                var mintext = createtextblock();
                _labellist.Add(mintext);
                _beam.Children.Add(mintext);
                mintext.Text = System.Math.Round(_stress.YMin, 1) + " MPa";
                mintext.Foreground = exceedcolor;
                if (System.Math.Abs(_stress.YMin) >= _beam.MaxAllowableStress)
                {
                    mintext.Foreground = exceedcolor;
                }
                else
                {
                    mintext.Foreground = color;
                }
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext, _beam.Angle);
                Canvas.SetTop(mintext, _stress.YMin * coeff);
                Canvas.SetLeft(mintext, _stress.YMinPosition * 100 - mintext.Width / 2);

                var maxline = new PointCollection();
                maxline.Add(new Point(_stress.YMinPosition * 100, 0));
                maxline.Add(new Point(_stress.YMinPosition * 100, _stress.YMin * coeff));

                var maxspline = new CardinalSplineShape(maxline);
                if (System.Math.Abs(_stress.YMin) >= _beam.MaxAllowableStress)
                {
                    maxspline.Stroke = exceedcolor;
                }
                else
                {
                    maxspline.Stroke = color;
                }
                maxspline.StrokeThickness = 1;
                Children.Add(maxspline);
            }

            if (_stress.YMax >= _beam.MaxAllowableStress)
            {
                var line = new PointCollection();
                line.Add(new Point(0, _beam.MaxAllowableStress * coeff));
                line.Add(new Point(_beam.Length * 100, _beam.MaxAllowableStress * coeff));

                var spline = new CardinalSplineShape(line);
                spline.Stroke = exceedcolor;
                spline.StrokeThickness = 1;
                Children.Add(spline);

                var starttext = createtextblock();
                _labellist.Add(starttext);
                _beam.Children.Add(starttext);
                starttext.Text = System.Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                starttext.Foreground = exceedcolor;
                MinSize(starttext);
                starttext.TextAlignment = TextAlignment.Center;
                RotateAround(starttext, _beam.Angle);

                Canvas.SetTop(starttext, _beam.MaxAllowableStress * coeff - starttext.Height);
                Canvas.SetLeft(starttext, -starttext.Width / 2);

                var endtext = createtextblock();
                _labellist.Add(endtext);
                _beam.Children.Add(endtext);
                endtext.Text = System.Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                endtext.Foreground = exceedcolor;
                MinSize(endtext);
                endtext.TextAlignment = TextAlignment.Center;
                RotateAround(endtext, _beam.Angle);

                Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
                Canvas.SetTop(endtext, _beam.MaxAllowableStress * coeff - endtext.Height);
            }

            if (System.Math.Abs(_stress.YMin) >= _beam.MaxAllowableStress)
            {
                var line = new PointCollection();
                line.Add(new Point(0, -_beam.MaxAllowableStress * coeff));
                line.Add(new Point(_beam.Length * 100, -_beam.MaxAllowableStress * coeff));

                var spline = new CardinalSplineShape(line);
                spline.Stroke = exceedcolor;
                spline.StrokeThickness = 1;
                Children.Add(spline);

                var starttext = createtextblock();
                _labellist.Add(starttext);
                _beam.Children.Add(starttext);
                starttext.Text = System.Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                starttext.Foreground = exceedcolor;
                MinSize(starttext);
                starttext.TextAlignment = TextAlignment.Center;
                RotateAround(starttext, _beam.Angle);
                Canvas.SetTop(starttext, -_beam.MaxAllowableStress * coeff);

                Canvas.SetLeft(starttext, -starttext.Width / 2);

                var endtext = createtextblock();
                _labellist.Add(endtext);
                _beam.Children.Add(endtext);
                endtext.Text = System.Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                endtext.Foreground = exceedcolor;
                MinSize(endtext);
                endtext.TextAlignment = TextAlignment.Center;
                RotateAround(endtext, _beam.Angle);

                Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
                Canvas.SetTop(endtext, -_beam.MaxAllowableStress * coeff);
            }
        }

        public double Calculate(double x)
        {
            return _stress.Calculate(x);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            showlabellist();
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
            hidelabellist();
        }

        private void addspline(PointCollection points, bool isred)
        {
            var spline = new CardinalSplineShape(points);
            if (isred)
            {
                spline.Stroke = exceedcolor;
            }
            else
            {
                spline.Stroke = color;
            }
            spline.StrokeThickness = 1;
            spline.MouseMove += _mw.stressmousemove;
            spline.MouseEnter += _mw.mouseenter;
            spline.MouseLeave += _mw.mouseleave;
            Children.Add(spline);
        }

        private void clearlabellist()
        {
            foreach (var label in _labellist)
            {
                _beam.Children.Remove(label);
            }
            _labellist.Clear();
        }

        private void showlabellist()
        {
            foreach (var label in _labellist)
            {
                label.Visibility = Visibility.Visible;
            }
        }

        private void hidelabellist()
        {
            foreach (var label in _labellist)
            {
                label.Visibility = Visibility.Collapsed;
            }
        }

        public void RemoveLabels()
        {
            foreach (var label in _labellist)
            {
                _beam.Children.Remove(label);
            }
        }
    }
}
