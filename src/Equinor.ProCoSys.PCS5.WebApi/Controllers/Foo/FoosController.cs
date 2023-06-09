﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.PCS5.Command;
using Equinor.ProCoSys.PCS5.Command.FooCommands.CreateFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.CreateFooComment;
using Equinor.ProCoSys.PCS5.Command.FooCommands.CreateFooLink;
using Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFooAttachment;
using Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFooLink;
using Equinor.ProCoSys.PCS5.Command.FooCommands.OverwriteExistingFooAttachment;
using Equinor.ProCoSys.PCS5.Command.FooCommands.UpdateFoo;
using Equinor.ProCoSys.PCS5.Command.FooCommands.UpdateFooLink;
using Equinor.ProCoSys.PCS5.Command.FooCommands.UploadNewFooAttachment;
using Equinor.ProCoSys.PCS5.Command.FooCommands.VoidFoo;
using Equinor.ProCoSys.PCS5.Query.Attachments;
using Equinor.ProCoSys.PCS5.Query.Comments;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFoo;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooAttachmentDownloadUrl;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooAttachments;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooComments;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFoosInProject;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooLinks;
using Equinor.ProCoSys.PCS5.Query.Links;
using Equinor.ProCoSys.PCS5.WebApi.Middleware;
using ServiceResult;

namespace Equinor.ProCoSys.PCS5.WebApi.Controllers.Foo;

[ApiController]
[Route("Foos")]
public class FoosController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoosController(IMediator mediator) => _mediator = mediator;

    #region Foos
    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet("{guid}")]
    public async Task<ActionResult<FooDetailsDto>> GetFooByGuid(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid)
    {
        var result = await _mediator.Send(new GetFooQuery(guid));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FooDto>>> GetFoosInProject(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [Required]
        [FromQuery] string projectName,
        [FromQuery] bool includeVoided = false)
    {
        var result = await _mediator.Send(new GetFoosInProjectQuery(projectName, includeVoided));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_CREATE, Permissions.APPLICATION_TESTER)]
    [HttpPost]
    public async Task<ActionResult<GuidAndRowVersion>> CreateFoo(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromBody] CreateFooDto dto)
    {
        var result = await _mediator.Send(new CreateFooCommand(dto.Title, dto.ProjectName));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_WRITE, Permissions.APPLICATION_TESTER)]
    [HttpPut("{guid}")]
    public async Task<ActionResult<string>> UpdateFoo(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromBody] UpdateFooDto dto)
    {
        var result = await _mediator.Send(
            new UpdateFooCommand(guid, dto.Title, dto.Text, dto.RowVersion));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_WRITE, Permissions.APPLICATION_TESTER)]
    [HttpPut("{guid}/Void")]
    public async Task<ActionResult<string>> VoidFoo(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromBody] RowVersionDto dto)
    {
        var result = await _mediator.Send(
            new VoidFooCommand(guid, dto.RowVersion));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_DELETE, Permissions.APPLICATION_TESTER)]
    [HttpDelete("{guid}")]
    public async Task<ActionResult> DeleteFoo(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromBody] RowVersionDto dto)
    {
        var result = await _mediator.Send(new DeleteFooCommand(guid, dto.RowVersion));
        return this.FromResult(result);
    }
    #endregion

    #region Links
    [AuthorizeAny(Permissions.FOO_ATTACH, Permissions.APPLICATION_TESTER)]
    [HttpPost("{guid}/Links")]
    public async Task<ActionResult<GuidAndRowVersion>> CreateFooLink(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromBody] CreateLinkDto dto)
    {
        var result = await _mediator.Send(new CreateFooLinkCommand(guid, dto.Title, dto.Url));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet("{guid}/Links")]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetFooLinks(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid)
    {
        var result = await _mediator.Send(new GetFooLinksQuery(guid));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_WRITE, Permissions.APPLICATION_TESTER)]
    [HttpPut("{guid}/Links/{linkGuid}")]
    public async Task<ActionResult<string>> UpdateFooLink(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromRoute] Guid linkGuid,
        [FromBody] UpdateLinkDto dto)
    {
        var result = await _mediator.Send(new UpdateFooLinkCommand(guid, linkGuid, dto.Title, dto.Url, dto.RowVersion));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_DELETE, Permissions.APPLICATION_TESTER)]
    [HttpDelete("{guid}/Links/{linkGuid}")]
    public async Task<ActionResult> DeleteLink(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromRoute] Guid linkGuid,
        [FromBody] RowVersionDto dto)
    {
        var result = await _mediator.Send(new DeleteFooLinkCommand(guid, linkGuid, dto.RowVersion));
        return this.FromResult(result);
    }
    #endregion

    #region Comments
    [AuthorizeAny(Permissions.FOO_WRITE, Permissions.APPLICATION_TESTER)]
    [HttpPost("{guid}/Comments")]
    public async Task<ActionResult<GuidAndRowVersion>> CreateFooComment(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromBody] CreateCommentDto dto)
    {
        var result = await _mediator.Send(new CreateFooCommentCommand(guid, dto.Text));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet("{guid}/Comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetFooComments(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid)
    {
        var result = await _mediator.Send(new GetFooCommentsQuery(guid));
        return this.FromResult(result);
    }
    #endregion

    #region Attachments
    [AuthorizeAny(Permissions.FOO_ATTACH, Permissions.APPLICATION_TESTER)]
    [HttpPost("{guid}/Attachments")]
    public async Task<ActionResult<GuidAndRowVersion>> UploadFooAttachment(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromForm] UploadNewAttachmentDto dto)
    {
        await using var stream = dto.File.OpenReadStream();

        var result = await _mediator.Send(new UploadNewFooAttachmentCommand(
            guid,
            dto.File.FileName,
            stream));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_ATTACH, Permissions.APPLICATION_TESTER)]
    [HttpPut("{guid}/Attachments")]
    public async Task<ActionResult<string>> OverwriteExistingFooAttachment(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromForm] OverwriteAttachmentDto dto)
    {
        await using var stream = dto.File.OpenReadStream();

        var result = await _mediator.Send(new OverwriteExistingFooAttachmentCommand(
            guid, 
            dto.File.FileName,
            dto.RowVersion,
            stream));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet("{guid}/Attachments")]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetFooAttachments(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid)
    {
        var result = await _mediator.Send(new GetFooAttachmentsQuery(guid));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_DELETE, Permissions.APPLICATION_TESTER)]
    [HttpDelete("{guid}/Attachments/{attachmentGuid}")]
    public async Task<ActionResult> DeleteAttachment(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromRoute] Guid attachmentGuid,
        [FromBody] RowVersionDto dto)
    {
        var result = await _mediator.Send(new DeleteFooAttachmentCommand(guid, attachmentGuid, dto.RowVersion));
        return this.FromResult(result);
    }

    [AuthorizeAny(Permissions.FOO_READ, Permissions.APPLICATION_TESTER)]
    [HttpGet("{guid}/Attachments/{attachmentGuid}")]
    public async Task<ActionResult<string>> GetFooAttachmentDownloadUrl(
        [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
        [Required]
        [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
        string plant,
        [FromRoute] Guid guid,
        [FromRoute] Guid attachmentGuid)
    {
        var result = await _mediator.Send(new GetFooAttachmentDownloadUrlQuery(guid, attachmentGuid));

        if (result.ResultType != ResultType.Ok)
        {
            return this.FromResult(result);
        }

        return Ok(result.Data.ToString());
    }
    #endregion
}
