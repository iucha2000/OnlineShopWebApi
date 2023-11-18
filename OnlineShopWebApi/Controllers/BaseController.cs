﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        public readonly ISender _mediator;
        //public readonly ILogger _logger;

        public BaseController(/*ILogger logger, */ISender mediator)
        {
            _mediator = mediator;
            //_logger = logger;
        }
    }
}
