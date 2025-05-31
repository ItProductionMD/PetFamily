using PetFamily.Application.Enums;

namespace PetFamily.Application.Dtos;
public class OrderBy<T>(T orderByProperty, OrderDirection orderDirection)
{
    public T Property = orderByProperty;
    public OrderDirection Direction = orderDirection;
}

