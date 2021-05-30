using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepo, IMapper mapper,
            IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger
        )
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepo.AddAsync(orderEntity);

            _logger.LogInformation($"Order {newOrder.Id} created successfully");

            await SendEmail(newOrder);

            return newOrder.Id;
        }

        private async Task SendEmail(Order order)
        {
            var email = new Email
                {To = order.EmailAddress, Body = $"Your order was created successfully", Subject = "ORDER CREATED"};
            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Order {order.Id} could not be created due to an error with the email service: {e.Message}"
                );
            }
        }
    }
}