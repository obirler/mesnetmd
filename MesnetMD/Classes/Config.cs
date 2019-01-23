using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MesnetMD.Classes
{
    public static class Config
    {
        public static string VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static double SimpsonStep = 0.0001;
        
        public static bool ShowDofInSupportTree = true;

        public static bool ShowFictionalSupportInTrees = true;

        public static Global.CalculationType Calculation = Global.CalculationType.SingleThreaded;

        public static SolidColorBrush InertiaColor = new SolidColorBrush(Colors.Indigo);

        public static SolidColorBrush ConcentratedLoadColor = new SolidColorBrush(Colors.Black);

        public static SolidColorBrush DistributedLoadColor = new SolidColorBrush(Colors.Black);

        public static SolidColorBrush ForceColor = new SolidColorBrush(Colors.Blue);

        public static SolidColorBrush MomentColor = new SolidColorBrush(Colors.Red);

        public static SolidColorBrush StressUnderColor = new SolidColorBrush(Colors.Green);

        public static SolidColorBrush StressOverColor = new SolidColorBrush(Colors.Red);
    }
}
