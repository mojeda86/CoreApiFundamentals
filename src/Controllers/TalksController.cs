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
    [Route("api/camps/{moniker}/[controller]")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<List<TalkModel>>> Get(string moniker)
        {
            try
            {
                var existingTalks = await campRepository.GetTalksByMonikerAsync(moniker, true);
                return mapper.Map<List<TalkModel>>(existingTalks);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var existingTalks = await campRepository.GetTalksByMonikerAsync(moniker, true);
                var existingTalkById = existingTalks.SingleOrDefault(t => t.TalkId == id);

                if (existingTalkById == null) return NotFound("Can't match any talk with the ID provided");

                return mapper.Map<TalkModel>(existingTalkById);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var existingCamp = await campRepository.GetCampAsync(moniker);
                if (existingCamp == null) return BadRequest("There is any Camp with that moniker");

                var talkModel = mapper.Map<Talk>(model);
                talkModel.Camp = existingCamp;

                if (model.Speaker == null) return BadRequest("Speaker Id is required");
                var existingSpeaker = campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (existingSpeaker == null) return BadRequest("Speaker ID was not found");
                //var speakerModel = mapper.Map<Speaker>(model.Speaker);
                talkModel.Speaker = existingSpeaker.Result;

                campRepository.Add(talkModel);

                if (await campRepository.SaveChangesAsync())
                {
                    var url = linkGenerator.GetPathByAction(HttpContext, "Get",
                        values: new { moniker, id = talkModel.TalkId });
                    return Created(url, mapper.Map<TalkModel>(talkModel));
                }
                return BadRequest("Something went wrong");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, TalkModel model, int id)
        {
            try
            {
                var existingTalk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (existingTalk == null) return NotFound("Talk Not Found");

                mapper.Map(model, existingTalk);

                if (model.Speaker != null)
                {
                    var existingSpeaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (existingSpeaker == null) return NotFound("Speaker Not Found");
                    existingTalk.Speaker = existingSpeaker;
                }

                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<TalkModel>(existingTalk);
                }
                return BadRequest("Something went wrong");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var existingTalks = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (existingTalks == null) return BadRequest("Talk not found");
                campRepository.Delete(existingTalks);

                if (await campRepository.SaveChangesAsync())
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                return BadRequest("failed to delete talk");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error from us");
            }
        }
    }
}
