using AutoMapper;
using eShop.Ordering.Database.SQL.Entities;

namespace eShop.Ordering.Database.SQL.Mappings;

public class StateViewJsonMapping : Profile
{
    public StateViewJsonMapping() : base()
    {
        CreateMap<OrderStateViewJson, OrderStateViewJson>();
        CreateMap<CustomerOrdersStateViewJson, CustomerOrdersStateViewJson>();
        CreateMap<StatusOrdersStateViewJson, StatusOrdersStateViewJson>();
        CreateMap<DailyOrdersStateViewJson, DailyOrdersStateViewJson>();
    }
}