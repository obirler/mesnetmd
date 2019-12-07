using System;
using System.Collections.Generic;
using System.IO;
using MesnetMD.Classes.IO;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Tools
{
    public static class MDSolver
    {
        public static void Calculate()
        {
            createdofpairs();
            writedofpairs();
            createglobalstiffnessmatrix();
            createglobalforcevector();
            solvethesystem();
            obtainbeamdisplacements();
        }

        private static void createdofpairs()
        {
            GlobalDofs = new List<DOF>();
            foreach (var pair in Global.Objects)
            {
                switch (pair.Value.Type)
                {
                    case Global.ObjectType.BasicSupport:
                        var bs = pair.Value as BasicSupport;
                        var rbdof = bs.DegreeOfFreedoms[0];
                        GlobalDofs.Add(rbdof);
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = pair.Value as SlidingSupport;
                        var hsdof = ss.DegreeOfFreedoms[0];
                        GlobalDofs.Add(hsdof);
                        var rsdof = ss.DegreeOfFreedoms[1];
                        GlobalDofs.Add(rsdof);
                        break;

                    case Global.ObjectType.FictionalSupport:
                        var fs = pair.Value as FictionalSupport;
                        var hfdof = fs.DegreeOfFreedoms[0];
                        GlobalDofs.Add(hfdof);
                        var vbdof = fs.DegreeOfFreedoms[1];
                        GlobalDofs.Add(vbdof);
                        var rfdof = fs.DegreeOfFreedoms[2];
                        GlobalDofs.Add(rfdof);
                        break;
                }
            }

            DofCount = GlobalDofs.Count;                   
        }

        private static void createglobalstiffnessmatrix()
        {
            GlobalStiffnessMatrix = new double[DofCount, DofCount];

            for (int i = 0; i < GlobalDofs.Count; i++)
            {
                for (int j = 0; j < GlobalDofs.Count; j++)
                {
                    double value = 0;
                    foreach (var member in GlobalDofs[i].Members)
                    {
                        var beam = member.Beam;
                        int index = getbeamindex(GlobalDofs[j].Members, beam);
                        if (index > -1)
                        {
                            int row = (int) member.Location;
                            int col = (int) GlobalDofs[j].Members[index].Location;
                            value += beam.StiffnessMatrix[row, col];
                        }
                    }

                    GlobalStiffnessMatrix[i, j] = value;
                }
            }

            MesnetMDDebug.WriteInformation("Global Stiffness Matrix:");
            Logger.WriteLine("Global Stiffness Matrix:");
            int r = GlobalStiffnessMatrix.GetLength(0);
            int c = GlobalStiffnessMatrix.GetLength(1);

            for (int i = 0; i < r; i++)
            {
                string str = String.Empty;

                for (int j = 0; j < c; j++)
                {
                    str += GlobalStiffnessMatrix[i, j].ToString("F15");
                    if (j != c - 1)
                    {
                        str += " ";
                    }
                }
                MesnetMDDebug.WriteInformation(str);
                Logger.WriteLine(str);
            }
        }

        private static void createglobalforcevector()
        {
            GlobalForceVector = new double[DofCount];

            MesnetMDDebug.WriteInformation("Global ShearForce Vector:");
            Logger.WriteLine("Global ShearForce Vector:");

            for (int i = 0; i < GlobalDofs.Count; i++)
            {
                double value = 0;
                foreach (var member in GlobalDofs[i].Members)
                {
                    var beam = member.Beam;
                    var index = (int) member.Location;
                    value += beam.ForceVector[index];
                }

                GlobalForceVector[i] = value;

                MesnetMDDebug.WriteInformation(GlobalForceVector[i].ToString("F15"));
                Logger.WriteLine(GlobalForceVector[i].ToString("F15"));
            }         
        }

        private static void solvethesystem()
        {
            GlobalDisplacementVector = MesnetMD.Classes.Math.Algebra.LinearEquationSolver(GlobalStiffnessMatrix, GlobalForceVector);

            MesnetMDDebug.WriteInformation("Global Displacement Vector:");
            Logger.WriteLine("Global Displacement Vector:");

            for (int i = 0; i < GlobalDisplacementVector.GetLength(0); i++)
            {
                MesnetMDDebug.WriteInformation(GlobalDisplacementVector[i].ToString("F15"));
                Logger.WriteLine(GlobalDisplacementVector[i].ToString("F15"));
            }
        }

        private static void obtainbeamdisplacements()
        {
            foreach (var pair in Global.Objects)
            {
                switch (pair.Value.Type)
                {
                    case Global.ObjectType.Beam:

                        var displacement = new double[6];

                        var beam = pair.Value as Beam;

                        for (int i = 0; i < GlobalDofs.Count; i++)
                        {
                            foreach (var member in GlobalDofs[i].Members)
                            {
                                if (Equals(beam, member.Beam))
                                {
                                    int index = (int)member.Location;

                                    displacement[index] = GlobalDisplacementVector[i];
                                    break;
                                }
                            }
                        }

                        beam.UpdateDirectForceVector(displacement);

                        break;
                }
            }
            /*for (int j = 0; j < Global.Objects.; j++)
            {
                switch (Global.Objects[j].Type)
                {
                    case Global.ObjectType.Beam:

                        var displacement = new double[6];

                        var beam = Global.Objects[j] as Beam;

                        for (int i = 0; i < GlobalDofs.Count; i++)
                        {
                            foreach (var member in GlobalDofs[i].Members)
                            {
                                if (Equals(beam, member.Beam))
                                {
                                    int index = (int)member.Location;

                                    displacement[index] = GlobalDisplacementVector[i];
                                    break;                                   
                                }
                            }
                        }

                        beam.UpdateDirectForceVector(displacement);

                        break;
                }
            }*/
            
        }

        public static int DofCount = 0;

        private static List<SupportItem> supports;

        public static List<DOF> GlobalDofs;

        public static double[,] GlobalStiffnessMatrix;

        public static double[] GlobalForceVector;

        public static double[] GlobalDisplacementVector;

        private static void writedofpairs()
        {
            using (var stw = new StreamWriter(@"dofpairs.txt"))
            {
                for (int j = 0; j < GlobalDofs.Count; j++)
                {
                    stw.WriteLine("-----------------" + j + "-----------------");
                    for (int i = 0; i < GlobalDofs[j].Members.Count; i++)
                    {
                        stw.WriteLine(" " + GlobalDofs[j].Members[i].Beam.Name + " " + GlobalDofs[j].Members[i].Location + " = " + (int)GlobalDofs[j].Members[i].Location);
                    }
                }
            }  
        }

        private static int getbeamindex(List<DOFMember> members, Beam beam)
        {
            foreach (var member in members)
            {
                if (Equals(member.Beam, beam))
                {
                    return members.IndexOf(member);
                }
            }
            return -1;
        }
    }
}
