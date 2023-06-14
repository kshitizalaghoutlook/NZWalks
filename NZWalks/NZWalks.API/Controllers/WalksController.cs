using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walks = await walkRepository.GetAllAsync();

            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetWalksAsync")]
        public async Task<IActionResult> GetWalksAsync(Guid id)
        {
            //Get Walk Domain object from database
            var walks = await walkRepository.GetAsync(id);

            //convert domain object to dto
            if(walks==null)
            {
                return NotFound();
            }
            var walksDTO = mapper.Map<Models.DTO.Walk>(walks);
            return Ok(walksDTO);
        }

        [HttpPost]
       public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {


            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            // Pass domain object to Repository to persist this
          await walkRepository.AddAsync(walkDomain);
            //Convert the domain object back to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };

            //Send DTO response back to client

            return CreatedAtAction(nameof(GetWalksAsync), new { id = walkDTO.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
       public async Task<IActionResult> UpdateWalkAync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //Convert DTO to domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId
            };

            // Pass details to Repository - Get Domain object in response (or null)
           walkDomain = await walkRepository.UpdateAsync(id, walkDomain);


            //Handle null ( not found )
            if (walkDomain == null)
            {
                return NotFound();
            }
            // Convert back Domain to DTO
            
                var walkDTO = new Models.DTO.Walk
                {
                    Length = walkDomain.Length,
                    Name = walkDomain.Name,
                    RegionId = walkDomain.RegionId,
                    WalkDifficultyId = walkDomain.WalkDifficultyId
                };
            
            // Return response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            //call Repository to delete walk

         var walkDomain =   await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound(id);
            }
            //var walkDTO  = new Models.DTO.Walk
            //{
            //    Length = walkDomain.Length,
            //    Name = walkDomain.Name,
            //    RegionId = walkDomain.RegionId,
            //    WalkDifficultyId = walkDomain.WalkDifficultyId
            //};

            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        }
    }
}
