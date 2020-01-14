using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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

        protected Ellipse _circle;

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

        public override void ResetSolution()
        {
            //todo: implement reset mechanism
        }

        public virtual void CircleShow()
        {
            _circle.Visibility = Visibility.Visible;
        }

        public virtual void CircleHide()
        {
            _circle.Visibility = Visibility.Collapsed;
        }

        public virtual void CircleSelect()
        {
            _circle.Stroke = new SolidColorBrush(Colors.Yellow);
        }

        public virtual void CircleUnSelect()
        {
            _circle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
        }
    }
}
