﻿using Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IExchangeRate
    {
        Task<ExchangeRatesModel> GetExchangeRates(string currency);
    }
}
