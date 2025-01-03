using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.Shared.DTO
{
    public record AdressDomainDTO
    {
        public string? Street { get; }
        public string? City { get; }
        public string? Country { get; }
        public string? Number { get; }
        public Dictionary<string, string?> DictionaryForValidate { get; }
        public AdressDomainDTO(string? street, string? city, string? country, string? number)
        {
            Street = street;
            City = city;
            Country = country;
            Number = number;
            DictionaryForValidate = new Dictionary<string, string?>
            {
                { "country", Street },
                { "city",City },
                { "street",Country},
                { "number", Number}
            };
        }
    }
}
