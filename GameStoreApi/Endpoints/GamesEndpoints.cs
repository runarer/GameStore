using System;
using GameStoreApi.Data;
using GameStoreApi.Dtos;
using GameStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreApi.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";
    private static readonly List<GameSummaryDto> games = [
        new(1, "Street Fighter II", "Fighting", 19.99M,new DateOnly(1992,7,15)),
        new(2, "Final Fantasy VII Rebirth", "RPG", 69.99M,new DateOnly(2024,2,29)),
        new(3, "Astro Bot", "Platformer", 59.99M,new DateOnly(2024,9,6)),
    ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) 
            => await dbContext.Games
                                .Include(game => game.Genre)
                                .Select(game => new GameSummaryDto(
                                    game.Id, 
                                    game.Name, 
                                    game.Genre!.Name,
                                    game.Price,
                                    game.ReleaseDate
                                ))
                                .AsNoTracking()
                                .ToListAsync());



        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
        {
            var game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() 
                                : Results.Ok(new GameDetailsDto(
                                    game.Id, 
                                    game.Name, 
                                    game.GenreId,
                                    game.Price,
                                    game.ReleaseDate)
                                );            
        })
        .WithName(GetGameEndPointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name, 
                GenreId = newGame.GenreId, 
                Price = newGame.Price, 
                ReleaseDate = newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            //Creates an Dto object so we dont expose our internal classes to the user
            // Thereby making it easier to change later.
            GameDetailsDto gameDto = new(game.Id, game.Name, game.GenreId,game.Price,game.ReleaseDate);

            return Results.CreatedAtRoute(GetGameEndPointName,new {id = gameDto.Id}, gameDto);
        });


        // PUT /games/{id}
        group.MapPut("/{id}", async (int id,UpdateGameDto updatedGame,GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);

            if (game is null)
                return Results.NotFound();

            game.Name = updatedGame.Name;
            game.GenreId = updatedGame.GenreId;
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

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
