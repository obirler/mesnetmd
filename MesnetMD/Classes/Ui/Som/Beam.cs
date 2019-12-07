using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MesnetMD.Classes.IO;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Graphics;
using MoreLinq;

namespace MesnetMD.Classes.Ui.Som
{
    public class Beam : SomItem, ISomItem
    {
        public Beam()
        {
            InitializeComponent();
        }

        public Beam(double length)
        {
            InitializeComponent(length);
        }

        public Beam(Canvas canvas, double length)
        {
            _canvas = canvas;

            InitializeComponent(length);

            AddTopLeft(_canvas, 10000, 10000);
        }
        
        static Beam()
        {
            if(File.Exists("stiffness.txt"))
            {
                File.Delete("stiffness.txt");
            }
        }

        private void InitializeComponent(double length=1.0)
        {
            _beamid = _beamcount++;
            Name = "Beam " + _beamid;
            Type = Global.ObjectType.Beam;
            _length = length;
            Width = length * 100;
            Height = 0;

            _core = new Rectangle();
            _core.Height = 4;
            _core.Width = Width;
            _core.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_core);

            SetLeft(_core, 0);
            SetBottom(_core, -2);

            var arrow = new Polygon();
            arrow.Points.Add(new Point(5, 50));
            arrow.Points.Add(new Point(0, 40));
            arrow.Points.Add(new Point(4, 40));
            arrow.Points.Add(new Point(4, 0));
            arrow.Points.Add(new Point(6, 0));
            arrow.Points.Add(new Point(6, 4));
            arrow.Points.Add(new Point(10, 40));

            arrow.HorizontalAlignment = HorizontalAlignment.Center;
            arrow.Height = 50;
            arrow.Width = 10;
            arrow.Fill = new SolidColorBrush(Colors.Red);

            rotateTransform = new RotateTransform();
            rotateTransform.CenterX = Width / 2;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = 0;
            RenderTransform = rotateTransform;

            _connector = new BeamConnector(this);

            createarrow();

            BindEvents();
        }

        private void InitializeVariables(double length)
        {
           _length = length;

            Width = length * 100;

            MesnetMDDebug.WriteInformation(Name + " has been created : length = " + _length + " m, Width = " + Width);

            RightSide = null;
            LeftSide = null;
            Canvas.SetLeft(_directionarrow, Width / 2 - _directionarrow.Width / 2);

            BindEvents();
        }

        private void createarrow()
        {
            _directionarrow = new Polygon();
            Children.Add(_directionarrow);
            _directionarrow.Height = 50;
            _directionarrow.Width = 10;
            _directionarrow.Points.Add(new Point(5, 0));
            _directionarrow.Points.Add(new Point(0, 10));
            _directionarrow.Points.Add(new Point(4, 10));
            _directionarrow.Points.Add(new Point(4, 50));
            _directionarrow.Points.Add(new Point(6, 50));
            _directionarrow.Points.Add(new Point(6, 10));
            _directionarrow.Points.Add(new Point(10,10));
            _directionarrow.Fill = new SolidColorBrush(Colors.Red);
            Canvas.SetLeft(_directionarrow, Width / 2-_directionarrow.Width / 2);
            _directionarrow.Visibility = Visibility.Collapsed;
        }

        public void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BeamCoreMouseDown;
            _core.MouseUp += mw.BeamCoreMouseUp;
            _core.MouseMove += mw.BeamCoreMouseMove;
        }

        #region internal variables

        private static int _beamcount = 0;

        private int _beamid;

        private bool selected;

        private double _length;

        private double _izero;

        private double _azero;

        private double _elasticity;    

        private double _leftpos;

        private double _toppos;

        public RotateTransform rotateTransform;

        //private Ellipse _startcircle;

        //private Ellipse _endcircle;

        private Rectangle _core;

        private Polygon _directionarrow;

        private BeamConnector _connector;

        KeyValueCollection _concentratedloads;

        private PiecewisePoly _distributedloads;

        private PiecewisePoly _loads;

        private PiecewisePoly _inertiappoly;

        private PiecewisePoly _areappoly;

        private PiecewisePoly _zeroforceconcpploy;

        private PiecewisePoly _zeroforcedistppoly;

        /// <summary>
        /// The shearForce when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeroforceppoly;

        /// <summary>
        /// The moment when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeromomentppoly;

        private PiecewisePoly _fixedendmomentppoly;

        private PiecewisePoly _fixedendforceppoly;

        private PiecewisePoly _axialforceppoly;

        private KeyValueCollection _stress;

        private PiecewisePoly _eppoly;

        private PiecewisePoly _dppoly;

        public SupportItem LeftSide;

        public SupportItem RightSide;

        public double LeftDistributionFactor;

        public double RightDistributionFactor;

        public Global.Direction circledirection;

        private Canvas _canvas;

        private Point corepoint;

        private ConcentratedLoad _concloaddiagram;

        private DistributedLoad _distloaddiagram;

        private Moment _momentdiagram;

        private ShearForce _feforcediagram;

        private AxialForce _axialforcediagram;

        private Moment _femomentdiagram;

        private Inertia _inertiadiagram;

        private Area _areadiagram;

        private Stress _stressdiagram;

        private bool _directionshown = false;

        /// <summary>
        /// The left support shearForce of the beam.
        /// </summary>
        private double _leftsupportforcedist;

        private double _leftsupportforceconc;

        /// <summary>
        /// The right support shearForce of the beam.
        /// </summary>
        private double _rightsupportforcedist;

        private double _rightsupportforceconc;

        /// <summary>
        /// The resultant shearForce that is the sum of the all acting shearForce on beam.
        /// </summary>
        private double _resultantforce;

        /// <summary>
        /// The acting point of the resultant shearForce.
        /// </summary>
        private double _resultantforcedistance;

        /// <summary>
        /// The left-end moment of the beam when the load is acting according to cross sign system.
        /// </summary>
        private double _ma;

        /// <summary>
        /// The left-end moment of the beam when the load is acting according to mohr sign system.
        /// </summary>
        private double _maclapeyron;

        /// <summary>
        /// The right-end moment of the beam when the load is acting according to cross sign system.
        /// </summary>
        private double _mb;

        /// <summary>
        /// The right-end moment of the beam when the load is acting according to mohr sign system.
        /// </summary>
        private double _mbclapeyron;

        private double _alfaa;

        private double _alfab;

        private double _beta;

        private double _ca;

        private double _cb;

        private double _fia;

        private double _fib;

        private double _gamaba;

        private double _gamaab;

        private double _ka;

        private double _kb;

        private double _angle;

        private double _maxmoment;

        private double _maxabsmoment;

        private double _minmoment;

        private double _maxforce;

        private double _maxabsforce;

        private double _minforce;

        private double _maxaxialforce;

        private double _maxabsaxialforce;

        private double _minaxialforce;

        private double _maxinertia;

        private double _maxarea;

        private double _maxstress;

        private double _maxabsstress;

        private double _maxconcload;

        private double _maxabsconcload;

        private double _maxdistload;

        private double _maxabsdistload;

        private TransformGeometry _tgeometry;

        private bool _leftcircleseleted;

        private bool _rightcircleselected;

        private bool _indexpassed;

        private bool _isbound = false;

        private bool _stressanalysis = false;

        private List<Global.Func> _deflection;

        private Global.Func _maxdeflection;

        private double _maxallowablestress;

        private bool _analyticalsolution = false;

        private double[,] _stiffnessmatrix;

        //indirect shearForce vector
        private double[] _idforcevector;

        //direct shearForce vector
        private double[] _dforcevector;

        //resultant shearForce vector
        private double[] _forcevector;

        private double[] _localforcevector;

        private double[,] _transformationmatrix;

        //base stiffness coefficients

        private double _mii;

        private double _mjj;

        private double _mij;

        private double _nii;

        #endregion

        #region methods

        public void Add(Canvas canvas)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                Global.AddObject(this);
                Canvas.SetZIndex(this, 1);
            }
            else
            {
                MesnetMDDebug.WriteWarning(Name + " : This beam has already been added to canvas!");
            }
        }

        /// <summary>
        /// A special add function that is used to add beam that is read from xml file.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void AddFromXml(Canvas canvas, double length)
        {
            _canvas = canvas;
            InitializeVariables(length);
            canvas.Children.Add(this);
            Canvas.SetZIndex(this, 1);
            Canvas.SetLeft(this, _leftpos);
            Canvas.SetTop(this, _toppos);

        }

        public void AddTopLeft(Canvas canvas, double x, double y)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                Global.AddObject(this);

                Canvas.SetLeft(this, x);

                if (Height > 0)
                {
                    Canvas.SetTop(this, y);
                }
                else
                {
                    Canvas.SetTop(this, y - 7);
                }

                Global.BeamCount++;
                _beamid = Global.BeamCount;
                Name = "Beam " + Global.BeamCount;

                SetTransformGeometry(canvas);
                SetAngleCenter(0);
            }
            else
            {
                MesnetMDDebug.WriteWarning(Name + " : This beam has already been added to canvas!");
            }
        }

        public void AddCenter(Canvas canvas, double x, double y)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                Global.AddObject(this);

                Canvas.SetLeft(this, x - Width / 2);

                if (Height > 0)
                {
                    Canvas.SetTop(this, y - Height / 2);
                }
                else
                {
                    Canvas.SetTop(this, y);
                }

                Global.BeamCount++;
                _beamid = Global.BeamCount;
                Name = "Beam " + Global.BeamCount;

                SetTransformGeometry(canvas);
            }
            else
            {
                MesnetMDDebug.WriteWarning(Name + " This beam has already been added to canvas!");
            }
        }

        /// <summary>
        /// Adds inertia moment function.
        /// </summary>
        /// <param name="inertiappoly">The inertia Piecewise Polynomial.</param>
        public void AddInertia(PiecewisePoly inertiappoly)
        {
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.PreciseMin;
            _maxinertia = _inertiappoly.Max;
            Global.WritePPolytoConsole(Name + " inertia added", inertiappoly);
        }

        /// <summary>
        /// Adds the modulus of elasticity.
        /// </summary>
        /// <param name="elasticitymodulus">The modulus of elasticity.</param>
        public void AddElasticity(double elasticitymodulus)
        {
            _elasticity = elasticitymodulus;
        }

        /// <summary>
        /// Adds the e. E stans for neutral axis distance.
        /// </summary>
        /// <param name="eppoly">The EPpoly.</param>
        public void AddE(PiecewisePoly eppoly)
        {
            _eppoly = eppoly;
        }

        /// <summary>
        /// Adds the d. D stands for the height of the beam on load direction
        /// </summary>
        /// <param name="dppoly">The DPpoly.</param>
        public void AddD(PiecewisePoly dppoly)
        {
            _dppoly = dppoly;
        }

        public void AddArea(PiecewisePoly areappoly)
        {
            _areappoly = areappoly;
            _azero = _areappoly.PreciseMin;
            _maxarea = _areappoly.Max;
            Global.WritePPolytoConsole(Name + " area added", areappoly);
        }

        /// <summary>
        /// Adds the distributed load to beam with specified direction.
        /// </summary>
        /// <param name="loadppoly">The desired distributed load piecewise polynomial.</param>
        public void AddLoad(PiecewisePoly loadppoly)
        {
            _distributedloads = loadppoly;
            _maxdistload = _distributedloads.Max;
            _maxabsdistload = _distributedloads.MaxAbs;
        }

        /// <summary>
        /// Adds the concentrated load to beam with specified direction.
        /// </summary>
        /// <param name="load">The desired list of concentrated load key value pair.</param>
        public void AddLoad(KeyValueCollection loadpairs)
        {
            _concentratedloads = loadpairs;
            _maxconcload = _concentratedloads.YMax;
            _maxabsconcload = _concentratedloads.YMaxAbs;
        }

        /// <summary>
        /// Sets the length of the Beam. It is used in xml reading purposes.
        /// </summary>
        /// <param name="length">The length of the beam.</param>
        public void SetLength(double length)
        {
            InitializeVariables(length);
        }

        /// <summary>
        /// Changes the length of the existing Beam.
        /// </summary>
        /// <param name="length">The desired length of the beam.</param>
        public void ChangeLength(double length)
        {
            double oldlength = _length;
            _length = length;
            Width = _length * 100;
            _tgeometry.ChangeWidth(Width);
            Canvas.SetLeft(_directionarrow, Width / 2 - _directionarrow.Width / 2);

            //If the length really is changed then the loads on the beam become meaningless (at least distributed loads), so remove them.
            if (System.Math.Abs(oldlength - length) > 0.00001)
            {
                RemoveConcentratedLoad();
                DestroyConcLoadDiagram();
                RemoveDistributedLoad();
                DestroyDistLoadDiagram();
            }
        }

        public void Remove()
        {
            Global.RemoveObject(this);
            _canvas.Children.Remove(this);
        }

        public void ChangeInertia(PiecewisePoly inertiappoly)
        {
            DestroyInertiaDiagram();
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.Min;
            _maxinertia = _inertiappoly.Max;
            Global.WritePPolytoConsole(Name + " inertia changed", inertiappoly);
        }

        /// <summary>
        /// Determines whether the beam was selected.
        /// </summary>
        /// <returns>True if the beam was selected. False if the beam was not selected.</returns>
        public bool IsSelected()
        {
            return selected;
        }

        /// <summary>
        /// It is executed when the beam was selected. It changes the beam color and shows circles.
        /// </summary>
        public override void Select()
        {
            //BringToFront(_canvas, this);
            _core.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            if (LeftSide is FreeSupportItem ls)
            {
                ls.CircleShow();
            }

            if (RightSide is FreeSupportItem rs)
            {
                rs.CircleShow();
            }

            selected = true;
        }

        /// <summary>
        /// Executed when the beam was unselected. It changes the beam color and hides circles.
        /// </summary>
        public override void UnSelect()
        {
            _core.Fill = new SolidColorBrush(Colors.Black);
            if (LeftSide is FreeSupportItem ls)
            {
                ls.CircleHide();
            }
            if (RightSide is FreeSupportItem rs)
            {
                rs.CircleHide();
            }
            circledirection = Global.Direction.None;
            selected = false;
            _tgeometry.HideCorners();
            MesnetMDDebug.WriteInformation(Name + " Beam unselected : left = " + Canvas.GetLeft(this) + " top = " + Canvas.GetTop(this));
        }

        public void SelectLeftCircle()
        {
            if (LeftSide is FreeSupportItem ls)
            {
                ls.CircleSelect();
            }
            circledirection = Global.Direction.Left;
            _leftcircleseleted = true;
        }

        public void SelectRightCircle()
        {
            if (RightSide is FreeSupportItem rs)
            {
                rs.CircleSelect();
            }
            circledirection = Global.Direction.Right;
            _rightcircleselected = true;
        }

        public void UnSelectCircle()
        {
            if (LeftSide is FreeSupportItem ls)
            {
                ls.CircleUnSelect();
            }
            if (RightSide is FreeSupportItem rs)
            {
                rs.CircleUnSelect();
            }
            circledirection = Global.Direction.None;
            _leftcircleseleted = false;
            _rightcircleselected = false;
        }

        /// <summary>
        /// Sets the position of the beam based on the center point of the beam. The origin is the top-left corner.
        /// </summary>
        /// <param name="x">The x (horizontal) component of desired point.</param>
        /// <param name="y">The y (vertical) component of desired point.</param>
        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(this, x - Width / 2);

            if (Height > 0)
            {
                Canvas.SetTop(this, y - Height / 2);
            }
            else
            {
                Canvas.SetTop(this, y - 7);
            }
        }

        /// <summary>
        /// Sets the position of desired circle point of the beam.
        /// </summary>
        /// <param name="direction">The direction of the circle.</param>
        /// <param name="x">The x (horizontal) component of desired point.</param>
        /// <param name="y">The y (vertical) component of desired point.</param>
        public void SetPosition(Global.Direction direction, double x, double y)
        {
            Vector delta = new Vector();

            switch (direction)
            {
                case Global.Direction.Left:

                    delta.X = x - this.LeftPoint.X;
                    delta.Y = y - this.LeftPoint.Y;

                    Move(delta);

                    break;

                case Global.Direction.Right:

                    delta.X = x - this.RightPoint.X;
                    delta.Y = y - this.RightPoint.Y;

                    Move(delta);

                    break;
            }
        }

        public void SetPosition(Global.Direction direction, Point point)
        {
            SetPosition(direction, point.X, point.Y);
        }

        /// <summary>
        /// Sets the position of the beam based on top-left point of it.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetAbsolutePosition(double x, double y)
        {
            Canvas.SetLeft(this, x);

            if (Height > 0)
            {
                Canvas.SetTop(this, y);
            }
            else
            {
                Canvas.SetTop(this, y - 7);
            }
        }

        /// <summary>
        /// Sets the position of the beam based on top-right point of it.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetAbsolutePosition(Point point)
        {
            Canvas.SetLeft(this, point.X);

            if (Height > 0)
            {
                Canvas.SetTop(this, point.Y);
            }
            else
            {
                Canvas.SetTop(this, point.Y - 7);
            }
        }

        public void SetTopLeft(double top, double left)
        {
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
            SetTransformGeometry(_canvas);
        }

        public void SetTransformGeometry(Canvas canvas)
        {
            _tgeometry = new TransformGeometry(this, canvas);
        }

        public void SetTransformGeometry(Point tl, Point tr, Point br, Point bl, Canvas canvas)
        {
            _tgeometry = new TransformGeometry(tl, tr, br, bl, _canvas);
        }

        /// <summary>
        /// Changes the position of the beam by the given amount.
        /// </summary>
        /// <param name="delta">The change vector.</param>
        public override void Move(Vector delta)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + delta.X);
            Canvas.SetTop(this, Canvas.GetTop(this) + delta.Y);
            _tgeometry.Move(delta);
        }

        /// <summary>
        /// Moves the supports of this beam to the point where the beam is placed.
        /// </summary>
        public void MoveSupports()
        {
            if (LeftSide != null)
            {
                switch (LeftSide.Type)
                {
                    case Global.ObjectType.LeftFixedSupport:

                        var ls = LeftSide as LeftFixedSupport;
                        ls.UpdatePosition(this);

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = LeftSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case Global.ObjectType.BasicSupport:

                        var bs = LeftSide as BasicSupport;
                        bs.UpdatePosition(this);

                        break;

                    case Global.ObjectType.FictionalSupport:
                        var fs = LeftSide as FictionalSupport;
                        fs.UpdatePosition(this);

                        break;
                }
            }

            if (RightSide != null)
            {
                switch (RightSide.Type)
                {
                    case Global.ObjectType.RightFixedSupport:

                        var rs = RightSide as RightFixedSupport;
                        rs.UpdatePosition(this);

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = RightSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case Global.ObjectType.BasicSupport:

                        var bs = RightSide as BasicSupport;
                        bs.UpdatePosition(this);

                        break;

                    case Global.ObjectType.FictionalSupport:
                        var fs = RightSide as FictionalSupport;
                        fs.UpdatePosition(this);

                        break;
                }
            }
        }

        public void AnimatedMove(Point newpoint)
        {
            double newX = newpoint.X;
            double newY = newpoint.Y;
            var top = Canvas.GetTop(this);
            var left = Canvas.GetLeft(this);
            TranslateTransform trans = new TranslateTransform();
            this.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(1));
            DoubleAnimation anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(1));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        /// <summary>
        /// Rotates the beam about its center point.
        /// </summary>
        /// <param name="angle">The desired angle.</param>
        public void SetAngleCenter(double angle)
        {
            double oldangle = rotateTransform.Angle;
            rotateTransform.CenterX = Width / 2;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = angle;
            _angle = angle;

            _tgeometry.RotateAboutCenter(angle - oldangle);
        }

        /// <summary>
        /// Rotates the beam about its left point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleLeft(double angle)
        {
            SetAngleCenter(angle);
            leftreconnect();
        }

        /// <summary>
        /// Rotates the beam about its right point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleRight(double angle)
        {
            SetAngleCenter(angle);
            rightreconnect();
        }

        /// <summary>
        /// Connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>        
        public void Connect(Global.Direction direction1, Beam oldbeam, Global.Direction direction2)
        {
            _connector.Connect(direction1, oldbeam, direction2);            
        }

        /// <summary>
        /// Circular connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>
        /// <exception cref="InvalidOperationException">
        /// In order to create circular beam system both beam to be connected need to be bound
        /// or
        /// In order to create circular beam system one of the beam to be connected need to have support on connection side
        /// or
        /// In order to create circular beam system one of the beam to be connected need to have support on connection side
        /// or
        /// Both beam has supports on the assembly points
        /// or
        /// Both beam has supports on the assembly points
        /// or
        /// Both beam has supports on the assembly points
        /// </exception>
        public void CircularConnect(Global.Direction direction1, Beam oldbeam, Global.Direction direction2)
        {
            _connector.CircularConnect(direction1, oldbeam, direction2);          
        }

        private void leftreconnect()
        {
            Beam leftbeam = null;
            var direction = Global.Direction.None;

            if (LeftSide != null)
            {
                switch (LeftSide.Type)
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = LeftSide as BasicSupport;
                        if (bs.Members.Count > 1)
                        {
                            foreach (Member member in bs.Members)
                            {
                                if (!member.Beam.Equals(this))
                                {
                                    leftbeam = member.Beam;
                                    direction = member.Direction;
                                    break;
                                }
                            }
                        }

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = LeftSide as SlidingSupport;
                        if (ss.Members.Count > 1)
                        {
                            foreach (Member member in ss.Members)
                            {
                                if (!member.Beam.Equals(this))
                                {
                                    leftbeam = member.Beam;
                                    direction = member.Direction;
                                    break;
                                }
                            }
                        }

                        break;

                    case Global.ObjectType.FictionalSupport:

                        var fs = LeftSide as FictionalSupport;
                        if (fs.Members.Count > 1)
                        {
                            foreach (var member in fs.Members)
                            {
                                if (!member.Beam.Equals(this))
                                {
                                    leftbeam = member.Beam;
                                    direction = member.Direction;
                                    break;
                                }
                            }
                        }

                        break;
                }
            }

            if (leftbeam != null)
            {
                switch (direction)
                {
                    case Global.Direction.Left:
                        SetPosition(Global.Direction.Left, leftbeam.LeftPoint);
                        break;

                    case Global.Direction.Right:
                        SetPosition(Global.Direction.Left, leftbeam.RightPoint);
                        break;
                }
                MoveSupports();
            }
        }

        private void rightreconnect()
        {
            Beam rightbeam = null;
            var direction = Global.Direction.None;

            if (RightSide != null)
            {
                switch (RightSide.Type)
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = RightSide as BasicSupport;
                        if (bs.Members.Count > 1)
                        {
                            foreach (Member member in bs.Members)
                            {
                                if (!member.Beam.Equals(this))
                                {
                                    rightbeam = member.Beam;
                                    direction = member.Direction;
                                    break;
                                }
                            }
                        }

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = RightSide as SlidingSupport;
                        if (ss.Members.Count > 1)
                        {
                            foreach (Member member in ss.Members)
                            {
                                if (!member.Beam.Equals(this))
                                {
                                    rightbeam = member.Beam;
                                    direction = member.Direction;
                                    break;
                                }
                            }
                        }

                        break;
                }
            }

            if (rightbeam != null)
            {
                switch (direction)
                {
                    case Global.Direction.Left:
                        SetPosition(Global.Direction.Right, rightbeam.LeftPoint);
                        break;

                    case Global.Direction.Right:
                        SetPosition(Global.Direction.Right, rightbeam.RightPoint);
                        break;
                }
                MoveSupports();
            }
        }

        /// <summary>
        /// Determines whether the specified point is inside of the rectangle geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is inside; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInside(Point point)
        {
            return _tgeometry.IsInsideOuter(point);
        }

        /// <summary>
        /// Shows the corners of the outer rectangle in canvas. It is used in debug purposes.
        /// </summary>
        /// <param name="radius">The radius of the corner circle.</param>
        public void ShowCorners(double radius)
        {
            _tgeometry.ShowCorners(radius);
        }

        /// <summary>
        /// Shows the corners of the outer rectangle in canvas with predefined values of 5 and 7. 
        /// It is used in debug purposes.
        /// </summary>
        public void ShowCorners()
        {
            ShowCorners(5, 7);
        }

        /// <summary>
        /// Shows the corners rectangle in canvas. It is used in debug purposes.
        /// </summary>
        /// <param name="radiusinner">The inner transform geometry circle radius.</param>
        /// <param name="radiusouter">The outer transform geometry circle radius.</param>
        public void ShowCorners(double radiusinner, double radiusouter)
        {
            //_tgeometry.ShowCorners(radiusinner, radiusouter);
        }

        public void HideCorners()
        {
            _tgeometry.HideCorners();
        }

        /// <summary>
        /// Brings to ui element to front by increasing z index in canvas.
        /// </summary>
        /// <param name="pParent">Parent canvas.</param>
        /// <param name="pToMove">Ui Element to bring front.</param>
        private void BringToFront(Canvas pParent, UserControl pToMove)
        {
            try
            {
                int currentIndex = Canvas.GetZIndex(pToMove);
                int zIndex = 0;
                int maxZ = 0;
                UserControl child;
                for (int i = 0; i < pParent.Children.Count; i++)
                {
                    if (pParent.Children[i] is UserControl &&
                        pParent.Children[i] != pToMove)
                    {
                        child = pParent.Children[i] as UserControl;
                        zIndex = Canvas.GetZIndex(child);
                        maxZ = System.Math.Max(maxZ, zIndex);
                        if (zIndex > currentIndex)
                        {
                            Canvas.SetZIndex(child, zIndex - 1);
                        }
                    }
                }
                Canvas.SetZIndex(pToMove, maxZ);
            }
            catch (Exception ex)
            {
            }
        }

        #region Diagram functions
        public void RemoveDistributedLoad()
        {
            DistributedLoad distload = null;
            foreach (var item in Children)
            {
                if (item is GraphicItem)
                {
                    var load = item as GraphicItem;
                    if (load.GraphicType == Global.GraphicType.DistibutedLoad)
                    {
                        distload = load as DistributedLoad;
                    }
                }
            }

            if (distload != null)
            {
                distload.RemoveLabels();
                Children.Remove(distload);
                _distributedloads = null;
                _maxdistload = Double.MinValue;
                _distloaddiagram = null;
            }
        }

        public void RemoveConcentratedLoad()
        {
            ConcentratedLoad concload = null;
            foreach (var item in Children)
            {
                if (item is GraphicItem)
                {
                    var load = item as GraphicItem;
                    if (load.GraphicType == Global.GraphicType.ConcentratedLoad)
                    {
                        concload = load as ConcentratedLoad;
                    }
                }
            }

            if (concload != null)
            {
                concload.RemoveLabels();
                Children.Remove(concload);
                _concentratedloads = null;
                _concloaddiagram = null;
                _maxconcload = double.MinValue;
            }
        }

        public void ShowDistLoadDiagram(int c)
        {
            if (_distloaddiagram != null)
            {
                _distloaddiagram.Show();
            }
            else if (_distributedloads?.Count > 0)
            {
                var load = new DistributedLoad(_distributedloads, this, c);
                Children.Add(load);
                Canvas.SetBottom(load, 0);
                Canvas.SetLeft(load, 0);
                _distloaddiagram = load;
            }
        }

        public void HideDistLoadDiagram()
        {
            if (_distloaddiagram != null)
            {
                _distloaddiagram.Hide();
            }
        }

        public void DestroyDistLoadDiagram()
        {
            if (_distloaddiagram != null)
            {
                _distloaddiagram.RemoveLabels();
                Children.Remove(_distloaddiagram);
                _distloaddiagram = null;
            }
        }

        public void ShowConcLoadDiagram(int c)
        {
            if (_concloaddiagram != null)
            {
                _concloaddiagram.Show();
            }
            else if (_concentratedloads?.Count > 0)
            {
                var concentratedload = new ConcentratedLoad(_concentratedloads, this, c);
                Children.Add(concentratedload);
                Canvas.SetBottom(concentratedload, 0);
                Canvas.SetLeft(concentratedload, 0);
                _concloaddiagram = concentratedload;
            }
        }

        public void HideConcLoadDiagram()
        {
            if (_concloaddiagram != null)
            {
                _concloaddiagram.Hide();
            }
        }

        public void DestroyConcLoadDiagram()
        {
            if (_concloaddiagram != null)
            {
                _concloaddiagram.RemoveLabels();
                Children.Remove(_concloaddiagram);
                _concloaddiagram = null;
            }
        }

        public void ShowFixedEndForceDiagram(int c)
        {
            if (_feforcediagram != null)
            {
                _feforcediagram.Show();
            }
            else
            {
                var force = new ShearForce(_fixedendforceppoly, this, c);
                Children.Add(force);
                Canvas.SetBottom(force, 0);
                Canvas.SetLeft(force, 0);
                _feforcediagram = force;
            }
        }

        public void HideFixedEndForceDiagram()
        {
            if (_feforcediagram != null)
            {
                _feforcediagram.Hide();
            }
        }

        public void DestroyFixedEndForceDiagram()
        {
            if (_feforcediagram != null)
            {
                _feforcediagram.RemoveLabels();
                Children.Remove(_feforcediagram);
                _feforcediagram = null;
            }
        }

        public void ShowAxialForceDiagram(int c)
        {
            if (_axialforcediagram != null)
            {
                _axialforcediagram.Show();
            }
            else
            {
                var axialforce = new AxialForce(_axialforceppoly, this, c);
                Children.Add(axialforce);
                Canvas.SetBottom(axialforce, 0);
                Canvas.SetLeft(axialforce, 0);
                _axialforcediagram = axialforce;
            }
        }

        public void HideAxialForceDiagram()
        {
            if (_axialforcediagram != null)
            {
                _axialforcediagram.Hide();
            }
        }

        public void DestroyAxialForceDiagram()
        {
            if (_axialforcediagram != null)
            {
                _axialforcediagram.RemoveLabels();
                Children.Remove(_axialforcediagram);
                _axialforcediagram = null;
            }
        }

        public void ShowFixedEndMomentDiagram(int c)
        {
            if (_femomentdiagram != null)
            {
                _femomentdiagram.Show();
            }
            else if (_fixedendmomentppoly?.Count > 0)
            {
                var moment = new Moment(_fixedendmomentppoly, this, c);
                Children.Add(moment);
                Canvas.SetBottom(moment, 0);
                Canvas.SetLeft(moment, 0);
                _femomentdiagram = moment;
            }
        }

        public void HideFixedEndMomentDiagram()
        {
            if (_femomentdiagram != null)
            {
                _femomentdiagram.Hide();
            }
        }

        public void DestroyFixedEndMomentDiagram()
        {
            if (_femomentdiagram != null)
            {
                _femomentdiagram.RemoveLabels();
                Children.Remove(_femomentdiagram);
                _femomentdiagram = null;
            }
        }

        public void ShowInertiaDiagram(int c)
        {
            if (_inertiadiagram != null)
            {
                _inertiadiagram.Show();
            }
            else
            {
                var inertia = new Inertia(_inertiappoly, this, c);
                Children.Add(inertia);
                Canvas.SetBottom(inertia, 0);
                Canvas.SetLeft(inertia, 0);
                _inertiadiagram = inertia;
            }
        }

        public void HideInertiaDiagram()
        {
            if (_inertiadiagram != null)
            {
                _inertiadiagram.Hide();
            }
        }

        public void DestroyInertiaDiagram()
        {
            if (_inertiadiagram != null)
            {
                _inertiadiagram.RemoveLabels();
                Children.Remove(_inertiadiagram);
                _inertiadiagram = null;
            }
        }

        public void ShowAreaDiagram(int c)
        {
            if (_areadiagram != null)
            {
                _areadiagram.Show();
            }
            else
            {
                var area = new Area(_areappoly, this, c);
                Children.Add(area);
                Canvas.SetBottom(area, 0);
                Canvas.SetLeft(area, 0);
                _areadiagram = area;
            }
        }

        public void HideAreaDiagram()
        {
            if (_areadiagram != null)
            {
                _areadiagram.Hide();
            }
        }

        public void DestroyAreaDiagram()
        {
            if (_areadiagram != null)
            {
                _areadiagram.RemoveLabels();
                Children.Remove(_areadiagram);
                _areadiagram = null;
            }
        }

        public void ShowDeflectionDiagram()
        {

        }

        public void HideDeflectionDiagram()
        {

        }

        public void ShowStressDiagram(int c)
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.Show();
            }
            else if (_stress?.Count > 0)
            {
                var stress = new Stress(_stress, this, c);
                Children.Add(stress);
                Canvas.SetBottom(stress, 0);
                Canvas.SetLeft(stress, 0);
                _stressdiagram = stress;
            }
        }

        public void HideStressDiagram()
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.Hide();
            }
        }

        public void DestroyStressDiagram()
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.RemoveLabels();
                Children.Remove(_stressdiagram);
                _stressdiagram = null;
            }
        }

        public void ShowDirectionArrow()
        {
            _directionarrow.Visibility = Visibility.Visible;
            _directionshown = true;
        }

        public void HideDirectionArrow()
        {
            _directionarrow.Visibility = Visibility.Collapsed;
            _directionshown = false;
        }
        #endregion

        #region SoM

        /// <summary>
        /// Finds the left and right support forces under the distributed load conditions.
        /// </summary>
        private void finddistributedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_distributedloads?.Count > 0)
            {
                var forcelist = new List<KeyValuePair<double, double>>();

                foreach (Poly load in _distributedloads)
                {
                    var forces = load.CalculateMagnitudeAndLocation();
                    forcelist.AddRange(forces);
                }

                //Moment from left support point
                double leftmoment = 0;
                foreach (var force in forcelist)
                {
                    leftmoment += force.Key * force.Value;
                }
                _rightsupportforcedist = leftmoment / _length;

                //Moment from right support point
                double rightmoment = 0;
                foreach (var force in forcelist)
                {
                    rightmoment += (_length - force.Key) * force.Value;
                }
                _leftsupportforcedist = rightmoment / _length;
            }
            else
            {
                _leftsupportforcedist = 0;
                _rightsupportforcedist = 0;
            }

            MesnetMDDebug.WriteInformation(Name + " : resultantforce = " + resultantforce + " resultantforcedistance = " + resultantforcedistance);

            MesnetMDDebug.WriteInformation(Name + " : leftsupportforcedist = " + _leftsupportforcedist + " rightsupportforcedist = " + _rightsupportforcedist);

        }

        private void findconcentratedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_concentratedloads?.Count > 0)
            {
                //Moment from left support point
                double leftmoment = 0;
                foreach (KeyValuePair<double, double> force in _concentratedloads)
                {
                    leftmoment += force.Key * force.Value;
                }
                _rightsupportforceconc = leftmoment / _length;

                //Moment from right support point
                double rightmoment = 0;
                foreach (KeyValuePair<double, double> force in _concentratedloads)
                {
                    rightmoment += (_length - force.Key) * force.Value;
                }
                _leftsupportforceconc = rightmoment / _length;
            }
            else
            {
                _leftsupportforceconc = 0;
                _rightsupportforceconc = 0;
            }

            MesnetMDDebug.WriteInformation(Name + " : resultantforcedistance = " + resultantforcedistance);

            MesnetMDDebug.WriteInformation(Name + " : leftsupportforceconc = " + _leftsupportforceconc + " rightsupportforceconc = " + _rightsupportforceconc);
        }

        #region Zero Condition

        private void findconcentratedzeroforce()
        {
            _zeroforceconcpploy = new PiecewisePoly();

            if (_concentratedloads?.Count > 0)
            {
                double leftforce = _leftsupportforceconc;

                if (_concentratedloads[0].Key > 0)
                {
                    var poly1 = new Poly(leftforce.ToString());
                    poly1.StartPoint = 0;
                    poly1.EndPoint = _concentratedloads[0].Key;
                    _zeroforceconcpploy.Add(poly1);
                }

                for (int i = 0; i < _concentratedloads.Count; i++)
                {
                    leftforce = leftforce - _concentratedloads[i].Value;

                    var poly = new Poly(leftforce.ToString());

                    poly.StartPoint = _concentratedloads[i].Key;
                    if (i + 1 < _concentratedloads.Count)
                    {
                        poly.EndPoint = _concentratedloads[i + 1].Key;
                    }
                    else
                    {
                        poly.EndPoint = _length;
                    }

                    _zeroforceconcpploy.Add(poly);
                }
                Global.WritePPolytoConsole(Name + " : _zeroforceconc", _zeroforceconcpploy);
            }
        }

        /// <summary>
        /// Finds the zero shearForce polynomial which is the shearForce polynomial when there is no fixed support in the end of the beam.
        /// </summary>
        private void finddistributedzeroforce()
        {
            _zeroforcedistppoly = new PiecewisePoly();

            if (_distributedloads?.Count > 0)
            {
                if (_distributedloads[0].StartPoint != 0)
                {
                    var ply = new Poly(_leftsupportforcedist.ToString());
                    ply.StartPoint = 0;
                    ply.EndPoint = _distributedloads[0].StartPoint;
                    _zeroforcedistppoly.Add(ply);
                }

                foreach (Poly load in _distributedloads)
                {
                    var index = _distributedloads.IndexOf(load);

                    double weightsbefore = findforcebefore(index);

                    if (index > 0)
                    {
                        if (_distributedloads[index - 1].EndPoint != _distributedloads[index].StartPoint)
                        {
                            var ply = new Poly(weightsbefore.ToString());
                            ply.StartPoint = _distributedloads[index - 1].EndPoint;
                            ply.EndPoint = _distributedloads[index].StartPoint;
                            _zeroforcedistppoly.Add(ply);
                        }
                    }

                    var poly = new Poly();

                    var integration = load.Integrate();
                    var zerovalue = load.Integrate().Calculate(load.StartPoint);
                    if (zerovalue != 0)
                    {
                        if (weightsbefore != 0)
                        {
                            poly = new Poly(weightsbefore.ToString()) - integration + new Poly(zerovalue.ToString());
                        }
                        else
                        {
                            poly = -1 * integration + new Poly(zerovalue.ToString());
                        }
                    }
                    else
                    {
                        if (weightsbefore != 0)
                        {
                            poly = new Poly(weightsbefore.ToString()) - integration;
                        }
                        else
                        {
                            poly = -1 * integration;
                        }
                    }
                    poly.StartPoint = load.StartPoint;
                    poly.EndPoint = load.EndPoint;
                    _zeroforcedistppoly.Add(poly);
                }
                _zeroforcedistppoly.Sort();

                if (_distributedloads.Last().EndPoint != _length)
                {
                    var weights = findforcebefore(_distributedloads.Count);
                    var ply = new Poly(weights.ToString());
                    ply.StartPoint = _distributedloads.Last().EndPoint;
                    ply.EndPoint = _length;
                    _zeroforcedistppoly.Add(ply);
                }

                Global.WritePPolytoConsole(Name + " : _zeroforcedist", _zeroforcedistppoly);
            }
        }

        /// <summary>
        /// Calculates the zero moment, the monet when the beam is bounded with basic supports on both sides.
        /// </summary>
        private void findzeromoment()
        {
            _zeromomentppoly = new PiecewisePoly();

            foreach (Poly force in _zeroforceppoly)
            {
                var index = _zeroforceppoly.IndexOf(force);
                var poly = new Poly();
                var integration = force.Integrate();
                var momentsbefore = findmomentbefore(index);
                var zerovalue = force.Integrate().Calculate(force.StartPoint);
                var constant = momentsbefore - zerovalue;

                MesnetMDDebug.WriteInformation(Name + " : integration = " + integration.ToString() + " momentsbefore = " + momentsbefore + " zeroforcevalue = " + zerovalue + " startpoint = " + force.StartPoint + " endpoint = " + force.EndPoint);

                if (constant != 0)
                {
                    poly = integration + new Poly(constant.ToString());
                }
                else
                {
                    poly = integration;
                }

                poly.StartPoint = force.StartPoint;
                poly.EndPoint = force.EndPoint;
                _zeromomentppoly.Add(poly);
                _zeromomentppoly.Sort();
            }

            Global.WritePPolytoConsole(Name + " : Zero Moment", _zeromomentppoly);
        }

        /// <summary>
        /// Calculates weights forces before the shearForce poly whose index is given.
        /// </summary>
        /// <param name="index">The load polynomial index.</param>
        /// <returns></returns>
        private double findforcebefore(int index)
        {
            double weights = _leftsupportforcedist;

            int indx = 0;

            while (indx < index)
            {
                var weight = _distributedloads[indx].DefiniteIntegral(_distributedloads[indx].StartPoint, _distributedloads[indx].EndPoint);
                weights -= weight;

                indx++;
            }

            return weights;
        }

        /// <summary>
        /// Calculates moments before the moment poly whose index is given.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private double findmomentbefore(int index)
        {
            double moments = 0;
            int indx = 0;

            while (indx < index)
            {
                var area = _zeroforceppoly[indx].DefiniteIntegral(_zeroforceppoly[indx].StartPoint, _zeroforceppoly[indx].EndPoint);
                moments += area;
                indx++;
            }

            return moments;
        }

        #endregion

        #region clapeyron

        /// <summary>
        /// Finds end moments in case of both end have fixed support.
        /// </summary>
        private void ffsolver()
        {
            if (_zeromomentppoly.Count > 0)
            {
                double ma1 = 0;
                double ma2 = 0;
                double mb1 = 0;
                double mb2 = 0;
                double r1 = 0;
                double r2 = 0;

                ///////////////////////////////////////////////////////////
                /////////////////Left Equation Solve///////////////////////
                //////////////////////////////////////////////////////////

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    //When the inertia distribution is constant dont waste time and cpu with simpson numerical integration, integrate it analytically.
                    //Since izero equals inertia the expression can be simplified
                    MesnetMDDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var moxp = _zeromomentppoly.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MesnetMDDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    var mox = _zeromomentppoly * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MesnetMDDebug.WriteInformation(Name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    var conjugatemoment = _zeromomentppoly.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / System.Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MesnetMDDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MesnetMDDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson6.AddData(_zeromomentppoly.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MesnetMDDebug.WriteInformation(Name + " : r2 = " + r2);
                }

                double[,] coefficients =
                {
                    {ma1, mb1},
                    {ma2, mb2},
                };

                double[] results =
                {
                    r1, r2
                };

                //////////////////////////////////////////////////////////

                var moments = MesnetMD.Classes.Math.Algebra.LinearEquationSolver(coefficients, results);

                //_ma = Math.Round(moments[0], 4);

                //_mb = Math.Round(moments[1], 4);

                _ma = moments[0];

                _mb = moments[1];
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetMDDebug.WriteInformation(Name + " : ma = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have fixed support when there is only one beam.
        /// </summary>
        private void ffsolverclapeyron()
        {
            if (_zeromomentppoly.Count > 0)
            {
                double ma1 = 0;
                double ma2 = 0;
                double mb1 = 0;
                double mb2 = 0;
                double r1 = 0;
                double r2 = 0;

                ///////////////////////////////////////////////////////////
                /////////////////Left Equation Solve///////////////////////
                //////////////////////////////////////////////////////////

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    //When the inertia distribution is constant dont waste time and cpu with simpson numerical integration, integrate it analytically.
                    //Since izero equals inertia the expression can be simplified
                    MesnetMDDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var moxp = _zeromomentppoly.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MesnetMDDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    var mox = _zeromomentppoly * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MesnetMDDebug.WriteInformation(Name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    var conjugatemoment = _zeromomentppoly.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / System.Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MesnetMDDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MesnetMDDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson6.AddData(_zeromomentppoly.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MesnetMDDebug.WriteInformation(Name + " : r2 = " + r2);
                }

                double[,] coefficients =
                {
                    {ma1, mb1},
                    {ma2, mb2},
                };

                double[] results =
                {
                    r1, r2
                };

                //////////////////////////////////////////////////////////

                var moments = Algebra.LinearEquationSolver(coefficients, results);

                //_ma = Math.Round(moments[0], 4);

                //_mb = Math.Round(moments[1], 4);

                _ma = moments[0];

                _mb = moments[1];
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetMDDebug.WriteInformation(Name + " : ma = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the left end has fixed support and the right and basic or sliding support.
        /// </summary>
        private void fbsolver()
        {
            if (_zeromomentppoly.Count > 0)
            {
                double ma1;
                double r1;

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    MesnetMDDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    var moxp = _zeromomentppoly.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
                else
                {
                    MesnetMDDebug.WriteInformation(Name + " : Analytical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MesnetMDDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    var conjugatemoment = _zeromomentppoly.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;
                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }
            MesnetMDDebug.WriteInformation(Name + " : ma = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the right end has fixed support and the left and basic or sliding support.
        /// </summary>
        private void bfsolver()
        {
            if (_zeromomentppoly.Count > 0)
            {
                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                double mb1;
                double r1;

                if (_analyticalsolution)
                {
                    MesnetMDDebug.WriteInformation(Name + " : Analytical solution started");
                    mb1 = _length / 3;
                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var mox = _zeromomentppoly * xppoly;
                    r1 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
                else
                {
                    MesnetMDDebug.WriteInformation(Name + " : Numerical solution started");
                    var simpson1 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson1.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    mb1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MesnetMDDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                    {
                        simpson3.AddData(_izero / _inertiappoly.Calculate(i) * _zeromomentppoly.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetMDDebug.WriteInformation(Name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetMDDebug.WriteInformation(Name + " : ma = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have basic or sliding support.
        /// </summary>
        private void bbsolver()
        {
            _mb = 0;
            _ma = 0;             

            MesnetMDDebug.WriteInformation(Name + " : ma = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " : mb = " + _mb);
        }          
                                              
        /// <summary>
        /// Finds end moments according to end moments that are found by solvers.
        /// </summary>
        private void findfixedendmomentcross()
        {
            var polylist = new List<Poly>();

            var constant = (_mb - _ma) / _length;

            if (System.Math.Abs(constant) < 0.00000001)
            {
                constant = 0.0;
            }

            foreach (Poly moment in _zeromomentppoly)
            {
                var poly = new Poly();
                var poly1 = new Poly(_ma.ToString(), moment.StartPoint, moment.EndPoint);
                var poly2 = new Poly("x", moment.StartPoint, moment.EndPoint);
                if (!constant.Equals(0.0))
                {
                    var poly3 = new Poly(constant.ToString(), moment.StartPoint, moment.EndPoint);
                    poly = moment - poly1 - poly2 * poly3;
                }
                else
                {
                    poly = moment - poly1;
                }
                poly.StartPoint = moment.StartPoint;
                poly.EndPoint = moment.EndPoint;
                polylist.Add(poly);
            }
            _fixedendmomentppoly = new PiecewisePoly(polylist);

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmomentppoly);
        }

        private void findfixedendmomentclapeyron()
        {
            var polylist = new List<Poly>();

            var constant = (_mb - _ma) / _length;

            if (System.Math.Abs(constant) < 0.00000001)
            {
                constant = 0.0;
            }

            foreach (Poly moment in _zeromomentppoly)
            {
                var resultpoly = new Poly();
                resultpoly.StartPoint = moment.StartPoint;
                resultpoly.EndPoint = moment.EndPoint;
                var mapoly = new Poly(_ma.ToString(), moment.StartPoint, moment.EndPoint);
                var xpoly = new Poly("x", moment.StartPoint, moment.EndPoint);

                if (!constant.Equals(0.0))
                {
                    var cpoly = new Poly(constant.ToString(), moment.StartPoint, moment.EndPoint);
                    resultpoly = moment + mapoly + xpoly * cpoly;
                }
                else
                {
                    resultpoly = moment + mapoly;
                }
                resultpoly.StartPoint = moment.StartPoint;
                resultpoly.EndPoint = moment.EndPoint;
                polylist.Add(resultpoly);
            }
            _fixedendmomentppoly = new PiecewisePoly(polylist);

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmomentppoly);
        }

        /// <summary>
        /// Calculates the deflection of the beam on selected point. The deflection toward beam's red arrow direction is accepted as positive.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public double Deflection(double x)
        {
            var simpson1 = new SimpsonsFirstIntegrator(0.0001);

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                var mom = _fixedendmomentppoly.Calculate(i);
                var iner = _inertiappoly.Calculate(i);

                simpson1.AddData(mom * (_length - i) / iner);
            }

            simpson1.Calculate();

            var int1 = simpson1.Result;

            var simpson2 = new SimpsonsFirstIntegrator(0.0001);

            for (double i = 0; i <= x; i = i + 0.0001)
            {
                var mom = _fixedendmomentppoly.Calculate(i);
                var iner = _inertiappoly.Calculate(i);

                simpson2.AddData(mom * (x - i) / iner);
            }

            simpson2.Calculate();

            var int2 = simpson2.Result;

            double deflection = x / _length * int1 - int2;

            return deflection;
        }

        public void ClapeyronCalculate()
        {
            MesnetMDDebug.WriteInformation(Name + " : ClapeyronCalculate has started to work");
            findconcentratedsupportforces();
            finddistributedsupportforces();
            findconcentratedzeroforce();
            finddistributedzeroforce();
            _zeroforceppoly = _zeroforceconcpploy + _zeroforcedistppoly;
            Global.WritePPolytoConsole(Name + " : Zero ShearForce", _zeroforceppoly);
            findzeromoment();
            canbesolvedanalytically();
            clapeyronsupportcase();
            findfixedendmomentclapeyron();

            MesnetMDDebug.WriteInformation(Name + " Left End Moment = " + _ma);
            MesnetMDDebug.WriteInformation(Name + " Right End Moment = " + _mb);
        }

        /// <summary>
        /// Calculates moments when there is only one beam presented.
        /// </summary>
        private void clapeyronsupportcase()
        {
            #region cross support cases

            switch (LeftSide.Type)
            {
                case Global.ObjectType.LeftFixedSupport:

                    switch (RightSide.Type)
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MesnetMDDebug.WriteInformation(Name + " : ffsolver has been executed");
                            ffsolverclapeyron();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MesnetMDDebug.WriteInformation(Name + " : fbsolver has been executed");
                            fbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MesnetMDDebug.WriteInformation(Name + " : fbsolver has been executed");
                            fbsolver();

                            break;
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    switch (RightSide.Type)
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;
                    }

                    break;

                case Global.ObjectType.SlidingSupport:

                    switch (RightSide.Type)
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MesnetMDDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;
                    }

                    break;
            }

            #endregion
        }
        
        /// <summary>
        /// Checks if the beam can be calculated analytically, without numerical integration.
        /// If the beam has constant and only one inertia polynomial, and if the beam has 
        /// integer-powered zero moment poly, the analytical solution can be done which is a way
        /// faster than numerical solution 
        /// </summary>
        private void canbesolvedanalytically()
        {
            //Check inertia ppoly has only one poly
            if (_inertiappoly.Count > 1)
            {
                _analyticalsolution = false;
                MesnetMDDebug.WriteInformation(Name + " : Analytical solution is not possible");
                return;
            }

            //Check if inertia ppoly is constant or not dependant on x
            if (_inertiappoly.Degree() > 0)
            {
                _analyticalsolution = false;
                MesnetMDDebug.WriteInformation(Name + " : Analytical solution is not possible");
                return;
            }

            //Check if zero moment ppoly has any term with non-integer power
            if (_zeromomentppoly.Count > 0)
            {
                foreach (Poly poly in _zeromomentppoly)
                {
                    foreach (Term term in poly.Terms)
                    {
                        if (term.Power % 1 != 0)
                        {
                            _analyticalsolution = false;
                            MesnetMDDebug.WriteInformation(Name + " : Analytical solution is not possible");
                            return;
                        }
                    }
                }
            }
            _analyticalsolution = true;
            MesnetMDDebug.WriteInformation(Name + " : Analytical solution is possible");
        }

        private void createbasestiffnescoefficients()
        {
            MesnetMDDebug.WriteInformation(Name + " : Base Stiffness Coefficients:");
            Logger.WriteLine(Name + " : Base Stiffness Coefficients");
            if (_inertiappoly.IsConstant())
            {
                _mii = 4;
                _mjj = 4;
                _mij = 2;
            }
            else
            {
                var x = new Poly("x");
                var xsquare = new Poly("x^2");

                var simpson1 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                {
                    simpson1.AddData(xsquare.Calculate(i) / _inertiappoly.Calculate(i));
                }
                simpson1.Calculate();
                double i1 = simpson1.Result;

                var simpson2 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                {
                    simpson2.AddData(1 / _inertiappoly.Calculate(i));
                }
                simpson2.Calculate();
                double i2 = simpson2.Result;

                var simpson3 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                {
                    simpson3.AddData(x.Calculate(i) / _inertiappoly.Calculate(i));
                }
                simpson3.Calculate();
                double i3 = simpson3.Result;

                double det = -_izero / _length * (i1 * i2 - System.Math.Pow(i3, 2));

                _mii = -i1 / det;

                _mjj = (-System.Math.Pow(_length, 2) * i2 + 2 * _length * i3 - i1) / det;

                _mij = -(_length * i3 - i1) / det;
            }

            MesnetMDDebug.WriteInformation(Name + $" : Mii= {_mii}");
            Logger.WriteLine("Mii= " + _mii);

            MesnetMDDebug.WriteInformation(Name + $" : Mjj= {_mjj}");
            Logger.WriteLine("Mjj= " + _mjj);

            MesnetMDDebug.WriteInformation(Name + $" : Mij= {_mij}");
            Logger.WriteLine("Mij= " + _mij);

            if (_areappoly.IsConstant())
            {
                _nii = 1;
            }
            else
            {
                var simpson4 = new SimpsonsFirstIntegrator(Config.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Config.SimpsonStep)
                {
                    simpson4.AddData(1 / _areappoly.Calculate(i));
                }
                simpson4.Calculate();
                double i4 = simpson4.Result;
                _nii = _length / (_azero * i4);
            }
            MesnetMDDebug.WriteInformation(Name + $" : Nii= {_nii}");
            Logger.WriteLine("Nii= " + _nii);
        }

        private void createstiffnessmatrix()
        {
            var E = _elasticity;
            var I = _izero * System.Math.Pow(10, -8); //Conversion from cm^4 to m^4
            var A = _azero * System.Math.Pow(10, -4); //Conversion from cm^2 to m^2
            var L = _length;
            var L2 = System.Math.Pow(_length, 2);
            var L3 = System.Math.Pow(_length, 3);
            var nii = _nii;
            var mii = _mii;
            var mjj = _mjj;
            var mij = _mij;

            _stiffnessmatrix = new double[6, 6];
            _stiffnessmatrix[0, 0] = nii * E * A / L;
            _stiffnessmatrix[0, 1] = 0;
            _stiffnessmatrix[0, 2] = 0;
            _stiffnessmatrix[0, 3] = -nii * E * A / L;
            _stiffnessmatrix[0, 4] = 0;
            _stiffnessmatrix[0, 5] = 0;
            _stiffnessmatrix[1, 0] = 0;
            _stiffnessmatrix[1, 1] = (mii + mjj + 2 * mij) * E * I / L3;
            _stiffnessmatrix[1, 2] = (mii + mij) * E * I / L2;
            _stiffnessmatrix[1, 3] = 0;
            _stiffnessmatrix[1, 4] = -(mii + mjj + 2 * mij) * E * I / L3;
            _stiffnessmatrix[1, 5] = (mjj + mij) * E * I / L2;
            _stiffnessmatrix[2, 0] = 0;
            _stiffnessmatrix[2, 1] = (mii + mij) * E * I / L2;
            _stiffnessmatrix[2, 2] = mii * E * I / L;
            _stiffnessmatrix[2, 3] = 0;
            _stiffnessmatrix[2, 4] = -(mii + mij) * E * I / L2;
            _stiffnessmatrix[2, 5] = mij * E * I / L;
            _stiffnessmatrix[3, 0] = -nii * E * A / L;
            _stiffnessmatrix[3, 1] = 0;
            _stiffnessmatrix[3, 2] = 0;
            _stiffnessmatrix[3, 3] = nii * E * A / L;
            _stiffnessmatrix[3, 4] = 0;
            _stiffnessmatrix[3, 5] = 0;
            _stiffnessmatrix[4, 0] = 0;
            _stiffnessmatrix[4, 1] = -(mii + mjj + 2 * mij) * E * I / L3;
            _stiffnessmatrix[4, 2] = -(mii + mij) * E * I / L2;
            _stiffnessmatrix[4, 3] = 0;
            _stiffnessmatrix[4, 4] = (mii + mjj + 2 * mij) * E * I / L3;
            _stiffnessmatrix[4, 5] = -(mjj + mij) * E * I / L2;
            _stiffnessmatrix[5, 0] = 0;
            _stiffnessmatrix[5, 1] = (mjj + mij) * E * I / L2;
            _stiffnessmatrix[5, 2] = mij * E * I / L;
            _stiffnessmatrix[5, 3] = 0;
            _stiffnessmatrix[5, 4] = -(mjj + mij) * E * I / L2;
            _stiffnessmatrix[5, 5] = mjj * E * I / L;

            MesnetMDDebug.WriteInformation(Name + " : Raw Stiffness Matrix:");
            Logger.WriteLine(Name + " : Raw Stiffness Matrix:");

            var str = String.Empty;

            for (int i = 0; i < _stiffnessmatrix.GetLength(0); i++)
            {
                str = String.Empty;
                for (int j = 0; j < _stiffnessmatrix.GetLength(1); j++)
                {
                    str += _stiffnessmatrix[i, j].ToString("F15");
                    if (j != _stiffnessmatrix.GetLength(1) - 1)
                    {
                        str += " ";
                    }
                }
                MesnetMDDebug.WriteInformation(Name + " : " + str);
                Logger.WriteLine(str);
            }

            //kXYZ=T^T.kxyz.T => Matrix transformation

            var T = _transformationmatrix;
            var TT = Algebra.Transpose(T);

            var m1 = Algebra.MultiplyMatrix(TT, _stiffnessmatrix);
            var m2 = Algebra.MultiplyMatrix(m1, T);
            _stiffnessmatrix = m2;

            MesnetMDDebug.WriteInformation(Name + " : Transformed Stiffness Matrix:");
            Logger.WriteLine(Name + " : Transformed Stiffness Matrix:");

            var str2 = String.Empty;

            for (int i = 0; i < _stiffnessmatrix.GetLength(0); i++)
            {
                str2 = String.Empty;
                for (int j = 0; j < _stiffnessmatrix.GetLength(1); j++)
                {
                    str2 += _stiffnessmatrix[i, j].ToString("F15");
                    if (j != _stiffnessmatrix.GetLength(1) - 1)
                    {
                        str2 += " ";
                    }
                }
                MesnetMDDebug.WriteInformation(Name + " : " + str2);
                Logger.WriteLine(str2);
            }
        }

        private void createforcevector()
        {
            createindirectforcevector();

            createdirectforcevector();

            MesnetMDDebug.WriteInformation(Name + " : ShearForce Vector:");

            Logger.WriteLine(Name + " : ShearForce Vector:");

            _forcevector = new double[6];

            for (int i = 0; i < _forcevector.GetLength(0); i++)
            {
                _forcevector[i] = _dforcevector[i] -_idforcevector[i];
                MesnetMDDebug.WriteInformation(Name + " : " + _forcevector[i].ToString("F15"));
                Logger.WriteLine(_forcevector[i].ToString("F15"));
            }
        }

        private void createindirectforcevector()
        {
            _idforcevector = new double[6];
            //Converting kN to N by multiplying with 1000
            _idforcevector[0] = -_fixedendforceppoly.Calculate(0) * Math.Algebra.SinD(_angle) * 1000;
            _idforcevector[1] = _fixedendforceppoly.Calculate(0) * Math.Algebra.CosD(_angle) * 1000;
            _idforcevector[2] = -_fixedendmomentppoly.Calculate(0) * 1000;
            _idforcevector[3] = _fixedendforceppoly.Calculate(_length) * Math.Algebra.SinD(_angle) * 1000;
            _idforcevector[4] = -_fixedendforceppoly.Calculate(_length) * Math.Algebra.CosD(_angle) * 1000;
            _idforcevector[5] = _fixedendmomentppoly.Calculate(_length) * 1000;
             
            MesnetMDDebug.WriteInformation(Name + " : Indirect ShearForce Vector:");

            Logger.WriteLine(Name + " : Indirect ShearForce Vector:");

            for (int i = 0; i < _idforcevector.GetLength(0); i++)
            {
                MesnetMDDebug.WriteInformation(Name + " : " + _idforcevector[i].ToString("F15"));
                Logger.WriteLine(_idforcevector[i].ToString("F15"));
            }
        }
        
        private void createdirectforcevector()
        {
            //Todo: Also include direct shearForce and moments

            _dforcevector = new double[6];

            _dforcevector[0] = 0;
            _dforcevector[1] = 0;
            _dforcevector[2] = 0;
            _dforcevector[3] = 0;
            _dforcevector[4] = 0;
            _dforcevector[5] = 0;

            MesnetMDDebug.WriteInformation(Name + " : Direct ShearForce Vector:");

            Logger.WriteLine(Name + " : Direct ShearForce Vector:");

            for (int i = 0; i < _dforcevector.GetLength(0); i++)
            {
                MesnetMDDebug.WriteInformation(Name + " : " + _dforcevector[i].ToString("F15"));
                Logger.WriteLine(_dforcevector[i].ToString("F15"));
            }
        }

        private void createtransformationmatrix()
        {
            _transformationmatrix = new double[6,6];

            _transformationmatrix[0, 0] = Algebra.CosD(_angle);
            _transformationmatrix[0, 1] = Algebra.SinD(_angle);
            _transformationmatrix[0, 2] = 0;
            _transformationmatrix[0, 3] = 0;
            _transformationmatrix[0, 4] = 0;
            _transformationmatrix[0, 5] = 0;
            _transformationmatrix[1, 0] = -Algebra.SinD(_angle);
            _transformationmatrix[1, 1] = Algebra.CosD(_angle);
            _transformationmatrix[1, 2] = 0;
            _transformationmatrix[1, 3] = 0;
            _transformationmatrix[1, 4] = 0;
            _transformationmatrix[1, 5] = 0;
            _transformationmatrix[2, 0] = 0;
            _transformationmatrix[2, 1] = 0;
            _transformationmatrix[2, 2] = 1;
            _transformationmatrix[2, 3] = 0;
            _transformationmatrix[2, 4] = 0;
            _transformationmatrix[2, 5] = 0;
            _transformationmatrix[3, 0] = 0;
            _transformationmatrix[3, 1] = 0;
            _transformationmatrix[3, 2] = 0;
            _transformationmatrix[3, 3] = Algebra.CosD(_angle);
            _transformationmatrix[3, 4] = Algebra.SinD(_angle);
            _transformationmatrix[3, 5] = 0;
            _transformationmatrix[4, 0] = 0;
            _transformationmatrix[4, 1] = 0;
            _transformationmatrix[4, 2] = 0;
            _transformationmatrix[4, 3] = -Algebra.SinD(_angle);
            _transformationmatrix[4, 4] = Algebra.CosD(_angle);
            _transformationmatrix[4, 5] = 0;
            _transformationmatrix[5, 0] = 0;
            _transformationmatrix[5, 1] = 0;
            _transformationmatrix[5, 2] = 0;
            _transformationmatrix[5, 3] = 0;
            _transformationmatrix[5, 4] = 0;
            _transformationmatrix[5, 5] = 1;

            MesnetMDDebug.WriteInformation(Name + " : Transformation Matrix:");

            Logger.WriteLine(Name + " : Transformation Matrix:");

            var str = String.Empty;

            for (int i = 0; i < _transformationmatrix.GetLength(0); i++)
            {
                str = String.Empty;
                for (int j = 0; j < _transformationmatrix.GetLength(1); j++)
                {
                    str += _transformationmatrix[i, j].ToString("F15");
                    if (j != _transformationmatrix.GetLength(1) - 1)
                    {
                        str += " ";
                    }
                }
                MesnetMDDebug.WriteInformation(Name + " : " + str);
                Logger.WriteLine(str);
            }
        }

        private void obtainlocalforces()
        {
            _localforcevector = Algebra.DotProduct(_transformationmatrix, _forcevector);

            MesnetMDDebug.WriteInformation(Name + " : Local ShearForce Vector:");

            Logger.WriteLine(Name + " : Local ShearForce Vector:");

            for (int i = 0; i < _localforcevector.GetLength(0); i++)
            {
                MesnetMDDebug.WriteInformation(Name + " : " + _localforcevector[i].ToString("F15"));
                Logger.WriteLine(_localforcevector[i].ToString("F15"));
            }         
        }

        public void UpdateDirectForceVector(double[] diplacement)
        {
            var cross = Algebra.DotProduct(_stiffnessmatrix, diplacement);
            var result = Algebra.AddVectors(cross, _idforcevector);
            _forcevector = result;

            MesnetMDDebug.WriteInformation(Name + " : Updated ShearForce Vector:");

            Logger.WriteLine(Name + " : Updated ShearForce Vector:");

            for (int i = 0; i < _forcevector.GetLength(0); i++)
            {
                MesnetMDDebug.WriteInformation(Name + " : " + _forcevector[i].ToString("F15"));
                Logger.WriteLine(_forcevector[i].ToString("F15"));
            }
        }
        #endregion

        #region cross

        /// <summary>
        /// Chooses the solver to be executed according to supports in the way of Cross Method.
        /// </summary>
        private void mdsupportcases()
        {
            //In matrix displacement method, the shearForce vector is calculated when all of the displacement are zero (both side is fixed support)
            ffsolver();
        }

        /// <summary>
        /// Main function that prepares parameters and conducts solution for the beam.
        /// </summary>
        public void Calculate()
        {
            MesnetMDDebug.WriteInformation(Name + " : Calculate has started to work");

            //First, find reaction forces for concentrated loads
            findconcentratedsupportforces();

            //Then, find reaction forces for distributed loads
            finddistributedsupportforces();

            //Then, find shearForce distribution for concentrated loads in zero case (when both side of the beam bound with free supports
            findconcentratedzeroforce();

            //Then, find shearForce distribution for distributed loads in zero case
            finddistributedzeroforce();

            //Superposition to find resultant zero shearForce distribution
            _zeroforceppoly = _zeroforceconcpploy + _zeroforcedistppoly;

            Global.WritePPolytoConsole(Name + " : _zeroforce", _zeroforceppoly);

            //Then, find zero memont distribution according to resultant zero shearForce distribution
            findzeromoment();

            //Check if analytical solution is possible
            canbesolvedanalytically();

            //Then, find end moments for each beam based on cross support cases (assumes supports connecting 
            //more than on beams as fixed support
            mdsupportcases();

            //Then, find fixed end moments according to calculated end moments
            findfixedendmomentclapeyron();

            MesnetMDDebug.WriteInformation(Name + " : Ma = " + _ma);

            MesnetMDDebug.WriteInformation(Name + " : Mb = " + _mb);          

            //Unlike cross methos, we need to have fixedendforce before the solution so that the shearForce vector can be calculated.
            updateforces();

            //Create base stiffness coefficients
            createbasestiffnescoefficients();

            //Create transformation matrix
            createtransformationmatrix();

            //Create element stiffness matrix
            createstiffnessmatrix();

            //Create element shearForce vector
            createforcevector();
         
            MesnetMDDebug.WriteInformation(Name + " : Calculate has finished to work");
        }

        public override void ResetSolution()
        {
            _ma = 0;
            _mb = 0;
            _fixedendforceppoly = null;
            _fixedendmomentppoly = null;
            _zeroforceppoly = null;
            _zeroforceconcpploy = null;
            _zeroforcedistppoly = null;
            _zeromomentppoly = null;
            _maxmoment = Double.MinValue;
            _maxforce = Double.MinValue;
            _maxaxialforce = Double.MinValue;
            _maxstress = Double.MinValue;
            _maxabsstress = Double.MinValue;
            DestroyFixedEndMomentDiagram();
            DestroyFixedEndForceDiagram();
            DestroyAxialForceDiagram();
            DestroyStressDiagram();
        }

        #endregion

        #region post-cross

        /// <summary>
        /// Calculates final moment, shearForce and stress distributions after Cross loop according to the results
        /// </summary>
        public void PostProcessUpdate()
        {
            obtainlocalforces();

            updatemoments();

            updateforces();

            updateaxialforces();

            if (_stressanalysis)
            {
                updatestresses();
            }

            //updatedeflection();
        }

        public void PostClapeyronUpdate()
        {
            _maxmoment = _fixedendmomentppoly.Max;

            _maxabsmoment = _fixedendmomentppoly.MaxAbs;

            _minmoment = _fixedendmomentppoly.Min;

            _fixedendforceppoly = _fixedendmomentppoly.Derivate();

            Global.WritePPolytoConsole(Name + " : Fixed End ShearForce", _fixedendforceppoly);

            _maxforce = _fixedendforceppoly.Max;

            _maxabsforce = _fixedendforceppoly.MaxAbs;

            _minforce = _fixedendforceppoly.Min;

            if (_stressanalysis)
            {
                updatestresses();
            }
            //updatedeflection();
        }

        /// <summary>
        /// Updates the fixed end moment after cross loop.
        /// </summary>
        private void updatemoments()
        {
            double constant = 0;

            if (_zeromomentppoly.Length > 0)
            {                
                var ma = -_localforcevector[2] / 1000;

                var mb = _localforcevector[5] / 1000;

                constant = (mb - ma) / _length;
                var terms = new TermCollection();
                if (constant != 0)
                {
                    if (ma != 0)
                    {
                        var term1 = new Term(1, constant);
                        var term2 = new Term(0, ma);
                        terms.Add(term1);
                        terms.Add(term2);
                    }
                    else
                    {
                        var term1 = new Term(1, constant);
                        terms.Add(term1);
                    }
                }
                else
                {
                    if (ma != 0)
                    {
                        var term2 = new Term(0, ma);
                        terms.Add(term2);
                    }
                    else
                    {
                        //nothing to do
                    }
                }

                if (terms.Length > 0)
                {
                    var mdpoly = new Poly(terms, 0, _length);
                    var polies = new List<Poly>();
                    polies.Add(mdpoly);
                    var mdpolies = new PiecewisePoly(polies);

                    _fixedendmomentppoly = _zeromomentppoly + mdpolies;
                }
                else
                {
                    _fixedendmomentppoly = _zeromomentppoly;
                }
            }
            else
            {
                //There is no load on this beam
                var ma = -_localforcevector[2] / 1000;

                var mb = _localforcevector[5] / 1000;

                constant = (mb - ma) / _length;
                var terms = new TermCollection();
                if (constant != 0)
                {
                    if (ma != 0)
                    {
                        var term1 = new Term(1, constant);
                        var term2 = new Term(0, ma);
                        terms.Add(term1);
                        terms.Add(term2);
                    }
                    else
                    {
                        var term1 = new Term(1, constant);
                        terms.Add(term1);
                    }
                }
                else
                {
                    if (ma != 0)
                    {
                        var term2 = new Term(0, ma);
                        terms.Add(term2);
                    }
                    else
                    {
                        //nothing to do
                    }
                }

                if (terms.Length > 0)
                {
                    var mdpoly = new Poly(terms, 0, _length);
                    var polies = new List<Poly>();
                    polies.Add(mdpoly);
                    var mdpolies = new PiecewisePoly(polies);

                    _fixedendmomentppoly = mdpolies;
                }
                else
                {
                    var termzero = new Term(0, 0);
                    var termszero = new TermCollection();
                    termszero.Add(termzero);
                    var polyzero = new Poly(termszero);
                    var zeropolies = new List<Poly>();
                    zeropolies.Add(polyzero);
                    var zeroppoly = new PiecewisePoly(zeropolies);
                    _fixedendmomentppoly = zeroppoly;
                }
            }

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmomentppoly);

            _maxmoment = _fixedendmomentppoly.Max;

            _maxabsmoment = _fixedendmomentppoly.MaxAbs;

            _minmoment = _fixedendmomentppoly.Min;
        }

        /// <summary>
        /// Calculates final shearForce distribution after cross loop.
        /// </summary>
        private void updateforces()
        {
            _fixedendforceppoly = _fixedendmomentppoly.Derivate();

            Global.WritePPolytoConsole(Name + " : Fixed End ShearForce", _fixedendforceppoly);

            _maxforce = _fixedendforceppoly.Max;

            _maxabsforce = _fixedendforceppoly.MaxAbs;

            _minforce = _fixedendforceppoly.Min;
        }

        private void updateaxialforces()
        {
            var terms = new TermCollection();

            var ma = -_localforcevector[0] / 1000;

            var mb = _localforcevector[3] / 1000;

            double constant = (mb - ma) / _length;

            if (System.Math.Abs(constant) > System.Math.Pow(10, -8))
            {
                if (System.Math.Abs(ma) > System.Math.Pow(10, -8))
                {
                    var term1 = new Term(1, constant);
                    var term2 = new Term(0, ma);
                    terms.Add(term1);
                    terms.Add(term2);
                }
                else
                {
                    var term1 = new Term(1, constant);
                    terms.Add(term1);
                }
            }
            else
            {
                if (System.Math.Abs(ma) > System.Math.Pow(10, -8))
                {
                    var term1 = new Term(0, ma);
                    terms.Add(term1);
                }
                else
                {
                    var term1 = new Term(0, 0);
                    terms.Add(term1);
                }
            }

            var mdpoly = new Poly(terms, 0, _length);
            _axialforceppoly = new PiecewisePoly(mdpoly);

            _maxaxialforce = _axialforceppoly.Max;

            _maxabsaxialforce = _axialforceppoly.MaxAbs;

            _minaxialforce = _axialforceppoly.Min;
        }

        /// <summary>
        /// Calculates stress distribution after cross loop.
        /// </summary>
        private void updatestresses()
        {
            double precision = 0.001;
            _stress = new KeyValueCollection();
            double stress = 0;
            double y = 0;
            double e = 0;
            double d = 0;

            for (int i = 0; i < _length / precision; i++)
            {
                e = _eppoly.Calculate(i * precision);
                d = _dppoly.Calculate(i * precision);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = System.Math.Pow(10, 3) * _fixedendmomentppoly.Calculate(i * precision) * y / (_inertiappoly.Calculate(i * precision));
                _stress.Add(i * precision, stress);
            }

            if (!_stress.ContainsKey(_length))
            {
                e = _eppoly.Calculate(_length);
                d = _dppoly.Calculate(_length);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = System.Math.Pow(10, 3) * _fixedendmomentppoly.Calculate(_length) * y / (_inertiappoly.Calculate(_length));
                _stress.Add(_length, stress);
            }

            _maxstress = _stress.YMax;
            _maxabsstress = _stress.YMaxAbs;
        }

        private void updatedeflection()
        {
            double precision = 0.001;
            var function = new List<Global.Func>();

            Global.Func value;
            value.id = 0;
            value.xposition = 0;
            value.yposition = 0;

            int id = 0;
            for (int i = 0; i < _length / precision; i++)
            {
                value.id = id++;
                value.xposition = i * precision;
                value.yposition = -_fixedendmomentppoly.Calculate(i * precision) / (_elasticity * _inertiappoly.Calculate(i * precision));
                function.Add(value);
            }

            var angle = TrapezeIntegrator.Integrate(function, precision);

            _deflection = TrapezeIntegrator.Integrate(angle, precision);

            _maxdeflection = _deflection.MaxBy(x => x.yposition);

            if (_maxdeflection.yposition > Global.MaxDeflection)
            {
                Global.MaxDeflection = _maxdeflection.yposition;
            }
        }

        public void PostMomentUpdate()
        {
            

        }

        #endregion

        #region ReDraw Functions

        public void ReDrawMoment(int c)
        {
            if (_femomentdiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing moment for c = " + c);
                _femomentdiagram.Draw(c);
            }
            else if (_fixedendmomentppoly?.Count > 0)
            {
                ShowFixedEndMomentDiagram(c);
            }
        }

        public void ReDrawDistLoad(int c)
        {
            if (_distloaddiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing distributed load for c = " + c);
                _distloaddiagram.Draw(c);
            }
            else if (_distributedloads?.Count > 0)
            {
                ShowDistLoadDiagram(c);
            }
        }

        public void ReDrawConcLoad(int c)
        {
            if (_concloaddiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing concentrated load for c = " + c);
                _concloaddiagram.Draw(c);
            }
            else if (_concentratedloads?.Count > 0)
            {
                ShowConcLoadDiagram(c);
            }
        }

        public void ReDrawInertia(int c)
        {
            if (_inertiadiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing inertia for c = " + c);
                _inertiadiagram.Draw(c);
            }
            else if (_inertiappoly?.Count > 0)
            {
                ShowInertiaDiagram(c);
            }
        }

        public void ReDrawArea(int c)
        {
            if (_areadiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing area for c = " + c);
                _areadiagram.Draw(c);
            }
            else if (_areappoly?.Count > 0)
            {
                ShowAreaDiagram(c);
            }
        }

        public void ReDrawForce(int c)
        {
            if (_feforcediagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing shearForce for c = " + c);
                _feforcediagram.Draw(c);
            }
            else if (_fixedendforceppoly?.Count > 0)
            {
                ShowFixedEndForceDiagram(c);
            }
        }

        public void ReDrawAxialForce(int c)
        {
            if (_axialforcediagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing axialForce for c = " + c);
                _axialforcediagram.Draw(c);
            }
            else if (_axialforceppoly?.Count > 0)
            {
                ShowAxialForceDiagram(c);
            }
        }

        public void ReDrawStress(int c)
        {
            if (_stressdiagram != null)
            {
                MesnetMDDebug.WriteInformation(Name + " : redrawing stress for c = " + c);
                _stressdiagram.Draw(c);
            }
            else if (_stress?.Count > 0)
            {
                ShowStressDiagram(c);
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Gets the stiffness matrix element. Assumes that matrix elements start with 1 instead of 0.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <returns></returns>
        public double GetStiffness(int row, int col)
        {
            return _stiffnessmatrix[row - 1, col - 1];
        }

        #region properties

        #region ui

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                Width = value * 100 + 14;
            }
        }

        public int BeamId
        {
            get { return _beamid; }
            set { _beamid = value; }
        }

        public double ElasticityModulus
        {
            get { return _elasticity; }
            set { _elasticity = value; }
        }

        public double LeftPos
        {
            get
            {
                _leftpos = Canvas.GetLeft(this);
                return _leftpos;
            }
            set
            {
                _leftpos = value;
                Canvas.SetLeft(this, _leftpos);
            }
        }

        public double TopPos
        {
            get
            {
                _toppos = Canvas.GetTop(this);
                return _toppos;
            }
            set
            {
                _toppos = value;
                Canvas.SetTop(this, _toppos);
            }
        }

        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
            set
            {
                var transform = value;
                _angle = transform.Angle;
                rotateTransform = transform;
            }
        }

        public TransformGeometry TGeometry
        {
            get { return _tgeometry; }
        }

        public double IZero
        {
            get { return _izero; }
            set { _izero = value; }
        }

        public PiecewisePoly Loads
        {
            get { return _loads; }
        }

        public KeyValueCollection ConcentratedLoads
        {
            get { return _concentratedloads; }
        }

        public PiecewisePoly DistributedLoads
        {
            get { return _distributedloads; }
        }

        public PiecewisePoly Inertia
        {
            get { return _inertiappoly; }
        }

        public PiecewisePoly Area
        {
            get { return _areappoly; }
            set { _areappoly = value; }
        }

        public PiecewisePoly ZeroForce
        {
            get { return _zeroforceppoly; }
        }

        public PiecewisePoly ZeroMoment
        {
            get { return _zeromomentppoly; }
        }

        public PiecewisePoly FixedEndMoment
        {
            get { return _fixedendmomentppoly; }
        }

        public PiecewisePoly FixedEndForce
        {
            get { return _fixedendforceppoly; }
        }

        public PiecewisePoly AxialForce
        {
            get { return _axialforceppoly; }
        }

        public KeyValueCollection Stress
        {
            get { return _stress; }
        }

        public PiecewisePoly Eppoly
        {
            get { return _eppoly; }
            set { _eppoly = value; }
        }

        public PiecewisePoly Dppoly
        {
            get { return _dppoly; }
            set { _dppoly = value; }
        }

        public bool PerformStressAnalysis
        {
            get { return _stressanalysis; }
            set { _stressanalysis = value; }
        }

        public double Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        private string AName
        {
            get { return Name; }
        }

        public Point LeftPoint
        {
            get { return _tgeometry.LeftPoint; }
        }

        public Point RightPoint
        {
            get { return _tgeometry.RightPoint; }
        }

        public double LeftEndMoment
        {
            get { return _ma; }
            set { _ma = value; }
        }

        public double RightEndMoment
        {
            get { return _mb; }
            set { _mb = value; }
        }

        public bool IndexPassed
        {
            get { return _indexpassed; }
            set { _indexpassed = value; }
        }

        public bool IsBound
        {
            get { return _isbound; }
            set { _isbound = value; }
        }

        public Global.Func MaxDeflection
        {
            get { return _maxdeflection; }
        }

        public double MaxAllowableStress
        {
            get { return _maxallowablestress; }
            set { _maxallowablestress = value; }
        }

        public bool IsLeftSelected
        {
            get
            {
                return _leftcircleseleted;
            }
        }

        public bool IsRightSelected
        {
            get
            {
                return _rightcircleselected;
            }
        }

        public bool DirectionShown
        {
            get { return _directionshown; }
            set { _directionshown = value; }
        }

        #endregion

        #region Cross Properties

        public double CarryOverAB
        {
            get
            {
                return System.Math.Round(_gamaab, 4);
            }
        }

        public double CarryOverBA
        {
            get { return System.Math.Round(_gamaba, 4); }
        }

        private double _stiffnessa;

        private double _stiffnessb;

        private double _newstiffnessa;

        private double _newstiffnessb;

        public double StiffnessA
        {
            get { return _stiffnessa; }
        }

        public double StiffnessB
        {
            get { return _stiffnessb; }
        }

        public double MaxMoment
        {
            get { return _maxmoment; }
        }

        public double MaxAbsMoment
        {
            get { return _maxabsmoment; }
        }

        public double MinMoment
        {
            get { return _minmoment; }
        }

        public double MaxForce
        {
            get { return _maxforce; }
        }

        public double MaxAbsForce
        {
            get { return _maxabsforce; }
        }

        public double MinForce
        {
            get { return _minforce; }
        }

        public double MaxAxialForce
        {
            get { return _maxaxialforce; }
        }

        public double MaxAbsAxialForce
        {
            get { return _maxabsaxialforce; }
        }

        public double MinAxialForce
        {
            get { return _minaxialforce; }
        }

        public double MaxInertia
        {
            get { return _maxinertia; }
        }

        public double MaxArea
        {
            get { return _maxarea; }
        }

        public double MaxStress
        {
            get { return _maxstress; }
        }

        public double MaxAbsStress
        {
            get { return _maxabsstress; }
        }

        public double MaxConcLoad
        {
            get { return _maxconcload; }
        }

        public double MaxAbsConcLoad
        {
            get { return _maxabsconcload; }
        }

        public double MaxDistLoad
        {
            get { return _maxdistload; }
        }

        public double MaxAbsDistLoad
        {
            get { return _maxabsdistload; }
        }
        #endregion

        public double[,] StiffnessMatrix
        {
            get { return _stiffnessmatrix; }
        }

        public double[] IForceVector
        {
            get { return _idforcevector; }
        }

        public double[] ForceVector
        {
            get { return _forcevector; }
        }

        #endregion
    }
}
