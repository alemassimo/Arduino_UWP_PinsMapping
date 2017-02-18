using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_Iot_PinMapping_UWP
{
    public class AModule
    {
        public string name { get; set; }
        public APin[] pins { get; set; }
        public string description { get; set; }
    }
}
