using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Volunteers;

public static class SharedVolunteerRequests
{
    public record SocialNetworksRequest(string Name,string Url);

    public record DonateDetailsRequest(string Name, string Description);

}
