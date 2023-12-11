﻿using Application.Services;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Images.Commands
{
    public record UploadImageCommand(IFormFile file, int productId) : IRequest<Result>;

    internal class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result>
    {
        private readonly IImageService _imageService;

        public UploadImageCommandHandler(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<Result> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var result = await _imageService.UploadImageAsync(request.file);

            return Result.Succeed();
        }
    }
}
