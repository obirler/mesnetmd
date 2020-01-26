using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.IO.Manifest;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using static MesnetMD.Classes.Global;
using Member = MesnetMD.Classes.Tools.Member;

namespace MesnetMD.Classes.Ui.Som
{
    public class SlidingSupport : RealFreeSupportItem
    {
        public SlidingSupport()
        {
            InitializeComponent();
        }

        public SlidingSupport(Canvas canvas)
        {
            InitializeComponent();
            canvas.Children.Add(this);
            AddObject(this);
        }

        public SlidingSupport(SlidingSupportManifest manifest) : base(manifest)
        {
        }

        private void InitializeComponent()
        {
            Name = "Sliding Support " + SupportId;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(rdof);
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

        protected override void createellipses()
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
  
        private Ellipse _e1;

        private Ellipse _e2;    
   
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
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }
    }
}
