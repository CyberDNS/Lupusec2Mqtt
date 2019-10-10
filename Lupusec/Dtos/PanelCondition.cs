using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Lupusec.Dtos
{
    public class PanelCondition
    {
        public Updates updates { get; set; }
        public Forms forms { get; set; }
    }

    public class Updates
    {
        public string mode_a1 { get; set; }
        public string mode_a2 { get; set; }
        public string dc_ex { get; set; }
        public string alarm_ex { get; set; }
        public string battery_ex { get; set; }
        public string battery_ok { get; set; }
        public string battery { get; set; }
        public string tamper_ok { get; set; }
        public string tamper { get; set; }
        public string interference_ok { get; set; }
        public string interference { get; set; }
        public string ac_activation_ok { get; set; }
        public string ac_activation { get; set; }
        public string sys_in_inst { get; set; }
        public string rssi { get; set; }
        public string sig_gsm_ok { get; set; }
        public string sig_gsm { get; set; }
    }

    public class Pcondform
    {
        public string mode { get; set; }
        public string f_arm { get; set; }
    }

    public class Forms
    {
        public Pcondform pcondform1 { get; set; }
        public Pcondform pcondform2 { get; set; }
    }
}
