﻿using Microsoft.Maker.RemoteWiring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_Iot_PinMapping_UWP
{
    public class APin
    {
        public string name { get; set; }
        public string pinNumber { get; set; }
        public PinMode pinMode { get; set; }
    }
}
