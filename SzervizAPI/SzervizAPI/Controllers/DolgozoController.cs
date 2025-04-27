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
    

    public class DolgozoController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            IEnumerable<Dolgozo> result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Dolgozok.ToList();
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
            Dolgozo result = null;
            using (var ctx = new SzervizContext())
            {
                result = ctx.Dolgozok.Where(x => x.Dolgozo_id == id).FirstOrDefault();
            }
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Dolgozo value)
        {
            using (var ctx = new SzervizContext())
            {
                if (value.Beosztas == null || value.Email_cim == null || value.Lakcim == null || value.Nev == null || value.Szemelyazonosito_igazolvany_szam == null || value.Telefonszam == null)
                {
                    return BadRequest("Hiányos adatok");
                }
                var newDolgozo = new Dolgozo
                {
                    Dolgozo_id = value.Dolgozo_id,
                    Nev = value.Nev,
                    Beosztas = value.Beosztas,
                    Lakcim = value.Lakcim,
                    Telefonszam = value.Telefonszam,
                    Email_cim = value.Email_cim,
                    Szemelyazonosito_igazolvany_szam = value.Szemelyazonosito_igazolvany_szam
                };
                ctx.Dolgozok.Add(newDolgozo);
                ctx.SaveChanges();
                return Content(HttpStatusCode.Created, new { newDolgozo.Dolgozo_id });
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody] Dolgozo value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Dolgozok.Where(x => x.Dolgozo_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                result2.Dolgozo_id = value.Dolgozo_id;
                result2.Nev = value.Nev;
                result2.Beosztas = value.Beosztas;
                result2.Lakcim = value.Lakcim;
                result2.Telefonszam = value.Telefonszam;
                result2.Email_cim = value.Email_cim;
                result2.Szemelyazonosito_igazolvany_szam = value.Szemelyazonosito_igazolvany_szam;
                ctx.SaveChanges();
                return Ok();
            }
        }


        // PATCH api/<controller>/5
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Dolgozo value)
        {
            using (var ctx = new SzervizContext())
            {
                var result2 = ctx.Dolgozok.Where(x => x.Dolgozo_id == id).FirstOrDefault();
                if (result2 == null)
                {
                    return NotFound();
                }
                if (value.Nev != null)
                {
                    result2.Nev = value.Nev;
                }
                if (value.Beosztas != null)
                {
                    result2.Beosztas = value.Beosztas;
                }
                if (value.Lakcim != null)
                {
                    result2.Lakcim = value.Lakcim;
                }
                if (value.Telefonszam != null)
                {
                    result2.Telefonszam = value.Telefonszam;
                }
                if (value.Email_cim != null)
                {
                    result2.Email_cim = value.Email_cim;
                }
                if (value.Szemelyazonosito_igazolvany_szam != null)
                {
                    result2.Szemelyazonosito_igazolvany_szam = value.Szemelyazonosito_igazolvany_szam;
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
                var result3 = ctx.Dolgozok.Where(x => x.Dolgozo_id == id).FirstOrDefault();
                if (result3 == null)
                {
                    return Content(HttpStatusCode.NotFound, "A dolgozó nem létezik");
                }
                ctx.Dolgozok.Remove(result3);
                ctx.SaveChanges();
                return Content(HttpStatusCode.NoContent, "");
            }
        }
    }
}