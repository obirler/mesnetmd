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
        public BasicSupport() : base(ObjectType.BasicSupport)
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Basic Support " + SupportId;
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(rdof);
        }

        public BasicSupport(Canvas canvas) : base(ObjectType.BasicSupport)
        {
            InitializeComponent();
            Members = new List<Member>();
            Name = "Basic Support " + SupportId;
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(rdof);
            canvas.Children.Add(this);           
            AddObject(this);
            BindEvents();
        }

        private void InitializeComponent()
        {
            Width = 26;
            Height = 16;
            rotateTransform = new RotateTransform();
            rotateTransform.CenterX = Width/2;
            rotateTransform.CenterY = Height;
            rotateTransform.Angle = 0;
            RenderTransform = rotateTransform;
            createtriangle();
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

        /// <summary>
        /// Creates the core which is the invisible portion that is used to collect click event.
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

        public void Add(Canvas canvas, double leftpos, double toppos)
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

            MyDebug.WriteInformation("Position has been set : " + left + " : " + right);
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

                            Canvas.SetLeft(this, beam.LeftPoint.X - Width/2);

                            Canvas.SetTop(this, beam.LeftPoint.Y - Height);

                            beam.LeftSide = this;

                            //Add rotational dof member
                            var ldofmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[0].Members.Add(ldofmember);

                            //beam.IsBound = true;

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - Width/2);

                            Canvas.SetTop(this, beam.RightPoint.Y - Height);

                            beam.RightSide = this;

                            var rdofmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[0].Members.Add(rdofmember);

                            //beam.IsBound = true;

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

                            //Add rotational dof member
                            var ldofmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[0].Members.Add(ldofmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            var rdofmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[0].Members.Add(rdofmember);

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

                            var ldofmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[0].Members.Add(ldofmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            var rdofmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[0].Members.Add(rdofmember);

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

                            var ldofmember = new DOFMember(beam, DOFLocation.LeftRotational);

                            DegreeOfFreedoms[0].Members.Add(ldofmember);

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            var rdofmember = new DOFMember(beam, DOFLocation.RightRotational);

                            DegreeOfFreedoms[0].Members.Add(rdofmember);

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
