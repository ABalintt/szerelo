using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzereloWPF.Models
{
    class Jarmu
    {
        public int JarmuId { get; set; }
        public string Rendszam { get; set; }
        public string Alvazszam { get; set; }
        public string Marka { get; set; }
        public string Modell { get; set; }
        public int GyartasiEv { get; set; }
        public int KmOraAllas { get; set; }
        public int UgyfelId { get; set; }
        public Ugyfel Ugyfel { get; set; }
    }
}
