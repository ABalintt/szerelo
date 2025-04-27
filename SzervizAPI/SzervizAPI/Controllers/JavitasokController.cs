using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SzervizAPI.Models;
using System.Data.Entity;
using SzervizAPI.Database;

namespace SzervizAPI.Controllers
{
    

    public class JavitasokController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            IEnumerable<Javitasok> result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Javitasok.ToList();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Javitasok result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Javitasok.Where(x => x.Javitas_id == id).FirstOrDefault();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Javitasok value)
        {
            using (var ctx = new SzervizContext())
            {
                if (value.Datum == default(DateTime) || value.Koltseg == null || value.Leiras == null || value.Megnevezes == null)
                {
                    return BadRequest("Hiányos adatok");
                }
                var newJavitas = new Javitasok
                {
                    Javitas_id = value.Javitas_id,
                    Megnevezes = value.Megnevezes,
                    Leiras = value.Leiras,
                    Koltseg = value.Koltseg,
                    Datum = value.Datum
                };
                ctx.Javitasok.Add(newJavitas);
                ctx.SaveChanges();
                return Content(HttpStatusCode.Created, new { newJavitas.Javitas_id });
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody] Javitasok value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Javitasok.Where(x => x.Javitas_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                result2.Javitas_id = value.Javitas_id;
                result2.Megnevezes = value.Megnevezes;
                result2.Leiras = value.Leiras;
                result2.Koltseg = value.Koltseg;
                result2.Datum = value.Datum;
                ctx.SaveChanges();
                return Ok();
            }
        }

        // PATCH api/<controller>/5
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Javitasok value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Javitasok.Where(x => x.Javitas_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                if (value.Megnevezes != null)
                {
                    result2.Megnevezes = value.Megnevezes;
                }
                if (value.Leiras != null)
                {
                    result2.Leiras = value.Leiras;
                }
                if (value.Koltseg != null)
                {
                    result2.Koltseg = value.Koltseg;
                }
                if (value.Datum != default(DateTime))
                {
                    result2.Datum = value.Datum;
                }
                ctx.SaveChanges();
                return Ok();
            }
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            using (var ctx = new SzervizContext())
            {
                var result3 = ctx.Javitasok.Where(x => x.Javitas_id == id).FirstOrDefault();
                if (result3 == null)
                {
                    return Content(HttpStatusCode.NotFound, "A javítás nem létezik");
                }
                ctx.Javitasok.Remove(result3);
                ctx.SaveChanges();
                return Content(HttpStatusCode.NoContent, "");
            }
        }
    }
}