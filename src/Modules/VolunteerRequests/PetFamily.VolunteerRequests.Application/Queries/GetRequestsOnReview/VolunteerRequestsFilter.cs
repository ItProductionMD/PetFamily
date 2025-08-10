namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

public class VolunteerRequestsFilter
{
    public List<string> Statuses { get; set; }

    public VolunteerRequestsFilter(List<string> statuses)
    {
        Statuses = statuses.ToHashSet().ToList();
    }
};
