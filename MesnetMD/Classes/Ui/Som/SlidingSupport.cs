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
    public class SlidingSupport : SupportItem, ISomItem, ISupportItem, IFreeSupportItem
    {
        public SlidingSupport() : base(ObjectType.SlidingSupport)
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Sliding Support " + SupportId;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(rdof);
        }

        public SlidingSupport(Canvas canvas) : base(ObjectType.SlidingSupport)
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Sliding Support " + SupportId;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(rdof);
            canvas.Children.Add(this);
            AddObject(this);
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

        /// <summary>
        /// Creates the triangle which is the visible portion of the support.
        /// </summary>
        private void createtriangle()
        {
            _triangle = new Polygon();
            _triangle.Points.Add(new Point(13, 16));
            _triangle.Points.Add(new Point(4, 0));
            _triangle.Points.Add(new Point(22, 0));
            _triangle.Points.Add(new Point(13, 16));
            _triangle.Points.Add(new Point(13, 13));
            _triangle.Points.Add(new Point(6.5, 1.5));
            _triangle.Points.Add(new Point(19.5, 1.5));
            _triangle.Points.Add(new Point(13, 13));
            _triangle.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_triangle);
        }

        private void createellipses()
        {
            _e1 = new Ellipse();
            _e1.Height = 5;
            _e1.Width = 5;
            _e1.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_e1);
            Canvas.SetBottom(_e1, 16);
            Canvas.SetLeft(_e1,6);

            _e2 = new Ellipse();
            _e2.Height = 5;
            _e2.Width = 5;
            _e2.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_e2);
            Canvas.SetBottom(_e2, 16);
            Canvas.SetLeft(_e2, 15);
        }

        /// <summary>
        /// Creates the core which is the invisble portion that is used to collect click event.
        /// </summary>
        private void createcore()
        {
            _core = new Polygon();
            _core.Points.Add(new Point(13, 13));
            _core.Points.Add(new Point(6.5, 1.5));
            _core.Points.Add(new Point(19.5, 1.5));
            _core.Points.Add(new Point(13, 13));
            _core.Points.Add(new Point(6.5, 1.5));
            _core.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_core);
        }

        private RotateTransform rotateTransform;

        private Polygon _triangle;

        private Polygon _core;

        private Ellipse _e1;

        private Ellipse _e2;

        public List<Member> Members;

        public void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BasicSupportMouseDown;
            _core.MouseUp += mw.BasicSupportMouseUp;
        }

        public void Select()
        {
            _triangle.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _e1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _e2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            _triangle.Fill = new SolidColorBrush(Colors.Black);
            _e1.Fill = new SolidColorBrush(Colors.Black);
            _e2.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        public void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);

            Canvas.SetLeft(this, leftpos);

            Canvas.SetTop(this, toppos);
        }

        public void UpdatePosition(Beam beam)
        {
            foreach (Member member in Members)
            {
                if (member.Beam == beam)
                {
                    switch (member.Direction)
                    {
                        case Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.LeftPoint.Y - Height);

                            beam.LeftSide = this;

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.RightPoint.Y - Height);

                            beam.RightSide = this;

                            break;
                    }
                    SetAngle(beam.Angle);
                }
            }
        }

        public void SetPosition(double x, double y)
        {
            var left = x - Width / 2;
            var right = y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MyDebug.WriteInformation("Position has been set : " + left + " : " + right);
        }

        public void SetPosition(Point point)
        {
            var left = point.X - Width / 2;
            var right = point.Y - Height / 2;

            Canvas.SetLeft(this, left);

            Canvas.SetTop(this, right);

            MyDebug.WriteWarning("Position has been set : " + left + " : " + right);
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
            _angle = angle;
        }

        public void AddBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);

                if (Members.Count == 1)
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.LeftPoint.Y - Height);

                            beam.LeftSide = this;

                            var lhdmember = new DOFMember(beam, DOFLocation.LeftHorizontal);

                            DegreeOfFreedoms[0].Members.Add(lhdmember);

                            var lrdmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[1].Members.Add(lrdmember);

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.RightPoint.Y - Height);

                            beam.RightSide = this;

                            var rhdmember = new DOFMember(beam, DOFLocation.RightHorizontal);

                            DegreeOfFreedoms[0].Members.Add(rhdmember);

                            var rrdmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[1].Members.Add(rrdmember);

                            break;
                    }

                    SetAngle(beam.Angle);
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            beam.LeftSide = this;

                            beam.IsBound = true;

                            var lhdmember = new DOFMember(beam, DOFLocation.LeftHorizontal);

                            DegreeOfFreedoms[0].Members.Add(lhdmember);

                            var lrdmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[1].Members.Add(lrdmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            var rhdmember = new DOFMember(beam, DOFLocation.RightHorizontal);

                            DegreeOfFreedoms[0].Members.Add(rhdmember);

                            var rrdmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[1].Members.Add(rrdmember);

                            break;
                    }
                }
            }
            else
            {
                MyDebug.WriteWarning("the beam is already added!");
            }
        }

        public void RemoveBeam(Beam beam)
        {
            Member remove = new Member();
            foreach (var member in Members)
            {
                if (member.Beam.Equals(beam))
                {
                    remove = member;
                    break;
                }
            }

            foreach (var dofmember in DegreeOfFreedoms[0].Members)
            {
                if (Equals(dofmember.Beam, remove.Beam))
                {
                    DegreeOfFreedoms[0].Members.Remove(dofmember);
                }
            }

            foreach (var dofmember in DegreeOfFreedoms[1].Members)
            {
                if (Equals(dofmember.Beam, remove.Beam))
                {
                    DegreeOfFreedoms[1].Members.Remove(dofmember);
                }
            }
            Members.Remove(remove);
        }

        public void SetBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);

                if (Members.Count == 1)
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            beam.LeftSide = this;

                            var lhdmember = new DOFMember(beam, DOFLocation.LeftHorizontal);

                            DegreeOfFreedoms[0].Members.Add(lhdmember);

                            var lrdmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[1].Members.Add(lrdmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            var rhdmember = new DOFMember(beam, DOFLocation.RightHorizontal);

                            DegreeOfFreedoms[0].Members.Add(rhdmember);

                            var rrdmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[1].Members.Add(rrdmember);

                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            beam.LeftSide = this;

                            beam.IsBound = true;

                            var lhdmember = new DOFMember(beam, DOFLocation.LeftHorizontal);

                            DegreeOfFreedoms[0].Members.Add(lhdmember);

                            var lrdmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[1].Members.Add(lrdmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            var rhdmember = new DOFMember(beam, DOFLocation.RightHorizontal);

                            DegreeOfFreedoms[0].Members.Add(rhdmember);

                            var rrdmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[1].Members.Add(rrdmember);

                            break;
                    }
                }
            }
            else
            {
                MyDebug.WriteWarning("the beam is already added!");
            }
        }
    }
}
