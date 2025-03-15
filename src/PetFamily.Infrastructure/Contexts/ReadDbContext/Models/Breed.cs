using System;
using System.Collections.Generic;

namespace PetFamily.Infrastructure.Contexts.ReadDbContext.Models;

public partial class Breed
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? SpeciesId { get; set; }

    public virtual Species? Species { get; set; }
}
