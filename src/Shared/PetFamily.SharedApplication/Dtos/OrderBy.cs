using PetFamily.SharedApplication.Enums;

namespace PetFamily.SharedApplication.Dtos;
public class OrderBy<T>(T orderByProperty, OrderDirection orderDirection)
{
    public T Property = orderByProperty;
    public OrderDirection Direction = orderDirection;
}

