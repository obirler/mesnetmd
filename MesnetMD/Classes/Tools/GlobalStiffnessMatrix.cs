using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesnetMD.Classes.Ui;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Tools
{
    public static class GlobalStiffnessMatrix
    {
        public static void Calculate()
        {
            createdofpairs();
        }

        private static void createdofpairs()
        {
            DofPairs = new Dictionary<int, DOF>();
            int count = 1;
            foreach (var pair in Global.Objects)
            {
                switch (pair.Value.Type)
                {
                    case Global.ObjectType.BasicSupport:
                        var bs = pair.Value as BasicSupport;
                        var rbdof = bs.DegreeOfFreedoms[0];
                        DofPairs.Add(count, rbdof);
                        count++;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = pair.Value as SlidingSupport;
                        var hsdof = ss.DegreeOfFreedoms[0];
                        DofPairs.Add(count, hsdof);
                        count++;
                        var rsdof = ss.DegreeOfFreedoms[1];
                        DofPairs.Add(count, rsdof);
                        count++;
                        break;

                    case Global.ObjectType.FictionalSupport:
                        var fs = pair.Value as FictionalSupport;
                        var hfdof = fs.DegreeOfFreedoms[0];
                        DofPairs.Add(count, hfdof);
                        count++;
                        var vbdof = fs.DegreeOfFreedoms[1];
                        DofPairs.Add(count, vbdof);
                        count++;
                        var rfdof = fs.DegreeOfFreedoms[2];
                        DofPairs.Add(count, rfdof);
                        count++;
                        break;
                }
            }

            DofCount = DofPairs.Count;
        }

        private static void sortsupports()
        {
            foreach (var pair in Global.Objects)
            {
                
            }
        }

        public static int DofCount = 0;

        private static List<SupportItem> supports;

        public static Dictionary<int, DOF> DofPairs;

        public static double[,] Matrix;
    }
}
