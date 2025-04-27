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
    
    public class IdopontController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            IEnumerable<Idopont> result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Idopontok.ToList();
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
            Idopont result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Idopontok.Where(x => x.Idopont_id == id).FirstOrDefault();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Idopont value)
        {
            using (var ctx = new SzervizContext())
            {
                if (value.Datum == null || value.nap == null || value.Statusz == null)
                {
                    return BadRequest("Hiányos adatok");
                }
                var newIdopont = new Idopont
                {
                    Idopont_id = value.Idopont_id,
                    Datum = value.Datum,
                    nap = value.nap,
                    Statusz = value.Statusz
                };
                ctx.Idopontok.Add(newIdopont);
                ctx.SaveChanges();
                return Content(HttpStatusCode.Created, new { newIdopont.Idopont_id });
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody] Idopont value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Idopontok.Where(x => x.Idopont_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                result2.Idopont_id = value.Idopont_id;
                result2.Datum = value.Datum;
                result2.nap = value.nap;
                result2.Statusz = value.Statusz;
                ctx.SaveChanges();
                return Ok();
            }
        }

        // PATCH api/<controller>/5
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Idopont value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Idopontok.Where(x => x.Idopont_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                if (value.Datum != default(DateTime))
                {
                    result2.Datum = value.Datum;
                }
               
                if (value.nap != null)
                {
                    result2.nap = value.nap;
                }
                if (value.Statusz != null)
                {
                    result2.Statusz = value.Statusz;
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
                var result3 = ctx.Idopontok.Where(x => x.Idopont_id == id).FirstOrDefault();
                if (result3 == null)
                {
                    return Content(HttpStatusCode.NotFound, "Az időpont nem létezik");
                }
                ctx.Idopontok.Remove(result3);
                ctx.SaveChanges();
                return Content(HttpStatusCode.NoContent, "");
            }
        }
    }
}