﻿using Application.Common.Persistence;
using Application.Services;
using Domain;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Order.Commands
{
    public record CheckoutOrderCommand (Guid userId, bool delivery, string? address): IRequest<Result<Domain.Entities.Order>>;

    internal class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<Domain.Entities.Order>>
    {
        private readonly IGenericRepository<Domain.Entities.Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entities.Product> _productRepo;
        private readonly IGenericRepository<Domain.Entities.User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRate _exchangeRate;
        private readonly IEmailSender _emailSender;

        public CheckoutOrderCommandHandler(IGenericRepository<Domain.Entities.Order> orderRepo, IGenericRepository<Domain.Entities.Product> productRepo,
            IGenericRepository<Domain.Entities.User> userRepo, IUnitOfWork unitOfWork, IExchangeRate exchangeRate, IEmailSender emailSender)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _exchangeRate = exchangeRate;
            _emailSender = emailSender;
        }

        public async Task<Result<Domain.Entities.Order>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(x => x.Id == request.userId, includes: "Orders");
            if(user.EmailVerified == false)
            {
                throw new EmailNotVerifiedException($"Email {user.Email} is not verified", 400);
            }

            var ongoingOrder = user.Orders.FirstOrDefault(x=> x.IsCompleted == false);

            if(ongoingOrder == null)
            {
                throw new OrderNotFoundException("Ongoing order not found", request.userId, 404);
            }

            var orderFromDb = await _orderRepo.GetByExpressionAsync(x => x.Id == ongoingOrder.Id, includes: "Products");

            var totalSum = orderFromDb.Products.Sum(x => x.Price);
            var rates = await _exchangeRate.GetExchangeRates(user.Currency);
            var totalSumConverted = Math.Round(totalSum / rates.conversion_rates.USD, 2);

            if(user.Balance < totalSumConverted)
            {
                return Result<Domain.Entities.Order>.Fail("No enough balance");
            }

            user.Balance -= totalSumConverted;

            orderFromDb.Products.ToList().ForEach(x => x.IsSold = true);
            orderFromDb.IsCompleted = true;
            orderFromDb.EndDate = DateTime.Now;
            await _emailSender.SendEmailAsync(user.Email, "Order confirmed", $"Your order is confirmed and set up for delivery. \nOrder number: {orderFromDb.Id}");

            if (request.delivery)
            {
                var deliveryAddress = string.IsNullOrWhiteSpace(request.address) ? user.Address : request.address;
                await _emailSender.SendEmailAsync(user.Email, "Order information", $"Your order will be sent to address: {deliveryAddress}");
            }

            await _unitOfWork.CommitAsync();

            return Result<Domain.Entities.Order>.Succeed(orderFromDb);
        }
    }
}
