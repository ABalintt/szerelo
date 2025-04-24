using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzereloWPF.Models
{
    class Ugyfel
    {
        public int UgyfelId { get; set; }
        public string Nev { get; set; }
        public string EmailCim { get; set; }
        public string Telefonszam { get; set; }
        public string Lakcim { get; set; }
        public List<Jarmu> Jarmuvek { get; set; } = new List<Jarmu>();
    }
}
