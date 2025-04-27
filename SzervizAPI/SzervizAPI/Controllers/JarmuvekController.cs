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
    

    public class JarmuvekController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            IEnumerable<Jarmuvek> result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Jarmuvek.ToList();
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
            Jarmuvek result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Jarmuvek.Where(x => x.Jarmu_id == id).FirstOrDefault();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Jarmuvek value)
        {
            using (var ctx = new SzervizContext())
            {
                if (value.Alvazszam == null || value.Gyartasi_ev == null || value.Km_ora_allas == null || value.Marka == null || value.Modell == null || value.Rendszam == null)
                {
                    return BadRequest("Hiányos adatok");
                }
                var newJarmu = new Jarmuvek
                {
                    Jarmu_id = value.Jarmu_id,
                    Rendszam = value.Rendszam,
                    Alvazszam = value.Alvazszam,
                    Marka = value.Marka,
                    Modell = value.Modell,
                    Gyartasi_ev = value.Gyartasi_ev,
                    Km_ora_allas = value.Km_ora_allas,
                    Ugyfel_id = value.Ugyfel_id
                };
                ctx.Jarmuvek.Add(newJarmu);
                ctx.SaveChanges();
                return Content(HttpStatusCode.Created, new { newJarmu.Jarmu_id });
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody] Jarmuvek value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Jarmuvek.Where(x => x.Jarmu_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                result2.Jarmu_id = value.Jarmu_id;
                result2.Rendszam = value.Rendszam;
                result2.Alvazszam = value.Alvazszam;
                result2.Marka = value.Marka;
                result2.Modell = value.Modell;
                result2.Gyartasi_ev = value.Gyartasi_ev;
                result2.Km_ora_allas = value.Km_ora_allas;
                result2.Ugyfel_id = value.Ugyfel_id;
                ctx.SaveChanges();
                return Ok();
            }
        }

        // PATCH api/<controller>/5
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Jarmuvek value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Jarmuvek.Where(x => x.Jarmu_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                if (value.Rendszam != null)
                {
                    result2.Rendszam = value.Rendszam;
                }
                if (value.Alvazszam != null)
                {
                    result2.Alvazszam = value.Alvazszam;
                }
                if (value.Marka != null)
                {
                    result2.Marka = value.Marka;
                }
                if (value.Modell != null)
                {
                    result2.Modell = value.Modell;
                }                                                                            
                if (value.Gyartasi_ev != null)                                                  
                {                                                                            
                    result2.Gyartasi_ev = value.Gyartasi_ev;                                 
                }                                                                            
                if (value.Km_ora_allas != null)                                                 
                {                                                                            
                    result2.Km_ora_allas = value.Km_ora_allas;                               
                }                                                                            
                if (value.Ugyfel_id != null)                                                 
                {                                                                            
                    result2.Ugyfel_id = value.Ugyfel_id;                                     
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
                var result3 = ctx.Jarmuvek.Where(x => x.Jarmu_id == id).FirstOrDefault();
                if (result3 == null)
                {
                    return Content(HttpStatusCode.NotFound, "A jármű nem létezik");
                }
                ctx.Jarmuvek.Remove(result3);
                ctx.SaveChanges();
                return Content(HttpStatusCode.NoContent, "");
            }
        }
    }
}