using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MesnetMD.Classes.Tools;

namespace MesnetMD.Classes.Ui.Som
{
    public class FictionalSupport : SupportItem
    {
        public FictionalSupport(): base(Global.ObjectType.FictionalSupport)
        {
            Members = new List<Member>();
            FID = fcount++;
            Name = "Fictional Support "+ FID;
            var hdof = new DOF(Global.DOFType.Horizontal);
            var vdof = new DOF(Global.DOFType.Vertical);
            var rdof = new DOF(Global.DOFType.Rotational);
            DegreeOfFreedoms.Add(hdof);
            DegreeOfFreedoms.Add(vdof);
            DegreeOfFreedoms.Add(rdof);
        }

        public List<Member> Members;

        public int FID= 0;

        private static int fcount = 0;

        public void AddBeam(Beam beam, Global.Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
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
                MyDebug.WriteWarning("the beam is already added!");
            }
        }

        public void RemoveBeam(Beam beam)
        {
            Member remove = new Member();
            foreach (var member in Members)
            {
                if (member.Beam.Equals(beam))
                {
                    remove = member;
                    break;
                }
            }

            for (int i = 0; i < DegreeOfFreedoms.Count; i++)
            {
                foreach (var dofmember in DegreeOfFreedoms[i].Members)
                {
                    if (Equals(dofmember.Beam, remove.Beam))
                    {
                        DegreeOfFreedoms[i].Members.Remove(dofmember);
                    }
                }
            }
            
            Members.Remove(remove);
        }      
    }
}
