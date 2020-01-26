using System;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Tools
{
    public class BeamConnector
    {
        public BeamConnector(Beam beam)
        {
            _beam = beam;
        }

        private Beam _beam;

        public void Connect(Global.Direction direction1, Beam oldbeam, Global.Direction direction2)
        {
            if (_beam.IsBound && oldbeam.IsBound)
            {
                throw new InvalidOperationException("Both beam has bound");
            }
            switch (direction1)
            {
                case Global.Direction.Left:

                    switch (direction2)
                    {
                        case Global.Direction.Left:

                            if (_beam.LeftSide is IRealSupportItem && oldbeam.LeftSide is IRealSupportItem)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftconnect(oldbeam);

                            break;

                        case Global.Direction.Right:

                            if (_beam.LeftSide is IRealSupportItem && oldbeam.RightSide is IRealSupportItem)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the right side of lodbeam.
                            leftrightconnect(oldbeam);

                            break;
                    }

                    break;

                case Global.Direction.Right:

                    switch (direction2)
                    {
                        case Global.Direction.Left:

                            if (_beam.RightSide is IRealSupportItem && oldbeam.LeftSide is IRealSupportItem)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftconnect(oldbeam);

                            break;

                        case Global.Direction.Right:

                            if (_beam.RightSide is IRealSupportItem && oldbeam.RightSide is IRealSupportItem)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Right side of this beam will be connected to the right side of oldbeam.                             
                            rightrightconnect(oldbeam);

                            break;
                    }

                    break;
            }

            _beam.IsBound = true;
            oldbeam.IsBound = true;
        }

        private void leftleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide is IRealSupportItem)
            {
                if (!(oldbeam.LeftSide is LeftFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, _beam.LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }

                    switch (oldbeam.LeftSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(_beam, Global.Direction.Left);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(_beam, Global.Direction.Left);
                            break;

                        case RightFixedSupport rs:
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
            else if (_beam.LeftSide is IRealSupportItem)
            {
                if (!(_beam.LeftSide is LeftFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, _beam.LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }

                    switch (_beam.LeftSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(oldbeam, Global.Direction.Left);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(oldbeam, Global.Direction.Left);
                            break;

                        case RightFixedSupport rs:
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
            else if (oldbeam.LeftSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Left, _beam.LeftPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }

                var fs = oldbeam.LeftSide as FictionalSupport;
                fs.AddBeam(_beam, Global.Direction.Left);
            }
            else if (_beam.LeftSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Left, _beam.LeftPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }

                var fs = _beam.LeftSide as FictionalSupport;
                fs.AddBeam(oldbeam, Global.Direction.Left);
            }
            else
            {
                //Both side is null. So we will add fictional support
            }
        }

        private void leftrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide is IRealSupportItem)
            {
                if (!(oldbeam.RightSide is RightFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        MesnetMDDebug.WriteInformation(_beam.Name + " : isbound : " + _beam.IsBound.ToString());
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, _beam.LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }

                    switch (oldbeam.RightSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(_beam, Global.Direction.Left);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(_beam, Global.Direction.Left);
                            break;

                        case LeftFixedSupport ls:
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
            else if (_beam.LeftSide is IRealSupportItem)
            {
                if (!(_beam.LeftSide is LeftFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, _beam.LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }

                    switch (_beam.LeftSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(oldbeam, Global.Direction.Right);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(oldbeam, Global.Direction.Right);
                            break;

                        case RightFixedSupport rs:
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
            else if (oldbeam.RightSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    MesnetMDDebug.WriteInformation(_beam.Name + " : isbound : " + _beam.IsBound.ToString());
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Right, _beam.LeftPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }

                var fs = oldbeam.RightSide as FictionalSupport;
                fs.AddBeam(_beam, Global.Direction.Left);
            }
            else if (_beam.LeftSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    MesnetMDDebug.WriteInformation(_beam.Name + " : isbound : " + _beam.IsBound.ToString());
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Right, _beam.LeftPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Left, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }

                var fs = _beam.LeftSide as FictionalSupport;
                fs.AddBeam(oldbeam, Global.Direction.Right);
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide is IRealSupportItem)
            {
                if (!(oldbeam.LeftSide is LeftFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, _beam.RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }

                    switch (oldbeam.LeftSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(_beam, Global.Direction.Right);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(_beam, Global.Direction.Right);
                            break;

                        case RightFixedSupport rs:
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
            else if (_beam.RightSide is IRealSupportItem)
            {
                if (!(_beam.RightSide is RightFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Left, _beam.RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                        _beam.MoveSupports();
                    }

                    switch (_beam.RightSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(oldbeam, Global.Direction.Left);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(oldbeam, Global.Direction.Left);
                            break;

                        case LeftFixedSupport ls:
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
            else if (oldbeam.LeftSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Left, _beam.RightPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }

                var fs = oldbeam.LeftSide as FictionalSupport;
                fs.AddBeam(_beam, Global.Direction.Right);
            }
            else if (_beam.RightSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Left, _beam.RightPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.LeftPoint);
                    _beam.MoveSupports();
                }

                var fs = _beam.RightSide as FictionalSupport;
                fs.AddBeam(oldbeam, Global.Direction.Left);
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide is IRealSupportItem)
            {
                if (!(oldbeam.RightSide is RightFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, _beam.RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }

                    switch (oldbeam.RightSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(_beam, Global.Direction.Right);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(_beam, Global.Direction.Right);
                            break;

                        case LeftFixedSupport ls:
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
            else if (_beam.RightSide is IRealSupportItem)
            {
                if (!(_beam.RightSide is RightFixedSupport))
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }
                    else if (_beam.IsBound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Global.Direction.Right, _beam.RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !_beam.IsBound)
                    {
                        //We will move this beam
                        _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                        _beam.MoveSupports();
                    }

                    switch (_beam.RightSide)
                    {
                        case SlidingSupport ss:
                            ss.AddBeam(oldbeam, Global.Direction.Right);
                            break;

                        case BasicSupport bs:
                            bs.AddBeam(oldbeam, Global.Direction.Right);
                            break;

                        case LeftFixedSupport ls:
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
            else if (oldbeam.RightSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Right, _beam.RightPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }

                var fs = oldbeam.RightSide as FictionalSupport;
                fs.AddBeam(_beam, Global.Direction.Right);
            }
            else if (_beam.RightSide is IFictionalSupportItem)
            {
                if (oldbeam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }
                else if (_beam.IsBound)
                {
                    //We will move the old beam
                    oldbeam.SetPosition(Global.Direction.Right, _beam.RightPoint);
                    oldbeam.MoveSupports();
                }
                else if (!oldbeam.IsBound && !_beam.IsBound)
                {
                    //We will move this beam
                    _beam.SetPosition(Global.Direction.Right, oldbeam.RightPoint);
                    _beam.MoveSupports();
                }

                var fs = _beam.RightSide as FictionalSupport;
                fs.AddBeam(oldbeam, Global.Direction.Right);
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
            if (!_beam.IsBound || !oldbeam.IsBound)
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

                            if (_beam.LeftSide == null && oldbeam.LeftSide == null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }
                            else if (_beam.LeftSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Left-Right

                        case Global.Direction.Right:

                            if (_beam.LeftSide != null && oldbeam.RightSide != null)
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

                            if (_beam.RightSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Right-Right

                        case Global.Direction.Right:

                            if (_beam.RightSide != null && oldbeam.RightSide != null)
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
                switch (oldbeam.LeftSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");
                        break;
                }
            }
            else if (_beam.LeftSide != null)
            {
                switch (_beam.LeftSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case RightFixedSupport rs:
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
                switch (oldbeam.RightSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(_beam, Global.Direction.Left);
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");
                        break;
                }
            }
            else if (_beam.LeftSide != null)
            {
                switch (_beam.LeftSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case  LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case RightFixedSupport rs:
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
                switch (oldbeam.RightSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException("The side that has a fixed support can not be connected.");

                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");
                        break;
                }
            }
            else if (_beam.RightSide != null)
            {
                switch (_beam.RightSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(oldbeam, Global.Direction.Right);
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case LeftFixedSupport ls:
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
                switch (oldbeam.LeftSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(_beam, Global.Direction.Right);
                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");
                        break;
                }
            }
            else if (_beam.RightSide != null)
            {
                switch (_beam.RightSide)
                {
                    case SlidingSupport ss:
                        ss.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case BasicSupport bs:
                        bs.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case FictionalSupport fs:
                        fs.AddBeam(oldbeam, Global.Direction.Left);
                        break;

                    case RightFixedSupport rs:
                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");
                        break;

                    case LeftFixedSupport ls:
                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");
                        break;
                }
            }
        }
    }
}
