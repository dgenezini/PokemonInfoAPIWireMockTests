using Microsoft.AspNetCore.Mvc;
using Refit;

namespace PokemonInfoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PokemonInfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{pokemonName}")]
        public async Task<ActionResult<PokemonInfo>> GetAsync(string pokemonName)
        {
            try
            {
                var pokeApi = RestService.For<IPokeApi>(_configuration["PokeApiBaseUrl"]);

                return Ok(await pokeApi.GetPokemonInfo(pokemonName));
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                return StatusCode(500);
            }
        }
    }
}