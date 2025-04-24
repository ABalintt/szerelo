using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzereloWPF.Models
{
    class Javitas
    {
        public int JavitasId { get; set; }
        public string Megnevezes { get; set; }
        public string Leiras { get; set; }
        public int Koltseg { get; set; }
        public DateTime Datum { get; set; }
        public List<Dolgozo> Dolgozok { get; set; } = new List<Dolgozo>();
    }
}
