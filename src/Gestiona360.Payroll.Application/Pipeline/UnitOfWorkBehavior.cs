// src/Gestiona360.Payroll.Application/Pipeline/UnitOfWorkBehavior.cs

using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Pipeline;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // ✅ Detección por convención: Solo aplicar transacción si el nombre termina en "Command"
        var requestTypeName = typeof(TRequest).Name;
        var isCommand = requestTypeName.EndsWith("Command");

        if (!isCommand)
        {
            // Es un Query, no necesita transacción
            return await next();
        }

        // Es un Command, aplicar transacción
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var response = await next();

            await _unitOfWork.CommitTransactionAsync();

            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error durante la transacción para {RequestName}", requestTypeName);
            throw;
        }
    }
}