using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Foglal
    {
        [Key]
        [Column(Order = 1)]
        public int Idopont_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public int Ugyfel_id { get; set; }

        [ForeignKey("Idopont_id")]
        public virtual Idopont Idopont { get; set; }

        [ForeignKey("Ugyfel_id")]
        public virtual Ugyfelek Ugyfelek { get; set; }
    }
}