using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    /// <summary>
    /// Command para actualizar una Acción de Personal existente.
    /// Solo permite actualizar acciones en estado "Borrador" (Draft).
    /// </summary>
    public class UpdatePersonalActionCommand : IRequest<Guid>
    {
        /// <summary>ID de la acción a actualizar</summary>
        public Guid Id { get; }

        /// <summary>Datos actualizados de la acción</summary>
        public UpdatePersonalActionDto Data { get; }

        public UpdatePersonalActionCommand(Guid id, UpdatePersonalActionDto data)
        {
            Id = id;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
