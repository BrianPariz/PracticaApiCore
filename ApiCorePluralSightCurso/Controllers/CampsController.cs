using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        //public async Task<IActionResult> Get()
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            //if (false)
            //    return this.BadRequest("Pasó algo malo");
            //if (false)
            //    return this.NotFound("No se encontró eso");

            try
            {
                var results = await this.repository.GetAllCampsAsync(includeTalks);

                CampModel[] models = this.mapper.Map<CampModel[]>(results);

                //podemos no utilizar el Ok() cuando ya le idicamos en el ActionResult que queremos que devuelva,
                //en ese caso va a devolver cod 200 si sale todo bien y manda el modelo que le pusimos
                //return Ok(models);
                return models;
            }
            catch (Exception)
            {
                //return BadRequest("Ej: fallo la base de datos"); como tambien se puede hacer lo de abajo,
                //con StatusCodes tenes la lista de los codigos de status
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await this.repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return this.mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await this.repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return this.mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }
        }

        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var result = await this.repository.GetCampAsync(model.Moniker);

                if (result != null) return BadRequest("Error: el objeto ya fue ingresado previamente");

                //linkgenerator solo esta disponible en core 2.2 en adelante
                var location = this.linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("No se pudo usar ese moniker");
                }

                var camp = this.mapper.Map<Camp>(model);
                this.repository.Add(camp);

                if (await this.repository.SaveChangesAsync())
                {
                    //created es un Ok() o sea un cod 200, pero retorna la ubicacion y el dato que se ingreso
                    //en este caso devuelvo el modelo de la entidad que ingrese que seguro ahora tiene mas datos autogenerados
                    return Created($"/api/camps/{camp.Moniker}", this.mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await this.repository.GetCampAsync(moniker);

                if (oldCamp == null) return NotFound($"Error: no se encontro el objeto con el moniker {moniker}");

                //toma los datos del primero y los aplica en el segundo
                this.mapper.Map(model, oldCamp);

                if (await this.repository.SaveChangesAsync())
                {
                    return this.mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await this.repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Error: no se encontro el objeto con el moniker {moniker}");

                this.repository.Delete(oldCamp);

                if (await this.repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "fallo la base");
            }

            return BadRequest();
        }
    }
}
