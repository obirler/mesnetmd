using System.Collections.Generic;
using System.Windows.Controls;
using MesnetMD.Classes.IO.Manifest.Base;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class SupportItem : SomItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportItem"/> class.
        /// All types of supports are derived from this class
        /// </summary>
        protected SupportItem()
        {
            SupportId = SupportCount++;
            InitializeComponent();
        }

        protected SupportItem(SupportManifest manifest) : base(manifest)
        {
            SupportId = manifest.SupportId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            switch (this)
            {
                case BasicSupport bs:

                    DOFCount = 1;

                    break;

                case SlidingSupport ss:

                    DOFCount = 2;

                    break;

                case FictionalSupport fs:

                    DOFCount = 3;

                    break;

                case RightFixedSupport rs:

                    DOFCount = 0;

                    break;

                case LeftFixedSupport ls:

                    DOFCount = 0;

                    break;
            }

            DegreeOfFreedoms = new List<DOF>();
        }

        public int SupportId { get; }

        private static int SupportCount { get; set; }

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
