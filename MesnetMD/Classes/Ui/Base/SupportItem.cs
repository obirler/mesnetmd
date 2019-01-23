using System.Collections.Generic;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Classes.Ui
{
    public class SupportItem : SomItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportItem"/> class.
        /// All types of supports are derived from this class
        /// </summary>
        public SupportItem(Global.ObjectType type)
        {
            Type = type;
            if (type != Global.ObjectType.FictionalSupport)
            {
                SupportId = supportcount++;
            }

            switch (type)
            {
                case Global.ObjectType.BasicSupport:
                    DOFCount = 1;
                    break;
                case Global.ObjectType.SlidingSupport:
                    DOFCount = 2;
                    break;
                case Global.ObjectType.FictionalSupport:
                    DOFCount = 3;
                    break;
                case Global.ObjectType.RightFixedSupport:
                    DOFCount = 0;
                    break;
                case Global.ObjectType.LeftFixedSupport:
                    DOFCount = 0;
                    break;
            }

            DegreeOfFreedoms = new List<DOF>();
        }

        public int SupportId=0;

        private static int supportcount = 1;

        protected bool _selected;

        public int DOFCount;

        public List<DOF> DegreeOfFreedoms;

    }
}
