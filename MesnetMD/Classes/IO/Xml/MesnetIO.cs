/*
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
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using MesnetMD.Classes.IO.Manifest;
using MesnetMD.Classes.Ui;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.IO.Xml
{
    public class MesnetIO
    {
        public MesnetIO()
        {           
        }

        private MainWindow _mw;

        public void WriteXml(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Objects");

                foreach (KeyValuePair<int, SomItem> item in Global.Objects)
                {
                    switch (item.Value.Type)
                    {
                        case Global.ObjectType.Beam:

                            var beam = item.Value as Beam;

                            var beamwriter = new BeamWriter(writer, beam);

                            beamwriter.Write();

                            break;

                        case Global.ObjectType.BasicSupport:

                            var bs = item.Value as BasicSupport;

                            var bswriter = new BasicSupportWriter(writer, bs);

                            bswriter.Write();

                            break;

                        case Global.ObjectType.SlidingSupport:

                            var ss = item.Value as SlidingSupport;

                            var sswriter = new SlidingSupportWriter(writer, ss);

                            sswriter.Write();

                            break;

                        case Global.ObjectType.LeftFixedSupport:

                            var ls = item.Value as LeftFixedSupport;

                            var lswiter = new LeftFixedSupportWriter(writer, ls);

                            lswiter.Write();

                            break;

                        case Global.ObjectType.RightFixedSupport:

                            var rs = item.Value as RightFixedSupport;

                            var rswriter = new RightFixedSupportWriter(writer, rs);

                            rswriter.Write();

                            break;
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public bool ReadXml(Canvas canvas, string path, MainWindow mw)
        {
            _mw = mw;
            _canvas = canvas;
            manifestlist = new List<ManifestBase>();

            XDocument doc;
            try
            {
                doc = XDocument.Load(path);
            }
            catch (Exception)
            {
                MessageBox.Show(Global.GetString("invalidfile"));
                _mw.Notify();
                return false;
            }
            
            foreach (XElement element in doc.Element("Objects").Elements())
            {
                switch (element.Name.ToString())
                {
                    case "Beam":
                        var beamreader = new BeamReader(element);
                        manifestlist.Add(beamreader.Read());
                        break;

                    case "BasicSupport":
                        var bsreader = new BasicSupportReader(element);
                        manifestlist.Add(bsreader.Read());
                        break;

                    case "SlidingSupport":
                        var ssreader = new BasicSupportReader(element);
                        manifestlist.Add(ssreader.Read());
                        break;

                    case "LeftFixedSupport":
                        var lsreader = new LeftFixedSupportReader(element);
                        manifestlist.Add(lsreader.Read());
                        break;

                    case "RightFixedSupport":
                        var rsreader = new RightFixedSupportReader(element);
                        manifestlist.Add(rsreader.Read());
                        break;
                }
            }
            setmaxvalues();
            
            Global.Objects.Clear();
            addtocanvas();
            connectobjects();
            setvariables();
            _mw.UpToolBar.UpdateLoadDiagrams();
            return true;
        }

        private void addtocanvas()
        {
            foreach(ManifestBase item in manifestlist)
            {
                switch (item.Type)
                {
                    case Global.ManifestType.BeamManifest:

                        var beammanifest = item as BeamManifest;
                        addbeam(beammanifest);

                        break;

                    case Global.ManifestType.BasicSupportManifest:

                        var bsmanifest = item as BasicSupportManifest;
                        addbasicsupport(bsmanifest);

                        break;

                    case Global.ManifestType.SlidingSupportManifest:

                        var ssmanifest = item as SlidingSupportManifest;
                        addslidingsupport(ssmanifest);

                        break;

                    case Global.ManifestType.LeftFixedSupportManifest:

                        var lsmanifest = item as LeftFixedSupportManifest;
                        addleftfixedsupport(lsmanifest);
                        break;

                    case Global.ManifestType.RightFixedSupportManifest:

                        var rsmanifest = item as RightFixedSupportManifest;
                        addrightfixedsupport(rsmanifest);
                        break;
                }

            }
            _canvas.UpdateLayout();
        }

        private void addbeam(BeamManifest beammanifest)
        {
            var beam = new Beam();
            beam.AddFromXml(_canvas, beammanifest.Length);
            beam.Id = beammanifest.Id;
            beam.Name = beammanifest.Name;
            beam.BeamId = beammanifest.BeamId;
            beam.SetTopLeft(beammanifest.TopPosition, beammanifest.LeftPosition);           
            beam.RotateTransform.CenterX = beammanifest.CenterX;
            beam.RotateTransform.CenterY = beammanifest.CenterY;
            beam.RotateTransform.Angle = beammanifest.Angle;
            beam.Angle = beam.RotateTransform.Angle;
            beam.SetTransformGeometry(beammanifest.TopLeft, beammanifest.TopRight, beammanifest.BottomRight, beammanifest.BottomLeft, _canvas);
            beam.IZero = beammanifest.IZero;
            beam.AddElasticity(beammanifest.Elasticity);
            beam.PerformStressAnalysis = beammanifest.PerformStressAnalysis;
            if(beam.PerformStressAnalysis)
            {
                beam.MaxAllowableStress = beammanifest.MaxAllowableStress;
            }
            beam.AddInertia(beammanifest.Inertias);
            if(beammanifest.DistributedLoads != null)
            {
                if(beammanifest.DistributedLoads.Count > 0)
                {
                    beam.AddLoad(beammanifest.DistributedLoads);
                }               
            }
            if (beammanifest.ConcentratedLoads != null)
            {
                if (beammanifest.ConcentratedLoads.Count > 0)
                {
                    beam.AddLoad(beammanifest.ConcentratedLoads);
                }                
            }
            if(beammanifest.EPolies != null)
            {
                if(beammanifest.EPolies.Count >0)
                {
                    beam.AddE(beammanifest.EPolies);
                }
            }
            if (beammanifest.DPolies != null)
            {
                if (beammanifest.DPolies.Count > 0)
                {
                    beam.AddD(beammanifest.DPolies);
                }
            }
            Global.AddObject(beam);
        }

        private void addbasicsupport(BasicSupportManifest supportmanifest)
        {
            var bs = new BasicSupport();
            bs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            bs.Id = supportmanifest.Id;
            bs.SupportId = supportmanifest.SupportId;
            bs.Name = supportmanifest.Name;
            bs.SetAngle(supportmanifest.Angle);
            Global.AddObject(bs);
        }

        private void addslidingsupport(SlidingSupportManifest supportmanifest)
        {
            var ss = new SlidingSupport();
            ss.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            ss.Id = supportmanifest.Id;
            ss.SupportId = supportmanifest.SupportId;
            ss.Name = supportmanifest.Name;
            ss.SetAngle(supportmanifest.Angle);
            Global.AddObject(ss);
        }

        private void addleftfixedsupport(LeftFixedSupportManifest supportmanifest)
        {
            var ls = new LeftFixedSupport();
            ls.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            ls.Id = supportmanifest.Id;
            ls.SupportId = supportmanifest.SupportId;
            ls.Name = supportmanifest.Name;
            ls.SetAngle(supportmanifest.Angle);
            Global.AddObject(ls);
        }

        private void addrightfixedsupport(RightFixedSupportManifest supportmanifest)
        {
            var rs = new RightFixedSupport();
            rs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            rs.Id = supportmanifest.Id;
            rs.SupportId = supportmanifest.SupportId;
            rs.Name = supportmanifest.Name;
            rs.SetAngle(supportmanifest.Angle);
            Global.AddObject(rs);
        }

        private void connectobjects()
        {
            foreach(KeyValuePair<int, SomItem> item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:
                        connectbeam(item.Value as Beam);
                        break;

                    case Global.ObjectType.BasicSupport:
                        connectbasicsupport(item.Value as BasicSupport);
                        break;

                    case Global.ObjectType.SlidingSupport:
                        connectslidingsupport(item.Value as SlidingSupport);
                        break;

                    case Global.ObjectType.LeftFixedSupport:
                        connectleftfixedsupport(item.Value as LeftFixedSupport);
                        break;

                    case Global.ObjectType.RightFixedSupport:
                        connectrightfixedsupport(item.Value as RightFixedSupport);
                        break;
                }
            }
        }

        private void connectbeam(Beam beam)
        {
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem.Type)
                {
                    case Global.ManifestType.BeamManifest:

                        var beammanifest = manifestitem as BeamManifest;
                        if (beam.Id == beammanifest.Id)
                        {
                            if (beammanifest.Connections.LeftSide != null)
                            {
                                beam.LeftSide = Global.GetObject(beammanifest.Connections.LeftSide.Id) as SupportItem;
                            }
                            else
                            {
                                beam.LeftSide = null;
                            }

                            if (beammanifest.Connections.RightSide != null)
                            {
                                beam.RightSide = Global.GetObject(beammanifest.Connections.RightSide.Id) as SupportItem;                             
                            }
                            else
                            {
                                beam.RightSide = null;
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectbasicsupport(BasicSupport support)
        {
            BasicSupportManifest manifest;
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem.Type)
                {
                    case Global.ManifestType.BasicSupportManifest:
                        manifest = manifestitem as BasicSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            foreach(Member item in manifest.Members)
                            {
                                var member = new Tools.Member();
                                member.Beam = Global.GetObject(item.Id) as Beam;
                                member.Direction = item.Direction;
                                support.Members.Add(member);
                            }
                            
                            if (support.Members.Count > 1)
                            {
                                foreach (var member in support.Members)
                                {
                                   member.Beam.IsBound = true;
                                }
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectslidingsupport(SlidingSupport support)
        {
            SlidingSupportManifest manifest;
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem.Type)
                {
                    case Global.ManifestType.SlidingSupportManifest:
                        manifest = manifestitem as SlidingSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            foreach (Member item in manifest.Members)
                            {
                                var member = new Tools.Member();
                                member.Beam = Global.GetObject(item.Id) as Beam;
                                member.Direction = item.Direction;
                                support.Members.Add(member);
                            }
                            if (support.Members.Count > 1)
                            {
                                foreach (var member in support.Members)
                                {
                                    member.Beam.IsBound = true;
                                }
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectleftfixedsupport(LeftFixedSupport support)
        {
            LeftFixedSupportManifest manifest;
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem.Type)
                {
                    case Global.ManifestType.LeftFixedSupportManifest:
                        manifest = manifestitem as LeftFixedSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            var member = new Tools.Member();
                            member.Beam = Global.GetObject(manifest.Member.Id) as Beam;
                            member.Direction = manifest.Member.Direction;
                            support.Member = member;
                            return;
                        }
                        break;
                }
            }
        }

        private void connectrightfixedsupport(RightFixedSupport support)
        {
            RightFixedSupportManifest manifest;
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem.Type)
                {
                    case Global.ManifestType.RightFixedSupportManifest:
                        manifest = manifestitem as RightFixedSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            var member = new Tools.Member();
                            member.Beam = Global.GetObject(manifest.Member.Id) as Beam;
                            member.Direction = manifest.Member.Direction;
                            support.Member = member;
                            return;
                        }
                        break;
                }
            }
        }

        private void setvariables()
        {
            Global.BeamCount = 0;
            Global.SupportCount = 0;
            foreach (KeyValuePair<int, SomItem> item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:
                        Global.BeamCount++;
                        break;

                    case Global.ObjectType.BasicSupport:
                        Global.SupportCount++;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        Global.SupportCount++;
                        break;

                    case Global.ObjectType.LeftFixedSupport:
                        Global.SupportCount++;
                        break;

                    case Global.ObjectType.RightFixedSupport:
                        Global.SupportCount++;
                        break;
                }
            }
        }

        private void setmaxvalues()
        {
            Global.MaxInertia = Double.MinValue;
            Global.MaxMoment = Double.MinValue;
            Global.MaxConcLoad = Double.MinValue;
            Global.MaxDistLoad = Double.MinValue;

            for (int i = 0; i < manifestlist.Count; i++)
            {
                switch (manifestlist[i].Type)
                {
                    case Global.ManifestType.BeamManifest:

                        var beammanifest = manifestlist[i] as BeamManifest;
                        if (beammanifest.Inertias.MaxAbs > Global.MaxInertia)
                        {
                            Global.MaxInertia = beammanifest.Inertias.MaxAbs;
                        }
                        if (beammanifest.DistributedLoads?.Count > 0)
                        {
                            if (beammanifest.DistributedLoads.MaxAbs > Global.MaxDistLoad)
                            {
                                Global.MaxDistLoad = beammanifest.DistributedLoads.MaxAbs;
                            }
                        }
                        if (beammanifest.ConcentratedLoads?.Count > 0)
                        {
                            if (beammanifest.ConcentratedLoads.YMaxAbs > Global.MaxConcLoad)
                            {
                                Global.MaxConcLoad = beammanifest.ConcentratedLoads.YMaxAbs;
                            }
                        }

                        break;
                }
            }
        }

        private Canvas _canvas;

        private List<ManifestBase> manifestlist;       
    }
}
