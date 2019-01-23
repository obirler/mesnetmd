using System.Collections.Generic;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Tools
{
    public class DOF
    {        
        public DOF(Global.DOFType type)
        {
            Type = type;
            Members = new List<DOFMember>();
        }

        public Global.DOFType Type;

        public List<DOFMember> Members;
    }

    public class DOFMember
    {
        public DOFMember(Beam beam, Global.DOFLocation location)
        {
            Beam = beam;
            Location = location;
        }

        public Beam Beam;

        public Global.DOFLocation Location;
    }
}