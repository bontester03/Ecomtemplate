using KhanHomeFloralLine.Api.Extensions;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/account")]
[Authorize]
public class AccountController(IAppDbContext dbContext) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        var user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                Phone = x.PhoneNumber,
                x.CreatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppNotFoundException("User not found");

        var addresses = await dbContext.UserAddresses
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc)
            .Select(x => new
            {
                x.Id,
                x.Label,
                x.RecipientName,
                x.Phone,
                x.AddressLine,
                x.Area,
                x.Emirate,
                x.Landmark,
                x.IsDefault,
                x.CreatedAtUtc,
                x.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.Phone,
            user.CreatedAtUtc,
            addresses
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");
        ValidateProfileRequest(request);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AppNotFoundException("User not found");

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.Phone.Trim();
        user.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] UpsertAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");
        ValidateAddressRequest(request);

        var address = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Label = request.Label.Trim(),
            RecipientName = request.RecipientName.Trim(),
            Phone = request.Phone.Trim(),
            AddressLine = request.AddressLine.Trim(),
            Area = request.Area.Trim(),
            Emirate = request.Emirate.Trim(),
            Landmark = request.Landmark?.Trim(),
            IsDefault = request.IsDefault,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.UserAddresses.Add(address);
        if (address.IsDefault)
        {
            await UnsetOtherDefaults(userId, address.Id, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(address);
    }

    [HttpPut("addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpsertAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");
        ValidateAddressRequest(request);

        var address = await dbContext.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId && x.IsActive, cancellationToken)
            ?? throw new AppNotFoundException("Address not found");

        address.Label = request.Label.Trim();
        address.RecipientName = request.RecipientName.Trim();
        address.Phone = request.Phone.Trim();
        address.AddressLine = request.AddressLine.Trim();
        address.Area = request.Area.Trim();
        address.Emirate = request.Emirate.Trim();
        address.Landmark = request.Landmark?.Trim();
        address.IsDefault = request.IsDefault;
        address.UpdatedAtUtc = DateTime.UtcNow;

        if (address.IsDefault)
        {
            await UnsetOtherDefaults(userId, address.Id, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("addresses/{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        var address = await dbContext.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId && x.IsActive, cancellationToken)
            ?? throw new AppNotFoundException("Address not found");

        address.IsActive = false;
        address.IsDefault = false;
        address.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task UnsetOtherDefaults(Guid userId, Guid currentAddressId, CancellationToken cancellationToken)
    {
        var others = await dbContext.UserAddresses
            .Where(x => x.UserId == userId && x.Id != currentAddressId && x.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var item in others)
        {
            item.IsDefault = false;
            item.UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    private static void ValidateProfileRequest(UpdateProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new AppValidationException("Full name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            throw new AppValidationException("Phone is required");
        }
    }

    private static void ValidateAddressRequest(UpsertAddressRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Label)
            || string.IsNullOrWhiteSpace(request.RecipientName)
            || string.IsNullOrWhiteSpace(request.Phone)
            || string.IsNullOrWhiteSpace(request.AddressLine)
            || string.IsNullOrWhiteSpace(request.Area)
            || string.IsNullOrWhiteSpace(request.Emirate))
        {
            throw new AppValidationException("Address label, recipient, phone, address line, area and emirate are required");
        }
    }

    public sealed record UpdateProfileRequest(string FullName, string Phone);

    public sealed record UpsertAddressRequest(
        string Label,
        string RecipientName,
        string Phone,
        string AddressLine,
        string Area,
        string Emirate,
        string? Landmark,
        bool IsDefault);
}
