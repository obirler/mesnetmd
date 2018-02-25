using System;
using System.Collections.Generic;
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
     
        private void InitializeComponent(double length=1.0)
        {
            _beamid = _beamcount++;
            Name = "Beam " + _beamid;
            Width = length * 100;
            Height = 0;

            _core = new Rectangle();
            _core.Height = 4;
            _core.Width = Width;
            _core.Fill = new SolidColorBrush(Colors.Black);
            Children.Add(_core);

            SetLeft(_core, 0);
            SetBottom(_core, -2);

            _startcircle = new Ellipse();
            _startcircle.Height = 14;
            _startcircle.Width = 14;
            _startcircle.Stroke = new SolidColorBrush(Colors.Green);
            _startcircle.StrokeThickness = 3;

            Children.Add(_startcircle);

            SetLeft(_startcircle, -7);
            SetBottom(_startcircle, -7);

            _startcircle.Visibility = Visibility.Collapsed;

            _endcircle = new Ellipse();
            _endcircle.Height = 14;
            _endcircle.Width = 14;
            _endcircle.Stroke = new SolidColorBrush(Colors.Green);
            _endcircle.StrokeThickness = 3;

            Children.Add(_endcircle);

            SetLeft(_endcircle, Width - 7);
            SetBottom(_endcircle, -7);
            _endcircle.Visibility = Visibility.Collapsed;

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

            _directionarrow = new Canvas();
            _directionarrow.Width = 50;
            _directionarrow.Height = 10;

            Children.Add(_directionarrow);
            Canvas.SetLeft(_directionarrow, Width / 2);
            Canvas.SetBottom(_directionarrow, 0);

            _directionarrow.Visibility = Visibility.Collapsed;

            rotateTransform = new RotateTransform();
        }

        public Beam(Canvas canvas, double length)
        {
            _canvas = canvas;

            InitializeComponent();

            InitializeVariables(length);

            AddTopLeft(_canvas, 10000, 10000);
        }

        private void InitializeVariables(double length)
        {
            _length = length;

            Width = length * 100;

            MyDebug.WriteInformation(Name + " has been created : length = " + _length + " m, Width = " + Width);

            RightSide = null;

            LeftSide = null;

            BindEvents();
        }

        public void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            _core.MouseDown += mw.BeamCoreMouseDown;
            _core.MouseUp += mw.BeamCoreMouseUp;
            _core.MouseMove += mw.BeamCoreMouseMove;
            _startcircle.MouseDown += mw.StartCircleMouseDown;
            _endcircle.MouseDown += mw.EndCircleMouseDown;
        }

        #region internal variables

        private static int _beamcount = 0;

        private int _beamid;

        private bool selected;

        private double _length;

        private double _izero;

        private double _elasticity;

        private bool _canbedragged = true;

        private double _leftpos;

        private double _toppos;

        public RotateTransform rotateTransform;

        private Ellipse _startcircle;

        private Ellipse _endcircle;

        private Rectangle _core;

        private Canvas _directionarrow;

        KeyValueCollection _concentratedloads;

        private PiecewisePoly _distributedloads;

        private PiecewisePoly _loads;

        private PiecewisePoly _inertiappoly;

        private PiecewisePoly _zeroforceconc;

        private PiecewisePoly _zeroforcedist;

        /// <summary>
        /// The force when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeroforce;

        /// <summary>
        /// The moment when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeromoment;

        private PiecewisePoly _fixedendmoment;

        private PiecewisePoly _fixedendforce;

        private KeyValueCollection _stress;

        private PiecewisePoly _e;

        private PiecewisePoly _d;

        public object LeftSide;

        public object RightSide;

        public double LeftDistributionFactor;

        public double RightDistributionFactor;

        public Global.Direction circledirection;

        private Canvas _canvas;

        private Point corepoint;

        private ConcentratedLoad _concload;

        private DistributedLoad _distload;

        private Force _force;

        private Moment _moment;

        private Force _feforce;

        private Moment _femoment;

        private Inertia _inertia;

        private Stress _stressdiagram;

        private bool _directionshown = false;

        /// <summary>
        /// The left support force of the beam.
        /// </summary>
        private double _leftsupportforcedist;

        private double _leftsupportforceconc;

        /// <summary>
        /// The right support force of the beam.
        /// </summary>
        private double _rightsupportforcedist;

        private double _rightsupportforceconc;

        /// <summary>
        /// The resultant force that is the sum of the all acting force on beam.
        /// </summary>
        private double _resultantforce;

        /// <summary>
        /// The acting point of the resultant force.
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

        private double _maxinertia;

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
                MyDebug.WriteWarning(Name + " : This beam has already been added to canvas!");
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
                MyDebug.WriteWarning(Name + " : This beam has already been added to canvas!");
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
                MyDebug.WriteWarning(Name + " This beam has already been added to canvas!");
            }
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

        /// <summary>
        /// Connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>        
        public void Connect(Global.Direction direction1, Beam oldbeam, Global.Direction direction2)
        {
            if (_isbound && oldbeam.IsBound)
            {
                throw new InvalidOperationException("Both beam has bound");
            }
            switch (direction1)
            {
                case Global.Direction.Left:

                    switch (direction2)
                    {
                        #region Left-Left

                        case Global.Direction.Left:

                            if (LeftSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftconnect(oldbeam);

                            break;

                        #endregion

                        #region Left-Right

                        case Global.Direction.Right:

                            if (LeftSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the right side of lodbeam.
                            leftrightconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;

                case Global.Direction.Right:

                    switch (direction2)
                    {
                        #region Right-Left

                        case Global.Direction.Left:

                            if (RightSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftconnect(oldbeam);

                            break;

                        #endregion

                        #region Right-Right

                        case Global.Direction.Right:

                            if (RightSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Right side of this beam will be connected to the right side of oldbeam.                             
                            rightrightconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;
            }

            _isbound = true;
            oldbeam.IsBound = true;
        }

        private void leftleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (oldbeam.LeftSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = oldbeam.LeftSide as SlidingSupport;
                            ss.AddBeam(this, Global.Direction.Left);

                            break;

                        case "BasicSupport":

                            var bs = oldbeam.LeftSide as BasicSupport;
                            bs.AddBeam(this, Global.Direction.Left);

                            break;

                        case "RightFixedSupport":

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (LeftSide != null)
            {
                if (LeftSide.GetType().Name != "LeftFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (LeftSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = LeftSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Global.Direction.Left);

                            break;

                        case "BasicSupport":

                            var bs = LeftSide as BasicSupport;
                            bs.AddBeam(oldbeam, Global.Direction.Left);

                            break;

                        case "RightFixedSupport":

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void leftrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                if (Global.GetObjectType(oldbeam.RightSide) != Global.ObjectType.RightFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        MyDebug.WriteInformation(Name + " : isbound : " + _isbound.ToString());
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (oldbeam.RightSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = oldbeam.RightSide as SlidingSupport;
                            ss.AddBeam(this, Global.Direction.Left);

                            break;

                        case "BasicSupport":

                            var bs = oldbeam.RightSide as BasicSupport;
                            bs.AddBeam(this, Global.Direction.Left);

                            break;

                        case "LeftFixedSupport":

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (LeftSide != null)
            {
                if (LeftSide.GetType().Name != "LeftFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (LeftSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = LeftSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Global.Direction.Right);

                            break;

                        case "BasicSupport":

                            var bs = LeftSide as BasicSupport;
                            bs.AddBeam(oldbeam, Global.Direction.Right);

                            break;

                        case "RightFixedSupport":

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (oldbeam.LeftSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = oldbeam.LeftSide as SlidingSupport;
                            ss.AddBeam(this, Global.Direction.Right);

                            break;

                        case "BasicSupport":

                            var bs = oldbeam.LeftSide as BasicSupport;
                            bs.AddBeam(this, Global.Direction.Right);

                            break;

                        case "RightFixedSupport":

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (RightSide != null)
            {
                if (RightSide.GetType().Name != "RightFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (RightSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = RightSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Global.Direction.Left);

                            break;

                        case "BasicSupport":

                            var bs = RightSide as BasicSupport;
                            bs.AddBeam(oldbeam, Global.Direction.Left);

                            break;

                        case "LeftFixedSupport":

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                if (oldbeam.RightSide.GetType().Name != "RightFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (oldbeam.RightSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = oldbeam.RightSide as SlidingSupport;
                            ss.AddBeam(this, Global.Direction.Right);

                            break;

                        case "BasicSupport":

                            var bs = oldbeam.RightSide as BasicSupport;
                            bs.AddBeam(this, Global.Direction.Right);

                            break;

                        case "LeftFixedSupport":

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (RightSide != null)
            {
                if (RightSide.GetType().Name != "RightFixedSupport")
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (RightSide.GetType().Name)
                    {
                        case "SlidingSupport":

                            var ss = RightSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Global.Direction.Right);

                            break;

                        case "BasicSupport":

                            var bs = RightSide as BasicSupport;
                            bs.AddBeam(oldbeam, Global.Direction.Right);

                            break;

                        case "LeftFixedSupport":

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
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
            if (!_isbound || !oldbeam.IsBound)
            {
                throw new InvalidOperationException("In order to create circular beam system both beam to be connected need to be bound");
            }

            switch (direction1)
            {
                case Global.Direction.Left:

                    switch (direction2)
                    {
                        #region Left-Left

                        case Global.Direction.Left:

                            if (LeftSide == null && oldbeam.LeftSide == null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }
                            else if (LeftSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Left-Right

                        case Global.Direction.Right:

                            if (LeftSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the right side of lodbeam.
                            leftrightcircularconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;

                case Global.Direction.Right:

                    switch (direction2)
                    {
                        #region Right-Left

                        case Global.Direction.Left:

                            if (RightSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Right-Right

                        case Global.Direction.Right:

                            if (RightSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Right side of this beam will be connected to the right side of oldbeam.                             
                            rightrightcircularconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;
            }
        }

        private void leftleftcircularconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                switch (oldbeam.LeftSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = oldbeam.LeftSide as SlidingSupport;
                        ss.AddBeam(this, Global.Direction.Left);

                        break;

                    case "BasicSupport":

                        var bs = oldbeam.LeftSide as BasicSupport;
                        bs.AddBeam(this, Global.Direction.Left);

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
            else if (LeftSide != null)
            {
                switch (LeftSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = LeftSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Global.Direction.Left);

                        break;

                    case "BasicSupport":

                        var bs = LeftSide as BasicSupport;
                        bs.AddBeam(oldbeam, Global.Direction.Left);

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
        }

        private void leftrightcircularconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                switch (oldbeam.RightSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = oldbeam.RightSide as SlidingSupport;
                        ss.AddBeam(this, Global.Direction.Left);

                        break;

                    case "BasicSupport":

                        var bs = oldbeam.RightSide as BasicSupport;
                        bs.AddBeam(this, Global.Direction.Left);

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
            else if (LeftSide != null)
            {
                switch (LeftSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = LeftSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Global.Direction.Right);

                        break;

                    case "BasicSupport":

                        var bs = LeftSide as BasicSupport;
                        bs.AddBeam(oldbeam, Global.Direction.Right);

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
        }

        private void rightrightcircularconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                switch (oldbeam.RightSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = oldbeam.RightSide as SlidingSupport;
                        ss.AddBeam(this, Global.Direction.Right);

                        break;

                    case "BasicSupport":

                        var bs = oldbeam.RightSide as BasicSupport;
                        bs.AddBeam(this, Global.Direction.Right);

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
            else if (RightSide != null)
            {
                switch (RightSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = RightSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Global.Direction.Right);

                        break;

                    case "BasicSupport":

                        var bs = RightSide as BasicSupport;
                        bs.AddBeam(oldbeam, Global.Direction.Right);

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
        }

        private void rightleftcircularconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                switch (oldbeam.LeftSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = oldbeam.LeftSide as SlidingSupport;
                        ss.AddBeam(this, Global.Direction.Right);

                        break;

                    case "BasicSupport":

                        var bs = oldbeam.LeftSide as BasicSupport;
                        bs.AddBeam(this, Global.Direction.Right);

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
            else if (RightSide != null)
            {
                switch (RightSide.GetType().Name)
                {
                    case "SlidingSupport":

                        var ss = RightSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Global.Direction.Left);

                        break;

                    case "BasicSupport":

                        var bs = RightSide as BasicSupport;
                        bs.AddBeam(oldbeam, Global.Direction.Left);

                        break;

                    case "RightFixedSupport":

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case "LeftFixedSupport":

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
        }

        private void leftreconnect()
        {
            Beam leftbeam = null;
            var direction = Global.Direction.None;

            switch (Global.GetObjectType(LeftSide))
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

            switch (Global.GetObjectType(RightSide))
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
                _distload = null;
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
                _concload = null;
                _maxconcload = double.MinValue;
            }
        }

        public void ShowDistLoadDiagram(int c)
        {
            if (_distload != null)
            {
                _distload.Show();
            }
            else if (_distributedloads?.Count > 0)
            {
                var load = new DistributedLoad(_distributedloads, this, c);
                Children.Add(load);
                Canvas.SetBottom(load, 0);
                Canvas.SetLeft(load, 0);
                _distload = load;
            }
        }

        public void HideDistLoadDiagram()
        {
            if (_distload != null)
            {
                _distload.Hide();
            }
        }

        public void DestroyDistLoadDiagram()
        {
            if (_distload != null)
            {
                _distload.RemoveLabels();
                Children.Remove(_distload);
                _distload = null;
            }
        }

        public void ShowConcLoadDiagram(int c)
        {
            if (_concload != null)
            {
                _concload.Show();
            }
            else if (_concentratedloads?.Count > 0)
            {
                var concentratedload = new ConcentratedLoad(_concentratedloads, this, c);
                Children.Add(concentratedload);
                Canvas.SetBottom(concentratedload, 0);
                Canvas.SetLeft(concentratedload, 0);
                _concload = concentratedload;
            }
        }

        public void HideConcLoadDiagram()
        {
            if (_concload != null)
            {
                _concload.Hide();
            }
        }

        public void DestroyConcLoadDiagram()
        {
            if (_concload != null)
            {
                _concload.RemoveLabels();
                Children.Remove(_concload);
                _concload = null;
            }
        }

        public void ShowFixedEndForceDiagram(int c)
        {
            if (_feforce != null)
            {
                _feforce.Show();
            }
            else
            {
                var force = new Force(_fixedendforce, this, c);
                Children.Add(force);
                Canvas.SetBottom(force, 0);
                Canvas.SetLeft(force, 0);
                _feforce = force;
            }
        }

        public void HideFixedEndForceDiagram()
        {
            if (_feforce != null)
            {
                _feforce.Hide();
            }
        }

        public void DestroyFixedEndForceDiagram()
        {
            if (_feforce != null)
            {
                _feforce.RemoveLabels();
                Children.Remove(_feforce);
                _feforce = null;
            }
        }

        public void ShowFixedEndMomentDiagram(int c)
        {
            if (_femoment != null)
            {
                _femoment.Show();
            }
            else if (_fixedendmoment?.Count > 0)
            {
                var moment = new Moment(_fixedendmoment, this, c);
                Children.Add(moment);
                Canvas.SetBottom(moment, 0);
                Canvas.SetLeft(moment, 0);
                _femoment = moment;
            }
        }

        public void HideFixedEndMomentDiagram()
        {
            if (_femoment != null)
            {
                _femoment.Hide();
            }
        }

        public void DestroyFixedEndMomentDiagram()
        {
            if (_femoment != null)
            {
                _femoment.RemoveLabels();
                Children.Remove(_femoment);
                _femoment = null;
            }
        }

        public void ShowInertiaDiagram(int c)
        {
            if (_inertia != null)
            {
                _inertia.Show();
            }
            else
            {
                var inertia = new Inertia(_inertiappoly, this, c);
                Children.Add(inertia);
                Canvas.SetBottom(inertia, 0);
                Canvas.SetLeft(inertia, 0);
                _inertia = inertia;
            }
        }

        public void HideInertiaDiagram()
        {
            if (_inertia != null)
            {
                _inertia.Hide();
            }
        }

        public void DestroyInertiaDiagram()
        {
            if (_inertia != null)
            {
                _inertia.RemoveLabels();
                Children.Remove(_inertia);
                _inertia = null;
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

        /// <summary>
        /// Adds inertia moment function.
        /// </summary>
        /// <param name="inertiappoly">The inertia Piecewise Polynomial.</param>
        public void AddInertia(PiecewisePoly inertiappoly)
        {
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.Min;
            _maxinertia = _inertiappoly.Max;
            Global.WritePPolytoConsole(Name + " inertia added", inertiappoly);
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
        /// <param name="eppoly">The eppoly.</param>
        public void AddE(PiecewisePoly eppoly)
        {
            _e = eppoly;
        }

        /// <summary>
        /// Adds the d. D stands for the height of the beam on load direction
        /// </summary>
        /// <param name="dppoly">The dppoly.</param>
        public void AddD(PiecewisePoly dppoly)
        {
            _d = dppoly;
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
        public void Select()
        {
            //BringToFront(_canvas, this);
            _core.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            if (LeftSide != null)
            {
                if (LeftSide is LeftFixedSupport)
                {
                    _startcircle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _startcircle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _startcircle.Visibility = Visibility.Visible;
            }

            if (RightSide != null)
            {
                if (RightSide is RightFixedSupport)
                {
                    _endcircle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _endcircle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _endcircle.Visibility = Visibility.Visible;
            }

            selected = true;
        }

        public void SelectLeftCircle()
        {
            _endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            _startcircle.Stroke = new SolidColorBrush(Colors.Yellow);
            circledirection = Global.Direction.Left;
            _leftcircleseleted = true;
        }

        public void SelectRightCircle()
        {
            _startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            _endcircle.Stroke = new SolidColorBrush(Colors.Yellow);
            circledirection = Global.Direction.Right;
            _rightcircleselected = true;
        }

        public void UnSelectCircle()
        {
            _startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            _endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            circledirection = Global.Direction.None;
            _leftcircleseleted = false;
            _rightcircleselected = false;
        }

        /// <summary>
        /// Executed when the beam was unselected. It changes the beam color and hides circles.
        /// </summary>
        public void UnSelect()
        {
            if (selected)
            {
                _core.Fill = new SolidColorBrush(Colors.Black);
                _startcircle.Visibility = Visibility.Collapsed;
                _startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
                _endcircle.Visibility = Visibility.Collapsed;
                _endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
                circledirection = Global.Direction.None;
                selected = false;
                UnSelectCircle();
                _tgeometry.HideCorners();
                MyDebug.WriteInformation(Name + " Beam unselected : left = " + Canvas.GetLeft(this) + " top = " + Canvas.GetTop(this));
            }
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
            if (_canbedragged)
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
            else
            {
                MyDebug.WriteWarning(Name + " The beam to be dragged can not be dragged");
            }
        }

        /// <summary>
        /// Sets the position of the beam based on top-right point of it.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetAbsolutePosition(Point point)
        {
            if (_canbedragged)
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
            else
            {
                MyDebug.WriteWarning(Name + " : The beam to be dragged can not be dragged");
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
        public void Move(Vector delta)
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
                switch (LeftSide.GetType().Name)
                {
                    case "LeftFixedSupport":

                        var ls = LeftSide as LeftFixedSupport;
                        ls.UpdatePosition(this);

                        break;

                    case "SlidingSupport":

                        var ss = LeftSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case "BasicSupport":

                        var bs = LeftSide as BasicSupport;
                        bs.UpdatePosition(this);

                        break;
                }
            }

            if (RightSide != null)
            {
                switch (RightSide.GetType().Name)
                {
                    case "RightFixedSupport":

                        var rs = RightSide as RightFixedSupport;
                        rs.UpdatePosition(this);

                        break;

                    case "SlidingSupport":

                        var ss = RightSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case "BasicSupport":

                        var bs = RightSide as BasicSupport;
                        bs.UpdatePosition(this);

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

            MyDebug.WriteInformation(Name + " : resultantforce = " + resultantforce + " resultantforcedistance = " + resultantforcedistance);

            MyDebug.WriteInformation(Name + " : leftsupportforcedist = " + _leftsupportforcedist + " rightsupportforcedist = " + _rightsupportforcedist);

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

            MyDebug.WriteInformation(Name + " : resultantforcedistance = " + resultantforcedistance);

            MyDebug.WriteInformation(Name + " : leftsupportforceconc = " + _leftsupportforceconc + " rightsupportforceconc = " + _rightsupportforceconc);
        }

        #region Zero Condition

        private void findconcentratedzeroforce()
        {
            _zeroforceconc = new PiecewisePoly();

            if (_concentratedloads?.Count > 0)
            {
                double leftforce = _leftsupportforceconc;

                if (_concentratedloads[0].Key > 0)
                {
                    var poly1 = new Poly(leftforce.ToString());
                    poly1.StartPoint = 0;
                    poly1.EndPoint = _concentratedloads[0].Key;
                    _zeroforceconc.Add(poly1);
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

                    _zeroforceconc.Add(poly);
                }
                Global.WritePPolytoConsole(Name + " : _zeroforceconc", _zeroforceconc);
            }
        }

        /// <summary>
        /// Finds the zero force polynomial which is the force polynomial when there is no fixed support in the end of the beam.
        /// </summary>
        private void finddistributedzeroforce()
        {
            _zeroforcedist = new PiecewisePoly();

            if (_distributedloads?.Count > 0)
            {
                if (_distributedloads[0].StartPoint != 0)
                {
                    var ply = new Poly(_leftsupportforcedist.ToString());
                    ply.StartPoint = 0;
                    ply.EndPoint = _distributedloads[0].StartPoint;
                    _zeroforcedist.Add(ply);
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
                            _zeroforcedist.Add(ply);
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
                    _zeroforcedist.Add(poly);
                }
                _zeroforcedist.Sort();

                if (_distributedloads.Last().EndPoint != _length)
                {
                    var weights = findforcebefore(_distributedloads.Count);
                    var ply = new Poly(weights.ToString());
                    ply.StartPoint = _distributedloads.Last().EndPoint;
                    ply.EndPoint = _length;
                    _zeroforcedist.Add(ply);
                }

                Global.WritePPolytoConsole(Name + " : _zeroforcedist", _zeroforcedist);
            }
        }

        /// <summary>
        /// Calculates the zero moment, the monet when the beam is bounded with basic supports on both sides.
        /// </summary>
        private void findzeromoment()
        {
            _zeromoment = new PiecewisePoly();

            foreach (Poly force in _zeroforce)
            {
                var index = _zeroforce.IndexOf(force);
                var poly = new Poly();
                var integration = force.Integrate();
                var momentsbefore = findmomentbefore(index);
                var zerovalue = force.Integrate().Calculate(force.StartPoint);
                var constant = momentsbefore - zerovalue;

                MyDebug.WriteInformation(Name + " : integration = " + integration.ToString() + " momentsbefore = " + momentsbefore + " zeroforcevalue = " + zerovalue + " startpoint = " + force.StartPoint + " endpoint = " + force.EndPoint);

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
                _zeromoment.Add(poly);
                _zeromoment.Sort();
            }

            Global.WritePPolytoConsole(Name + " : Fixed End Force", _zeromoment);
        }

        /// <summary>
        /// Calculates weights forces before the force poly whose index is given.
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
                var area = _zeroforce[indx].DefiniteIntegral(_zeroforce[indx].StartPoint, _zeroforce[indx].EndPoint);
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
            if (_zeromoment.Count > 0)
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
                    MyDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MyDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MyDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    var mox = _zeromoment * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MyDebug.WriteInformation(Name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(Global.SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / System.Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MyDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MyDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson6.AddData(_zeromoment.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MyDebug.WriteInformation(Name + " : r2 = " + r2);
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

                _ma = -moments[0];

                _mb = -moments[1];
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MyDebug.WriteInformation(Name + " : ma = " + _ma);
            MyDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have fixed support when there is only one beam.
        /// </summary>
        private void ffsolverclapeyron()
        {
            if (_zeromoment.Count > 0)
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
                    MyDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MyDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MyDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    var mox = _zeromoment * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MyDebug.WriteInformation(Name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(Global.SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / System.Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MyDebug.WriteInformation(Name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MyDebug.WriteInformation(Name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson6.AddData(_zeromoment.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MyDebug.WriteInformation(Name + " : r2 = " + r2);
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

            MyDebug.WriteInformation(Name + " : ma = " + _ma);
            MyDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the left end has fixed support and the right and basic or sliding support.
        /// </summary>
        private void fbsolver()
        {
            if (_zeromoment.Count > 0)
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
                    MyDebug.WriteInformation(Name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
                else
                {
                    MyDebug.WriteInformation(Name + " : Analytical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MyDebug.WriteInformation(Name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(Global.SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;
                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }
            MyDebug.WriteInformation(Name + " : ma = " + _ma);
            MyDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the right end has fixed support and the left and basic or sliding support.
        /// </summary>
        private void bfsolver()
        {
            if (_zeromoment.Count > 0)
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
                    MyDebug.WriteInformation(Name + " : Analytical solution started");
                    mb1 = _length / 3;
                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    var mox = _zeromoment * xppoly;
                    r1 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
                else
                {
                    MyDebug.WriteInformation(Name + " : Numerical solution started");
                    var simpson1 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson1.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    mb1 = 1 / System.Math.Pow(_length, 2) * simpson1.Result;

                    MyDebug.WriteInformation(Name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson3.AddData(_izero / _inertiappoly.Calculate(i) * _zeromoment.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MyDebug.WriteInformation(Name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MyDebug.WriteInformation(Name + " : ma = " + _ma);
            MyDebug.WriteInformation(Name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have basic or sliding support.
        /// </summary>
        private void bbsolver()
        {
            _mb = 0;
            _ma = 0;

            MyDebug.WriteInformation(Name + " : ma = " + _ma);
            MyDebug.WriteInformation(Name + " : mb = " + _mb);
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

            foreach (Poly moment in _zeromoment)
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
            _fixedendmoment = new PiecewisePoly(polylist);

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmoment);
        }

        private void findfixedendmomentclapeyron()
        {
            var polylist = new List<Poly>();

            var constant = (_mb - _ma) / _length;

            if (System.Math.Abs(constant) < 0.00000001)
            {
                constant = 0.0;
            }

            foreach (Poly moment in _zeromoment)
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
            _fixedendmoment = new PiecewisePoly(polylist);

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmoment);
        }

        /// <summary>
        /// Calculates the deflection of the beam on selected point. The deflection toward beam's red arrow direction is accepted as positive.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public double Deflection(double x)
        {
            var simpson1 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                var mom = _fixedendmoment.Calculate(i);
                var iner = _inertiappoly.Calculate(i);

                simpson1.AddData(mom * (_length - i) / iner);
            }

            simpson1.Calculate();

            var int1 = simpson1.Result;

            var simpson2 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= x; i = i + 0.0001)
            {
                var mom = _fixedendmoment.Calculate(i);
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
            MyDebug.WriteInformation(Name + " : ClapeyronCalculate has started to work");
            findconcentratedsupportforces();
            finddistributedsupportforces();
            findconcentratedzeroforce();
            finddistributedzeroforce();
            _zeroforce = _zeroforceconc + _zeroforcedist;
            Global.WritePPolytoConsole(Name + " : Zero Force", _zeroforce);
            findzeromoment();
            canbesolvedanalytically();
            clapeyronsupportcase();
            findfixedendmomentclapeyron();

            MyDebug.WriteInformation(Name + " Left End Moment = " + _ma);
            Logger.WriteLine(Name + " Left End Moment = " + _ma);
            MyDebug.WriteInformation(Name + " Right End Moment = " + _mb);
            Logger.WriteLine(Name + " Right End Moment = " + _mb);
        }

        /// <summary>
        /// Calculates moments when there is only one beam presented.
        /// </summary>
        private void clapeyronsupportcase()
        {
            #region cross support cases

            switch (Global.GetObjectType(LeftSide))
            {
                case Global.ObjectType.LeftFixedSupport:

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                            ffsolverclapeyron();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                            fbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                            fbsolver();

                            break;
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;
                    }

                    break;

                case Global.ObjectType.SlidingSupport:

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case Global.ObjectType.BasicSupport:

                            MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            MyDebug.WriteInformation(Name + " : bbsolver has been executed");
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
                MyDebug.WriteInformation(Name + " : Analytical solution is not possible");
            }

            //Check if inertia ppoly is constant or not dependant on x
            if (_inertiappoly.Degree() > 0)
            {
                _analyticalsolution = false;
                MyDebug.WriteInformation(Name + " : Analytical solution is not possible");
            }

            //Check if zero moment ppoly has any term with non-integer power
            if (_zeromoment.Count > 0)
            {
                foreach (Poly poly in _zeromoment)
                {
                    foreach (Term term in poly.Terms)
                    {
                        if (term.Power % 1 != 0)
                        {
                            _analyticalsolution = false;
                            MyDebug.WriteInformation(Name + " : Analytical solution is not possible");
                        }
                    }
                }
            }
            _analyticalsolution = true;
            MyDebug.WriteInformation(Name + " : Analytical solution is possible");
        }

        #endregion

        #region cross

        /// <summary>
        /// Conducts cross-balancing moment from the given direction to the other direction of the beam.
        /// </summary>
        /// <param name="direction">The first direction.</param>
        public void Conduct(Global.Direction direction, double moment)
        {
            #region code

            switch (direction)
            {
                case Global.Direction.Left:

                    double conductmoment;

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.BasicSupport:

                            BasicSupport bs = RightSide as BasicSupport;

                            if (bs.Members.Count > 1)
                            {
                                conductmoment = moment * CarryOverAB;

                                _mb = _mb + conductmoment;

                                Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because right side of this beam is bounded to a free basic support(" + bs.Name + ")");
                            }

                            break;

                        case Global.ObjectType.SlidingSupport:

                            SlidingSupport ss = RightSide as SlidingSupport;

                            if (ss.Members.Count > 1)
                            {
                                conductmoment = moment * CarryOverAB;

                                _mb = _mb + conductmoment;

                                Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because right side of this beam is bounded to a free sliding support(" + ss.Name + ")");
                            }

                            break;

                        case Global.ObjectType.RightFixedSupport:

                            RightFixedSupport rs = RightSide as RightFixedSupport;

                            conductmoment = moment * CarryOverAB;

                            _mb = _mb + conductmoment;

                            Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);

                            Logger.WriteLine(this.Name + " : " + rs.Name + " is a fixed support. So there will be no seperation in this support");

                            break;
                    }

                    break;

                case Global.Direction.Right:

                    double conductmoment1;

                    switch (Global.GetObjectType(LeftSide))
                    {
                        case Global.ObjectType.BasicSupport:

                            BasicSupport bs = LeftSide as BasicSupport;

                            if (bs.Members.Count > 1)
                            {
                                conductmoment1 = moment * CarryOverBA;

                                _ma = _ma + conductmoment1;

                                Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because left side of this beam is bounded to a free basic support(" + bs.Name + ")");
                            }

                            break;

                        case Global.ObjectType.SlidingSupport:

                            SlidingSupport ss = LeftSide as SlidingSupport;

                            if (ss.Members.Count > 1)
                            {
                                conductmoment1 = moment * CarryOverBA;

                                _ma = _ma + conductmoment1;

                                Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because left side of this beam is bounded to a free sliding support (" + ss.Name + ")");
                            }

                            break;

                        case Global.ObjectType.LeftFixedSupport:

                            LeftFixedSupport ls = LeftSide as LeftFixedSupport;

                            conductmoment1 = moment * CarryOverBA;

                            _ma = _ma + conductmoment1;

                            Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);

                            Logger.WriteLine(this.Name + " : " + ls.Name + " is a fixed support. So there will be no seperation in this support");

                            break;
                    }

                    break;
            }

            #endregion
        }

        /// <summary>
        /// Calculates alfa, beta, gama, k and fi coefficients.
        /// </summary>
        private void findcrosscoefficients()
        {
            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            var lxpoly = new Poly(_length.ToString() + "-x");
            lxpoly.StartPoint = 0;
            lxpoly.EndPoint = _length;

            var xppoly = new PiecewisePoly();
            xppoly.Add(x);

            if (_analyticalsolution)
            {
                MyDebug.WriteInformation(Name + " : Analytical solution started");
                _alfaa = 1.0 / 3;
                MyDebug.WriteInformation(Name + " : alfaa = " + _alfaa);
                Logger.WriteLine(this.Name + " : alfaa = " + _alfaa);

                _alfab = 1.0 / 3;
                MyDebug.WriteInformation(Name + " : alfab = " + _alfab);
                Logger.WriteLine(this.Name + " : alfab = " + _alfab);

                _beta = 1.0 / 6;
                MyDebug.WriteInformation(Name + " : beta = " + _beta);
                Logger.WriteLine(Name + " : beta = " + _beta);
            }
            else
            {
                MyDebug.WriteInformation(Name + " : Numerical solution started");
                var simpson1 = new SimpsonIntegrator(Global.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                {
                    simpson1.AddData(System.Math.Pow(lxpoly.Calculate(i), 2) / _inertiappoly.Calculate(i));
                }

                simpson1.Calculate();

                _alfaa = _izero / System.Math.Pow(_length, 3) * simpson1.Result;

                MyDebug.WriteInformation(Name + " : alfaa = " + _alfaa);

                Logger.WriteLine(this.Name + " : alfaa = " + _alfaa);

                var simpson2 = new SimpsonIntegrator(Global.SimpsonStep);

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                {
                    simpson2.AddData(xsquare.Calculate(i) / _inertiappoly.Calculate(i));
                }

                simpson2.Calculate();

                _alfab = _izero / System.Math.Pow(_length, 3) * simpson2.Result;

                MyDebug.WriteInformation(Name + " : alfab = " + _alfab);

                Logger.WriteLine(Name + " : alfab = " + _alfab);

                var simpson3 = new SimpsonIntegrator(Global.SimpsonStep);

                for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                {
                    simpson3.AddData((lxpoly.Calculate(i) * x.Calculate(i)) / _inertiappoly.Calculate(i));
                }

                simpson3.Calculate();

                _beta = _izero / System.Math.Pow(_length, 3) * simpson3.Result;

                MyDebug.WriteInformation(Name + " : beta = " + _beta);

                Logger.WriteLine(Name + " : beta = " + _beta);
            }

            if (_zeromoment.Count > 0)
            {
                var mox = _zeromoment * xppoly;

                if (_analyticalsolution)
                {
                    _ka = 6.0 * _izero / System.Math.Pow(_length, 2) * mox.DefiniteIntegral(0, _length);
                    MyDebug.WriteInformation(Name + " : ka = " + _ka);
                    Logger.WriteLine(Name + " : ka = " + _ka);

                    var lxppoly = new PiecewisePoly();
                    lxppoly.Add(lxpoly);

                    var mlx = _zeromoment * lxppoly;

                    _kb = 6.0 * _izero / System.Math.Pow(_length, 2) * mlx.DefiniteIntegral(0, _length);
                    Logger.WriteLine(Name + " : kb = " + _kb);
                    MyDebug.WriteInformation(Name + " : kb = " + _kb);
                }
                else
                {
                    var simpson4 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson4.AddData(mox.Calculate(i) / _inertiappoly.Calculate(i));
                    }

                    simpson4.Calculate();

                    _ka = 6 * _izero / System.Math.Pow(_length, 2) * simpson4.Result;

                    MyDebug.WriteInformation(Name + " : ka = " + _ka);

                    Logger.WriteLine(Name + " : ka = " + _ka);

                    var simpson5 = new SimpsonIntegrator(Global.SimpsonStep);

                    for (double i = 0; i <= _length; i = i + Global.SimpsonStep)
                    {
                        simpson5.AddData((_zeromoment.Calculate(i) * lxpoly.Calculate(i)) / _inertiappoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    _kb = 6 * _izero / System.Math.Pow(_length, 2) * simpson5.Result;

                    Logger.WriteLine(Name + " : kb = " + _kb);

                    MyDebug.WriteInformation(Name + " : kb = " + _kb);
                }
            }
            else
            {
                _ka = 0;
                _kb = 0;
            }

            _fia = _length * (_kb / 6 + _mb * _beta + _ma * _alfaa) / (_elasticity * _izero);
            MyDebug.WriteInformation(Name + " : fia = " + _fia);

            _fib = -_length * (_ka / 6 + _ma * _beta + _mb * _alfab) / (_elasticity * _izero);
            MyDebug.WriteInformation(Name + " : fib = " + _fib);

            _gamaba = _beta / _alfaa;
            MyDebug.WriteInformation(Name + " : gamaba = " + _gamaba);

            _gamaab = _beta / _alfab;
            MyDebug.WriteInformation(Name + " : gamaab = " + _gamaab);

            #region stiffnesses with support cases

            switch (Global.GetObjectType(LeftSide))
            {
                case Global.ObjectType.LeftFixedSupport:

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MyDebug.WriteInformation(Name + " : stiffness case 1");

                            _stiffnessa = 0;

                            _stiffnessb = 0;

                            break;

                        case Global.ObjectType.BasicSupport:

                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(Name + " : stiffness case 2");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MyDebug.WriteInformation(Name + " : stiffness case 3");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;

                        case Global.ObjectType.SlidingSupport:

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(Name + " : stiffness case 4");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MyDebug.WriteInformation(Name + " : stiffness case 5");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : stiffness case 6");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 7");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 8");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 9");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : stiffness case 10");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 11");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 12");
                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 13");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 14");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;

                case Global.ObjectType.SlidingSupport:

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : stiffness case 15");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 16");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 17");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : tiffness case 18");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 19");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : stiffness case 20");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 21");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 22");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 23");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : stiffness case 24");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;
            }

            _stiffnessa = System.Math.Round(_stiffnessa, 5);

            _stiffnessb = System.Math.Round(_stiffnessb, 5);

            MyDebug.WriteInformation(Name + " : StiffnessA = " + _stiffnessa);

            Logger.WriteLine(Name + " : StiffnessA = " + _stiffnessa);

            MyDebug.WriteInformation(Name + " : StiffnessB = " + _stiffnessb);

            Logger.WriteLine(Name + " : StiffnessB = " + _stiffnessb);
            #endregion
        }

        /// <summary>
        /// Chooses the solver to be executed according to supports in the way of Cross Method.
        /// </summary>
        private void crosssupportcases()
        {
            #region cross support cases

            switch (Global.GetObjectType(LeftSide))
            {
                case Global.ObjectType.LeftFixedSupport:

                    switch (Global.GetObjectType(RightSide))
                    {
                        case Global.ObjectType.RightFixedSupport:

                            MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                            ffsolver();

                            break;

                        case Global.ObjectType.BasicSupport:

                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                                fbsolver();
                            }

                            break;

                        case Global.ObjectType.SlidingSupport:

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                                fbsolver();
                            }

                            break;
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                ffsolver();

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                bfsolver();

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;

                case Global.ObjectType.SlidingSupport:

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                ffsolver();

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (Global.GetObjectType(RightSide))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                bfsolver();

                                break;

                            case Global.ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(Name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(Name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;
            }

            #endregion
        }

        /// <summary>
        /// Main function that prepares parameters and conducts Cross Solution for the beam.
        /// </summary>
        public void CrossCalculate()
        {
            MyDebug.WriteInformation(Name + " : CrossCalculate has started to work");

            findconcentratedsupportforces();

            finddistributedsupportforces();

            findconcentratedzeroforce();

            finddistributedzeroforce();

            _zeroforce = _zeroforceconc + _zeroforcedist;

            Global.WritePPolytoConsole(Name + " : _zeroforce", _zeroforce);

            findzeromoment();

            canbesolvedanalytically();

            crosssupportcases();

            findfixedendmomentcross();

            findcrosscoefficients();

            _maclapeyron = _ma;

            MyDebug.WriteInformation(Name + " : Clapeyron Ma = " + _maclapeyron);

            Logger.WriteLine(Name + " : Clapeyron Ma = " + _maclapeyron);

            _mbclapeyron = _mb;

            MyDebug.WriteInformation(Name + " : Clapeyron Mb = " + _mbclapeyron);

            Logger.WriteLine(Name + " : Clapeyron Mb = " + _mbclapeyron);

            //Normal to Cross sign conversion

            if (Deflection(0.001) < 0)
            {
                _ma = Algebra.Negative(_ma);
            }
            else
            {
                _ma = Algebra.Positive(_ma);
            }

            if (Deflection(_length - 0.001) < 0)
            {
                _mb = Algebra.Positive(_mb);
            }
            else
            {
                _mb = Algebra.Negative(_mb);
            }

            MyDebug.WriteInformation(Name + " : Ma = " + _ma);

            Logger.WriteLine(Name + " : Cross Ma = " + _ma);

            Logger.WriteLine(Name + " : Cross Mb = " + _mb);

            Logger.NextLine();

            MyDebug.WriteInformation(Name + " : Mb = " + _mb);

            MyDebug.WriteInformation(Name + " : CrossCalculate has finished to work");
        }

        public void ResetSolution()
        {
            _ma = 0;
            _mb = 0;
            _fixedendforce = null;
            _fixedendmoment = null;
            _zeroforce = null;
            _zeroforceconc = null;
            _zeroforcedist = null;
            _zeromoment = null;
            _maxforce = Double.MinValue;
            _maxabsmoment = Double.MinValue;
            _minmoment = Double.MaxValue;
            _maxforce = Double.MinValue;
            _maxabsforce = Double.MinValue;
            _minforce = Double.MinValue;
            _maxstress = Double.MinValue;
            _maxabsstress = Double.MinValue;
            DestroyFixedEndMomentDiagram();
            DestroyFixedEndForceDiagram();
            DestroyStressDiagram();
        }

        #endregion

        #region post-cross

        /// <summary>
        /// Calculates final moment, force and stress distributions after Cross loop according to the results
        /// </summary>
        public void PostCrossUpdate()
        {
            updatemoments();

            updateforces();

            if (_stressanalysis)
            {
                updatestresses();
            }

            //updatedeflection();
        }

        public void PostClapeyronUpdate()
        {
            _maxmoment = _fixedendmoment.Max;

            _maxabsmoment = _fixedendmoment.MaxAbs;

            _minmoment = _fixedendmoment.Min;

            _fixedendforce = _fixedendmoment.Derivate();

            Global.WritePPolytoConsole(Name + " : Fixed End Force", _fixedendforce);

            _maxforce = _fixedendforce.Max;

            _maxabsforce = _fixedendforce.MaxAbs;

            _minforce = _fixedendforce.Min;

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
            var polylist = new List<Poly>();

            double constant = 0;

            if (_zeromoment.Length > 0)
            {
                //Cross to normal convention sign conversion
                if (Deflection(0.001) < 0)
                {
                    if (_ma > 0)
                    {
                        _ma = Algebra.Negative(_ma);
                    }
                    else
                    {
                        _ma = Algebra.Positive(_ma);
                    }
                }
                else
                {
                    if (_ma > 0)
                    {
                        _ma = Algebra.Negative(_ma);
                    }
                    else
                    {
                        _ma = Algebra.Positive(_ma);
                    }
                }

                if (Deflection(_length - 0.001) < 0)
                {
                    if (_mb > 0)
                    {
                        _mb = Algebra.Positive(_mb);
                    }
                    else
                    {
                        _mb = Algebra.Negative(_mb);
                    }
                }
                else
                {
                    if (_mb > 0)
                    {
                        _mb = Algebra.Positive(_mb);
                    }
                    else
                    {
                        _mb = Algebra.Negative(_mb);
                    }
                }

                constant = (_mb - _ma) / _length;

                foreach (Poly moment in _zeromoment)
                {
                    var poly = new Poly();
                    var poly1 = new Poly(_ma.ToString());
                    poly1.StartPoint = moment.StartPoint;
                    poly1.EndPoint = moment.EndPoint;
                    var poly2 = new Poly("x");
                    poly2.StartPoint = moment.StartPoint;
                    poly2.EndPoint = moment.EndPoint;

                    if (constant != 0)
                    {
                        if (System.Math.Abs(constant) < 0.0001)
                        {
                            poly = moment + poly1;
                        }
                        else
                        {
                            var poly3 = new Poly(constant.ToString());
                            poly = moment + poly1 + poly2 * poly3;
                        }
                    }
                    else
                    {
                        poly = moment + poly1;
                    }
                    poly.StartPoint = moment.StartPoint;
                    poly.EndPoint = moment.EndPoint;
                    polylist.Add(poly);
                }
            }
            else
            {
                //There is no load on this beam
                if (_ma > 0)
                {
                    _ma = Algebra.Negative(_ma);
                }
                else
                {
                    _ma = Algebra.Positive(_ma);
                }

                if (_mb > 0)
                {
                    _mb = Algebra.Positive(_mb);
                }
                else
                {
                    _mb = Algebra.Negative(_mb);
                }

                constant = (_mb - _ma) / _length;

                if (System.Math.Abs(constant) < 0.000001)
                {
                    constant = 0.0;
                }

                var poly = new Poly();
                var poly1 = new Poly(_ma.ToString());
                poly1.StartPoint = 0;
                poly1.EndPoint = _length;
                var poly2 = new Poly("x");
                poly2.StartPoint = 0;
                poly2.EndPoint = _length;

                if (constant != 0)
                {
                    var poly3 = new Poly(constant.ToString());
                    poly = poly1 + poly2 * poly3;
                }
                else
                {
                    poly = poly1;
                }

                poly.StartPoint = 0;
                poly.EndPoint = _length;
                polylist.Add(poly);
            }

            _fixedendmoment = new PiecewisePoly(polylist);

            Global.WritePPolytoConsole(Name + " : Fixed End Moment", _fixedendmoment);

            _maxmoment = _fixedendmoment.Max;

            _maxabsmoment = _fixedendmoment.MaxAbs;

            _minmoment = _fixedendmoment.Min;
        }

        /// <summary>
        /// Calculates final force distribution after cross loop.
        /// </summary>
        private void updateforces()
        {
            _fixedendforce = _fixedendmoment.Derivate();

            Global.WritePPolytoConsole(Name + " : Fixed End Force", _fixedendforce);

            _maxforce = _fixedendforce.Max;

            _maxabsforce = _fixedendforce.MaxAbs;

            _minforce = _fixedendforce.Min;
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
                e = _e.Calculate(i * precision);
                d = _d.Calculate(i * precision);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = System.Math.Pow(10, 3) * _fixedendmoment.Calculate(i * precision) * y / (_inertiappoly.Calculate(i * precision));
                _stress.Add(i * precision, stress);
            }

            if (!_stress.ContainsKey(_length))
            {
                e = _e.Calculate(_length);
                d = _d.Calculate(_length);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = System.Math.Pow(10, 3) * _fixedendmoment.Calculate(_length) * y / (_inertiappoly.Calculate(_length));
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
                value.yposition = -_fixedendmoment.Calculate(i * precision) / (_elasticity * _inertiappoly.Calculate(i * precision));
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

        #endregion

        #region ReDraw Functions

        public void ReDrawMoment(int c)
        {
            if (_femoment != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing moment for c = " + c);
                _femoment.Draw(c);
            }
            else if (_fixedendmoment?.Count > 0)
            {
                ShowFixedEndMomentDiagram(c);
            }
        }

        public void ReDrawDistLoad(int c)
        {
            if (_distload != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing distributed load for c = " + c);
                _distload.Draw(c);
            }
            else if (_distributedloads?.Count > 0)
            {
                ShowDistLoadDiagram(c);
            }
        }

        public void ReDrawConcLoad(int c)
        {
            if (_concload != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing concentrated load for c = " + c);
                _concload.Draw(c);
            }
            else if (_concentratedloads?.Count > 0)
            {
                ShowConcLoadDiagram(c);
            }
        }

        public void ReDrawInertia(int c)
        {
            if (_inertia != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing inertia for c = " + c);
                _inertia.Draw(c);
            }
            else if (_inertiappoly?.Count > 0)
            {
                ShowInertiaDiagram(c);
            }
        }

        public void ReDrawForce(int c)
        {
            if (_feforce != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing force for c = " + c);
                _feforce.Draw(c);
            }
            else if (_fixedendforce?.Count > 0)
            {
                ShowFixedEndForceDiagram(c);
            }
        }

        public void ReDrawStress(int c)
        {
            if (_stressdiagram != null)
            {
                MyDebug.WriteInformation(Name + " : redrawing stress for c = " + c);
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

        #region properties

        #region ui


        /// <summary>
        /// Gets or sets a value indicating whether the beam can be dragged in the canvas.
        /// </summary>
        /// <value>
        /// <c>true</c> if the beam can be dragged; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeDragged
        {
            get
            {
                return _canbedragged;
            }
            set
            {
                _canbedragged = value;
            }
        }

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

        public PiecewisePoly Inertias
        {
            get { return _inertiappoly; }
        }

        public PiecewisePoly ZeroForce
        {
            get { return _zeroforce; }
        }

        public PiecewisePoly ZeroMoment
        {
            get { return _zeromoment; }
        }

        public PiecewisePoly FixedEndMoment
        {
            get { return _fixedendmoment; }
        }

        public PiecewisePoly FixedEndForce
        {
            get { return _fixedendforce; }
        }

        public KeyValueCollection Stress
        {
            get { return _stress; }
        }

        public PiecewisePoly E
        {
            get { return _e; }
            set { _e = value; }
        }

        public PiecewisePoly D
        {
            get { return _d; }
            set { _d = value; }
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

        public double MaxInertia
        {
            get { return _maxinertia; }
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

        #endregion

    }
}
