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
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.IO.Xml
{
    public class MesnetMDIO
    {
        public MesnetMDIO()
        {           
        }

        private MainWindow _mw;

        public void WriteXml(string path)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Objects");

                foreach (KeyValuePair<int, SomItem> item in Global.Objects)
                {
                    switch (item.Value)
                    {
                        case Beam beam:
                            var beamwriter = new BeamWriter(writer, beam);
                            beamwriter.Write();
                            break;

                        case BasicSupport bs:
                            var bswriter = new BasicSupportWriter(writer, bs);
                            bswriter.Write();
                            break;

                        case SlidingSupport ss:
                            var sswriter = new SlidingSupportWriter(writer, ss);
                            sswriter.Write();
                            break;

                        case LeftFixedSupport ls:

                            var lswiter = new LeftFixedSupportWriter(writer, ls);
                            lswiter.Write();
                            break;

                        case RightFixedSupport rs:
                            var rswriter = new RightFixedSupportWriter(writer, rs);
                            rswriter.Write();
                            break;

                        case FictionalSupport fs:
                            var fswriter = new FictionalSupportWriter(writer, fs);
                            fswriter.Write();
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

                    case "FictionalSupport":
                        var fsreader = new FictionalSupportReader(element);
                        manifestlist.Add(fsreader.Read());
                        break;
                }
            }
            setmaxvalues();
            
            Global.Objects.Clear();
            _canvas.Children.Clear();
            addtocanvas();
            connectobjects();
            setvariables();
            _mw.UpToolBar.UpdateLoadDiagrams();
            return true;
        }

        private void addtocanvas()
        {
            foreach(ManifestBase item in 
                manifestlist)
            {
                switch (item)
                {
                    case BeamManifest beammanifest:
                        addbeam(beammanifest);
                        break;

                    case BasicSupportManifest bsmanifest:
                        addbasicsupport(bsmanifest);
                        break;

                    case SlidingSupportManifest ssmanifest:
                        addslidingsupport(ssmanifest);
                        break;

                    case LeftFixedSupportManifest lsmanifest:
                        addleftfixedsupport(lsmanifest);
                        break;

                    case RightFixedSupportManifest rsmanifest:
                        addrightfixedsupport(rsmanifest);
                        break;

                    case FictionalSupportManifest fsmanifest:
                        addfictionalsupport(fsmanifest);
                        break;
                }

            }
            
            _canvas.UpdateLayout();
        }

        private void addbeam(BeamManifest beammanifest)
        {
            var beam = new Beam(beammanifest);
            beam.IZero = beammanifest.IZero;
            beam.AddElasticity(beammanifest.Elasticity);
            beam.PerformStressAnalysis = beammanifest.PerformStressAnalysis;
            if(beam.PerformStressAnalysis)
            {
                beam.MaxAllowableStress = beammanifest.MaxAllowableStress;
            }
            beam.AddInertia(beammanifest.Inertias);
            beam.Add(_canvas);
            beam.SetTopLeft(beammanifest.TopPosition, beammanifest.LeftPosition);
            beam.SetTransformGeometry(beammanifest.TopLeft, beammanifest.TopRight, beammanifest.BottomRight, beammanifest.BottomLeft, _canvas);
            //beam.AddArea(beammanifest.);
            if (beammanifest.DistributedLoads != null)
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
        }

        private void addbasicsupport(BasicSupportManifest supportmanifest)
        {
            var bs = new BasicSupport(supportmanifest);
            bs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            bs.SetAngle(supportmanifest.Angle);
            Global.AddObject(bs);
        }

        private void addslidingsupport(SlidingSupportManifest supportmanifest)
        {
            var ss = new SlidingSupport(supportmanifest);
            ss.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            ss.SetAngle(supportmanifest.Angle);
            Global.AddObject(ss);
        }

        private void addleftfixedsupport(LeftFixedSupportManifest supportmanifest)
        {
            var ls = new LeftFixedSupport(supportmanifest);
            ls.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            ls.SetAngle(supportmanifest.Angle);
            Global.AddObject(ls);
        }

        private void addrightfixedsupport(RightFixedSupportManifest supportmanifest)
        {
            var rs = new RightFixedSupport(supportmanifest);
            rs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            rs.SetAngle(supportmanifest.Angle);
            Global.AddObject(rs);
        }

        private void addfictionalsupport(FictionalSupportManifest supportmanifest)
        {
            var fs = new FictionalSupport(supportmanifest);
            fs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            //fs.SetAngle(supportmanifest.Angle);
            Global.AddObject(fs);
        }

        private void connectobjects()
        {
            foreach(KeyValuePair<int, SomItem> item in Global.Objects)
            {
                switch (item.Value)
                {
                    case Beam beam:
                        connectbeam(beam);
                        break;

                    case BasicSupport bs:
                        connectbasicsupport(bs);
                        break;

                    case SlidingSupport ss:
                        connectslidingsupport(ss);
                        break;

                    case LeftFixedSupport ls:
                        connectleftfixedsupport(ls);
                        break;

                    case RightFixedSupport rs:
                        connectrightfixedsupport(rs);
                        break;

                    case FictionalSupport fs:
                        connectfictionalsupport(fs);
                        break;
                }
            }
        }

        private void connectbeam(Beam beam)
        {
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case BeamManifest beammanifest:
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
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case BasicSupportManifest manifest:
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
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case SlidingSupportManifest manifest:
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
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case LeftFixedSupportManifest manifest:
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
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case RightFixedSupportManifest manifest:
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

        private void connectfictionalsupport(FictionalSupport support)
        {
            foreach (ManifestBase manifestitem in manifestlist)
            {
                switch (manifestitem)
                {
                    case FictionalSupportManifest manifest:
                        if (support.Id == manifest.Id)
                        {
                            foreach (Member item in manifest.Members)
                            {
                                var member = new Tools.Member();
                                member.Beam = Global.GetObject(item.Id) as Beam;
                                member.Direction = item.Direction;
                                support.Members.Add(member);
                            }
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
                if (item.Value is Beam)
                {
                    Global.BeamCount++;
                }
                else if (item.Value is SupportItem)
                {
                    Global.SupportCount++;
                }

                /*switch (item.Value.ObjectType)
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
                }*/
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
                switch (manifestlist[i])
                {
                    case BeamManifest beammanifest:
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
