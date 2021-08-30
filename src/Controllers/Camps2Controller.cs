using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps")]
    [ApiVersion("2.0")]
    [ApiController]
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public Camps2Controller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CampModel>>> Get(bool includeTalks = false)
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync(includeTalks);
                List<CampModel> modelsToReturn = mapper.Map<List<CampModel>>(camps);
                return (modelsToReturn);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something wrong happened from our side");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<CampModel>>> Get11(bool includeTalks = true)
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync(includeTalks);
                List<CampModel> modelsToReturn = mapper.Map<List<CampModel>>(camps);
                return (modelsToReturn);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something wrong happened from our side");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something wrong happened");
            }
        }

        //[HttpGet("search")]
        //public async Task<ActionResult<List<CampModel>>> Get(DateTime theDate, bool includeTalks=false)
        //{
        //    try
        //    {
        //        var camp = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
        //        if (camp == null) return NotFound();
        //        return mapper.Map<List<CampModel>>(camp);
        //    }
        //    catch (Exception)
        //    {
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, "Something wrong happened");
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existingCamp = await campRepository.GetCampAsync(model.Moniker);
                if (existingCamp != null) return BadRequest("Moniker must be unique");

                var location = linkGenerator.GetPathByAction("Get", "Camps",
                    new { moniker = model.Moniker });

                if (string.IsNullOrEmpty(location))
                {
                    return BadRequest();
                }

                var campToAdd = mapper.Map<Camp>(model);
                campRepository.Add(campToAdd);

                if (await campRepository.SaveChangesAsync())
                {
                    return Created(location, mapper.Map<CampModel>(campToAdd));
                }
                else
                {
                    return BadRequest();
                }

                return StatusCode(StatusCodes.Status400BadRequest, "something wrong on the request");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var existingCamp = await campRepository.GetCampAsync(moniker);
                if (existingCamp == null) return NotFound("Camp wasn't found");

                mapper.Map(model, existingCamp);

                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<CampModel>(existingCamp);
                }
                return BadRequest("something wrong with the request");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var existingCamp = await campRepository.GetCampAsync(moniker);
                if (existingCamp == null) return NotFound("Camp not founded");

                campRepository.Delete(existingCamp);
                if (await campRepository.SaveChangesAsync())
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                return BadRequest("something wrong with the request");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }
    }
}
