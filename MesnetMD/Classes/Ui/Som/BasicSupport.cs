using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using static MesnetMD.Classes.Global;

namespace MesnetMD.Classes.Ui.Som
{
    public class BasicSupport : SupportItem, ISomItem, ISupportItem, IFreeSupportItem
    {
        public BasicSupport()
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Basic Support " + SupportId;
        }

        public BasicSupport(Canvas canvas)
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Basic Support " + SupportId;
            canvas.Children.Add(this);
            AddObject(this);
            BindEvents();
        }

        private void InitializeComponent()
        {
            Width = 26;
            Height = 16;
            rotateTransform = new RotateTransform(13, 0, 0);
            LayoutTransform = rotateTransform;
            createtriangle();
            createcore();
        }

        /// <summary>
        /// Creates the triangle which is the visible portion of the support.
        /// </summary>
        private void createtriangle()
        {
            _triangle = new Polygon();
            _triangle.Points.Add(new Point(13, 0));
            _triangle.Points.Add(new Point(4, 16));
            _triangle.Points.Add(new Point(22, 16));
            _triangle.Points.Add(new Point(13, 0));
            _triangle.Points.Add(new Point(6.5, 14.5));
            _triangle.Points.Add(new Point(19.5, 14.5));
            _triangle.Points.Add(new Point(13, 3));
            _triangle.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_triangle);
        }

        /// <summary>
        /// Creates the core which is the invisble portion that is used to collect click event.
        /// </summary>
        private void createcore()
        { 
            _core = new Polygon();
            _core.Points.Add(new Point(13, 3));
            _core.Points.Add(new Point(6.5, 14.5));
            _core.Points.Add(new Point(19.5, 14.5));
            _core.Points.Add(new Point(13, 3));
            _core.Points.Add(new Point(6.5, 14.5));
            _core.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_core);
        }

        private RotateTransform rotateTransform;

        private Polygon _triangle;

        private Polygon _core;

        public List<Member> Members;

        public void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BasicSupportMouseDown;
            _core.MouseUp += mw.BasicSupportMouseUp;
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void UnSelect()
        {
            throw new NotImplementedException();
        }

        public void ResetSolution()
        {
            throw new NotImplementedException();
        }

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            throw new NotImplementedException();
        }

        public void UpdatePosition(Beam beam)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(Point point)
        {
            throw new NotImplementedException();
        }

        public void SetAngle(double angle)
        {
            throw new NotImplementedException();
        }

        public void AddBeam(Beam beam, Global.Direction direction)
        {
            throw new NotImplementedException();
        }

        public void RemoveBeam(Beam beam)
        {
            throw new NotImplementedException();
        }

        public void SetBeam(Beam beam, Global.Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
