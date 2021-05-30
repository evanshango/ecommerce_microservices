using System.Collections.Generic;
using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQuery : IRequest<List<OrdersVm>>
    {
        public string Username { get; }

        public GetOrdersListQuery(string username)
        {
            Username = username;
        }
    }
}