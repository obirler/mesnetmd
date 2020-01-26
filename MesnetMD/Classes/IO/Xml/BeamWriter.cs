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

using System.Collections.Generic;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.IO.Xml
{
    public class BeamWriter
    {
        public BeamWriter(System.Xml.XmlWriter writer, Beam beam)
        {
            _writer = writer;

            _beam = beam;                
        }

        public void Write()
        {                
            _writer.WriteStartElement("Beam");

            _writer.WriteStartElement("BeamProperties");

            _writer.WriteElementString("length", _beam.Length.ToString());

            _writer.WriteElementString("name", _beam.Name.ToString());

            _writer.WriteElementString("id", _beam.Id.ToString());

            _writer.WriteElementString("beamid", _beam.BeamId.ToString());

            _writer.WriteElementString("izero", _beam.IZero.ToString());

            _writer.WriteElementString("elasticity", _beam.ElasticityModulus.ToString());

            _writer.WriteElementString("leftposition", _beam.LeftPos.ToString());

            _writer.WriteElementString("topposition", _beam.TopPos.ToString());

            _writer.WriteElementString("performstressanalysis", _beam.PerformStressAnalysis.ToString());

            if (_beam.PerformStressAnalysis)
            {
                _writer.WriteElementString("maxallowablestress", _beam.MaxAllowableStress.ToString());
            }

            _writer.WriteStartElement("RotateTransform");

            _writer.WriteElementString("centerx", _beam.rotateTransform.CenterX.ToString());

            _writer.WriteElementString("centery", _beam.rotateTransform.CenterY.ToString());

            _writer.WriteElementString("angle", _beam.rotateTransform.Angle.ToString());

            _writer.WriteEndElement();

            _writer.WriteStartElement("TransformGeometry");

            _writer.WriteElementString("topleft", _beam.TGeometry.InnerTopLeft.X + ";" + _beam.TGeometry.InnerTopLeft.Y);

            _writer.WriteElementString("topright", _beam.TGeometry.InnerTopRight.X + ";" + _beam.TGeometry.InnerTopRight.Y);

            _writer.WriteElementString("bottomleft", _beam.TGeometry.InnerBottomLeft.X + ";" + _beam.TGeometry.InnerBottomLeft.Y);

            _writer.WriteElementString("bottomright", _beam.TGeometry.InnerBottomRight.X + ";" + _beam.TGeometry.InnerBottomRight.Y);

            _writer.WriteEndElement();

            _writer.WriteEndElement();

            writeinertias();

            writeareas();

            writeloads();

            if(_beam.PerformStressAnalysis)
            {
                writestressproperties();
            }

            writeconnections();
        
            _writer.WriteEndElement();
        }

        private void writeinertias()
        {
            _writer.WriteStartElement("Inertias");

            foreach(Math.Poly poly in _beam.Inertia)
            {
                _writer.WriteStartElement("Inertia");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void writeareas()
        {
            _writer.WriteStartElement("Areas");

            foreach (Math.Poly poly in _beam.Area)
            {
                _writer.WriteStartElement("Area");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void writeloads()
        {
            _writer.WriteStartElement("Loads");

            if(_beam.ConcentratedLoads?.Count > 0)
            {
                _writer.WriteStartElement("ConcentratedLoads");
                foreach (KeyValuePair<double, double> pair in _beam.ConcentratedLoads)
                {
                    _writer.WriteStartElement("ConcentratedLoad");
                    _writer.WriteElementString("magnitude", pair.Value.ToString());
                    _writer.WriteElementString("location", pair.Key.ToString());
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }

            if(_beam.DistributedLoads?.Count > 0)
            {
                _writer.WriteStartElement("DistributedLoads");
                foreach (Math.Poly poly in _beam.DistributedLoads)
                {
                    _writer.WriteStartElement("DistributedLoad");
                    _writer.WriteElementString("expression", poly.ToString());
                    _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                    _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }
          
            _writer.WriteEndElement();
        }

        private void writestressproperties()
        {
            _writer.WriteStartElement("EPolies");

            foreach (Math.Poly poly in _beam.Eppoly)
            {
                _writer.WriteStartElement("EPoly");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();

            _writer.WriteStartElement("DPolies");

            foreach (Math.Poly poly in _beam.Dppoly)
            {
                _writer.WriteStartElement("DPoly");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void writeconnections()
        {
            _writer.WriteStartElement("Connections");

            if (_beam.LeftSide != null)
            {
                _writer.WriteStartElement("LeftSide");

                switch (_beam.LeftSide)
                {
                    case BasicSupport bs:
                        _writer.WriteElementString("type", "BasicSupport");
                        _writer.WriteElementString("id", bs.Id.ToString());
                        _writer.WriteElementString("supportid", bs.SupportId.ToString());
                        break;

                    case SlidingSupport ss:
                        _writer.WriteElementString("type", "SlidingSupport");
                        _writer.WriteElementString("id", ss.Id.ToString());
                        _writer.WriteElementString("supportid", ss.SupportId.ToString());
                        break;

                    case LeftFixedSupport ls:
                        _writer.WriteElementString("type", "LeftFixedSupport");
                        _writer.WriteElementString("id", ls.Id.ToString());
                        _writer.WriteElementString("supportid", ls.SupportId.ToString());
                        break;
                }

                _writer.WriteEndElement();
            }

            if (_beam.RightSide != null)
            {
                _writer.WriteStartElement("RightSide");

                switch (_beam.RightSide)
                {
                    case BasicSupport bs:

                        _writer.WriteElementString("type", "BasicSupport");
                        _writer.WriteElementString("id", bs.Id.ToString());
                        _writer.WriteElementString("supportid", bs.SupportId.ToString());
                        break;

                    case SlidingSupport ss:
                        _writer.WriteElementString("type", "SlidingSupport");
                        _writer.WriteElementString("id", ss.Id.ToString());
                        _writer.WriteElementString("supportid", ss.SupportId.ToString());
                        break;

                    case RightFixedSupport rs:
                        _writer.WriteElementString("type", "RightFixedSupport");
                        _writer.WriteElementString("id", rs.Id.ToString());
                        _writer.WriteElementString("supportid", rs.SupportId.ToString());
                        break;
                }

                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        System.Xml.XmlWriter _writer;

        Beam _beam;
    }   
}
