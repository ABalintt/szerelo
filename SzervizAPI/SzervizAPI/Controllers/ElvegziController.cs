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
    public class ElvegziController : ApiController
    {
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Elvegzi value)
        {
            if (value.Javitas_id <= 0 || value.Dolgozo_id <= 0)
            {
                return BadRequest("Hiányos vagy érvénytelen adatok (Javitas_id és Dolgozo_id szükséges).");
            }

            using (var ctx = new SzervizContext())
            {
                var newElvegzi = new Elvegzi
                {
                    Javitas_id = value.Javitas_id,
                    Dolgozo_id = value.Dolgozo_id
                };

                ctx.Elvegzi.Add(newElvegzi);
                ctx.SaveChanges();

                return Content(HttpStatusCode.Created, new { newElvegzi.Javitas_id, newElvegzi.Dolgozo_id });
            }
        }
    }
}