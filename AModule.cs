using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_Iot_PinMapping_UWP
{
    public class AModule
    {
        public AModule(string Name,string Description)
        {
            name = Name;
            description = Description;
            pins = new List<APin>();
        }
        public string name { get; set; }
        public List<APin> pins { get; set; }
        public string description { get; set; }
    }
}
