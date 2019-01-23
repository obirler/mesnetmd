using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using static MesnetMD.Classes.Global;

namespace MesnetMD.Classes.Ui.Som
{
    public class BasicSupport : RealFreeSupportItem
    {
        public BasicSupport() : base(ObjectType.BasicSupport)
        {
            Name = "Basic Support " + SupportId;
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(rdof);
        }

        public BasicSupport(Canvas canvas) : base(ObjectType.BasicSupport)
        {
            Name = "Basic Support " + SupportId;
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(rdof);
            canvas.Children.Add(this);           
            AddObject(this);
            BindEvents();
        }

        /// <summary>
        /// Creates the triangle which is the visible portion of the support.
        /// </summary>
        protected override void createtriangle()
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
        protected override void createcore()
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
   
        public override void AddBeam(Beam beam, Global.Direction direction)
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
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }

        public override void SetBeam(Beam beam, Global.Direction direction)
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
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }
    }
}
