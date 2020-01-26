using System;
using System.Collections.Generic;
using System.Linq;
using MesnetMD.Classes.IO.Manifest;

namespace MesnetMD.Classes.IO.Xml
{

    class FictionalSupportReader
    {
        public FictionalSupportReader(System.Xml.Linq.XElement supportelement)
        {
            _supportelement = supportelement;
            _support = new FictionalSupportManifest();
        }

        public FictionalSupportManifest Read()
        {
            readproperties();

            readmembers();

            return _support;
        }

        private void readproperties()
        {
            var propelement = _supportelement.Elements().Where(x => x.Name == "SupportProperties").First();

            foreach (var item in propelement.Elements())
            {
                switch (item.Name.ToString())
                {
                    case "id":
                        _support.Id = Convert.ToInt32(item.Value);
                        break;
                    case "supportid":
                        _support.SupportId = Convert.ToInt32(item.Value);
                        break;
                    case "name":
                        _support.Name = item.Value;
                        break;
                    case "angle":
                        _support.Angle = Convert.ToDouble(item.Value);
                        break;
                    case "leftposition":
                        _support.LeftPosition = Convert.ToDouble(item.Value);
                        break;
                    case "topposition":
                        _support.TopPosition = Convert.ToDouble(item.Value);
                        break;
                }
            }
        }

        private void readmembers()
        {
            var memberselement = _supportelement.Elements().Where(x => x.Name == "Members").First();

            var members = new List<Member>();

            foreach (var item in memberselement.Elements())
            {
                var member = new Member();
                foreach (var memberitem in item.Elements())
                {
                    switch (memberitem.Name.ToString())
                    {
                        case "id":
                            member.Id = Convert.ToInt32(memberitem.Value);
                            break;
                        case "beamid":
                            member.BeamId = Convert.ToInt32(memberitem.Value);
                            break;
                        case "name":
                            member.Name = memberitem.Value;
                            break;
                        case "direction":

                            if (memberitem.Value == "Left")
                            {
                                member.Direction = Global.Direction.Left;
                            }
                            else if (memberitem.Value == "Right")
                            {
                                member.Direction = Global.Direction.Right;
                            }
                            break;
                    }
                }
                members.Add(member);
            }

            if (members.Count > 0)
            {
                _support.Members = members;
            }
        }

        System.Xml.Linq.XElement _supportelement;

        FictionalSupportManifest _support;
    }
}
