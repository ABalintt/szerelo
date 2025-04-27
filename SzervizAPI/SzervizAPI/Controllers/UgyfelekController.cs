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
    public class UgyfelekController : ApiController
    {
       
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            IEnumerable<Ugyfelek> result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Ugyfelek.ToList();
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
            Ugyfelek result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Ugyfelek.Where(x => x.Ugyfel_id == id).FirstOrDefault();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Ugyfelek value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var ctx = new SzervizContext())
            {
                if (value.Nev == null || value.Email_cim == null || value.Telefonszam == null || value.Lakcim == null)
                {
                    return BadRequest("Hiányos adatok");
                }
                var newUgyfel = new Ugyfelek
                {
                    Nev = value.Nev,
                    Email_cim = value.Email_cim,
                    Telefonszam = value.Telefonszam,
                    Lakcim = value.Lakcim
                };
                ctx.Ugyfelek.Add(newUgyfel);
                ctx.SaveChanges();
                return Content(HttpStatusCode.Created, new { newUgyfel.Ugyfel_id });
            }
        }


            // PUT api/<controller>/5
            public IHttpActionResult Put(int id, [FromBody] Ugyfelek value)
        {
            using (var ctx = new SzervizContext())
            {
                var result1 = ctx.Ugyfelek.Where(x => x.Ugyfel_id == id).FirstOrDefault();
                if (result1 == null)
                {
                    return NotFound();
                }
                result1.Ugyfel_id = value.Ugyfel_id;
                result1.Nev = value.Nev;
                result1.Email_cim = value.Email_cim;
                result1.Telefonszam = value.Telefonszam;
                result1.Lakcim = value.Lakcim;
                ctx.SaveChanges();
                return Ok();
            }
        }


        // PATCH api/<controller>/5
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Ugyfelek value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Ugyfelek.Where(x => x.Ugyfel_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                if (value.Nev != null)
                {
                    result2.Nev = value.Nev;
                }
                if (value.Email_cim != null)
                {
                    result2.Email_cim = value.Email_cim;
                }
                if (value.Telefonszam != null)
                {
                    result2.Telefonszam = value.Telefonszam;
                }
                if (value.Lakcim != null)
                {
                    result2.Lakcim = value.Lakcim;
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
                var result2 = ctx.Ugyfelek.Where(x => x.Ugyfel_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return Content(HttpStatusCode.NotFound, "Az ügyfél nem létezik");
                }
                ctx.Ugyfelek.Remove(result2);
                ctx.SaveChanges();
                return Content(HttpStatusCode.NoContent, "");
            }
        }
    }
}