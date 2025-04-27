using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Jarmuvek
    {
        [Key]
        public int? Jarmu_id { get; set; }
        public string Rendszam { get; set; }
        public string Alvazszam { get; set; }
        public string Marka { get; set; }
        public string Modell { get; set; }
        public int? Gyartasi_ev { get; set; }
        public int? Km_ora_allas { get; set; }

        [ForeignKey("Ugyfelek")]
        public int? Ugyfel_id { get; set; }
        public virtual Ugyfelek Ugyfelek { get; set; }
    }

}