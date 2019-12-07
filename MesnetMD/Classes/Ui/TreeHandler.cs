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
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;
using MesnetMD.Xaml.User_Controls;
using TreeView = System.Windows.Controls.TreeView;

namespace MesnetMD.Classes.Ui
{
    public class TreeHandler
    {
        public TreeHandler(MainWindow mw)
        {
            _mw = mw;
            _beamtree = _mw.tree;
            _supporttree = _mw.supporttree;
        }

        private MainWindow _mw;

        private TreeView _beamtree;

        private TreeView _supporttree;

        public bool BeamTreeItemSelectedEventEnabled = true;

        public bool SupportTreeItemSelectedEventEnabled = true;

        #region Beam Tree Methods and Events

        /// <summary>
        /// Updates given beam in the beam tree view.
        /// </summary>
        /// <param name="beam">The beam.</param>
        public void UpdateBeamTree(Beam beam)
        {
            var beamitem = new TreeViewBeamItem(beam);
            bool exists = false;

            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    item.Items.Clear();
                    beamitem = item;
                    exists = true;
                    break;
                }
            }

            BeamItem bitem;

            if (!exists)
            {
                bitem = new BeamItem(beam);
                beamitem.Header = bitem;
                _beamtree.Items.Add(beamitem);
            }
            else
            {
                bitem = new BeamItem(beam);
                beamitem.Header = bitem;
            }

            if (beam.PerformStressAnalysis)
            {
                if (beam.Stress != null)
                {
                    if (beam.Stress.YMaxAbs >= beam.MaxAllowableStress)
                    {
                        bitem.SetCritical(true);
                    }
                    else
                    {
                        bitem.SetCritical(false);
                    }
                }
            }
            else
            {
                bitem.SetCritical(false);
            }

            beamitem.Selected += BeamTreeItemSelected;

            var arrowitem = new TreeViewItem();
            var arrowbutton = new ButtonItem();
            if (!beam.DirectionShown)
            {
                arrowbutton.SetName(Global.GetString("showdirection"));
            }
            else
            {
                arrowbutton.SetName(Global.GetString("hidedirection"));
            }
            arrowitem.Header = arrowbutton;
            arrowbutton.content.Click += _mw.arrow_Click;
            beamitem.Items.Add(arrowitem);

            var lengthitem = new TreeViewItem();
            lengthitem.Header = new LengthItem(Global.GetString("length") + " : " + beam.Length + " m");
            beamitem.Items.Add(lengthitem);

            var leftsideitem = new BeamSupportItem();

            if (beam.LeftSide != null)
            {
                string leftname = Global.GetString("null");
                switch (beam.LeftSide.Type)
                {
                    case Global.ObjectType.LeftFixedSupport:
                        var ls = beam.LeftSide as LeftFixedSupport;
                        leftname = Global.GetString("leftfixedsupport") + " " + ls.SupportId;
                        leftsideitem.Support = ls;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = beam.LeftSide as SlidingSupport;
                        leftname = Global.GetString("slidingsupport") + " " + ss.SupportId;
                        leftsideitem.Support = ss;
                        break;

                    case Global.ObjectType.BasicSupport:
                        var bs = beam.LeftSide as BasicSupport;
                        leftname = Global.GetString("basicsupport") + " " + bs.SupportId;
                        leftsideitem.Support = bs;
                        break;

                    case Global.ObjectType.FictionalSupport:
                        if (Config.ShowFictionalSupportInTrees)
                        {
                            var fs = beam.LeftSide as FictionalSupport;
                            leftname = fs.Name;
                            leftsideitem.Support = fs;
                        }
                        else
                        {
                            leftsideitem.Header.Text = Global.GetString("leftside") + " : " + Global.GetString("null");
                            beamitem.Items.Add(leftsideitem);
                        }
                        break;
                }
                leftsideitem.Header.Text = Global.GetString("leftside") + " : " + leftname;
                beamitem.Items.Add(leftsideitem);
            }
            else
            {
                leftsideitem.Header.Text = Global.GetString("leftside") + " : " + Global.GetString("null");
                beamitem.Items.Add(leftsideitem);
            }

            var rightsideitem = new BeamSupportItem();

            if (beam.RightSide != null)
            {
                string rightname = Global.GetString("null");
                switch (beam.RightSide.Type)
                {
                    case Global.ObjectType.RightFixedSupport:
                        var rs = beam.RightSide as RightFixedSupport;
                        rightname = Global.GetString("rightfixedsupport") + " " + rs.SupportId;
                        rightsideitem.Support = rs;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = beam.RightSide as SlidingSupport;
                        rightname = Global.GetString("slidingsupport") + " " + ss.SupportId;
                        rightsideitem.Support = ss;
                        break;

                    case Global.ObjectType.BasicSupport:
                        var bs = beam.RightSide as BasicSupport;
                        rightname = Global.GetString("basicsupport") + " " + bs.SupportId;
                        rightsideitem.Support = bs;
                        break;

                    case Global.ObjectType.FictionalSupport:
                        if (Config.ShowFictionalSupportInTrees)
                        {
                            var fs = beam.RightSide as FictionalSupport;
                            rightname = fs.Name;
                            rightsideitem.Support = fs;
                        }
                        else
                        {
                            rightsideitem.Header.Text = Global.GetString("rightside") + " : " + Global.GetString("null");
                            beamitem.Items.Add(rightsideitem);
                        }
                        break;
                }
                rightsideitem.Header.Text = Global.GetString("rightside") + " : " + rightname;
                beamitem.Items.Add(rightsideitem);
            }
            else
            {
                rightsideitem.Header.Text = Global.GetString("rightside") + " : " + Global.GetString("null");
                beamitem.Items.Add(rightsideitem);
            }

            var elasticityitem = new TreeViewItem();
            elasticityitem.Header = new ElasticityItem(Global.GetString("elasticity") + " : " + beam.ElasticityModulus + " GPa");
            beamitem.Items.Add(elasticityitem);

            var inertiaitem = new TreeViewItem();

            inertiaitem.Header = new InertiaItem(Global.GetString("inertia"));
            beamitem.Items.Add(inertiaitem);

            foreach (Poly inertiapoly in beam.Inertia)
            {
                var inertiachilditem = new TreeViewItem();
                inertiachilditem.Header = inertiapoly.GetString(15) + " ,  " + inertiapoly.StartPoint + " <= x <= " + inertiapoly.EndPoint;
                inertiaitem.Items.Add(inertiachilditem);
            }

            if (beam.ConcentratedLoads?.Count > 0)
            {
                var concloaditem = new TreeViewItem();
                concloaditem.Header = new ConcentratedLoadItem(Global.GetString("concentratedloads"));
                beamitem.Items.Add(concloaditem);

                foreach (KeyValuePair<double, double> item in beam.ConcentratedLoads)
                {
                    var concloadchilditem = new TreeViewItem();
                    concloadchilditem.Header = System.Math.Round(item.Value, 4) + " ,  " + System.Math.Round(item.Key, 4) + " m";
                    concloaditem.Items.Add(concloadchilditem);
                }
            }

            if (beam.DistributedLoads?.Count > 0)
            {
                var distloaditem = new TreeViewItem();
                distloaditem.Header = new LoadItem(Global.GetString("distributedloads"));
                beamitem.Items.Add(distloaditem);

                foreach (Poly distloadpoly in beam.DistributedLoads)
                {
                    var distloadchilditem = new TreeViewItem();
                    distloadchilditem.Header = distloadpoly.GetString(10) + " ,  " + distloadpoly.StartPoint + " <= x <= " + distloadpoly.EndPoint;
                    distloaditem.Items.Add(distloadchilditem);
                }
            }

            if (beam.FixedEndForce != null)
            {
                var forcetitem = new TreeViewItem();
                forcetitem.Header = new ForceItem(Global.GetString("forcefunction"));
                beamitem.Items.Add(forcetitem);

                foreach (Poly forcepoly in beam.FixedEndForce)
                {
                    var forcechilditem = new TreeViewItem();
                    forcechilditem.Header = forcepoly.GetString(10) + " ,  " + forcepoly.StartPoint + " <= x <= " + forcepoly.EndPoint;
                    forcetitem.Items.Add(forcechilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(Global.GetString("information"));
                infoitem.Header = info;
                forcetitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = Global.GetString("leftside") + " : " + System.Math.Round(beam.FixedEndForce.Calculate(0), 4) + " kN";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = Global.GetString("rightside") + " : " + System.Math.Round(beam.FixedEndForce.Calculate(beam.Length), 4) + " kN";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = Global.GetString("maxvalue") + " : " + System.Math.Round(beam.FixedEndForce.Max, 4) + " kN";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = Global.GetString("maxloc") + " : " + System.Math.Round(beam.FixedEndForce.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = Global.GetString("minvalue") + " : " + System.Math.Round(beam.FixedEndForce.Min, 4) + " kN";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = Global.GetString("minloc") + " : " + System.Math.Round(beam.FixedEndForce.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var forceexplorer = new ForceExplorer();
                forceexplorer.xvalue.Text = "0";
                forceexplorer.funcvalue.Text = System.Math.Round(beam.FixedEndForce.Calculate(0), 4).ToString();
                forceexplorer.xvalue.TextChanged += _mw.fixedendforceexplorer_TextChanged;
                exploreritem.Header = forceexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.FixedEndMoment != null)
            {
                var momentitem = new TreeViewItem();
                momentitem.Header = new MomentItem(Global.GetString("momentfunction"));
                beamitem.Items.Add(momentitem);

                foreach (Poly momentpoly in beam.FixedEndMoment)
                {
                    var momentchilditem = new TreeViewItem();
                    momentchilditem.Header = momentpoly.GetString(10) + " ,  " + momentpoly.StartPoint + " <= x <= " + momentpoly.EndPoint;
                    momentitem.Items.Add(momentchilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(Global.GetString("information"));
                infoitem.Header = info;
                momentitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = Global.GetString("leftside") + " : " + System.Math.Round(beam.FixedEndMoment.Calculate(0), 4) + " kNm";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = Global.GetString("rightside") + " : " + System.Math.Round(beam.FixedEndMoment.Calculate(beam.Length), 4) + " kNm";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = Global.GetString("maxvalue") + " : " + System.Math.Round(beam.FixedEndMoment.Max, 4) + " kNm";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = Global.GetString("maxloc") + " : " + System.Math.Round(beam.FixedEndMoment.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = Global.GetString("minvalue") + " : " + System.Math.Round(beam.FixedEndMoment.Min, 4) + " kNm";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = Global.GetString("minloc") + " : " + System.Math.Round(beam.FixedEndMoment.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var momentexplorer = new MomentExplorer();
                momentexplorer.xvalue.Text = "0";
                momentexplorer.funcvalue.Text = System.Math.Round(beam.FixedEndMoment.Calculate(0), 4).ToString();
                momentexplorer.xvalue.TextChanged += _mw.fixedendmomentexplorer_TextChanged;
                exploreritem.Header = momentexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.PerformStressAnalysis && beam.Stress != null)
            {
                var stressitem = new TreeViewItem();
                stressitem.Header = new StressItem(Global.GetString("stressdist"));
                beamitem.Items.Add(stressitem);

                var infoitem = new TreeViewItem();
                var info = new Information(Global.GetString("information"));
                infoitem.Header = info;
                stressitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = Global.GetString("leftside") + " : " + System.Math.Round(beam.Stress.Calculate(0), 4) + " MPa";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = Global.GetString("rightside") + " : " + System.Math.Round(beam.Stress.Calculate(beam.Length), 4) + " MPa";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = Global.GetString("maxvalue") + " : " + System.Math.Round(beam.Stress.YMax, 4) + " MPa";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = Global.GetString("maxloc") + " : " + System.Math.Round(beam.Stress.YMaxPosition, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = Global.GetString("minvalue") + " : " + System.Math.Round(beam.Stress.YMin, 4) + " MPa";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = Global.GetString("minloc") + " : " + System.Math.Round(beam.Stress.YMinPosition, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var stressexplorer = new StressExplorer();
                stressexplorer.xvalue.Text = "0";
                stressexplorer.funcvalue.Text = System.Math.Round(beam.Stress.Calculate(0), 4).ToString();
                stressexplorer.xvalue.TextChanged += _mw.stressexplorer_TextChanged;
                exploreritem.Header = stressexplorer;
                infoitem.Items.Add(exploreritem);
            }
        }

        /// <summary>
        /// Removes TreeViewBeamItem from beam tree
        /// </summary>
        /// <param name="beam">The beam of the TreeViewBeamItem.</param>
        public void RemoveBeamTree(Beam beam)
        {
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (item.Beam.Equals(beam))
                {
                    _beamtree.Items.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates all the tree view items.
        /// </summary>
        public void UpdateAllBeamTree()
        {
            MesnetMDDebug.WriteInformation("Update All Tree Started");

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:
                        Beam beam = (Beam)item.Value;
                        UpdateBeamTree(beam);
                        break;
                }
            }
        }

        public void BeamTreeItemSelected(object sender, RoutedEventArgs e)
        {
            if (BeamTreeItemSelectedEventEnabled)
            {
                _mw.Reset();

                var treeitem = sender as TreeViewItem;
                var beam = (treeitem.Header as BeamItem).Beam;

                SelectBeamItem(beam);

                foreach (var item in Global.Objects)
                {
                    switch (item.Value.Type)
                    {
                        case Global.ObjectType.Beam:

                            var beam1 = item.Value as Beam;

                            if (Equals(beam1, beam))
                            {
                                _mw.SelectBeam(beam1);
                                return;
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Selects the beam item corresponds to given beam in beam tree.
        /// </summary>
        /// <param name="beam">The beam.</param>
        public void SelectBeamItem(Beam beam)
        {
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    BeamTreeItemSelectedEventEnabled = false;
                    item.IsSelected = true;
                    BeamTreeItemSelectedEventEnabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Unselects all beam items in beam tree.
        /// </summary>
        public void UnSelectAllBeamItem()
        {
            MesnetMDDebug.WriteInformation("UnSelectAllBeamItem executed");
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                item.Selected -= BeamTreeItemSelected;
                item.IsSelected = false;
                item.Selected += BeamTreeItemSelected;
            }
        }

        #endregion

        #region Support Tree Methods and Events

        /// <summary>
        /// Updates given support in the support tree view.
        /// </summary>
        /// <param name="support">The support.</param>
        public void UpdateSupportTree(SupportItem support)
        {
            var supportitem = new TreeViewSupportItem(support);
            bool exists = false;

            switch (support.Type)
            {
                case Global.ObjectType.SlidingSupport:

                    var slidingsup = support as SlidingSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (item.Support.Type is Global.ObjectType.SlidingSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header = new SlidingSupportItem(slidingsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =  new SlidingSupportItem(slidingsup);
                    }

                    var slmembersitem = new TreeViewItem();
                    slmembersitem.Header = Global.GetString("connections");
                    supportitem.Items.Add(slmembersitem);

                    foreach (Member member in slidingsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var ssbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = ssbeamitem;
                        slmembersitem.Items.Add(memberitem);
                    }

                    if (Config.ShowDofInSupportTree)
                    {
                        var dofsitem = new TreeViewItem();
                        dofsitem.Header = "Degree Of Freedoms";
                        supportitem.Items.Add(dofsitem);
                        foreach (var dof in slidingsup.DegreeOfFreedoms)
                        {
                            var dofitem= new TreeViewItem();
                            dofitem.Header = "Dof";
                            dofsitem.Items.Add(dofitem);
                            switch (dof.Type)
                            {
                                case Global.DOFType.Horizontal:
                                    var hitem= new TreeViewItem();
                                    hitem.Header = "Type: Horizontal";
                                    dofitem.Items.Add(hitem);
                                    var hmitem = new TreeViewItem();
                                    hmitem.Header = "Dof Members";
                                    dofitem.Items.Add(hmitem);
                                    foreach (var dofmember in dof.Members)
                                    {
                                        var dmitem= new TreeViewItem();
                                        string locname=String.Empty;
                                        dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                        hmitem.Items.Add(dmitem);
                                    }

                                    break;

                                case Global.DOFType.Rotational:
                                    var ritem = new TreeViewItem();
                                    ritem.Header = "Type: Rotational";
                                    dofitem.Items.Add(ritem);
                                    var rmitem = new TreeViewItem();
                                    rmitem.Header = "Dof Members";
                                    dofitem.Items.Add(rmitem);
                                    foreach (var dofmember in dof.Members)
                                    {
                                        var dmitem = new TreeViewItem();
                                        string locname = String.Empty;
                                        dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                        rmitem.Items.Add(dmitem);
                                    }

                                    break;
                            }
                        }
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    var basicsup = support as BasicSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (item.Support.Type is Global.ObjectType.BasicSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;                      
                    }
                    else
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                    }

                    var bsmembersitem = new TreeViewItem();
                    bsmembersitem.Header = Global.GetString("connections");
                    supportitem.Items.Add(bsmembersitem);

                    foreach (Member member in basicsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var bsbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = bsbeamitem;
                        bsmembersitem.Items.Add(memberitem);
                    }

                    if (Config.ShowDofInSupportTree)
                    {
                        var dofsitem = new TreeViewItem();
                        dofsitem.Header = "Degree Of Freedoms";
                        supportitem.Items.Add(dofsitem);
                        foreach (var dof in basicsup.DegreeOfFreedoms)
                        {
                            var dofitem = new TreeViewItem();
                            dofitem.Header = "Dof";
                            dofsitem.Items.Add(dofitem);
                            switch (dof.Type)
                            {
                                case Global.DOFType.Rotational:
                                    var ritem = new TreeViewItem();
                                    ritem.Header = "Type: Rotational";
                                    dofitem.Items.Add(ritem);
                                    var rmitem = new TreeViewItem();
                                    rmitem.Header = "Dof Members";
                                    dofitem.Items.Add(rmitem);
                                    foreach (var dofmember in dof.Members)
                                    {
                                        var dmitem = new TreeViewItem();
                                        string locname = String.Empty;
                                        dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                        rmitem.Items.Add(dmitem);
                                    }

                                    break;
                            }
                        }
                    }

                    break;

                case Global.ObjectType.LeftFixedSupport:

                    var lfixedsup = support as LeftFixedSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (item.Support.Type is Global.ObjectType.LeftFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                    }

                    var lfmembersitem = new TreeViewItem();
                    lfmembersitem.Header = Global.GetString("connection");
                    supportitem.Items.Add(lfmembersitem);

                    var lfmemberitem = new TreeViewItem();

                    var lfbeamitem = new SupportBeamItem(lfixedsup.Member.Beam.BeamId, lfixedsup.Member.Direction,
                        lfixedsup.Member.Moment);

                    lfmemberitem.Header = lfbeamitem;

                    lfmembersitem.Items.Add(lfmemberitem);

                    break;

                case Global.ObjectType.RightFixedSupport:

                    var rfixedsup = support as RightFixedSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (item.Support.Type is Global.ObjectType.RightFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }

                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new RightFixedSupportItem(rfixedsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                           new RightFixedSupportItem(rfixedsup);
                    }

                    var rfmembersitem = new TreeViewItem();
                    rfmembersitem.Header = Global.GetString("connection");
                    supportitem.Items.Add(rfmembersitem);

                    var rfmemberitem = new TreeViewItem();

                    var rfbeamitem = new SupportBeamItem(rfixedsup.Member.Beam.BeamId, rfixedsup.Member.Direction,
                        rfixedsup.Member.Moment);

                    rfmemberitem.Header = rfbeamitem;

                    rfmembersitem.Items.Add(rfmemberitem);

                    break;

                case Global.ObjectType.FictionalSupport:

                    if (Config.ShowFictionalSupportInTrees)
                    {
                        var fsup = support as FictionalSupport;

                        foreach (TreeViewSupportItem item in _supporttree.Items)
                        {
                            if (item.Support.Type is Global.ObjectType.FictionalSupport)
                            {
                                if (Equals(supportitem.Support, item.Support))
                                {
                                    item.Items.Clear();
                                    supportitem = item;
                                    exists = true;
                                    break;
                                }
                            }
                        }

                        if (!exists)
                        {
                            supportitem.Header = new FictionalSupportItem(fsup);
                            _supporttree.Items.Add(supportitem);
                            supportitem.Selected += SupportTreeItemSelected;
                        }
                        else
                        {
                            supportitem.Header = new FictionalSupportItem(fsup);
                        }

                        var fmembersitem = new TreeViewItem();
                        fmembersitem.Header = Global.GetString("connections");
                        supportitem.Items.Add(fmembersitem);

                        foreach (Member member in fsup.Members)
                        {
                            var memberitem = new TreeViewItem();
                            var bsbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                            memberitem.Header = bsbeamitem;
                            fmembersitem.Items.Add(memberitem);
                        }

                        if (Config.ShowDofInSupportTree)
                        {
                            var dofsitem = new TreeViewItem();
                            dofsitem.Header = "Degree Of Freedoms";
                            supportitem.Items.Add(dofsitem);
                            foreach (var dof in fsup.DegreeOfFreedoms)
                            {
                                var dofitem = new TreeViewItem();
                                dofitem.Header = "Dof";
                                dofsitem.Items.Add(dofitem);
                                switch (dof.Type)
                                {
                                    case Global.DOFType.Horizontal:
                                        var hitem = new TreeViewItem();
                                        hitem.Header = "Type: Horizontal";
                                        dofitem.Items.Add(hitem);
                                        var hmitem = new TreeViewItem();
                                        hmitem.Header = "Dof Members";
                                        dofitem.Items.Add(hmitem);
                                        foreach (var dofmember in dof.Members)
                                        {
                                            var dmitem = new TreeViewItem();
                                            string locname = String.Empty;
                                            dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                            hmitem.Items.Add(dmitem);
                                        }

                                        break;

                                    case Global.DOFType.Vertical:
                                        var vitem = new TreeViewItem();
                                        vitem.Header = "Type: Vertical";
                                        dofitem.Items.Add(vitem);
                                        var vmitem = new TreeViewItem();
                                        vmitem.Header = "Dof Members";
                                        dofitem.Items.Add(vmitem);
                                        foreach (var dofmember in dof.Members)
                                        {
                                            var dmitem = new TreeViewItem();
                                            string locname = String.Empty;
                                            dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                            vmitem.Items.Add(dmitem);
                                        }

                                        break;

                                    case Global.DOFType.Rotational:
                                        var ritem = new TreeViewItem();
                                        ritem.Header = "Type: Rotational";
                                        dofitem.Items.Add(ritem);
                                        var rmitem = new TreeViewItem();
                                        rmitem.Header = "Dof Members";
                                        dofitem.Items.Add(rmitem);
                                        foreach (var dofmember in dof.Members)
                                        {
                                            var dmitem = new TreeViewItem();
                                            string locname = String.Empty;
                                            dmitem.Header = dofmember.Beam.Name + " " + doflocname(dofmember.Location);
                                            rmitem.Items.Add(dmitem);
                                        }

                                        break;
                                }
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Removes TreeViewSupportItem from support tree.
        /// </summary>
        /// <param name="support">The support of the TreeViewSupportItem.</param>
        public void RemoveSupportTree(SupportItem support)
        {
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                if (item.Support.Equals(support))
                {
                    _supporttree.Items.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates all the support tree view items.
        /// </summary>
        public void UpdateAllSupportTree()
        {
            MesnetMDDebug.WriteInformation("Update All Support Tree Started");
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.SlidingSupport:

                        var sliding = item.Value as SlidingSupport;
                        UpdateSupportTree(sliding);

                        break;

                    case Global.ObjectType.BasicSupport:

                        var basic = item.Value as BasicSupport;
                        UpdateSupportTree(basic);

                        break;

                    case Global.ObjectType.LeftFixedSupport:

                        var left = item.Value as LeftFixedSupport;
                        UpdateSupportTree(left);

                        break;

                    case Global.ObjectType.RightFixedSupport:

                        var right = item.Value as RightFixedSupport;
                        UpdateSupportTree(right);

                        break;

                    case Global.ObjectType.FictionalSupport:

                        if (Config.ShowFictionalSupportInTrees)
                        {
                            var fictional = item.Value as FictionalSupport;
                            UpdateSupportTree(fictional);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Selects the support item corresponds to given support in support tree.
        /// </summary>
        /// <param name="support">The support.</param>
        public void SelectSupportItem(SupportItem support)
        {
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                if (Equals(support, item.Support))
                {
                    SupportTreeItemSelectedEventEnabled = false;
                    item.IsSelected = true;
                    SupportTreeItemSelectedEventEnabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Unselects all support items in support tree.
        /// </summary>
        public void UnSelectAllSupportItem()
        {
            MesnetMDDebug.WriteInformation("UnSelectAllSupportItem executed");
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                item.Selected -= SupportTreeItemSelected;
                item.IsSelected = false;
                item.Selected += SupportTreeItemSelected;
            }
        }

        /// <summary>
        /// Executed when any support item selected in the treeview. It selects and highlights corresponding support.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SupportTreeItemSelected(object sender, RoutedEventArgs e)
        {
            if (SupportTreeItemSelectedEventEnabled)
            {
                MesnetMDDebug.WriteInformation("SupportTreeItemSelected");
                _mw.Reset();
                var treeitem = sender as TreeViewItem;

                if (treeitem.Header is SlidingSupportItem ssitem)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.SlidingSupport:

                                var slidingsupport = item.Value as SlidingSupport;

                                if (Equals(ssitem.Support, slidingsupport))
                                {
                                    slidingsupport.Select();
                                    _mw.selectesupport = slidingsupport;
                                    SelectSupportItem(slidingsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is BasicSupportItem bsitem)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.BasicSupport:

                                var basicsupport = item.Value as BasicSupport;

                                if (Equals(bsitem.Support, basicsupport))
                                {
                                    basicsupport.Select();
                                    _mw.selectesupport = basicsupport;
                                    SelectSupportItem(basicsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is LeftFixedSupportItem lsitem)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.LeftFixedSupport:

                                var leftfixedsupport = item.Value as LeftFixedSupport;

                                if (Equals(lsitem.Support, leftfixedsupport))
                                {
                                    leftfixedsupport.Select();
                                    _mw.selectesupport = leftfixedsupport;
                                    SelectSupportItem(leftfixedsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is RightFixedSupportItem rsitem)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.RightFixedSupport:

                                var rightfixedsupport = item.Value as RightFixedSupport;

                                if (Equals(rsitem.Support, rightfixedsupport))
                                {
                                    rightfixedsupport.Select();
                                    _mw.selectesupport = rightfixedsupport;
                                    SelectSupportItem(rightfixedsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is FictionalSupportItem fsitem)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.FictionalSupport:

                                var fictionalsupport = item.Value as FictionalSupport;

                                if (Equals(fsitem.Support, fictionalsupport))
                                {
                                    fictionalsupport.Select();
                                    _mw.selectesupport = fictionalsupport;
                                    SelectSupportItem(fictionalsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
            }
        }

        public void DeleteSupportItem(SupportItem support)
        {
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                if (Equals(item.Support, support))
                {
                    _supporttree.Items.Remove(item);
                    break;
                }
            }
        }
        #endregion

        private string doflocname(Global.DOFLocation loc)
        {
            string locname = String.Empty;           
            switch (loc)
            {
                case Global.DOFLocation.LeftHorizontal:
                    locname = "LeftHorizontal";
                    break;
                case Global.DOFLocation.LeftVertical:
                    locname = "LeftVertical";
                    break;
                case Global.DOFLocation.LeftRotational:
                    locname = "LeftRotational";
                    break;
                case Global.DOFLocation.RightHorizontal:
                    locname = "RightHorizontal";
                    break;
                case Global.DOFLocation.RightVertical:
                    locname = "RightVertical";
                    break;
                case Global.DOFLocation.RightRotational:
                    locname = "RightRotational";
                    break;
            }

            return locname;
        }
    }
}
