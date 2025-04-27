using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Dolgozo
    {
        [Key]
        public int? Dolgozo_id { get; set; }
        public string Nev { get; set; }
        public string Beosztas { get; set; }
        public string Lakcim { get; set; }
        public string Telefonszam { get; set; }
        public string Email_cim { get; set; }
        public string Szemelyazonosito_igazolvany_szam { get; set; }
    }

}