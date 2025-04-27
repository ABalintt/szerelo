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
    public class FoglalController : ApiController
    {
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Foglal value)
        {
            if (value.Idopont_id <= 0 || value.Ugyfel_id <= 0)
            {
                return BadRequest("Hiányos vagy érvénytelen adatok (Idopont_id és Ugyfel_id szükséges).");
            }

            using (var ctx = new SzervizContext())
            {
                var newFoglal = new Foglal
                {
                    Idopont_id = value.Idopont_id,
                    Ugyfel_id = value.Ugyfel_id
                };

                ctx.Foglal.Add(newFoglal);
                ctx.SaveChanges();

                return Content(HttpStatusCode.Created, new { newFoglal.Idopont_id, newFoglal.Ugyfel_id });
            }
        }
    }
}