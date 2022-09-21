using Refit;

namespace PokemonInfoAPI.Controllers;

public interface IPokeApi
{
    [Get("/pokemon/{pokemonName}")]
    Task<PokemonInfo> GetPokemonInfo(string pokemonName);
}