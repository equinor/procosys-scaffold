﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.PCS5.Command;
using Equinor.ProCoSys.PCS5.Command.FooCommands.CreateFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.EditFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.VoidFoo;
using Equinor.ProCoSys.PCS5.Query.GetFooById;
using Equinor.ProCoSys.PCS5.Query.GetFoosInProject;
using Equinor.ProCoSys.PCS5.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.PCS5.WebApi.Controllers.Foo
{
    [ApiController]
    [Route("Foos")]
    public class FoosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoosController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.FOO_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<FooDetailsDto>> GetFooById(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetFooByIdQuery(id));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.FOO_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FooDto>>> GetFoosInProject(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromQuery] string projectName,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetFoosInProjectQuery(projectName, includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.FOO_CREATE)]
        [HttpPost]
        public async Task<ActionResult<IdAndRowVersion>> CreateFoo(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateFooDto dto)
        {
            var result = await _mediator.Send(
                new CreateFooCommand(dto.Title, dto.ProjectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.FOO_WRITE)]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> EditFoo(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] EditFooDto dto)
        {
            var result = await _mediator.Send(
                new EditFooCommand(id, dto.Title, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.FOO_WRITE)]
        [HttpPut("{id}/Void")]
        public async Task<ActionResult<string>> VoidFoo(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] RowVersionDto dto)
        {
            var result = await _mediator.Send(
                new VoidFooCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.FOO_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteFoo(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] RowVersionDto dto)
        {
            var result = await _mediator.Send(new DeleteFooCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }
    }
}
