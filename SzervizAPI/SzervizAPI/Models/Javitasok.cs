using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Javitasok
    {
        [Key]
        public int? Javitas_id { get; set; }
        public string Megnevezes { get; set; }
        public string Leiras { get; set; }
        public int? Koltseg { get; set; }
        public DateTime Datum { get; set; }
    }
}