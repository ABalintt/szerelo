using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Ugyfelek
    {
        [Key]
        public int? Ugyfel_id { get; set; }
        public string Nev { get; set; }
        public string Email_cim { get; set; }
        public string Telefonszam { get; set; }
        public string Lakcim { get; set; }
    }
}