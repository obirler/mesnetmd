using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MesnetMD.Classes.IO.Manifest;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using Member = MesnetMD.Classes.Tools.Member;

namespace MesnetMD.Classes.Ui.Som
{
    public class FictionalSupport : FreeSupportItem, IFictionalSupportItem
    {
        public FictionalSupport(Canvas canvas)
        {
            InitializeVariables(canvas);
            InitializeComponent();
        }

        public FictionalSupport()
        {
            InitializeComponent();
        }

        public FictionalSupport(FictionalSupportManifest manifest) : base(manifest)
        {
            InitializeComponent();
        }

        private void InitializeVariables(Canvas canvas)
        {
            FID = fcount++;
            Name = "Fictional Support " + FID;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var vdof = new DOF(Global.DOFType.Vertical);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(vdof);
            DegreeOfFreedoms.Add(rdof);
            Global.AddObject(this);
            canvas.Children.Add(this);
        }

        private void InitializeComponent()
        {
            Width = 14;
            Height = 14;
            createcircle();
            createcirclecore();
            bindevents();
        }

        private void createcircle()
        {
            _circle = new Ellipse();
            _circle.Width = 14;
            _circle.Height = 14;
            _circle.StrokeThickness = 3;
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            Children.Add(_circle);
            CircleHide();
        }

        private void createcirclecore()
        {
            _circlecore = new Ellipse();
            _circlecore.Width = 14;
            _circlecore.Height = 14;
            _circlecore.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_circlecore);
        }

        private void bindevents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _circlecore.MouseDown += mw.CircleMouseDown;
        }

        private Ellipse _circlecore;

        public int FID = 0;

        private static int fcount = 1;

        public override void Select()
        {
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            CircleShow();
            _selected = true;
        }

        public override void UnSelect()
        {
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            CircleHide();
            _selected = false;
        }

        public override void CircleSelect()
        {
            CircleShow();
            _circle.Stroke = new SolidColorBrush(Colors.Yellow);
        }

        public override void CircleUnSelect()
        {
            CircleShow();
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
        }

        public override void AddBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);
                switch (direction)
                {
                    case Global.Direction.Left:

                        Canvas.SetLeft(this, beam.LeftPoint.X - Width / 2);
                        Canvas.SetTop(this, beam.LeftPoint.Y - Height / 2);

                        beam.LeftSide = this;

                        var lhdofmember = new DOFMember(beam, Global.DOFLocation.LeftHorizontal);
                        DegreeOfFreedoms[0].Members.Add(lhdofmember);
                        var lvdofmember = new DOFMember(beam, Global.DOFLocation.LeftVertical);
                        DegreeOfFreedoms[1].Members.Add(lvdofmember);
                        var lrdofmember = new DOFMember(beam, Global.DOFLocation.LeftRotational);
                        DegreeOfFreedoms[2].Members.Add(lrdofmember);

                        break;

                    case Global.Direction.Right:

                        Canvas.SetLeft(this, beam.RightPoint.X - Width / 2);
                        Canvas.SetTop(this, beam.RightPoint.Y - Height / 2);

                        beam.RightSide = this;

                        var rhdofmember = new DOFMember(beam, Global.DOFLocation.RightHorizontal);
                        DegreeOfFreedoms[0].Members.Add(rhdofmember);
                        var rvdofmember = new DOFMember(beam, Global.DOFLocation.RightVertical);
                        DegreeOfFreedoms[1].Members.Add(rvdofmember);
                        var rrdofmember = new DOFMember(beam, Global.DOFLocation.RightRotational);
                        DegreeOfFreedoms[2].Members.Add(rrdofmember);

                        break;
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

                switch (direction)
                {
                    case Global.Direction.Left:

                        beam.LeftSide = this;

                        var lhdofmember = new DOFMember(beam, Global.DOFLocation.LeftHorizontal);

                        DegreeOfFreedoms[0].Members.Add(lhdofmember);

                        var lvdofmember = new DOFMember(beam, Global.DOFLocation.LeftVertical);

                        DegreeOfFreedoms[1].Members.Add(lvdofmember);

                        var lrdofmember = new DOFMember(beam, Global.DOFLocation.LeftRotational);

                        DegreeOfFreedoms[2].Members.Add(lrdofmember);

                        break;

                    case Global.Direction.Right:

                        var rhdofmember = new DOFMember(beam, Global.DOFLocation.RightHorizontal);

                        DegreeOfFreedoms[0].Members.Add(rhdofmember);

                        var rvdofmember = new DOFMember(beam, Global.DOFLocation.RightVertical);

                        DegreeOfFreedoms[1].Members.Add(rvdofmember);

                        var rrdofmember = new DOFMember(beam, Global.DOFLocation.RightRotational);

                        DegreeOfFreedoms[2].Members.Add(rrdofmember);

                        break;
                }
            }
            else
            {
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }

        /// <summary>
        /// Updates the position of the support according to the beam that is bounded.
        /// </summary>
        /// <param name="beam">The reference beam.</param>
        public void UpdatePosition(Beam beam)
        {
            foreach (Member member in Members)
            {
                if (Equals(member.Beam, beam))
                {
                    switch (member.Direction)
                    {
                        case Global.Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.LeftPoint.Y - Height / 2);

                            beam.LeftSide = this;

                            break;

                        case Global.Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - Width / 2);

                            Canvas.SetTop(this, beam.RightPoint.Y - Height / 2);

                            beam.RightSide = this;

                            break;
                    }
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
    }
}
