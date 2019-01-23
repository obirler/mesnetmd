using System.Collections.Generic;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Classes.Ui.Som
{
    public class FictionalSupport : FreeSupportItem, IFictionalSupportItem
    {
        public FictionalSupport(): base(Global.ObjectType.FictionalSupport)
        {         
            FID = fcount++;
            Name = "Fictional Support "+ FID;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var vdof = new DOF(Global.DOFType.Vertical);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(vdof);
            DegreeOfFreedoms.Add(rdof);
            Global.AddObject(this);
        }
    
        public int FID = 0;

        private static int fcount = 1;

        public override void AddBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);
                switch (direction)
                {
                    case Global.Direction.Left:

                        beam.LeftSide = this;

                        var lhdofmember = new DOFMember(beam, Global.DOFLocation.LeftHorizontal);

                        DegreeOfFreedoms[0].Members.Add(lhdofmember);

                        var lvdofmember = new DOFMember(beam, Global.DOFLocation.LeftVertical);

                        DegreeOfFreedoms[1].Members.Add(lvdofmember);

                        var lrdofmember = new DOFMember(beam, Global.DOFLocation.LeftRotational);

                        DegreeOfFreedoms[2].Members.Add(lrdofmember);

                        break;

                    case Global.Direction.Right:

                        beam.RightSide = this;

                        var rhdofmember = new DOFMember(beam, Global.DOFLocation.RightHorizontal);

                        DegreeOfFreedoms[0].Members.Add(rhdofmember);

                        var rvdofmember = new DOFMember(beam, Global.DOFLocation.RightVertical);

                        DegreeOfFreedoms[1].Members.Add(rvdofmember);

                        var rrdofmember = new DOFMember(beam, Global.DOFLocation.RightRotational);

                        DegreeOfFreedoms[2].Members.Add(rrdofmember);

                        break;
                }
            }
            else
            {
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }

        public override void SetBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);

                switch (direction)
                {
                    case Global.Direction.Left:

                        beam.LeftSide = this;

                        var lhdofmember = new DOFMember(beam, Global.DOFLocation.LeftHorizontal);

                        DegreeOfFreedoms[0].Members.Add(lhdofmember);

                        var lvdofmember = new DOFMember(beam, Global.DOFLocation.LeftVertical);

                        DegreeOfFreedoms[1].Members.Add(lvdofmember);

                        var lrdofmember = new DOFMember(beam, Global.DOFLocation.LeftRotational);

                        DegreeOfFreedoms[2].Members.Add(lrdofmember);

                        break;

                    case Global.Direction.Right:

                        var rhdofmember = new DOFMember(beam, Global.DOFLocation.RightHorizontal);

                        DegreeOfFreedoms[0].Members.Add(rhdofmember);

                        var rvdofmember = new DOFMember(beam, Global.DOFLocation.RightVertical);

                        DegreeOfFreedoms[1].Members.Add(rvdofmember);

                        var rrdofmember = new DOFMember(beam, Global.DOFLocation.RightRotational);

                        DegreeOfFreedoms[2].Members.Add(rrdofmember);

                        break;
                }
            }
            else
            {
                MesnetMDDebug.WriteWarning("the beam is already added!");
            }
        }
    }
}
