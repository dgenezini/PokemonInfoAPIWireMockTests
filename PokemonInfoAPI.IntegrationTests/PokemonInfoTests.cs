using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PokemonInfoAPI.IntegrationTests
{
    public class PokemonInfoTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PokemonInfoTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task Get_Existing_Pokemon_Returns_200()
        {
            //Arrange
            var WireMockSvr = WireMockServer.Start();

            var Factory = _factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSetting("PokeApiBaseUrl", WireMockSvr.Url);
                });

            var HttpClient = Factory.CreateClient();

            Fixture fixture = new Fixture();

            var ResponseObj = fixture.Create<PokemonInfo>();
            var ResponseObjJson = JsonSerializer.Serialize(ResponseObj);

            WireMockSvr
                .Given(Request.Create()
                    .WithPath("/pokemon/charmander")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(ResponseObjJson)
                    .WithHeader("Content-Type", "application/json")
                    .WithStatusCode(HttpStatusCode.OK));

            //Act
            var HttpResponse = await HttpClient.GetAsync("/pokemoninfo/charmander");

            //Assert
            HttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var ResponseJson = await HttpResponse.Content.ReadAsStringAsync();
            var PokemonInfo = JsonSerializer.Deserialize<PokemonInfo>(ResponseJson);

            PokemonInfo.Should().BeEquivalentTo(ResponseObj);

            WireMockSvr.Stop();
        }

        [Fact]
        public async Task Get_NotExisting_Pokemon_Returns_404()
        {
            //Arrange
            var WireMockSvr = WireMockServer.Start();

            var Factory = _factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSetting("PokeApiBaseUrl", WireMockSvr.Url);
                });

            var HttpClient = Factory.CreateClient();

            Fixture fixture = new Fixture();

            WireMockSvr
                .Given(Request.Create()
                    .WithPath("/pokemon/picapau")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithStatusCode(HttpStatusCode.NotFound));

            //Act
            var HttpResponse = await HttpClient.GetAsync("/pokemoninfo/picapau");

            //Assert
            HttpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            WireMockSvr.Stop();
        }
    }
}
