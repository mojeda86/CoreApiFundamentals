using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
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

        [HttpGet("search")]
        public async Task<ActionResult<List<CampModel>>> Get(DateTime theDate, bool includeTalks=false)
        {
            try
            {
                var camp = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (camp == null) return NotFound();
                return mapper.Map<List<CampModel>>(camp);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something wrong happened");
            }
        }
    }
}
