using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Idopont
    {
        [Key]
        public int? Idopont_id { get; set; }
        public DateTime Datum { get; set; }
        public string nap { get; set; }
        public string Statusz { get; set; }
    }
}