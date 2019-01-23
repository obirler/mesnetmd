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

using System.Windows;
using System.Windows.Media;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui
{
    public class UpToolBar
    {
        public UpToolBar(MainWindow mw)
        {
            _mw = mw;

            bindevents();

            hideall();
        }

        private MainWindow _mw;

        private bool _inertiaslider = true;

        private bool _distloadslider = true;

        private bool _concloadslider = true;

        private bool _momentslider = true;

        private bool _forceslider = true;

        private bool _stressslider = true;

        private void hideall()
        {
            _mw.inertiaborder.Visibility = Visibility.Collapsed;
            _mw.areaborder.Visibility = Visibility.Collapsed;
            _mw.distloadborder.Visibility = Visibility.Collapsed;
            _mw.concloadborder.Visibility = Visibility.Collapsed;
            _mw.momentborder.Visibility = Visibility.Collapsed;
            _mw.forceborder.Visibility = Visibility.Collapsed;
            _mw.axialforceborder.Visibility = Visibility.Collapsed;
            _mw.stressborder.Visibility = Visibility.Collapsed;
        }

        private void bindevents()
        {
            _mw.solvebtn.Click += solvebtn_Click;

            _mw.inertiaexpander.Expanded += inertiaexpander_Expanded;
            _mw.inertiaexpander.Collapsed += inertiaexpander_Collapsed;
            _mw.inertiaslider.ValueChanged += inertiaslider_ValueChanged;

            _mw.areaexpander.Expanded += areaexpander_Expanded;
            _mw.areaexpander.Collapsed += areaexpander_Collapsed;
            _mw.areaslider.ValueChanged += areaslider_ValueChanged;

            _mw.distloadexpander.Expanded += distloadexpander_Expanded;
            _mw.distloadexpander.Collapsed += distloadexpander_Collapsed;
            _mw.distloadslider.ValueChanged += distloadslider_ValueChanged;

            _mw.concloadexpander.Expanded += concloadexpander_Expanded;
            _mw.concloadexpander.Collapsed += concloadexpander_Collapsed;
            _mw.concloadslider.ValueChanged += concloadslider_ValueChanged;

            _mw.momentexpander.Expanded += momentexpander_Expanded;
            _mw.momentexpander.Collapsed += momentexpander_Collapsed;
            _mw.momentslider.ValueChanged += momentslider_ValueChanged;

            _mw.forceexpander.Expanded += forceexpander_Expanded;
            _mw.forceexpander.Collapsed += forceexpander_Collapsed;
            _mw.forceslider.ValueChanged += forceslider_ValueChanged;

            _mw.axialforceexpander.Expanded += axiaforceexpander_Expanded;
            _mw.axialforceexpander.Collapsed += axiaforceexpander_Collapsed;
            _mw.axialforceslider.ValueChanged += axiaforceslider_ValueChanged;

            _mw.stressexpander.Expanded += stressexpander_Expanded;
            _mw.stressexpander.Collapsed += stressexpander_Collapsed;
            _mw.stressslider.ValueChanged += stressslider_ValueChanged;
        }

        private void solvebtn_Click(object sender, RoutedEventArgs e)
        {
            _mw.InitializeSolution();
        }

        public void ActivateInertia()
        {
            _mw.inertiaborder.Visibility = Visibility.Visible;
        }

        public void DeActivateInertia()
        {
            _mw.inertiaborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateArea()
        {
            _mw.areaborder.Visibility = Visibility.Visible;
        }

        public void DeActivateArea()
        {
            _mw.areaborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateDistLoad()
        {
            _mw.distloadborder.Visibility = Visibility.Visible;
        }

        public void DeActivateDistLoad()
        {
            _mw.distloadborder.Visibility = Visibility.Collapsed;
        }

        public void ShowDistLoad()
        {
            _mw.distloadexpander.IsExpanded = true;
        }

        public void ActivateConcLoad()
        {
            _mw.concloadborder.Visibility = Visibility.Visible;
        }

        public void DeActivateConcLoad()
        {
            _mw.concloadborder.Visibility = Visibility.Collapsed;
        }

        public void ShowConcLoad()
        {
            _mw.concloadexpander.IsExpanded = true;
        }

        public void ActivateMoment()
        {
            _mw.momentborder.Visibility = Visibility.Visible;
        }

        public void DeActivateMoment()
        {
            _mw.momentborder.Visibility = Visibility.Collapsed;
        }

        public void ShowMoments()
        {
            _mw.momentexpander.IsExpanded = true;
        }

        public void ActivateForce()
        {
            _mw.forceborder.Visibility = Visibility.Visible;
        }

        public void DeActivateForce()
        {
            _mw.forceborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateAxialForce()
        {
            _mw.axialforceborder.Visibility = Visibility.Visible;
        }

        public void DeActivateAxialForce()
        {
            _mw.axialforceborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateStress()
        {
            _mw.stressborder.Visibility = Visibility.Visible;
        }

        public void DeActivateStress()
        {
            _mw.stressborder.Visibility = Visibility.Collapsed;
        }

        public void DeActivateAll()
        {
            DeActivateInertia();
            DeActivateArea();
            DeActivateDistLoad();
            DeActivateConcLoad();
            DeActivateMoment();
            DeActivateForce();
            DeActivateAxialForce();
            DeActivateStress();
        }

        public void CollapseInertia()
        {
            _mw.inertiaexpander.IsExpanded = false;
        }

        public void CollapseArea()
        {
            _mw.areaexpander.IsExpanded = false;
        }

        public void CollapseDistLoad()
        {
            _mw.distloadexpander.IsExpanded = false;
        }

        public void CollapseConcLoad()
        {
            _mw.concloadexpander.IsExpanded = false;
        }

        public void CollapseMoment()
        {
            _mw.momentexpander.IsExpanded = false;
        }

        public void CollapseForce()
        {
            _mw.forceexpander.IsExpanded = false;
        }

        public void CollapseAxialForce()
        {
            _mw.axialforceexpander.IsExpanded = false;
        }

        public void CollapseStress()
        {
            _mw.stressexpander.IsExpanded = false;
        }

        public void CollapseAll()
        {
            CollapseInertia();
            CollapseArea();
            CollapseConcLoad();
            CollapseDistLoad();
            CollapseMoment();
            CollapseForce();
            CollapseAxialForce();
            CollapseStress();
        }

        #region diagram updates

        public void UpdateInertiaDiagrams()
        {
            if (Global.AnyInertia())
            {
                Global.UpdateMaxInertia();
                ActivateInertia();
                if (_mw.inertiaexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item .Value as Beam;
                                beam.ReDrawInertia((int)_mw.inertiaslider.Value);
                                break;
                        }
                    }
                }
            }
            else
            {
                DeActivateInertia();
                CollapseInertia();
            }
        }

        public void UpdateAreaDiagrams()
        {
            if (Global.AnyArea())
            {
                Global.UpdateMaxArea();
                ActivateArea();
                if (_mw.areaexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawArea((int)_mw.areaslider.Value);
                                break;
                        }
                    }
                }
            }
            else
            {
                DeActivateArea();
                CollapseArea();
            }
        }

        public void UpdateDistloadDiagrams()
        {           
            if (Global.AnyDistLoad())
            {
                Global.UpdateMaxDistLoad();
                ActivateDistLoad();
                if (_mw.distloadexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawDistLoad((int)_mw.distloadslider.Value);
                                break;
                        }
                    }
                }
                else
                {
                    _mw.distloadexpander.IsExpanded = true;
                }
            }
            else
            {
                CollapseDistLoad();
                DeActivateDistLoad();
            }
        }

        public void UpdateConcloadDiagrams()
        {           
            if (Global.AnyConcLoad())
            {
                Global.UpdateMaxConcLoad();
                ActivateConcLoad();
                if (_mw.concloadexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawConcLoad((int)_mw.concloadslider.Value);
                                break;
                        }
                    }
                }
                else
                {
                    _mw.concloadexpander.IsExpanded = true;
                }
            }
            else
            {
                CollapseConcLoad();
                DeActivateConcLoad();
            }
        }

        public void UpdateMomentDiagrams(bool show = false)
        {
            if (Global.AnyMoment())
            {
                Global.UpdateMaxMoment();
                ActivateMoment();
                if (_mw.momentexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawMoment((int)_mw.momentslider.Value);
                                break;
                        }
                    }
                }
                if(show)
                {
                    _mw.momentexpander.IsExpanded = true;
                }
            }           
        }

        public void UpdateForceDiagrams(bool show = false)
        {
            if (Global.AnyForce())
            {
                Global.UpdateMaxForce();
                ActivateForce();
                if (_mw.forceexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawForce((int)_mw.forceslider.Value);
                                break;
                        }
                    }
                }
                if (show)
                {
                    _mw.forceexpander.IsExpanded = true;
                }
            }           
        }

        public void UpdateAxialForceDiagrams(bool show = false)
        {
            if (Global.AnyAxialForce())
            {
                Global.UpdateMaxAxialForce();
                ActivateAxialForce();
                if (_mw.axialforceexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawAxialForce((int)_mw.axialforceslider.Value);
                                break;
                        }
                    }
                }
                if (show)
                {
                    _mw.axialforceexpander.IsExpanded = true;
                }
            }
        }

        public void UpdateStressDiagrams(bool show = false)
        {            
            if (Global.AnyStress())
            {
                Global.UpdateMaxStress();
                ActivateStress();
                if (_mw.stressexpander.IsExpanded)
                {
                    foreach (var item in Global.Objects)
                    {
                        switch (item.Value.Type)
                        {
                            case Global.ObjectType.Beam:

                                var beam = item.Value as Beam;
                                beam.ReDrawStress((int)_mw.stressslider.Value);
                                break;
                        }
                    }
                }
                if (show)
                {
                    _mw.stressexpander.IsExpanded = true;
                }
            }          
        }

        /// <summary>
        /// Updates the max value of diagrams and shows concentrated and distributed loads.
        /// It is used before the solution
        /// </summary>
        public void UpdateLoadDiagrams()
        {
            UpdateInertiaDiagrams();
            UpdateAreaDiagrams();
            UpdateDistloadDiagrams();
            UpdateConcloadDiagrams();
        }

        /// <summary>
        /// Updates all diagrams, max value and draws according to expander states.
        /// </summary>
        public void UpdateAllDiagrams()
        {
            UpdateInertiaDiagrams();
            UpdateAreaDiagrams();
            UpdateDistloadDiagrams();
            UpdateConcloadDiagrams();
            UpdateForceDiagrams();
            UpdateAxialForceDiagrams();
            UpdateMomentDiagrams();
            UpdateStressDiagrams();
        }

        #endregion

        #region Toolbar Events

        private void inertiaexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.inertiaborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawInertia((int)_mw.inertiaslider.Value);

                        break;
                }
            }
        }

        private void inertiaexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.inertiaborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyInertiaDiagram();

                        break;
                }
            }
        }

        private void inertiaslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawInertia((int)e.NewValue);

                        break;
                }
            }
        }

        private void areaexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.areaborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawArea((int)_mw.areaslider.Value);

                        break;
                }
            }
        }

        private void areaexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.areaborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyAreaDiagram();

                        break;
                }
            }
        }

        private void areaslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawArea((int)e.NewValue);

                        break;
                }
            }
        }

        private void distloadexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.distloadborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawDistLoad((int)_mw.distloadslider.Value);

                        break;
                }
            }
        }

        private void distloadexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.distloadborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyDistLoadDiagram();

                        break;
                }
            }
        }

        private void distloadslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("distloadslider value changed : " + e.NewValue);
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.DistributedLoads != null)
                        {
                            if (beam.DistributedLoads.Count > 0)
                            {
                                beam.ReDrawDistLoad((int)e.NewValue);
                            }
                        }
                        break;
                }
            }
        }

        private void concloadexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.concloadborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawConcLoad((int)_mw.concloadslider.Value);

                        break;
                }
            }
        }

        private void concloadexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.concloadborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyConcLoadDiagram();

                        break;
                }
            }
        }

        private void concloadslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("distloadslider value changed : " + e.NewValue);
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
                                beam.ReDrawConcLoad((int)e.NewValue);
                            }
                        }
                        break;
                }
            }
        }

        private void momentexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.momentborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawMoment((int)_mw.momentslider.Value);

                        break;
                }
            }
        }

        private void momentexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.momentborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyFixedEndMomentDiagram();

                        break;
                }
            }
        }

        private void momentslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("momentslider value changed : " + e.NewValue);
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawMoment((int)e.NewValue);

                        break;
                }
            }
        }

        private void forceexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.forceborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawForce((int)_mw.forceslider.Value);

                        break;
                }
            }
        }

        private void forceexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.forceborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyFixedEndForceDiagram();

                        break;
                }
            }
        }

        private void forceslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("forceslider value changed : " + e.NewValue);
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawForce((int)e.NewValue);

                        break;
                }
            }
        }

        private void axiaforceexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.axialforceborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawAxialForce((int)_mw.axialforceslider.Value);

                        break;
                }
            }
        }

        private void axiaforceexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.axialforceborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyAxialForceDiagram();

                        break;
                }
            }
        }

        private void axiaforceslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("axialforceslider value changed : " + e.NewValue);
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawAxialForce((int)e.NewValue);

                        break;
                }
            }
        }

        private void stressexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.stressborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.ReDrawStress((int)_mw.stressslider.Value);

                        break;
                }
            }
        }

        private void stressexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.stressborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        Beam beam = item.Value as Beam;
                        beam.DestroyStressDiagram();

                        break;
                }
            }
        }

        private void stressslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MesnetMDDebug.WriteInformation("stressslider value changed : " + e.NewValue);
            foreach (var item in Global.Objects)
            {
                switch (item.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var beam = item.Value as Beam;
                        beam.ReDrawStress((int)e.NewValue);

                        break;
                }
            }
        }
        
        #endregion
    }
}
