using CourseLibrary.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseLibrary.API.DbContexts;

public class CourseLibraryContext : DbContext
{
    public CourseLibraryContext(DbContextOptions<CourseLibraryContext> options)
       : base(options)
    {
    }

    // base DbContext constructor ensures that Books and Authors are not null after
    // having been constructed.  Compiler warning ("uninitialized non-nullable property")
    // can safely be ignored with the "null-forgiving operator" (= null!)

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // seed the database with dummy data
        modelBuilder.Entity<Author>().HasData(
            new Author("Berry", "Griffin Beak Eldritch", "Ships")
            {
                Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                DateOfBirth = new DateTime(1980, 7, 23)
            },
            new Author("Nancy", "Swashbuckler Rye", "Rum")
            {
                Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                DateOfBirth = new DateTime(1978, 5, 21)
            },
            new Author("Eli", "Ivory Bones Sweet", "Singing")
            {
                Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                DateOfBirth = new DateTime(1957, 12, 16)
            },
            new Author("Arnold", "The Unseen Stafford", "Singing")
            {
                Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                DateOfBirth = new DateTime(1957, 3, 6)
            },
            new Author("Seabury", "Toxic Reyson", "Maps")
            {
                Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                DateOfBirth = new DateTime(1956, 11, 23)
            },
            new Author("Rutherford", "Fearless Cloven", "General debauchery")
            {
                Id = Guid.Parse("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                DateOfBirth = new DateTime(1981, 4, 5)
            },
            new Author("Atherton", "Crow Ridley", "Rum")
            {
                Id = Guid.Parse("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                DateOfBirth = new DateTime(1982, 10, 11)
            }
            );

        modelBuilder.Entity<Course>().HasData(
           new Course("Commandeering a Ship Without Getting Caught")
           {
               Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
               AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
               Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
           },
           new Course("Overthrowing Mutiny")
           {
               Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
               AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
               Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
           },
           new Course("Avoiding Brawls While Drinking as Much Rum as You Desire")
           {
               Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
               AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
               Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk."
           },
           new Course("Singalong Pirate Hits")
           {
               Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
               AuthorId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
               Description = "In this course you'll learn how to sing all-time favourite pirate songs without sounding like you actually know the words or how to hold a note."
           }
           );

        // fix to allow sorting on DateTimeOffset when using Sqlite, based on
        // https://blog.dangl.me/archive/handling-datetimeoffset-in-sqlite-with-entity-framework-core/
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // Sqlite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754 
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset)
                        || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}


