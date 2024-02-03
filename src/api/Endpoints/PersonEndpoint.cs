using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Model;
using Data;
using System.ComponentModel.DataAnnotations;

namespace Endpoints;

public static class PersonEndpoint
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Person").WithTags(nameof(Person));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Person.ToListAsync();
        })
        .WithName("GetAllPeople")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Person>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Person.AsNoTracking()
                .FirstOrDefaultAsync(model => model.id == id)
                is Person model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetPersonById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest<List<ValidationResult>>>> (int id, Person person, AppDbContext db) =>
        {
             if (!person.TryValidate(person, out var validationResults))
            {
                return TypedResults.BadRequest(validationResults);
            }

            var affected = await db.Person
                .Where(model => model.id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.FirstName, person.FirstName)
                    .SetProperty(m => m.Age, person.Age)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdatePerson")
        .WithOpenApi();

        //how to activate data annotation validations here ?

        group.MapPost("/", async Task<Results<Created<Person>, BadRequest<List<ValidationResult>>>> (
            Person person, AppDbContext db) =>
        {
            if (!person.TryValidate(person, out var validationResults))
            {
                return TypedResults.BadRequest(validationResults);
            }
            db.Person.Add(person);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Person/{person.id}", person);
        })
        .WithName("CreatePerson")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Person
                .Where(model => model.id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeletePerson")
        .WithOpenApi();
    }
}
