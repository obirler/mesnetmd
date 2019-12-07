using System.Collections.Generic;
using System.Windows.Controls;
using MesnetMD.Classes.Tools;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class SupportItem : SomItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportItem"/> class.
        /// All types of supports are derived from this class
        /// </summary>
        protected SupportItem(Global.ObjectType type)
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

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);

            Canvas.SetLeft(this, leftpos);

            Canvas.SetTop(this, toppos);
        }
    }
}
