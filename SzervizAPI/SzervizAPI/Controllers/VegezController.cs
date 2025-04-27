using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SzervizAPI.Database;
using SzervizAPI.Models;

namespace SzervizAPI.Controllers
{
    public class VegezController : ApiController
    {
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Vegez value)
        {
            if (value == null)
            {
                return BadRequest("A kérés törzse nem tartalmaz adatot vagy hibás formátumú.");
            }

            if (value.Jarmu_id <= 0 || value.Javitas_id <= 0)
            {
                return BadRequest("Hiányos vagy érvénytelen adatok (Jarmu_id és Javitas_id szükséges).");
            }

            using (var ctx = new SzervizContext())
            {
                var jarmuExists = ctx.Jarmuvek.Any(j => j.Jarmu_id == value.Jarmu_id);
                var javitasExists = ctx.Javitasok.Any(j => j.Javitas_id == value.Javitas_id);
                if (!jarmuExists || !javitasExists)
                {
                    return BadRequest("A megadott jármű vagy javítás ID nem létezik.");
                }

                var newVegez = new Vegez
                {
                    Jarmu_id = value.Jarmu_id,
                    Javitas_id = value.Javitas_id
                };

                ctx.Vegez.Add(newVegez);
                ctx.SaveChanges();

                return Content(HttpStatusCode.Created, new { newVegez.Jarmu_id, newVegez.Javitas_id });
            }
        }
    }
}