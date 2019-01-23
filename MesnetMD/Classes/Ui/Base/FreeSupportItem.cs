using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class FreeSupportItem : SupportItem
    {
        protected FreeSupportItem(Global.ObjectType type) : base(type)
        {
            Members = new List<Member>();
        }

        public List<Member> Members;

        public DirectLoad Loads;

        public virtual void AddBeam(Beam beam, Global.Direction direction)
        {
        }

        public virtual void RemoveBeam(Beam beam)
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

        public virtual void SetBeam(Beam beam, Global.Direction direction)
        {
        }
    }
}
