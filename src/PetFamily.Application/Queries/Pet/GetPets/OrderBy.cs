namespace PetFamily.Application.Queries.Pet.GetPets;

public class OrderBy
{
    public string OrderByProperty { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public OrderBy(string orderBy, OrderDirection orderDirection)
    {
        OrderByProperty = orderBy;
        OrderDirection = orderDirection;
    }
}
