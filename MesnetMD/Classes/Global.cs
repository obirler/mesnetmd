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
using System.Globalization;
using System.Threading;
using System.Windows;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;
using MesnetMD.Properties;

namespace MesnetMD.Classes
{
    public static class Global
    {
        public static Dictionary<int, SomItem> Objects = new Dictionary<int, SomItem>();

        public static void UpdateMaxInertia()
        {
            MaxInertia = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.Inertia?.Count > 0)
                        {
                            if (beam.MaxInertia > MaxInertia)
                            {
                                MaxInertia = beam.MaxInertia;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxArea()
        {
            MaxArea = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.Area?.Count > 0)
                        {
                            if (beam.MaxArea > MaxArea)
                            {
                                MaxArea = beam.MaxArea;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxDistLoad()
        {
            MaxDistLoad = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.DistributedLoads?.Count > 0)
                        {
                            if (beam.MaxAbsDistLoad > MaxDistLoad)
                            {
                                MaxDistLoad = beam.MaxAbsDistLoad;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxConcLoad()
        {
            MaxConcLoad = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.ConcentratedLoads?.Count > 0)
                        {
                            if (beam.MaxAbsConcLoad > MaxConcLoad)
                            {
                                MaxConcLoad = beam.MaxAbsConcLoad;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxMoment()
        {            
            MaxMoment = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                       
                        if (beam.FixedEndMoment?.Count > 0)
                        {
                            if (beam.MaxAbsMoment > MaxMoment)
                            {
                                MaxMoment = beam.MaxAbsMoment;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxForce()
        {
            MaxForce = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.FixedEndForce?.Count > 0)
                        {
                            if (beam.MaxAbsForce > MaxForce)
                            {
                                MaxForce = beam.MaxAbsForce;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxAxialForce()
        {
            MaxAxialForce = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.AxialForce?.Count > 0)
                        {
                            if (beam.MaxAbsAxialForce > MaxAxialForce)
                            {
                                MaxAxialForce = beam.MaxAbsAxialForce;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateMaxStress()
        {
            MaxStress = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.Stress?.Count > 0)
                        {
                            if (beam.MaxAbsStress > MaxStress)
                            {
                                MaxStress = beam.MaxAbsStress;
                            }
                        }

                        break;
                }
            }
        }

        public static void UpdateAllMaxValues()
        {
            MaxInertia = Double.MinValue;
            MaxArea = Double.MinValue;         
            MaxDistLoad = Double.MinValue;
            MaxConcLoad = Double.MinValue;
            MaxMoment = Double.MinValue;
            MaxForce = Double.MinValue;
            MaxAxialForce = Double.MinValue;
            MaxStress = Double.MinValue;

            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.MaxInertia > MaxInertia)
                        {
                            MaxInertia = beam.MaxInertia;
                        }
                        if (beam.MaxArea > MaxArea)
                        {
                            MaxArea = beam.MaxArea;
                        }
                        if (beam.DistributedLoads?.Count > 0)
                        {
                            if (beam.MaxAbsDistLoad > MaxDistLoad)
                            {
                                MaxDistLoad = beam.MaxAbsDistLoad;
                            }
                        }
                        if (beam.ConcentratedLoads?.Count > 0)
                        {
                            if (beam.MaxAbsConcLoad > MaxConcLoad)
                            {
                                MaxConcLoad = beam.MaxAbsConcLoad;
                            }
                        }
                        if (beam.FixedEndMoment?.Count > 0)
                        {
                            if (beam.MaxMoment > MaxMoment)
                            {
                                MaxMoment = beam.MaxMoment;
                            }
                        }
                        if (beam.FixedEndForce?.Count > 0)
                        {
                            if (beam.MaxForce > MaxForce)
                            {
                                MaxForce = beam.MaxForce;
                            }
                        }
                        if (beam.AxialForce?.Count > 0)
                        {
                            if (beam.MaxAxialForce > MaxAxialForce)
                            {
                                MaxAxialForce = beam.MaxAxialForce;
                            }
                        }
                        if (beam.Stress?.Count > 0)
                        {
                            if (beam.MaxStress > MaxStress)
                            {
                                MaxStress = beam.MaxStress;
                            }
                        }
                        break;
                }
            }           
        }

        public static bool AnyInertia()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.Inertia?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyArea()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.Area?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyDistLoad()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.DistributedLoads?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyConcLoad()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.ConcentratedLoads?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyStress()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.Stress?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyMoment()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.FixedEndMoment?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyForce()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.FixedEndForce?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        public static bool AnyAxialForce()
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;
                        if (beam.AxialForce?.Count > 0)
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the language of the application using system language.
        /// </summary>
        public static void SetLanguageDictionary()
        {
            if (App.Current.Resources.MergedDictionaries.Count != 0)
            {
                App.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "tr-TR":

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);
                    Settings.Default.language = "tr-TR";

                    break;

                default:

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);
                    Settings.Default.language = "en-EN";

                    break;
            }
            Settings.Default.Save();
            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        /// <summary>
        /// Sets the language of the application to the given language.
        /// </summary>
        /// <param name="lang">The desired language.</param>
        public static void SetLanguageDictionary(string lang)
        {
            if (App.Current.Resources.MergedDictionaries.Count != 0)
            {
                App.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "tr-TR":

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);
                    Settings.Default.language = "tr-TR";

                    break;

                default:

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);
                    Settings.Default.language = "en-EN";

                    break;
            }
            Settings.Default.Save();
            App.Current.Resources.MergedDictionaries.Add(dict);
        }      

        /// <summary>
        /// Gets the string by key from the current application language.
        /// </summary>
        /// <param name="key">The key of the desired string.</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return App.Current.Resources.MergedDictionaries[0][key].ToString();
        }

        public static void AddObject(SomItem obj)
        {
            Objects.Add(obj.Id, obj);
        }

        public static void RemoveObject(SomItem obj)
        {
            Objects.Remove(obj.Id);
        }

        public static Beam GetBeam(string Name)
        {
            foreach (var item in Objects)
            {
                switch (item.Value.Type)
                {
                    case ObjectType.Beam:

                        var beam = item.Value as Beam;

                        if (beam.Name == Name)
                        {
                            return beam;
                        }

                        break;                       
                }
            }
            return null;
        }

        public static SomItem GetObject(int id)
        {
            return Objects[id];
        }

        public static void SetDecimalSeperator()
        {
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public static double Scale = 1;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            None
        }

        public enum FunctionType
        {
            Fixed,
            Variable
        }

        public enum SupportType
        {
            BasicSupoort,
            SlidingSupport,
            LeftFixedSupport,
            RightFixedSupport,
            FictionalSupport
        }

        public enum CalculationType
        {
            SingleThreaded,
            MultiThreaded
        }

        public enum ObjectType
        {
            Beam,
            LeftFixedSupport,
            RightFixedSupport,
            BasicSupport,
            SlidingSupport,
            FictionalSupport,
            None
        }

        public enum GraphicType
        {
            ConcentratedLoad,
            DistibutedLoad,
            Inertia,
            Area,
            Force,
            Moment,
            Stress
        }

        public enum DialogResult
        {
            None,
            Yes,
            No,
            Cancel
        }

        public enum ItemType
        {
            SomItem,
            GraphicItem
        }

        public enum ManifestType
        {
            BeamManifest,
            BasicSupportManifest,
            SlidingSupportManifest,
            LeftFixedSupportManifest,
            RightFixedSupportManifest
        }

        public enum DOFType
        {
            Horizontal,
            Vertical,
            Rotational
        }

        public enum DOFLocation
        {
            LeftHorizontal=0,
            LeftVertical=1,
            LeftRotational=2,
            RightHorizontal=3,
            RightVertical=4,
            RightRotational=5
        }

        public enum SimpsonIntegrationType
        {
            First,
            Second
        }

        public static void WritePPolytoConsole(string message, PiecewisePoly ppoly)
        {
            foreach (Poly poly in ppoly)
            {
                MesnetMDDebug.WriteInformation(message + " : " + poly.ToString() + " , " + poly.StartPoint + " <= x <= " + poly.EndPoint);
            }
        }

        public static int BeamCount = 0;

        public static int SupportCount = 0;

        public struct Func
        {
            public int id;
            public double xposition;
            public double yposition;
        }

        public static List<string> LogList = new List<string>();


        #region Definitions

        public static double MaxMoment = Double.MinValue;

        public static double MaxForce = Double.MinValue;

        public static double MaxAxialForce = Double.MinValue;

        public static double MaxLoad = Double.MinValue;

        public static double MaxDeflection = Double.MinValue;

        public static double MaxStress = Double.MinValue;

        public static double MaxInertia = Double.MinValue;

        public static double MaxArea = Double.MinValue;

        public static double MaxDistLoad = Double.MinValue;

        public static double MaxConcLoad = Double.MinValue;

        #endregion
    }
}
