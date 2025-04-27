using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SzervizAPI.Models
{
    public class Elvegzi
    {
        [Key]
        [Column(Order = 1)]
        public int Javitas_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public int Dolgozo_id { get; set; }

        [ForeignKey("Javitas_id")]
        public virtual Javitasok Javitasok { get; set; }

        [ForeignKey("Dolgozo_id")]
        public virtual Dolgozo Dolgozo { get; set; }
    }
}