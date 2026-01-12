using System;
using GameStoreApi.Data;
using GameStoreApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameStoreApi.Endpoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        // GET /genres
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres.Select(genre => new GenreDto(genre.Id, genre.Name)).AsNoTracking().ToListAsync()
        );
    }
}
