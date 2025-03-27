using System;
using System.Collections.Generic;

namespace PetFamily.Infrastructure.Contexts.ReadDbContext.Models;

public partial class Species
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Breed> Breeds { get; set; } = new List<Breed>();
}
