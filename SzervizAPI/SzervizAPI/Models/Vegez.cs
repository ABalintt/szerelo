using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Vegez
    {
        [Key]
        [Column(Order = 1)]
        public int Jarmu_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public int Javitas_id { get; set; }

        [ForeignKey("Jarmu_id")]
        public virtual Jarmuvek Jarmuvek { get; set; }

        [ForeignKey("Javitas_id")]
        public virtual Javitasok Javitasok { get; set; }
    }
}