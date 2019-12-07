﻿/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using MesnetMD.Classes;
using MesnetMD.Classes.IO;
using MesnetMD.Classes.IO.Xml;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Graphics;
using MesnetMD.Classes.Ui.Som;
using MesnetMD.Properties;
using MesnetMD.Xaml.Pages;
using MesnetMD.Xaml.User_Controls;
using ZoomAndPan;

namespace MesnetMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (Settings.Default.language != "none")
            {
                Global.SetLanguageDictionary(Settings.Default.language);
            }
            else
            {
                Global.SetLanguageDictionary();
            }

            Global.SetDecimalSeperator();

            InitializeComponent();

            _uptoolbar = new UpToolBar(this);

            _treehandler = new TreeHandler(this);

            scaleslider.Value = zoomAndPanControl.ContentScale;

            zoomAndPanControl.MaxContentScale = 12;

            scaleslider.Maximum = zoomAndPanControl.MaxContentScale;
            scaleslider.Minimum = 0;

            scroller.ScrollToHorizontalOffset(9990);
            scroller.ScrollToVerticalOffset(9990);

            bwupdate = new BackgroundWorker();
            bwupdate.DoWork += bwupdate_DoWork;

            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += timer_Tick;

            Canvas.SetZIndex(viewbox, 1);
            var tests = new TestCases(this);
            tests.RegisterTests();

            if (App.AssociationPath != null)
            {
                open(App.AssociationPath);
                _savefilepath = App.AssociationPath;
            }
        }

        private Point selectpoint;

        private Point circlelocation;

        private Point mousedownpoint;

        private Point mouseuppoint;

        private Beam tempbeam;

        private Beam selectedbeam;

        private Beam assemblybeam;

        public SupportItem selectesupport;

        private int beamcount = 0;

        private DispatcherTimer timer = new DispatcherTimer();

        private double beamangle = 0;

        private bool assembly = false;

        private int _leftcount = 0;

        private int _rightcount = 0;

        private double _maxstress = 150;

        private string _savefilepath = null;

        private UpToolBar _uptoolbar;

        private TreeHandler _treehandler;

        #region zoomandpancontrol

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the canvas that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        private Point origContentMouseUpPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous canvas scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        public static BackgroundWorker bwupdate;

        private bool mousemoved = false;

        private bool writetodebug = true;

        /// <summary>
        /// Only make mouse handling mode none.
        /// Used in dragging.
        /// </summary>
        private void SetMouseHandlingModeNone()
        {
            mouseHandlingMode = MouseHandlingMode.None;
        }

        private void SetMouseHandlingMode(string sender, MouseHandlingMode mode)
        {
            mouseHandlingMode = mode;

            if (mouseHandlingMode == MouseHandlingMode.None)
            {
                btnonlybeammode();
                btndisablerotation();
            }

            /*if (writetodebug)
            {
                MyDebug.WriteInformation(mode.ToString());
            }*/
        }

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousemoved = false;
            canvas.Focus();
            Keyboard.Focus(canvas);

            if (mouseHandlingMode == MouseHandlingMode.CircularBeamConnection)
            {
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
                return;
            }

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(canvas);
            //MyDebug.WriteInformation("zoomAndPanControl_MouseDown origContentMouseDownPoint :", origContentMouseDownPoint.X + " : " + origContentMouseDownPoint.Y);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 && (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                SetMouseHandlingMode("zoomAndPanControl_MouseDown", MouseHandlingMode.Zooming);
            }
            else if (mouseButtonDown == MouseButton.Left && mouseHandlingMode != MouseHandlingMode.BeamPlacing && !assembly)
            {
                // Just a plain old left-down initiates panning mode.
                SetMouseHandlingMode("zoomAndPanControl_MouseDown", MouseHandlingMode.Panning);
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "mouse handling mode : " + mouseHandlingMode.ToString());
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomAndPanControl.CaptureMouse();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the canvas.
                        //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "Shift + left-click zooms in");
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the canvas.
                        //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "Shift + left-click zooms out");
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "drag-zooming");
                    ApplyDragZoomRect();
                }
                else if (mouseHandlingMode == MouseHandlingMode.BeamPlacing)
                {
                    var x = origContentMouseDownPoint.X;
                    var y = origContentMouseDownPoint.Y;

                    tempbeam.AddCenter(canvas, x, y);
                    tempbeam.SetAngleCenter(beamangle);
                    var fictionalsupport1 = new FictionalSupport(canvas);
                    fictionalsupport1.AddBeam(tempbeam, Global.Direction.Left);

                    var fictionalsupport2 = new FictionalSupport(canvas);
                    fictionalsupport2.AddBeam(tempbeam, Global.Direction.Right);

                    Notify("beamput");
                    ResetSolution();
                    _uptoolbar.UpdateLoadDiagrams();
                    _treehandler.UpdateAllBeamTree();
                    _treehandler.UpdateAllSupportTree();

                    Reset();

                    //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "beam has been put");
                }

                zoomAndPanControl.ReleaseMouseCapture();
                SetMouseHandlingMode("zoomAndPanControl_MouseUp", MouseHandlingMode.None);
                e.Handled = true;
            }

            origContentMouseUpPoint = e.GetPosition(canvas);

            if (origContentMouseDownPoint == origContentMouseUpPoint && !mousemoved)
            {
                if (selectedbeam != null)
                {
                    //selectedbeam.ShowCorners(5);
                    if (selectedbeam.IsInside(origContentMouseUpPoint))
                    {
                        return;
                    }
                }
                zoomAndPanControl_Clicked();
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point coord = e.GetPosition(canvas);
#if DEBUG
            coordinate.Text = "X : " + Math.Round(coord.X, 4) + " Y : " + Math.Round(coord.Y, 4);
#else
            coordinate.Text = "X : " + Math.Round(coord.X - 10000, 4) + " Y : " + Math.Round(10000 - coord.Y, 4);
#endif

            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "panning");
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(canvas);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "zooming");
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    SetMouseHandlingMode("zoomAndPanControl_MouseMove", MouseHandlingMode.DragZooming);
                    Point curContentMousePoint = e.GetPosition(canvas);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "drag zooming");
                //
                // When in drag zooming mode continously upda.te the position of the rectangle
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(canvas);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        private void zoomAndPanControl_Clicked()
        {
            MesnetMDDebug.WriteInformation("Clicked!");
            Reset();
            Notify();
        }

        private void zoomAndPanControl_ScaleChanged(object sender, EventArgs eventArgs)
        {
            if (Global.Scale > 0)
            {
                horizontalrect.Height = 1 / Global.Scale;
                horizontalrect.Width = 50 / Global.Scale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / Global.Scale;
                verticalrect.Width = 1 / Global.Scale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(canvas);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(canvas);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in canvas coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            var newscale = zoomAndPanControl.ContentScale - 0.01;
            zoomAndPanControl.ZoomAboutPoint(newscale, contentZoomCenter);
            Global.Scale = newscale;
            scaletext.Text = Global.Scale.ToString();
            scaleslider.Value = Global.Scale;
            Notify("zoomedout");

            /*if (newscale > 0)
            {
                horizontalrect.Height = 1 / newscale;
                horizontalrect.Width = 50 / newscale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / newscale;
                verticalrect.Width = 1 / newscale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }*/

            //MyDebug.WriteInformation("ZoomOut", "Canvas Scale = " + newscale);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in canvas coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            var newscale = zoomAndPanControl.ContentScale + 0.01;
            zoomAndPanControl.ZoomAboutPoint(newscale, contentZoomCenter);
            Global.Scale = newscale;
            scaletext.Text = Global.Scale.ToString();
            scaleslider.Value = Global.Scale;
            Notify("zoomedin");

            /*if (newscale > 0)
            {
                horizontalrect.Height = 1 / newscale;
                horizontalrect.Width = 50 / newscale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / newscale;
                verticalrect.Width = 1 / newscale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }*/
            //MyDebug.WriteInformation("ZoomIn", "Canvas Scale = " + newscale);
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from canvas coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            //zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));     

            zoomAndPanControl.ZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            double scaleX = zoomAndPanControl.ContentViewportWidth / contentWidth;
            double scaleY = zoomAndPanControl.ContentViewportHeight / contentHeight;
            double newScale = zoomAndPanControl.ContentScale * Math.Min(scaleX, scaleY);
            Global.Scale = newScale;
            scaleslider.Value = Global.Scale;
            scaletext.Text = Global.Scale.ToString();

            /*var timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(zoomAndPanControl.AnimationDuration);

            timer.Tick += delegate
            {
                timer.Stop();
                double scaleX = zoomAndPanControl.ContentViewportWidth/contentWidth;
                double scaleY = zoomAndPanControl.ContentViewportHeight/contentHeight;
                double newScale = zoomAndPanControl.ContentScale*Math.Min(scaleX, scaleY);
                Global.Scale = newScale;
                scaleslider.Value = Global.Scale;
                scaletext.Text = Global.Scale.ToString();
                
            };

            timer.Start();*/

            dragZoomCanvas.Visibility = Visibility.Collapsed;

            //FadeOutDragZoomRect();
        }

        /// <summary>
        /// Fade out the drag zoom rectangle.    
        /// </summary>      
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        /// <summary>
        /// Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.    
        /// </summary>  
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
            prevZoomScale = zoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }

        private void zoomAndPanControl_ContentOffsetXChanged(object sender, EventArgs e)
        {
            mousemoved = true;
        }

        private void zoomAndPanControl_ContentOffsetYChanged(object sender, EventArgs e)
        {
            mousemoved = true;
        }
        #endregion

        #region Beam Component Events

        /// <summary>
        /// Event raised when a mouse button is clicked
        ///  down over beam rectangle.
        /// </summary>
        public void BeamCoreMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            Keyboard.Focus(canvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                //MyDebug.WriteInformation("Object_MouseDown", "Shift key is held down special zooming logic is executed, returning");
                return;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                //
                //MyDebug.WriteInformation("Object_MouseDown", "MouseHandlingMode.None, returning");
                return;
            }

            origContentMouseDownPoint = e.GetPosition(canvas);

            mousedownpoint = e.GetPosition(canvas);

            //MyDebug.WriteInformation("Object_MouseDown", "mousedownpoint : " + beammousedownpoint.X + " : " + beammousedownpoint.Y);

            if (!assembly)
            {
                SetMouseHandlingMode("core_MouseDown", MouseHandlingMode.Dragging);
                var core = (Rectangle)sender;
                core.CaptureMouse();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over beam rectangle.
        /// </summary>
        public void BeamCoreMouseUp(object sender, MouseButtonEventArgs e)
        {
            var core = (Rectangle)sender;

            if (!assembly && mouseHandlingMode != MouseHandlingMode.Dragging)
            {
                // We are not in dragging mode.
                MesnetMDDebug.WriteInformation("not dragging not assembly, returning");
                return;
            }

            mouseuppoint = e.GetPosition(canvas);

            MesnetMDDebug.WriteInformation("mouseuppoint : " + mouseuppoint.X + " : " + mouseuppoint.Y);

            if (mouseuppoint.Equals(mousedownpoint))
            {
                MesnetMDDebug.WriteInformation("beam core clicked");
                var beam = core.Parent as Beam;
                if (beam.IsSelected())
                {
                    MesnetMDDebug.WriteInformation("beam is already selected");
                    handlebeamdoubleclick(beam);
                }
                else
                {
                    if (!assembly)
                    {
                        UnselectAll();
                        _treehandler.UnSelectAllBeamItem();
                        _treehandler.UnSelectAllSupportItem();
                    }
                    SelectBeam(beam);

                    _treehandler.SelectBeamItem(beam);
                }
            }

            if (mouseHandlingMode == MouseHandlingMode.Dragging)
            {
                //Dont disable buttons after dragging
                SetMouseHandlingModeNone();
                core.ReleaseMouseCapture();
                e.Handled = true;
                return;
            }

            if (!assembly)
            {
                SetMouseHandlingMode("core_MouseUp", MouseHandlingMode.None);
            }

            core.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved over an object.
        /// </summary>
        public void BeamCoreMouseMove(object sender, MouseEventArgs e)
        {
            var core = (Rectangle)sender;
            if (mouseHandlingMode != MouseHandlingMode.Dragging)
            {
                // We are not in rectangle dragging mode, so don't do anything.
                core.ReleaseMouseCapture();
                //MyDebug.WriteInformation("Object_MouseMove", "dragging, returning");
                return;
            }

            Point curContentPoint = e.GetPosition(canvas);
            coordinate.Text = "X : " + Math.Round(curContentPoint.X - 10000, 4) + " Y : " + Math.Round(curContentPoint.Y - 10000, 4);
            Vector DragVector = curContentPoint - origContentMouseDownPoint;

            //MyDebug.WriteInformation("Object_MouseMove", "moved to " + curContentPoint.X.ToString() + " : " + curContentPoint.Y.ToString());

            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.

            origContentMouseDownPoint = curContentPoint;

            foreach (var item in Global.Objects)
            {
                if (item.Value is SomItem)
                {
                    var somitem = item.Value as SomItem;
                    somitem.Move(DragVector);
                }
                /*switch (Global.GetObjectType(item))
                {
                    case "Beam":

                        var beam = item.Value as Beam;

                        beam.Move(DragVector);

                        break;

                    default:

                        var support = item.Value as SupportItem;
                        Canvas.SetLeft(support, Canvas.GetLeft(support) + DragVector.X);
                        Canvas.SetTop(support, Canvas.GetTop(support) + DragVector.Y);
                        break;

                }*/
            }

            e.Handled = true;
        }

        public void CircleMouseDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;
            var fs = ellipse.Parent as FreeSupportItem;

            if (selectedbeam is null)
            {
                fs.Select();
            }
            else
            {
                foreach (var member in fs.Members)
                {
                    if (Equals(member.Beam, selectedbeam))
                    {
                        switch (member.Direction)
                        {
                            case Global.Direction.Left:
                                StartCircleMouseDown(member.Beam);
                                break;

                            case Global.Direction.Right:
                                EndCircleMouseDown(member.Beam);
                                break;
                        }
                    }
                }
            }

            e.Handled = true;
        }

        public void StartCircleMouseDown(Beam beam)
        {
            MesnetMDDebug.WriteInformation("Left circle selected");

            if (beam.IsSelected())
            {
                if (assemblybeam != null)
                {
                    if (!Equals(beam, assemblybeam))
                    {
                        //There should be a beam that either start circle or end circle selected. So, assemble this beam to that beam
                        switch (assemblybeam.circledirection)
                        {
                            case Global.Direction.Left:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.LeftSide != null && beam.LeftSide != null)
                                    {
                                        if (assemblybeam.LeftSide.Type != Global.ObjectType.LeftFixedSupport &&
                                            beam.LeftSide.Type != Global.ObjectType.LeftFixedSupport)
                                        {
                                            SetMouseHandlingMode("StartCircle_MouseDown",
                                                MouseHandlingMode.CircularBeamConnection);
                                            //Both beam is bound. This will be a circular beam system, so the user want to add a beam between 
                                            //two selected beam instead of connecting them
                                            var beamdialog = new BeamPrompt(assemblybeam.LeftPoint, beam.LeftPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.InertiaPpoly);
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.EPpoly);
                                                    newbeam.AddD((beamdialog.DPpoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Left);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Global.Direction.Right, beam, Global.Direction.Left);
                                                Notify("beamput");
                                                _uptoolbar.UpdateLoadDiagrams();
                                                _treehandler.UpdateAllBeamTree();
                                                _treehandler.UpdateAllSupportTree();
                                                Reset(MouseHandlingMode.CircularBeamConnection);
                                                return;
                                            }
                                            //If the user cancels the dialog, reser the system.
                                            Reset();
                                            return;
                                        }
                                    }
                                }
                                else if (assemblybeam.LeftSide is IRealSupportItem && beam.LeftSide is IRealSupportItem)
                                {
                                    //If both beam has support on selected sides do nothing
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.LeftSide != null || beam.LeftSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support
                                    beam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Left);
                                    Notify("beamput");
                                }

                                break;

                            case Global.Direction.Right:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.RightSide != null && beam.LeftSide != null)
                                    {
                                        if (assemblybeam.RightSide.Type != Global.ObjectType.RightFixedSupport &&
                                            beam.LeftSide.Type != Global.ObjectType.LeftFixedSupport)
                                        {
                                            SetMouseHandlingMode("StartCircle_MouseDown",
                                                MouseHandlingMode.CircularBeamConnection);
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            var beamdialog = new BeamPrompt(assemblybeam.RightPoint, beam.LeftPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.InertiaPpoly);
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.EPpoly);
                                                    newbeam.AddD((beamdialog.DPpoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Right);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Global.Direction.Right, beam, Global.Direction.Left); ;
                                                Notify("beamput");
                                                _uptoolbar.UpdateLoadDiagrams();
                                                _treehandler.UpdateAllBeamTree();
                                                _treehandler.UpdateAllSupportTree();
                                                Reset(MouseHandlingMode.CircularBeamConnection);
                                                return;
                                            }
                                            //If the user cancels the dialog, reser the system.
                                            Reset();
                                            return;
                                        }
                                    }
                                }
                                else if (assemblybeam.RightSide is IRealSupportItem && beam.LeftSide is IRealSupportItem)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.RightSide != null || beam.LeftSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support
                                    beam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Right);
                                    Notify("beamput");
                                }

                                break;
                        }
                        Reset();
                        _treehandler.UpdateAllBeamTree();
                        _treehandler.UpdateAllSupportTree();
                        return;
                    }
                }

                beam.SelectLeftCircle();
                circlelocation = beam.LeftPoint;
                assembly = true;
                assemblybeam = beam;

                if (beam.LeftSide != null)
                {
                    //Check if there is a fixed support bonded to beam's selected side. If there is, the user should not be able to put any support or beam to selected side, 
                    //so disable the fixed support button
                    switch (beam.LeftSide.Type)
                    {
                        case Global.ObjectType.LeftFixedSupport:
                            btnonlybeammode();
                            break;

                        case Global.ObjectType.BasicSupport:
                            btnbeamandnodalforcemode();
                            Notify("addbeam");
                            break;

                        case Global.ObjectType.SlidingSupport:
                            btnbeamandnodalforcemode();
                            Notify("addbeam");
                            break;
                        case Global.ObjectType.FictionalSupport:
                            btnfictionalsupportmode();
                            Notify("addbeamorsupport");
                            break;
                    }
                }
                else
                {
                    btnassemblymode();
                    Notify("selectsupport");
                }

                MesnetMDDebug.WriteInformation("Circle location = " + circlelocation.X + " : " + circlelocation.Y);

                selectedbeam = beam;
            }
        }

        public void EndCircleMouseDown(Beam beam)
        {
            MesnetMDDebug.WriteInformation("Right circle selected");

            if (beam.IsSelected())
            {
                if (assemblybeam != null)
                {
                    if (!Equals(beam, assemblybeam))
                    {
                        //There is a beam that either start circle or end circle selected. So, assemble this beam to that beam
                        switch (assemblybeam.circledirection)
                        {
                            case Global.Direction.Left:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.LeftSide != null && beam.RightSide != null)
                                    {
                                        if (assemblybeam.LeftSide.Type != Global.ObjectType.LeftFixedSupport &&
                                            beam.RightSide.Type != Global.ObjectType.RightFixedSupport)
                                        {
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            SetMouseHandlingMode("EndCircle_MouseDown", MouseHandlingMode.CircularBeamConnection);
                                            var beamdialog = new BeamPrompt(assemblybeam.LeftPoint, beam.RightPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.InertiaPpoly);
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.EPpoly);
                                                    newbeam.AddD((beamdialog.DPpoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Left);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Global.Direction.Right, beam, Global.Direction.Right);
                                                Notify("beamput");
                                                _uptoolbar.UpdateLoadDiagrams();
                                                _treehandler.UpdateAllBeamTree();
                                                _treehandler.UpdateAllSupportTree();
                                                Reset(MouseHandlingMode.CircularBeamConnection);
                                                return;
                                            }
                                            //If the user cancels the dialog, reser the system.
                                            Reset();
                                            return;
                                        }
                                    }
                                }
                                else if (assemblybeam.LeftSide is IRealSupportItem && beam.RightSide is IRealSupportItem)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.LeftSide != null || beam.RightSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support

                                    beam.Connect(Global.Direction.Right, assemblybeam, Global.Direction.Left);
                                    Notify("beamput");
                                }

                                break;

                            case Global.Direction.Right:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.RightSide != null && beam.RightSide != null)
                                    {
                                        if (assemblybeam.RightSide.Type != Global.ObjectType.RightFixedSupport &&
                                            beam.RightSide.Type != Global.ObjectType.RightFixedSupport)
                                        {
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            SetMouseHandlingMode("EndCircle_MouseDown", MouseHandlingMode.CircularBeamConnection);
                                            var beamdialog = new BeamPrompt(assemblybeam.RightPoint, beam.RightPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.InertiaPpoly);
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.EPpoly);
                                                    newbeam.AddD((beamdialog.DPpoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Global.Direction.Left, assemblybeam, Global.Direction.Right);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Global.Direction.Right, beam, Global.Direction.Right);
                                                Notify("beamput");
                                                _uptoolbar.UpdateLoadDiagrams();
                                                _treehandler.UpdateAllBeamTree();
                                                _treehandler.UpdateAllSupportTree();
                                                Reset(MouseHandlingMode.CircularBeamConnection);
                                                return;
                                            }
                                            //If the user cancels the dialog, reser the system.
                                            Reset();
                                            return;
                                        }
                                    }
                                }
                                else if (assemblybeam.RightSide != null && beam.RightSide != null)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.RightSide != null || beam.RightSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support                                                                        
                                    beam.Connect(Global.Direction.Right, assemblybeam, Global.Direction.Right);
                                    Notify("beamput");
                                }

                                break;
                        }
                        Reset();
                        _treehandler.UpdateAllBeamTree();
                        _treehandler.UpdateAllSupportTree();
                        return;
                    }
                }

                beam.SelectRightCircle();
                circlelocation = beam.RightPoint;
                assembly = true;
                assemblybeam = beam;

                if (beam.RightSide != null)
                {
                    //check if there is a fixed support bonded to beam's selected side. If there is the user should not put any support selected side, 
                    //so disable the fixed support button
                    switch (beam.RightSide.Type)
                    {
                        case Global.ObjectType.RightFixedSupport:
                            btnonlybeammode();
                            break;

                        case Global.ObjectType.BasicSupport:
                            btnbeamandnodalforcemode();
                            Notify("addbeam");
                            break;

                        case Global.ObjectType.SlidingSupport:
                            btnbeamandnodalforcemode();
                            Notify("addbeam");
                            break;
                        case Global.ObjectType.FictionalSupport:
                            btnfictionalsupportmode();
                            Notify("addbeamorsupport");
                            break;
                    }
                }
                else
                {
                    btnassemblymode();
                    Notify("selectsupport");
                }

                MesnetMDDebug.WriteInformation("Beam Left = " + Canvas.GetLeft(beam) + " : Beam Top = " + Canvas.GetTop(beam));

                MesnetMDDebug.WriteInformation("Circle location = " + circlelocation.X + " : " + circlelocation.Y);

                selectedbeam = beam;
            }
        }

        #endregion

        #region Support Events

        public void FreeSupportMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            mousedownpoint = e.GetPosition(canvas);
            e.Handled = true;
        }

        public void FreeSupportMouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            mouseuppoint = e.GetPosition(canvas);

            if (mouseuppoint.Equals(mousedownpoint))
            {
                MesnetMDDebug.WriteInformation("Free support clicked");

                var core = sender as Shape;
                var fs = core.Parent as FreeSupportItem;

                UnselectAll();
                _treehandler.UnSelectAllBeamItem();
                _treehandler.UnSelectAllSupportItem();
                fs.Select();
                _treehandler.SelectSupportItem(fs);
                selectesupport = fs;
                if (fs is FictionalSupport)
                {
                    if (fs.Members.Count > 1)
                    {
                        
                    }
                }
                btndisableall();
            }
            e.Handled = true;
        }

        public void FixedSupportMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            mousedownpoint = e.GetPosition(canvas);
            e.Handled = true;
        }

        public void FixedSupportMouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            mouseuppoint = e.GetPosition(canvas);

            if (mouseuppoint.Equals(mousedownpoint))
            {
                var core = sender as Polygon;
                var rf = core.Parent as RealFixedSupportItem;

                UnselectAll();
                _treehandler.UnSelectAllBeamItem();
                _treehandler.UnSelectAllSupportItem();
                rf.Select();
                _treehandler.SelectSupportItem(rf);
                selectesupport = rf;
                btndisableall();
            }
            e.Handled = true;
        } 

        #endregion

        #region Left Toolbar Button Events

        private void beambtn_Click(object sender, RoutedEventArgs e)
        {
            //Check if there are any beam in the canvas
            if (Global.Objects.Any(x => x.Value.Type is Global.ObjectType.Beam))
            {
                var beamdialog = new BeamPrompt();
                beamdialog.maxstresstbx.Text = _maxstress.ToString();
                beamdialog.Owner = this;
                if ((bool)beamdialog.ShowDialog())
                {
                    if (assembly)
                    {
                        //There must be a selected beam whose start circle or end circle is also selected. So place the beam to the circle location.
                        //Because we will immediately connect this beam, we must use the constructor with canvas.
                        var beam = new Beam(canvas, beamdialog.beamlength);

                        switch (selectedbeam.circledirection)
                        {
                            case Global.Direction.Left:

                                if (selectedbeam.LeftSide != null)
                                {
                                    if (selectedbeam.LeftSide.Type != Global.ObjectType.LeftFixedSupport)
                                    {
                                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                                        beam.AddInertia(beamdialog.InertiaPpoly);
                                        beam.AddArea(beamdialog.AreaPpoly);

                                        if ((bool)beamdialog.stresscbx.IsChecked)
                                        {
                                            beam.PerformStressAnalysis = true;
                                            beam.AddE(beamdialog.EPpoly);
                                            beam.AddD((beamdialog.DPpoly));
                                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                            beam.MaxAllowableStress = _maxstress;
                                        }
                                        beamangle = beamdialog.angle;
                                        beam.Connect(Global.Direction.Left, selectedbeam, Global.Direction.Left);
                                        beam.SetAngleLeft(beamangle);

                                        var fictionalsupport = new FictionalSupport(canvas);
                                        fictionalsupport.AddBeam(beam, Global.Direction.Right);

                                        Notify("beamput");
                                        ResetSolution();
                                        _uptoolbar.UpdateLoadDiagrams();
                                        _treehandler.UpdateAllBeamTree();
                                        _treehandler.UpdateAllSupportTree();
                                    }
                                }
                                else
                                {
                                    beam.Remove();
                                    Reset();
                                    return;
                                }

                                break;

                            case Global.Direction.Right:

                                if (selectedbeam.RightSide != null)
                                {
                                    if (selectedbeam.RightSide.Type != Global.ObjectType.RightFixedSupport)
                                    {
                                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                                        beam.AddInertia(beamdialog.InertiaPpoly);
                                        beam.AddArea(beamdialog.AreaPpoly);
                                        if ((bool)beamdialog.stresscbx.IsChecked)
                                        {
                                            beam.PerformStressAnalysis = true;
                                            beam.AddE(beamdialog.EPpoly);
                                            beam.AddD((beamdialog.DPpoly));
                                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                            beam.MaxAllowableStress = _maxstress;
                                        }
                                        beamangle = beamdialog.angle;
                                        beam.Connect(Global.Direction.Left, selectedbeam, Global.Direction.Right);
                                        beam.SetAngleLeft(beamangle);

                                        var fictionalsupport = new FictionalSupport(canvas);
                                        fictionalsupport.AddBeam(beam, Global.Direction.Right);

                                        Notify("beamput");
                                        ResetSolution();
                                        _uptoolbar.UpdateLoadDiagrams();
                                        _treehandler.UpdateAllBeamTree();
                                        _treehandler.UpdateAllSupportTree();
                                    }
                                }
                                else
                                {
                                    beam.Remove();
                                    Reset();
                                    return;
                                }

                                break;
                        }

                        //UpdateBeamTree(beam);
                        Reset();
                    }
                    else
                    {
                        //There is no start or end circle selected beam in the canvas. So place the beam where the user want.
                        var beam = new Beam(beamdialog.beamlength);
                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                        beam.AddInertia(beamdialog.InertiaPpoly);
                        beam.AddArea(beamdialog.AreaPpoly);
                        if ((bool)beamdialog.stresscbx.IsChecked)
                        {
                            beam.PerformStressAnalysis = true;
                            beam.AddE(beamdialog.EPpoly);
                            beam.AddD((beamdialog.DPpoly));
                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                            beam.MaxAllowableStress = _maxstress;
                        }
                        beamangle = beamdialog.angle;

                        tempbeam = beam;
                        SetMouseHandlingMode("beambtn_Click", MouseHandlingMode.BeamPlacing);
                        Notify("clickforbeam");
                    }
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                //There are no beam in the canvas. 
                //So, the user wants to put the beam wherever he want and he will connect it later.
                var beamdialog = new BeamPrompt();
                beamdialog.maxstresstbx.Text = _maxstress.ToString();
                beamdialog.Owner = this;
                if ((bool)beamdialog.ShowDialog())
                {
                    var beam = new Beam(beamdialog.beamlength);
                    beam.AddElasticity(beamdialog.beamelasticitymodulus);
                    beam.AddInertia(beamdialog.InertiaPpoly);
                    beam.AddArea(beamdialog.AreaPpoly);
                    if ((bool)beamdialog.stresscbx.IsChecked)
                    {
                        beam.PerformStressAnalysis = true;
                        beam.AddE(beamdialog.EPpoly);
                        beam.AddD((beamdialog.DPpoly));
                        _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                        beam.MaxAllowableStress = _maxstress;
                    }
                    beamangle = beamdialog.angle;
                    tempbeam = beam;
                    SetMouseHandlingMode("beambtn_Click", MouseHandlingMode.BeamPlacing);
                    Notify("clickforbeam");
                }
                else
                {
                    Reset();
                }
            }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsdialog = new SettingsPrompt();
            settingsdialog.Owner = this;

            settingsdialog.ShowDialog();
        }

        private void concentratedloadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var concentratedloadprompt = new ConcentratedLoadPrompt(selectedbeam);
                concentratedloadprompt.Owner = this;

                if ((bool)concentratedloadprompt.ShowDialog())
                {
                    selectedbeam.RemoveConcentratedLoad();
                    var load = concentratedloadprompt.Loads;
                    selectedbeam.AddLoad(load);

                    _uptoolbar.UpdateConcloadDiagrams();

                    Notify("concloadput");
                    ResetSolution();
                    _treehandler.UpdateBeamTree(selectedbeam);
                    assemblybeam = null;
                    assembly = false;
                    UnselectAll();
                    btndisableall();
                    SetMouseHandlingMode("concentratedloadbtn_Click", MouseHandlingMode.None);
                }
                else
                {
                    Reset();
                }
            }
        }

        private void distributedloadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var distloadprompt = new DistributedLoadPrompt(selectedbeam);
                distloadprompt.Owner = this;

                if ((bool)distloadprompt.ShowDialog())
                {
                    selectedbeam.RemoveDistributedLoad();
                    var ppoly = new PiecewisePoly(distloadprompt.Loadpolies);
                    selectedbeam.AddLoad(ppoly);

                    _uptoolbar.UpdateDistloadDiagrams();

                    Notify("distloadput");
                    ResetSolution();
                    _treehandler.UpdateBeamTree(selectedbeam);
                    assemblybeam = null;
                    assembly = false;
                    UnselectAll();
                    btndisableall();
                    SetMouseHandlingMode("distributedloadbtn_Click", MouseHandlingMode.None);
                }
                else
                {
                    Reset();
                }
            }
        }

        private void zoominbtn_Click(object sender, RoutedEventArgs e)
        {
            zoomAndPanControl.AnimatedZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
            Notify("zoomedin");
        }

        private void zoomoutbtn_Click(object sender, RoutedEventArgs e)
        {
            zoomAndPanControl.AnimatedZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
            Notify("zoomedout");
        }

        private void fixedsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                switch (selectedbeam.circledirection)
                {
                    case Global.Direction.Left:
                        if (selectedbeam.LeftSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.LeftSide as FictionalSupport;
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        var leftfixedsupport = new LeftFixedSupport(canvas);
                        leftfixedsupport.AddBeam(selectedbeam);
                        Notify("fixedsupportput");
                        break;

                    case Global.Direction.Right:
                        if (selectedbeam.RightSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.RightSide as FictionalSupport;
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        var rightfixedsupport = new RightFixedSupport(canvas);
                        rightfixedsupport.AddBeam(selectedbeam);
                        Notify("fixedsupportput");
                        break;

                    default:

                        MesnetMDDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                _treehandler.UpdateAllSupportTree();
                _treehandler.UpdateAllBeamTree();
                SetMouseHandlingMode("fixedsupportbtn_Click", MouseHandlingMode.None);
            }
            else
            {
                MesnetMDDebug.WriteWarning("selected beam is null!");
            }

            Reset();
        }

        private void basicsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var basicsupport = new BasicSupport(canvas);

                switch (selectedbeam.circledirection)
                {
                    case Global.Direction.Left:

                        if (selectedbeam.LeftSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.LeftSide as FictionalSupport;
                            foreach (var member in fs.Members)
                            {
                                if (!basicsupport.Members.Contains(member))
                                {
                                    basicsupport.AddBeam(member.Beam, member.Direction);
                                }
                            }
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        else
                        {
                            basicsupport.AddBeam(selectedbeam, Global.Direction.Left);
                        }                     

                        break;

                    case Global.Direction.Right:

                        if (selectedbeam.RightSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.RightSide as FictionalSupport;
                            foreach (var member in fs.Members)
                            {
                                if (!basicsupport.Members.Contains(member))
                                {
                                    basicsupport.AddBeam(member.Beam, member.Direction);
                                }
                            }
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        else
                        {
                            basicsupport.AddBeam(selectedbeam, Global.Direction.Right);
                        }

                        break;

                    default:

                        MesnetMDDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                Notify("basicsupportput");
                SetMouseHandlingMode("basicsupportbtn_Click", MouseHandlingMode.None);
                _treehandler.UpdateAllSupportTree();
                _treehandler.UpdateAllBeamTree();
            }
            else
            {
                MesnetMDDebug.WriteWarning("selected beam is null!");
            }
            Reset();
        }

        private void slidingsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var slidingsupport = new SlidingSupport(canvas);

                switch (selectedbeam.circledirection)
                {
                    case Global.Direction.Left:

                        if (selectedbeam.LeftSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.LeftSide as FictionalSupport;
                            foreach (var member in fs.Members)
                            {
                                if (!slidingsupport.Members.Contains(member))
                                {
                                    slidingsupport.AddBeam(member.Beam, member.Direction);
                                }
                            }
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        else
                        {
                            slidingsupport.AddBeam(selectedbeam, Global.Direction.Left);
                        }

                        break;

                    case Global.Direction.Right:

                        if (selectedbeam.RightSide.Type is Global.ObjectType.FictionalSupport)
                        {
                            var fs = selectedbeam.RightSide as FictionalSupport;
                            foreach (var member in fs.Members)
                            {
                                if (!slidingsupport.Members.Contains(member))
                                {
                                    slidingsupport.AddBeam(member.Beam, member.Direction);
                                }
                            }
                            _treehandler.RemoveSupportTree(fs);
                            Global.RemoveObject(fs);
                        }
                        else
                        {
                            slidingsupport.AddBeam(selectedbeam, Global.Direction.Right);
                        }

                        break;

                    default:

                        MesnetMDDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                SetMouseHandlingMode("slidingsupportbtn_Click", MouseHandlingMode.None);
                Notify("slidingsupportput");
                _treehandler.UpdateAllSupportTree();
                _treehandler.UpdateAllBeamTree();
            }
            else
            {
                MesnetMDDebug.WriteWarning("selected beam is null!");
            }
            Reset();
        }

        private void rotatebtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (selectedbeam != null)
            {
                if (selectedbeam.LeftSide != null && selectedbeam.RightSide != null && selectedbeam.IsBound)
                {
                    return;
                }

                var rotateprompt = new RotatePrompt();
                rotateprompt.Owner = this;
                if (selectedbeam.LeftSide != null || selectedbeam.RightSide != null)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        if (selectedbeam.LeftSide == null && selectedbeam.RightSide != null)
                        {
                            selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));

                            switch (GetObjectType(selectedbeam.RightSide))
                            {
                                case ObjectType.RightFixedSupport:

                                    var rs = selectedbeam.RightSide as RightFixedSupport;
                                    rs.UpdatePosition(selectedbeam);

                                    break;

                                case ObjectType.SlidingSupport:

                                    var ss = selectedbeam.RightSide as SlidingSupport;
                                    if (ss.Members.Count == 1)
                                    {
                                        ss.UpdatePosition(selectedbeam);
                                    }

                                    break;

                                case ObjectType.BasicSupport:

                                    var bs = selectedbeam.RightSide as BasicSupport;
                                    if (bs.Members.Count == 1)
                                    {
                                        bs.UpdatePosition(selectedbeam);
                                    }

                                    break;
                            }
                        }
                        else if (selectedbeam.RightSide == null && selectedbeam.LeftSide != null)
                        {
                            selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));

                            switch (selectedbeam.LeftSide.GetType().Name)
                            {
                                case "LeftFixedSupport":

                                    var rs = selectedbeam.LeftSide as LeftFixedSupport;
                                    rs.UpdatePosition(selectedbeam);

                                    break;

                                case "SlidingSupport":

                                    var ss = selectedbeam.LeftSide as SlidingSupport;
                                    if (ss.Members.Count == 1)
                                    {
                                        ss.UpdatePosition(selectedbeam);
                                    }

                                    break;

                                case "BasicSupport":

                                    var bs = selectedbeam.LeftSide as BasicSupport;
                                    if (bs.Members.Count == 1)
                                    {
                                        bs.UpdatePosition(selectedbeam);
                                    }

                                    break;
                            }
                        }
                        else if (selectedbeam.RightSide != null && selectedbeam.LeftSide != null && !selectedbeam.IsBound)
                        {
                            if (selectedbeam.IsLeftSelected)
                            {
                                selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                            else if (selectedbeam.IsRightSelected)
                            {
                                selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                            else
                            {
                                selectedbeam.SetAngleCenter(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                        }
                    }
                }
                else if (selectedbeam.IsLeftSelected)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
                else if (selectedbeam.IsRightSelected)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
                else
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleCenter(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
            }
            */
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MesnetMDDebug.WriteInformation("Esc key down");
                Reset();
                MesnetMDDebug.WriteInformation("mouse handling mode has been set to None");
            }
            else if (e.Key == Key.Delete)
            {
                MesnetMDDebug.WriteInformation("Delete key down");
                if (selectedbeam != null)
                {
                    handledeletebeam();
                }
                else if (selectesupport != null)
                {
                    hendledeletesupport();
                }
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = Math.Round(Convert.ToDouble(e.NewValue), 3);
            zoomAndPanControl.ZoomAboutPoint(value, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
            Global.Scale = value;
            scaletext.Text = value.ToString();
        }

        private void scaletext_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var value = Math.Round(Convert.ToDouble(scaletext.Text), 3);
                scaletext.Text = value.ToString();

                var timer = new DispatcherTimer();

                timer.Interval = TimeSpan.FromSeconds(zoomAndPanControl.AnimationDuration);

                if (value > 10)
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(10, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));

                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Global.Scale = 10;
                        scaleslider.Value = Global.Scale;
                    };
                }
                else if (value < 0)
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(0, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));

                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Global.Scale = 0;
                        scaleslider.Value = Global.Scale;
                    };
                }
                else
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(value, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Global.Scale = value;
                        scaleslider.Value = Global.Scale;
                    };
                }

                //timer.Start();
            }
            catch (Exception)
            { }
        }

        #endregion

        /// <summary>
        /// Checks the beam whether it is connected to other beams.
        /// </summary>
        /// <param name="beam">The beam.</param>
        /// <returns>False if the beam is connected to the other beams from both sides.</returns>
        private bool checkbeam(Beam beam)
        {
            if (beam.LeftSide == null || beam.RightSide == null)
            {
                return true;
            }

            if (beam.LeftSide != null)
            {
                switch (beam.LeftSide.Type)
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = beam.LeftSide as BasicSupport;
                        if (bs.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = beam.LeftSide as SlidingSupport;
                        if (ss.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case Global.ObjectType.LeftFixedSupport:

                        return true;
                }
            }

            if (beam.RightSide != null)
            {
                switch (beam.RightSide.Type)
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = beam.RightSide as BasicSupport;
                        if (bs.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = beam.RightSide as SlidingSupport;
                        if (ss.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case Global.ObjectType.RightFixedSupport:

                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Executed when a beam double-clicked.
        /// It arranges the beam.
        /// </summary>
        /// <param name="beam">The beam.</param>
        private void handlebeamdoubleclick(Beam beam)
        {
            var isfree = checkbeam(beam);
            var beamdialog = new BeamPrompt(beam, isfree);
            beamdialog.maxstresstbx.Text = _maxstress.ToString();
            beamdialog.Owner = this;
            if ((bool)beamdialog.ShowDialog())
            {
                if (isfree)
                {
                    //If the beam is free which means at least at one side it is not bounded to a beam, its length could be changed. 
                    //So, the beam should be moved toward the side that is free and its support should also be moved.
                    bool handled = false;
                    if (beam.RightSide != null)
                    {
                        switch (beam.RightSide.Type)
                        {
                            case Global.ObjectType.BasicSupport:
                                {
                                    var bs = beam.RightSide as BasicSupport;
                                    if (bs.Members.Count == 1)
                                    {
                                        //The beam is free on the right side
                                        beam.SetAngleLeft(beamdialog.angle);
                                        beam.ChangeLength(beamdialog.beamlength);
                                        beam.MoveSupports();
                                        handled = true;
                                    }
                                }
                                break;

                            case Global.ObjectType.SlidingSupport:
                                {
                                    var ss = beam.RightSide as SlidingSupport;
                                    if (ss.Members.Count == 1)
                                    {
                                        //The beam is free on the right side
                                        beam.SetAngleLeft(beamdialog.angle);
                                        beam.ChangeLength(beamdialog.beamlength);
                                        beam.MoveSupports();
                                        handled = true;
                                    }
                                }
                                break;

                            case Global.ObjectType.RightFixedSupport:
                                {
                                    //The beam is free on the right side
                                    beam.SetAngleLeft(beamdialog.angle);
                                    beam.ChangeLength(beamdialog.beamlength);
                                    beam.MoveSupports();
                                    handled = true;
                                }
                                break;
                        }
                    }

                    if (!handled)
                    {
                        if (beam.LeftSide != null)
                        {
                            switch (beam.LeftSide.Type)
                            {
                                case Global.ObjectType.BasicSupport:
                                    {
                                        var bs = beam.LeftSide as BasicSupport;
                                        if (bs.Members.Count == 1)
                                        {
                                            //The beam is free on the left side
                                            beam.SetAngleRight(beamdialog.angle);
                                            Point oldpoint = new Point(beam.RightPoint.X, beam.RightPoint.Y);
                                            beam.ChangeLength(beamdialog.beamlength);
                                            Vector delta = new Vector();
                                            delta.X = oldpoint.X - beam.RightPoint.X;
                                            delta.Y = oldpoint.Y - beam.RightPoint.Y;
                                            beam.Move(delta);
                                            beam.MoveSupports();
                                        }
                                    }
                                    break;

                                case Global.ObjectType.SlidingSupport:
                                    {
                                        var ss = beam.LeftSide as SlidingSupport;
                                        if (ss.Members.Count == 1)
                                        {
                                            //The beam is free on the left side
                                            beam.SetAngleRight(beamdialog.angle);
                                            Point oldpoint = new Point(beam.RightPoint.X, beam.RightPoint.Y);
                                            beam.ChangeLength(beamdialog.beamlength);
                                            Vector delta = new Vector();
                                            delta.X = oldpoint.X - beam.RightPoint.X;
                                            delta.Y = oldpoint.Y - beam.RightPoint.Y;
                                            beam.Move(delta);
                                            beam.MoveSupports();
                                        }
                                    }
                                    break;

                                case Global.ObjectType.LeftFixedSupport:
                                    {
                                        //The beam is free on the left side
                                        beam.SetAngleRight(beamdialog.angle);
                                        Point oldpoint = new Point(beam.RightPoint.X, beam.RightPoint.Y);
                                        beam.ChangeLength(beamdialog.beamlength);
                                        Vector delta = new Vector();
                                        delta.X = oldpoint.X - beam.RightPoint.X;
                                        delta.Y = oldpoint.Y - beam.RightPoint.Y;
                                        beam.Move(delta);
                                        beam.MoveSupports();
                                    }
                                    break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        if (beam.LeftSide == null && beam.RightSide == null)
                        {
                            //The beam is free on both sides
                            beam.SetAngleCenter(beamdialog.angle);
                            beam.ChangeLength(beamdialog.beamlength);
                            beam.MoveSupports();
                            handled = true;
                        }
                    }

                    if (!handled)
                    {
                        if (beam.RightSide == null)
                        {
                            //The beam is free on the right side
                            beam.SetAngleLeft(beamdialog.angle);
                            beam.ChangeLength(beamdialog.beamlength);
                            beam.MoveSupports();
                            handled = true;
                        }
                    }

                    if (!handled)
                    {
                        if (beam.LeftSide == null)
                        {
                            //The beam is free on the left side
                            beam.SetAngleRight(beamdialog.angle);
                            Point oldpoint = new Point(beam.RightPoint.X, beam.RightPoint.Y);
                            beam.ChangeLength(beamdialog.beamlength);
                            Vector delta = new Vector();
                            delta.X = oldpoint.X - beam.RightPoint.X;
                            delta.Y = oldpoint.Y - beam.RightPoint.Y;
                            beam.Move(delta);
                            beam.MoveSupports();
                        }
                    }
                }
                beam.ShowCorners(3, 5);
                beam.AddElasticity(beamdialog.beamelasticitymodulus);
                beam.ChangeInertia(beamdialog.InertiaPpoly);

                if ((bool)beamdialog.stresscbx.IsChecked)
                {
                    beam.PerformStressAnalysis = true;
                    beam.AddE(beamdialog.EPpoly);
                    beam.AddD((beamdialog.DPpoly));
                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                    beam.MaxAllowableStress = _maxstress;
                }

                ResetSolution();
                _uptoolbar.UpdateAllDiagrams();
                Notify("beamarranged");
                _treehandler.UpdateAllBeamTree();
                _treehandler.UpdateAllSupportTree();
                Reset();
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// Deletes selected beam.
        /// </summary>
        private void handledeletebeam()
        {
            if (selectedbeam != null)
            {
                if (askfordelete("askforbeamdelete"))
                {
                    if (selectedbeam.RightSide != null)
                    {
                        switch (selectedbeam.RightSide.Type)
                        {
                            case Global.ObjectType.BasicSupport:
                                var bsr = selectedbeam.RightSide as BasicSupport;
                                if (bsr.Members.Count > 1)
                                {
                                    bsr.RemoveBeam(selectedbeam);
                                }
                                else
                                {
                                    deleteSupport(bsr);
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:
                                var ssr = selectedbeam.RightSide as SlidingSupport;
                                if (ssr.Members.Count > 1)
                                {
                                    ssr.RemoveBeam(selectedbeam);
                                }
                                else
                                {
                                    deleteSupport(ssr);
                                }

                                break;

                            case Global.ObjectType.RightFixedSupport:

                                var rs = selectedbeam.RightSide as RightFixedSupport;
                                deleteSupport(rs);

                                break;
                        }
                    }

                    if (selectedbeam.LeftSide != null)
                    {
                        switch (selectedbeam.LeftSide.Type)
                        {
                            case Global.ObjectType.BasicSupport:
                                var bsl = selectedbeam.LeftSide as BasicSupport;
                                if (bsl.Members.Count > 1)
                                {
                                    bsl.RemoveBeam(selectedbeam);
                                }
                                else
                                {
                                    deleteSupport(bsl);
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:
                                var ssl = selectedbeam.LeftSide as SlidingSupport;
                                if (ssl.Members.Count > 1)
                                {
                                    ssl.RemoveBeam(selectedbeam);
                                }
                                else
                                {
                                    deleteSupport(ssl);
                                }

                                break;

                            case Global.ObjectType.LeftFixedSupport:

                                var ls = selectedbeam.LeftSide as RightFixedSupport;
                                deleteSupport(ls);

                                break;
                        }
                    }

                    deleteBeam(selectedbeam);
                    UnselectAll();
                    Notify("beamdeleted");
                    ResetSolution();
                    _uptoolbar.UpdateAllDiagrams();
                    _treehandler.UpdateAllBeamTree();
                    _treehandler.UpdateAllSupportTree();
                }
            }
        }

        private void hendledeletesupport()
        {
            if (selectesupport != null)
            {
                switch (selectesupport.Type)
                {
                    case Global.ObjectType.BasicSupport:
                        var bs = selectesupport as BasicSupport;
                        if (bs.Members.Count == 1)
                        {
                            if (askfordelete("askforsupportdelete"))
                            {
                                var beam = bs.Members.First().Beam;
                                switch (bs.Members.First().Direction)
                                {
                                    case Global.Direction.Left:
                                        beam.LeftSide = null;
                                        break;

                                    case Global.Direction.Right:
                                        beam.RightSide = null;
                                        break;
                                }
                                deleteSupport(selectesupport);
                                UnselectAll();
                                Notify("supportdeleted");
                                ResetSolution();
                                _uptoolbar.UpdateAllDiagrams();
                                _treehandler.UpdateAllBeamTree();
                                _treehandler.UpdateAllSupportTree();
                            }
                            else
                            {
                                UnselectAll();
                            }
                        }
                        else
                        {
                            Notify("supportcantdeleted");
                            UnselectAll();
                        }

                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = selectesupport as SlidingSupport;
                        if (ss.Members.Count == 1)
                        {
                            if (askfordelete("askforsupportdelete"))
                            {
                                var beam = ss.Members.First().Beam;
                                switch (ss.Members.First().Direction)
                                {
                                    case Global.Direction.Left:
                                        beam.LeftSide = null;
                                        break;

                                    case Global.Direction.Right:
                                        beam.RightSide = null;
                                        break;
                                }
                                deleteSupport(selectesupport);
                                UnselectAll();
                                Notify("supportdeleted");
                                ResetSolution();
                                _uptoolbar.UpdateAllDiagrams();
                                _treehandler.UpdateAllBeamTree();
                                _treehandler.UpdateAllSupportTree();
                            }
                            else
                            {
                                UnselectAll();
                            }
                        }
                        else
                        {
                            Notify("supportcantdeleted");
                            UnselectAll();
                        }
                        break;

                    case Global.ObjectType.LeftFixedSupport:
                        if (askfordelete("askforsupportdelete"))
                        {
                            var ls = selectesupport as LeftFixedSupport;
                            var beam = ls.Member.Beam;
                            switch (ls.Member.Direction)
                            {
                                case Global.Direction.Left:
                                    beam.LeftSide = null;
                                    break;

                                case Global.Direction.Right:
                                    beam.RightSide = null;
                                    break;
                            }
                            deleteSupport(selectesupport);
                            UnselectAll();
                            Notify("supportdeleted");
                            ResetSolution();
                            _uptoolbar.UpdateAllDiagrams();
                            _treehandler.UpdateAllBeamTree();
                            _treehandler.UpdateAllSupportTree();
                        }
                        else
                        {
                            UnselectAll();
                        }
                        break;

                    case Global.ObjectType.RightFixedSupport:
                        if (askfordelete("askforsupportdelete"))
                        {
                            var rs = selectesupport as RightFixedSupport;
                            var beam = rs.Member.Beam;
                            switch (rs.Member.Direction)
                            {
                                case Global.Direction.Left:
                                    beam.LeftSide = null;
                                    break;

                                case Global.Direction.Right:
                                    beam.RightSide = null;
                                    break;
                            }
                            deleteSupport(selectesupport);
                            UnselectAll();
                            Notify("supportdeleted");
                            ResetSolution();
                            _uptoolbar.UpdateAllDiagrams();
                            _treehandler.UpdateAllBeamTree();
                            _treehandler.UpdateAllSupportTree();
                        }
                        else
                        {
                            UnselectAll();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Prompts a message window to user whether he/she really wants to delete the beam.
        /// </summary>
        /// <returns>true if user accept to delete the specified beam, otherwise false</returns>
        private bool askfordelete(string messagekey)
        {
            var prompt = new MessagePrompt(Global.GetString(messagekey));
            prompt.Owner = this;
            if ((bool)prompt.ShowDialog())
            {
                switch (prompt.Result)
                {
                    case Classes.Global.DialogResult.Yes:
                        //The user has accepted to delete the beam           
                        return true;

                    case Classes.Global.DialogResult.No:
                        //The user dont want to delete the beam
                        return false;

                    case Classes.Global.DialogResult.Cancel:
                        //The user cancelled the dialog, abort the operation
                        return false;

                    case Classes.Global.DialogResult.None:
                        //Something we dont know happened, abort the operation
                        return false;
                }
            }
            return false;
        }

        private void deleteBeam(Beam beam)
        {
            _treehandler.RemoveBeamTree(beam);
            canvas.Children.Remove(beam);
            Global.RemoveObject(beam);
            //Global.Objects.Remove(beam);
            MesnetMDDebug.WriteInformation(beam.Name + " deleted");
        }

        private void deleteSupport(object support)
        {
            /*_treehandler.RemoveSupportTree(support);
            switch (Global.GetObjectType(support))
            {
                case Global.ObjectType.BasicSupport:
                    var bs = support as BasicSupport;
                    canvas.Children.Remove(bs);
                    MyDebug.WriteInformation(bs.Name + " deleted");

                    break;

                case Global.ObjectType.SlidingSupport:
                    var ss = support as SlidingSupport;
                    canvas.Children.Remove(ss);
                    MyDebug.WriteInformation(ss.Name + " deleted");
                    break;

                case Global.ObjectType.LeftFixedSupport:
                    var ls = support as LeftFixedSupport;
                    canvas.Children.Remove(ls);
                    MyDebug.WriteInformation(ls.Name + " deleted");
                    break;

                case Global.ObjectType.RightFixedSupport:
                    var rs = support as RightFixedSupport;
                    canvas.Children.Remove(rs);
                    MyDebug.WriteInformation(rs.Name + " deleted");
                    break;
            }*/
        }

        #region Button Modes

        /// <summary>
        /// Only beam button is enabled.
        /// </summary>
        private void btndisableall()
        {
            //beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        /// <summary>
        /// Enables load buttons.
        /// </summary>
        private void btnloadmode()
        {
            //beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = true;
            distributedloadbtn.IsEnabled = true;
        }

        private void btnassemblymode()
        {
            //assembly = true;
            beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = true;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        /// <summary>
        /// Enables only beam button.
        /// </summary>
        private void btnonlybeammode()
        {
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
            directloadbtn.IsEnabled = false;
        }

        private void btnbeamandnodalforcemode()
        {
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
            directloadbtn.IsEnabled = true;
        }
        
        /// <summary>
        /// Enables only beam and support buttons.
        /// </summary>
        private void btnfictionalsupportmode()
        {
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = true;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
            directloadbtn.IsEnabled = true;
        }

        private void btnfreesupportmode()
        {
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = true;
        }


        /// <summary>
        /// Enables only beam and free support buttons. 
        /// Is being called when clicked on a fictional support who has more than one beams
        /// </summary>
        private void btninsidefictionalsupportmode()
        {
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
            directloadbtn.IsEnabled = true;
        }

        private void supportonlymode()
        {
            beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = true;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        private void btnenablerotation()
        {
            rotatebtn.IsEnabled = true;
        }

        private void btndisablerotation()
        {
            rotatebtn.IsEnabled = false;
        }

        #endregion

        /// <summary>
        /// Unselects all the Objects in the canvas.
        /// </summary>
        private void UnselectAll()
        {
            foreach (var item in Global.Objects)
            {
                var som = item.Value;
                som.UnSelect();              
            }
            selectedbeam = null;
            selectesupport = null;
        }

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
                        maxZ = Math.Max(maxZ, zIndex);
                        if (zIndex > currentIndex)
                        {
                            Canvas.SetZIndex(child, zIndex - 1);
                        }
                    }
                }
                Canvas.SetZIndex(pToMove, maxZ);

                Canvas.SetZIndex(viewbox, maxZ + 1);
            }
            catch (Exception ex)
            {
            }
        }

        #region Beam and Suppport Tree Events and Functions

        /// <summary>
        /// Shows or hides the direction arrow on the beam
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void arrow_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var beamitem = showbuttonitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.DirectionShown)
            {
                beam.ShowDirectionArrow();
                uc.content.Content = Global.GetString("hidedirection");
            }
            else
            {
                beam.HideDirectionArrow();
                uc.content.Content = Global.GetString("showdirection");
            }
        }

        #endregion

        private void bwupdate_DoWork(object sender, DoWorkEventArgs e)
        {
            Global.SetDecimalSeperator();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _treehandler.UpdateAllBeamTree();
                _treehandler.UpdateAllSupportTree();
            }));
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        #region Menubar SOM Graphics and Functions

        public void fixedendforceexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as MomentExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as MomentItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var forcevalue = Math.Round(beam.FixedEndForce.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = forcevalue.ToString();
            }
            catch (Exception)
            { }
        }

        public void fixedendmomentexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as MomentExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as MomentItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var momentvalue = Math.Round(beam.FixedEndMoment.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = momentvalue.ToString();
            }
            catch (Exception)
            { }
        }

        public void stressexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as StressExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as StressItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var stressvalue = Math.Round(beam.Stress.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = stressvalue.ToString();
            }
            catch (Exception)
            { }
        }

        public void distloadmousemove(object sender, MouseEventArgs e)
        {
            DistributedLoad load = (sender as CardinalSplineShape).Parent as DistributedLoad;
            var mousepoint = e.GetPosition(load);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(load.LoadPpoly.Calculate(mousepoint.X / 100), 4) + " kN/m";
            viewbox.Height = 20 / Global.Scale;
        }

        public void inertiamousemove(object sender, MouseEventArgs e)
        {
            Inertia inertia = (sender as CardinalSplineShape).Parent as Inertia;
            var mousepoint = e.GetPosition(inertia);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(inertia.InertiaPpoly.Calculate(mousepoint.X / 100), 4) + " cm^4";
            viewbox.Height = 20 / Global.Scale;
        }

        public void areamousemove(object sender, MouseEventArgs e)
        {
            Area area = (sender as CardinalSplineShape).Parent as Area;
            var mousepoint = e.GetPosition(area);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(area.AreaPpoly.Calculate(mousepoint.X / 100), 4) + " cm^2";
            viewbox.Height = 20 / Global.Scale;
        }

        public void momentmousemove(object sender, MouseEventArgs e)
        {
            Moment moment = (sender as CardinalSplineShape).Parent as Moment;
            var mousepoint = e.GetPosition(moment);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(moment.MomentPpoly.Calculate(mousepoint.X / 100), 4) + " kNm";
            viewbox.Height = 20 / Global.Scale;
        }

        public void forcemousemove(object sender, MouseEventArgs e)
        {
            ShearForce shearForce = (sender as CardinalSplineShape).Parent as ShearForce;
            var mousepoint = e.GetPosition(shearForce);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(shearForce.ShearForcePpoly.Calculate(mousepoint.X / 100), 4) + " kN";
            viewbox.Height = 20 / Global.Scale;
        }

        public void axialforcemousemove(object sender, MouseEventArgs e)
        {
            AxialForce axialForce = (sender as CardinalSplineShape).Parent as AxialForce;
            var mousepoint = e.GetPosition(axialForce);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(axialForce.AxialForcePpoly.Calculate(mousepoint.X / 100), 4) + " kN";
            viewbox.Height = 20 / Global.Scale;
        }

        public void stressmousemove(object sender, MouseEventArgs e)
        {
            Stress stress = (sender as CardinalSplineShape).Parent as Stress;
            var mousepoint = e.GetPosition(stress);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(stress.Calculate(mousepoint.X / 100), 4) + " MPa";
            viewbox.Height = 20 / Global.Scale;
        }

        public void mouseenter(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Visible;
        }

        public void mouseleave(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Collapsed;
        }

        #endregion

        /// <summary>
        /// Resets the system.
        /// </summary>
        public void Reset()
        {
            tempbeam = null;
            assemblybeam = null;
            assembly = false;
            UnselectAll();
            btndisableall();
            _treehandler.UnSelectAllBeamItem();
            _treehandler.UnSelectAllSupportItem();
            SetMouseHandlingMode("Reset", MouseHandlingMode.None);
        }

        /// <summary>
        /// Resets the system and sets specified mouse mode.
        /// </summary>
        private void Reset(MouseHandlingMode mousemode)
        {
            tempbeam = null;
            assemblybeam = null;
            assembly = false;
            UnselectAll();
            btndisableall();
            SetMouseHandlingMode("Reset", mousemode);
        }

        /// <summary>
        /// Selects the given beam.
        /// </summary>
        /// <param name="beam">The beam to be selected.</param>
        public void SelectBeam(Beam beam)
        {
            beam.Select();
            selectedbeam = beam;

            //BringToFront(canvas, beam);
            btnloadmode();

            if (selectedbeam.LeftSide != null && selectedbeam.RightSide != null && selectedbeam.IsBound)
            {
                btndisablerotation();
            }
            else
            {
                btnenablerotation();
            }
        }

        #region Cross Solve

        /// <summary>
        /// Initializes the cross solution
        /// </summary>
        private void Calculate()
        {
            //CrossSolve concurently executes Calculate in every beam.
            var crossdialog = new CrossSolve(this);
            crossdialog.Owner = this;
            notify.Text = "Solving";
            if ((bool)crossdialog.ShowDialog())
            {
                Notify("solved");
                if (Global.BeamCount > 1)
                {
                    UpdateBeams();
                }
                else
                {
                    UpdateBeam();
                }
                Logger.CloseLogger();
                _treehandler.UpdateAllBeamTree();
                _treehandler.UpdateAllSupportTree();
            }
        }

        public void CrossLoop()
        {
            /*int step = 0;
            bool stop = false;
            List<bool> checklist = new List<bool>();

            Logger.NextLine();
            Logger.WriteLine("Cross Solver Initialized");
            Logger.NextLine();

            while (!stop)
            {
                checklist.Clear();
                MyDebug.WriteInformation("Step : " + step);
                Logger.WriteLine("**********************************************STEP : " + step + "*************************************************");
                Logger.NextLine();

                if (step % 2 == 0)
                {
                    foreach (var support in Global.Objects)
                    {
                        switch (Global.GetObjectType(support))
                        {
                            case Global.ObjectType.BasicSupport:

                                var bs = support as BasicSupport;

                                if (bs.CrossIndex % 2 == 0 && bs.CrossIndex <= step && bs.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + bs.MomentDifference + " at BasicSupport, " + bs.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(bs.Name + " : cross index = " + bs.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(bs.Seperate());
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var ss = support as SlidingSupport;

                                if (ss.CrossIndex % 2 == 0 && ss.CrossIndex <= step && ss.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + ss.MomentDifference + " at SlidingSupport, " + ss.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(ss.Name + " : cross index = " + ss.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(ss.Seperate());
                                }

                                break;
                        }
                    }
                }
                else
                {
                    foreach (var support in Global.Objects)
                    {
                        switch (Global.GetObjectType(support))
                        {
                            case Global.ObjectType.BasicSupport:

                                var bs = support as BasicSupport;

                                if (bs.CrossIndex % 2 == 1 && bs.CrossIndex <= step && bs.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + bs.MomentDifference + " at BasicSupport, " + bs.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(bs.Name + " : cross index = " + bs.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(bs.Seperate());
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var ss = support as SlidingSupport;

                                if (ss.CrossIndex % 2 == 1 && ss.CrossIndex <= step && ss.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + ss.MomentDifference + " at SlidingSupport, " + ss.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(ss.Name + " : cross index = " + ss.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(ss.Seperate());
                                }

                                break;
                        }
                    }
                }
                step++;

                if (checklist.All(x => x == true))
                {
                    stop = true;
                }

                if (stop)
                {
                    MyDebug.WriteInformation("stopped");
                }
            }

            Logger.WriteLine("Cross loop stopped");
            Logger.NextLine();

            foreach (var item in Global.Objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        Beam beam = (Beam)item;
                        MyDebug.WriteInformation("beam " + beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        Logger.WriteLine(beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        MyDebug.WriteInformation("beam " + beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        Logger.WriteLine(beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        break;
                }
            }

            Logger.CloseLogger();*/
        }

        /// <summary>
        /// Updates all beams after Cross loop.
        /// </summary>
        public void UpdateBeams()
        {
            bool stressananalysis = false;

            /*System.Threading.Tasks.Parallel.ForEach(Global.Objects, (item) =>
            {
                Global.SetDecimalSeperator();
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.PostCrossUpdate();
                        MyDebug.WriteInformation(beam.Name + " has been updated");

                        break;
                }
            });*/
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.PostProcessUpdate();
                        MesnetMDDebug.WriteInformation(beam.Name + " has been updated");

                        break;
                }

            }

            _uptoolbar.CollapseArea();
            _uptoolbar.CollapseInertia();
            _uptoolbar.CollapseConcLoad();
            _uptoolbar.CollapseDistLoad();
            _uptoolbar.UpdateMomentDiagrams(true);
            _uptoolbar.UpdateForceDiagrams();
            _uptoolbar.UpdateAxialForceDiagrams();
            _uptoolbar.UpdateStressDiagrams();
        }

        /// <summary>
        /// Updates beams after Clapeyron.
        /// </summary>
        public void UpdateBeam()
        {
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.PostClapeyronUpdate();
                        MesnetMDDebug.WriteInformation(beam.Name + " has been updated");
                        _uptoolbar.CollapseArea();
                        _uptoolbar.CollapseInertia();
                        _uptoolbar.CollapseConcLoad();
                        _uptoolbar.CollapseDistLoad();
                        _uptoolbar.UpdateMomentDiagrams(true);
                        _uptoolbar.UpdateForceDiagrams();
                        _uptoolbar.UpdateAxialForceDiagrams();
                        _uptoolbar.UpdateStressDiagrams();

                        break;
                }
            }
        }

        public void WriteCarryoverFactors()
        {
            foreach (var item in Global.Objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        var beam = item.Value as Beam;
                        MesnetMDDebug.WriteInformation(beam.Name + " : Carryover AB = " + beam.CarryOverAB);

                        MesnetMDDebug.WriteInformation(beam.Name + " : Carryover BA = " + beam.CarryOverBA);

                        break;
                }
            }
        }

        /// <summary>
        /// Returns the support object whose moment difference is maximum.
        /// </summary>
        /// <returns></returns>
        public object MaxMomentSupport()
        {
            /*var list = new Dictionary<int, double>();
            foreach (var item in Global.Objects)
            {
                switch (Global.GetObjectType(item))
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = item as BasicSupport;

                        bs.CalculateTotalStiffness();

                        foreach (Member member in bs.Members)
                        {
                            var beam = member.Beam;
                            var direction = member.Direction;
                            double coeff = 0;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    coeff = beam.StiffnessA / bs.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " left side stiffness = " + coeff);

                                    break;

                                case Global.Direction.Right:

                                    coeff = beam.StiffnessB / bs.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " right side stiffness = " + coeff);

                                    break;
                            }
                        }

                        list.Add(bs.Id, Math.Abs(bs.MomentDifference));

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = item as SlidingSupport;

                        list.Add(ss.Id, Math.Abs(ss.MomentDifference));

                        ss.CalculateTotalStiffness();

                        foreach (Member member in ss.Members)
                        {
                            var beam = member.Beam;
                            var direction = member.Direction;
                            double coeff = 0;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    coeff = beam.StiffnessA / ss.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " left side stiffness = " + coeff);

                                    break;

                                case Global.Direction.Right:

                                    coeff = beam.StiffnessB / ss.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " right side stiffness = " + coeff);

                                    break;
                            }
                        }

                        break;

                    case Global.ObjectType.LeftFixedSupport:

                        //We search max moment in sliding or basic supports. If the max moment is at one fixed support the algorithm does not run correctly and immediately finishes 

                        break;

                    case Global.ObjectType.RightFixedSupport:

                        //We search max moment in sliding or basic supports. If the max moment is at one fixed support the algorithm does not run correctly and immediately finishes

                        break;
                }
            }

            int id = list.MaxBy(x => x.Value).Key;

            return Global.GetObject(id);*/
            return null;
        }

        /// <summary>
        /// Indexes all the supports. The support that has max moment difference has index of 0 and its neighbours has one and their neighbours has 2 etc.
        /// </summary>
        public void IndexAll(object support)
        {
            /*MyDebug.WriteInformation("IndexAll started");

            string startsupport = SupportName(support);

            Graph graph = new Graph();

            #region create graph
            foreach (var item in Global.Objects)
            {
                switch (Global.GetObjectType(item))
                {
                    case Global.ObjectType.BasicSupport:
                        {
                            var bs = item as BasicSupport;

                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in bs.Members)
                            {
                                Beam beam = member.Beam;
                                Global.Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Global.Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Global.Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(bs), supportdict);

                            break;
                        }

                    case Global.ObjectType.SlidingSupport:
                        {
                            var ss = item as SlidingSupport;

                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in ss.Members)
                            {
                                Beam beam = member.Beam;
                                Global.Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Global.Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Global.Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(ss), supportdict);

                            break;
                        }

                    case Global.ObjectType.LeftFixedSupport:
                        {
                            var ls = item as LeftFixedSupport;

                            var supportdict = new Dictionary<string, int>();

                            Beam beam = ls.Member.Beam;
                            Global.Direction direction = ls.Member.Direction;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Global.Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(ls), supportdict);

                            break;
                        }

                    case Global.ObjectType.RightFixedSupport:
                        {
                            var rs = item as RightFixedSupport;

                            var supportdict = new Dictionary<string, int>();

                            Beam beam = rs.Member.Beam;
                            Global.Direction direction = rs.Member.Direction;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Global.Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(rs), supportdict);

                            break;
                        }
                }
            }
            #endregion

            foreach (var supportname in graph.Vertices.Keys)
            {
                int index = graph.ShortestPath(startsupport, supportname).Count;

                foreach (var item in Global.Objects)
                {
                    switch (Global.GetObjectType(item))
                    {
                        case Global.ObjectType.BasicSupport:

                            var bs = item as BasicSupport;

                            if (bs.Name == supportname)
                            {
                                bs.CrossIndex = index;
                            }

                            break;

                        case Global.ObjectType.SlidingSupport:

                            var ss = item as SlidingSupport;

                            if (ss.Name == supportname)
                            {
                                ss.CrossIndex = index;
                            }

                            break;

                        case Global.ObjectType.LeftFixedSupport:

                            var ls = item as LeftFixedSupport;

                            if (ls.Name == supportname)
                            {
                                ls.CrossIndex = index;
                            }

                            break;

                        case Global.ObjectType.RightFixedSupport:

                            var rs = item as RightFixedSupport;

                            if (rs.Name == supportname)
                            {
                                rs.CrossIndex = index;
                            }

                            break;
                    }
                }
            }

            writecrossindexes();*/
        }

        /// <summary>
        /// Tries indexing the beam system. Returns false if there are any exception in indexing, true otherwise.
        /// It is used to check if there are any seperate beam systems.
        /// </summary>
        /// <returns></returns>
        public bool TryIndex()
        {
            /*object startsup = null;
            Graph graph = new Graph();

            #region create graph
            foreach (var item in Global.Objects)
            {
                switch (Global.GetObjectType(item))
                {
                    case Global.ObjectType.BasicSupport:
                        {
                            var bs = item.Value as BasicSupport;
                            if (startsup == null)
                            {
                                startsup = bs;
                            }
                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in bs.Members)
                            {
                                Beam beam = member.Beam;
                                Global.Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Global.Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Global.Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(bs), supportdict);

                            break;
                        }

                    case Global.ObjectType.SlidingSupport:
                        {
                            var ss = item.Value as SlidingSupport;
                            if (startsup == null)
                            {
                                startsup = ss;
                            }
                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in ss.Members)
                            {
                                Beam beam = member.Beam;
                                Global.Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Global.Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Global.Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(ss), supportdict);

                            break;
                        }

                    case Global.ObjectType.LeftFixedSupport:
                        {
                            var ls = item.Value as LeftFixedSupport;
                            if (startsup == null)
                            {
                                startsup = ls;
                            }
                            var supportdict = new Dictionary<string, int>();

                            Beam beam = ls.Member.Beam;
                            Global.Direction direction = ls.Member.Direction;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Global.Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(ls), supportdict);

                            break;
                        }

                    case Global.ObjectType.RightFixedSupport:
                        {
                            var rs = item.Value as RightFixedSupport;
                            if (startsup == null)
                            {
                                startsup = rs;
                            }
                            var supportdict = new Dictionary<string, int>();

                            Beam beam = rs.Member.Beam;
                            Global.Direction direction = rs.Member.Direction;

                            switch (direction)
                            {
                                case Global.Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Global.Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(rs), supportdict);

                            break;
                        }
                }
            }
            #endregion

            string startsupport = SupportName(startsup);

            try
            {
                foreach (var supportname in graph.Vertices.Keys)
                {
                    int index = graph.ShortestPath(startsupport, supportname).Count;

                    foreach (var item in Global.Objects)
                    {
                        switch (Global.GetObjectType(item))
                        {
                            case Global.ObjectType.BasicSupport:

                                var bs = item as BasicSupport;

                                if (bs.Name == supportname)
                                {
                                    bs.CrossIndex = index;
                                }

                                break;

                            case Global.ObjectType.SlidingSupport:

                                var ss = item as SlidingSupport;

                                if (ss.Name == supportname)
                                {
                                    ss.CrossIndex = index;
                                }

                                break;

                            case Global.ObjectType.LeftFixedSupport:

                                var ls = item as LeftFixedSupport;

                                if (ls.Name == supportname)
                                {
                                    ls.CrossIndex = index;
                                }

                                break;

                            case Global.ObjectType.RightFixedSupport:

                                var rs = item as RightFixedSupport;

                                if (rs.Name == supportname)
                                {
                                    rs.CrossIndex = index;
                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            */
            return true;
        }

        private void writecrossindexes()
        {
            /*foreach (var item in Global.Objects)
            {
                switch (Global.GetObjectType(item))
                {
                    case Global.ObjectType.BasicSupport:
                        {
                            var bs = item as BasicSupport;

                            MyDebug.WriteInformation(bs.Name + " crossindex = " + bs.CrossIndex);

                            break;
                        }

                    case Global.ObjectType.SlidingSupport:
                        {
                            var ss = item as SlidingSupport;

                            MyDebug.WriteInformation(ss.Name + " crossindex = " + ss.CrossIndex);

                            break;
                        }

                    case Global.ObjectType.LeftFixedSupport:
                        {
                            var ls = item as LeftFixedSupport;

                            MyDebug.WriteInformation(ls.Name + " crossindex = " + ls.CrossIndex);

                            break;
                        }

                    case Global.ObjectType.RightFixedSupport:
                        {
                            var rs = item as RightFixedSupport;

                            MyDebug.WriteInformation(rs.Name + " crossindex = " + rs.CrossIndex);

                            break;
                        }
                }
            }*/
        }
        #endregion

        public void DisableTestMenus()
        {
            foreach (MenuItem menuitem in testmenu.Items)
            {
                menuitem.IsEnabled = false;
            }
        }

        private void enabletestmenus()
        {
            foreach (MenuItem menuitem in testmenu.Items)
            {
                menuitem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Click event for the button that deletes all objects on the canvas.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void delete_All_Click(object sender, RoutedEventArgs e)
        {
            var prompt = new MessagePrompt(Global.GetString("askfordeleteall"));
            prompt.Owner = this;
            if ((bool)prompt.ShowDialog())
            {
                switch (prompt.Result)
                {
                    case Classes.Global.DialogResult.Yes:
                        //The user has accepted to delete the beam    
                        resetsystem();
                        break;
                }
            }
        }

        /// <summary>
        /// Reset all the variables as in start up. Clears all objects and tree views.
        /// </summary>
        private void resetsystem()
        {
            clearcanvas();
            Global.Objects.Clear();
            Global.MaxMoment = Double.MinValue;
            Global.MaxForce = Double.MinValue;
            Global.MaxInertia = Double.MinValue;
            Global.MaxLoad = Double.MinValue;
            Global.MaxDistLoad = Double.MinValue;
            Global.MaxConcLoad = Double.MinValue;
            Global.MaxDeflection = Double.MinValue;
            Global.MaxStress = Double.MinValue;
            Global.BeamCount = 0;
            Global.SupportCount = 0;
            tree.Items.Clear();
            supporttree.Items.Clear();
            enabletestmenus();
            _savefilepath = null;
            _uptoolbar.DeActivateAll();
            _uptoolbar.CollapseAll();
        }

        /// <summary>
        /// Retrieves the specified support name.
        /// </summary>
        /// <param name="support">The support.</param>
        /// <returns>The name of the support.</returns>
        private string SupportName(SupportItem support)
        {
            string name = null;
            switch (support.Type)
            {
                case Global.ObjectType.LeftFixedSupport:

                    var ls = support as LeftFixedSupport;

                    name = ls.Name;

                    break;

                case Global.ObjectType.RightFixedSupport:

                    var rs = support as RightFixedSupport;

                    name = rs.Name;

                    break;

                case Global.ObjectType.BasicSupport:

                    var bs = support as BasicSupport;

                    name = bs.Name;

                    break;

                case Global.ObjectType.SlidingSupport:

                    var ss = support as SlidingSupport;

                    name = ss.Name;

                    break;
            }
            return name;
        }

        public void UpdateLanguages()
        {
            _treehandler.UpdateAllBeamTree();

            _treehandler.UpdateAllSupportTree();
        }

        private void hidediagrams()
        {
            _uptoolbar.CollapseInertia();
            _uptoolbar.CollapseArea();
            _uptoolbar.CollapseForce();
            _uptoolbar.CollapseAxialForce();
            _uptoolbar.CollapseMoment();
            _uptoolbar.CollapseStress();
        }

        #region File Menubar Events and Functions

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            menunew();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            menuopen();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            menusave();
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            menusaveas();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            menuexit();
        }

        private void menunew()
        {
            MesnetMDDebug.WriteInformation("Open menu clicked");
            if (Global.Objects.Count > 0)
            {
                MesnetMDDebug.WriteInformation("there are at least one pbject in the workspace");
                var prompt = new MessagePrompt(Global.GetString("asktosavebeforeopenning"));
                prompt.Owner = this;
                if ((bool)prompt.ShowDialog())
                {
                    switch (prompt.Result)
                    {
                        case Classes.Global.DialogResult.Yes:
                            {
                                //the user has accepted to save
                                var ioxml = new MesnetIO();

                                if (_savefilepath != null)
                                {
                                    Notify("filesaving");
                                    //the file has been saved before, so save with the previous path
                                    MesnetMDDebug.WriteInformation("save file path exists " + _savefilepath);
                                    ioxml.WriteXml(_savefilepath);
                                    MesnetMDDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                    Notify("filesave");
                                }
                                else
                                {
                                    //the file has not been saved before, open file save dialog
                                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                    saveFileDialog.Filter = Global.GetString("filefilter");
                                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                    {
                                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                        saveFileDialog.InitialDirectory = directory;
                                        MesnetMDDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                                    }
                                    else
                                    {
                                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        MesnetMDDebug.WriteInformation("there is no path exists in save file path settings");
                                    }

                                    if ((bool)saveFileDialog.ShowDialog())
                                    {
                                        Notify("filesaving");
                                        string path = saveFileDialog.FileName;
                                        MesnetMDDebug.WriteInformation("user selected a file from save file dialog: " + path);
                                        ioxml.WriteXml(path);
                                        MesnetMDDebug.WriteInformation("xml file has been written to " + path);
                                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                                        Notify("filesave");
                                    }
                                    else
                                    {
                                        MesnetMDDebug.WriteInformation("User closed the dialog, aborting saving and new operation");
                                        return;
                                    }
                                }
                            }
                            break;

                        case Classes.Global.DialogResult.No:
                            //The user dont want to save current file, do nothing
                            MesnetMDDebug.WriteInformation("Dialog result No");
                            break;

                        case Classes.Global.DialogResult.Cancel:
                            //The user cancelled the dialog, abort the operation
                            MesnetMDDebug.WriteInformation("Dialog result Cancel");
                            return;

                        case Classes.Global.DialogResult.None:
                            //Something we dont know happened, abort the operation
                            MesnetMDDebug.WriteInformation("Dialog result None");
                            return;
                    }
                }
            }

            resetsystem();
        }

        private void menuopen()
        {
            MesnetMDDebug.WriteInformation("Open menu clicked");
            if (Global.Objects.Count > 0)
            {
                MesnetMDDebug.WriteInformation("there are at least one pbject in the workspace");
                var prompt = new MessagePrompt(Global.GetString("asktosavebeforeopenning"));
                prompt.Owner = this;
                if ((bool)prompt.ShowDialog())
                {
                    switch (prompt.Result)
                    {
                        case Classes.Global.DialogResult.Yes:
                            {
                                //the user has accepted to save
                                var ioxml = new MesnetIO();

                                if (_savefilepath != null)
                                {
                                    Notify("filesaving");
                                    //the file has been saved before, so save with the previous path
                                    MesnetMDDebug.WriteInformation("save file path exists " + _savefilepath);
                                    ioxml.WriteXml(_savefilepath);
                                    MesnetMDDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                    Notify("filesave");
                                }
                                else
                                {
                                    //the file has not been saved before, open file save dialog
                                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                    saveFileDialog.Filter = Global.GetString("filefilter");
                                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                    {
                                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                        saveFileDialog.InitialDirectory = directory;
                                        MesnetMDDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                                    }
                                    else
                                    {
                                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        MesnetMDDebug.WriteInformation("there is no path exists in save file path settings");
                                    }

                                    if ((bool)saveFileDialog.ShowDialog())
                                    {
                                        Notify("filesaving");
                                        string path = saveFileDialog.FileName;
                                        MesnetMDDebug.WriteInformation("user selected a file from save file dialog: " + path);
                                        ioxml.WriteXml(path);
                                        MesnetMDDebug.WriteInformation("xml file has been written to " + path);
                                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                                        Notify("filesave");
                                    }
                                    else
                                    {
                                        MesnetMDDebug.WriteInformation("User closed the dialog, aborting saving and opening operation");
                                        return;
                                    }
                                }
                            }
                            break;

                        case Classes.Global.DialogResult.No:
                            //The user dont want to save current file, do nothing
                            MesnetMDDebug.WriteInformation("Dialog result No");
                            break;

                        case Classes.Global.DialogResult.Cancel:
                            //The user cancelled the dialog, abort the operation
                            MesnetMDDebug.WriteInformation("Dialog result Cancel");
                            return;

                        case Classes.Global.DialogResult.None:
                            //Something we dont know happened, abort the operation
                            MesnetMDDebug.WriteInformation("Dialog result None");
                            return;
                    }
                }
            }

            resetsystem();

            var openxmlio = new MesnetIO();
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = Global.GetString("filefilter");
            if (MesnetSettings.IsSettingExists("openpath", "mainwindow"))
            {
                string directory = MesnetSettings.ReadSetting("openpath", "mainwindow");
                openFileDialog.InitialDirectory = directory;
                MesnetMDDebug.WriteInformation("there is a path exists in open file path settings: " + directory);
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                MesnetMDDebug.WriteInformation("there is no path exists in save file path settings");
            }

            if ((bool)openFileDialog.ShowDialog())
            {
                Notify("filereading");
                string path = openFileDialog.FileName;
                MesnetMDDebug.WriteInformation("user selected a file from open file dialog: " + path);
                try
                {
                    if (openxmlio.ReadXml(canvas, path, this))
                    {
#if (DEBUG)
                        /*foreach (var item in Global.Objects)
                        {
                            if (item.Value.Type is Global.ObjectType.Beam)
                            {
                                var beam = item.Value as Beam;

                                beam.ShowCorners(5, 5);

                            }
                        }*/
#endif
                        _treehandler.UpdateAllBeamTree();
                        _treehandler.UpdateAllSupportTree();
                        MesnetMDDebug.WriteInformation("xml file has been read from " + path);
                        MesnetSettings.WriteSetting("openpath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                        Notify("fileread");
                    }
                    else
                    {
                        MesnetMDDebug.WriteWarning("xml file could not be read!");
                        Notify("filereaderror");
                    }
                }
                catch (Exception e)
                {
                    MesnetMDDebug.WriteError(e.Message);
                    MessageBox.Show("File could not be read!");
                    Notify("filereaderror");
                    Global.Objects.Clear();
                    canvas.Children.Clear();
                    _treehandler.UpdateAllBeamTree();
                    _treehandler.UpdateAllSupportTree();
                    resetsystem();
                    return;
                }
            }
            else
            {
                MesnetMDDebug.WriteInformation("User closed the dialog, aborting opening operation");
                return;
            }
        }

        private void menusave()
        {
            if (Global.Objects.Count > 0)
            {
                var ioxml = new MesnetIO();
                if (_savefilepath != null)
                {
                    Notify("filesaving");
                    //the file has been saved before, so save with the previous path
                    MesnetMDDebug.WriteInformation("save file path exists " + _savefilepath);
                    ioxml.WriteXml(_savefilepath);
                    MesnetMDDebug.WriteInformation("xml file has been written to " + _savefilepath);
                    Notify("filesave");
                }
                else
                {
                    //the file has not been saved before, open file save dialog
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    saveFileDialog.Filter = Global.GetString("filefilter");
                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                    {
                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                        saveFileDialog.InitialDirectory = directory;
                        MesnetMDDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                    }
                    else
                    {
                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        MesnetMDDebug.WriteInformation("there is no path exists in save file path settings");
                    }

                    if ((bool)saveFileDialog.ShowDialog())
                    {
                        Notify("filesaving");
                        string path = saveFileDialog.FileName;
                        MesnetMDDebug.WriteInformation("user selected a file from save file dialog: " + path);
                        ioxml.WriteXml(path);
                        MesnetMDDebug.WriteInformation("xml file has been written to " + path);
                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                        Notify("filesave");
                        _savefilepath = path;
                    }
                    else
                    {
                        MesnetMDDebug.WriteInformation("User closed the dialog, aborting saving operation");
                        return;
                    }
                }
            }
        }

        private void menusaveas()
        {
            if (Global.Objects.Count > 0)
            {
                var ioxml = new MesnetIO();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = Global.GetString("filefilter");
                if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                {
                    string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                    saveFileDialog.InitialDirectory = directory;
                    MesnetMDDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                }
                else
                {
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    MesnetMDDebug.WriteInformation("there is no path exists in save file path settings");
                }

                if ((bool)saveFileDialog.ShowDialog())
                {
                    Notify("filesaving");
                    string path = saveFileDialog.FileName;
                    MesnetMDDebug.WriteInformation("user selected a file from save file dialog: " + path);
                    ioxml.WriteXml(path);
                    MesnetMDDebug.WriteInformation("xml file has been written to " + path);
                    MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                    Notify("filesave");
                    _savefilepath = path;
                }
                else
                {
                    MesnetMDDebug.WriteInformation("User closed the dialog, aborting saving operation");
                    return;
                }
            }
        }

        private void menuexit()
        {
            if (Global.Objects.Count > 0)
            {
                MesnetMDDebug.WriteInformation("there are at least one object in the workspace");
                var prompt = new MessagePrompt(Global.GetString("asktosavebeforeopenning"));
                prompt.Owner = this;

                try
                {
                    if ((bool)prompt.ShowDialog())
                    {
                        switch (prompt.Result)
                        {
                            case Classes.Global.DialogResult.Yes:
                                {
                                    //the user has accepted to save
                                    var ioxml = new MesnetIO();
                                    if (_savefilepath != null)
                                    {
                                        Notify("filesaving");
                                        //the file has been saved before, so save with the previous path
                                        MesnetMDDebug.WriteInformation("save file path exists " + _savefilepath);
                                        ioxml.WriteXml(_savefilepath);
                                        MesnetMDDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                        Notify("filesave");
                                    }
                                    else
                                    {
                                        //the file has not been saved before, open file save dialog
                                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                        saveFileDialog.Filter = Global.GetString("filefilter");
                                        if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                        {
                                            string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                            saveFileDialog.InitialDirectory = directory;
                                            MesnetMDDebug.WriteInformation(
                                                "there is a path exists in save file path settings: " + directory);
                                        }
                                        else
                                        {
                                            saveFileDialog.InitialDirectory =
                                                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                            MesnetMDDebug.WriteInformation(
                                                "there is no path exists in save file path settings");
                                        }

                                        if ((bool)saveFileDialog.ShowDialog())
                                        {
                                            Notify("filesaving");
                                            string path = saveFileDialog.FileName;
                                            MesnetMDDebug.WriteInformation("user selected a file from save file dialog: " +
                                                                     path);
                                            ioxml.WriteXml(path);
                                            MesnetMDDebug.WriteInformation("xml file has been written to " + path);
                                            MesnetSettings.WriteSetting("savepath",
                                                System.IO.Path.GetDirectoryName(path), "mainwindow");
                                            Notify("filesave");
                                            _savefilepath = path;
                                        }
                                        else
                                        {
                                            MesnetMDDebug.WriteInformation(
                                                "User closed the dialog, aborting saving operation");
                                            return;
                                        }
                                    }
                                }
                                break;

                            case Classes.Global.DialogResult.No:
                                //The user dont want to save current file, do nothing
                                MesnetMDDebug.WriteInformation("Dialog result No");
                                break;

                            case Classes.Global.DialogResult.Cancel:
                                //The user cancelled the dialog, abort the operation
                                MesnetMDDebug.WriteInformation("Dialog result Cancel");
                                return;

                            case Classes.Global.DialogResult.None:
                                //Something we dont know happened, abort the operation
                                MesnetMDDebug.WriteInformation("Dialog result None");
                                return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
            Application.Current.Shutdown();
        }

        private void open(string path)
        {
            Notify("filereading");
            var openxmlio = new MesnetIO();
            openxmlio.ReadXml(canvas, path, this);
            _treehandler.UpdateAllBeamTree();
            _treehandler.UpdateAllSupportTree();
            MesnetMDDebug.WriteInformation("xml file has been read from " + path);
            Notify("fileread");
        }

        #endregion

        private void clearcanvas()
        {
            foreach (KeyValuePair<int, SomItem> item in Global.Objects)
            {
                canvas.Children.Remove(item.Value);
            }
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menunew();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menuopen();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menusave();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            menuexit();
        }

        private void corner_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Global.Objects)
            {
                if (item.Value.Type is Global.ObjectType.Beam)
                {
                    var beam = item.Value as Beam;
                    beam.ShowCorners(5, 5);
                }
            }
        }

        /// <summary>
        /// Checks if the beam system can be solvable
        /// </summary>
        /// <returns></returns>
        private bool systemsolvable()
        {
            //check if there are any Objects
            if (Global.Objects.Count == 0)
            {
                notify.Text = Global.GetString("systemnotsolvable");
                return false;
            }

            //check if there are any beam whose one of end is not bounded
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.LeftSide == null)
                        {
                            notify.Text = Global.GetString("systemnotsolvable");
                            return false;
                        }
                        if (beam.RightSide == null)
                        {
                            notify.Text = Global.GetString("systemnotsolvable");
                            return false;
                        }

                        break;
                }
            }

            //check if there are any loads on any beam
            var loaded = false;
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.ConcentratedLoads != null)
                        {
                            if (beam.ConcentratedLoads.Count > 0)
                            {
                                loaded = true;
                            }
                        }
                        if (beam.DistributedLoads != null)
                        {
                            if (beam.DistributedLoads.Count > 0)
                            {
                                loaded = true;
                            }
                        }

                        break;
                }
            }
            if (!loaded)
            {
                notify.Text = Global.GetString("notloadedsystem");
                return false;
            }

            //check if there are any seperate beam systems
            if (!TryIndex())
            {
                notify.Text = Global.GetString("systemnotsolvable");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the solution.
        /// </summary>
        public void InitializeSolution()
        {
            if (systemsolvable())
            {
                hidediagrams();
                removediagrams();
                Calculate();
            }
        }

        /// <summary>
        /// Removes moment, force and stress diagrams if available.
        /// </summary>
        private void removediagrams()
        {
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.DestroyFixedEndMomentDiagram();
                        beam.DestroyFixedEndForceDiagram();
                        beam.DestroyAxialForceDiagram();
                        beam.DestroyStressDiagram();

                        break;
                }
            }
        }

        public void Notify(string key)
        {
            notify.Text = Global.GetString(key);
            notify.UpdateLayout();
        }

        public void Notify()
        {
            notify.Text = "";
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Default.language)
            {
                case "en-EN":

                    var abouten = new AboutWindowEn();
                    abouten.Owner = this;
                    abouten.ShowDialog();

                    break;

                case "tr-TR":

                    var abouttr = new AboutWindowTr();
                    abouttr.Owner = this;
                    abouttr.ShowDialog();

                    break;
            }
        }

        public void ResetSolution()
        {
            _uptoolbar.DeActivateForce();
            _uptoolbar.DeActivateMoment();
            _uptoolbar.DeActivateStress();
            _uptoolbar.CollapseForce();
            _uptoolbar.CollapseAxialForce();
            _uptoolbar.CollapseMoment();
            _uptoolbar.CollapseStress();

            foreach (var item in Global.Objects)
            {
                var som = item.Value;
                som.ResetSolution();                
            }

            MesnetMDDebug.WriteInformation("Solution reset");
        }

        private void DebugClick(object sender, RoutedEventArgs e)
        {
            
        }

        #region Properties

        public UpToolBar UpToolBar
        {
            get { return _uptoolbar; }
        }

        public TreeHandler TreeHandler
        {
            get { return _treehandler; }
        }

        #endregion
    }
}
