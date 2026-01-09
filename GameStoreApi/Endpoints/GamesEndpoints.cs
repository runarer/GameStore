using System;
using GameStoreApi.Data;
using GameStoreApi.Dtos;
using GameStoreApi.Models;

namespace GameStoreApi.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";
    private static readonly List<GameDto> games = [
        new(1, "Street Fighter II", "Fighting", 19.99M,new DateOnly(1992,7,15)),
        new(2, "Final Fantasy VII Rebirth", "RPG", 69.99M,new DateOnly(2024,2,29)),
        new(3, "Astro Bot", "Platformer", 59.99M,new DateOnly(2024,9,6)),
    ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        // GET /games
        group.MapGet("/games", () => games);



        // GET /games/1
        group.MapGet("/{id}", (int id) => 
        {
            var game = games.Find(game  => game.Id == id);
            
            return game is null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameEndPointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name, 
                GenreId = newGame.GenreId, 
                Price = newGame.Price, 
                ReleaseDate = newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            //Creates an Dto object so we dont expose our internal classes to the user
            // Thereby making it easier to change later.
            GameDetailsDto gameDto = new(game.Id, game.Name, game.GenreId,game.Price,game.ReleaseDate);

            return Results.CreatedAtRoute(GetGameEndPointName,new {id = gameDto.Id}, gameDto);
        });


        // PUT /games/{id}
        group.MapPut("/{id}", (int id,UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex( game => game.Id == id);

            if (index == -1)
                return Results.NotFound();

            games[index] = new GameDto(
                id, 
                updatedGame.Name, 
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        // DELETE /games/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });
    }
}
