using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GtKanu.Infrastructure.Database;

sealed class RoleSeeder : IEntityTypeConfiguration<IdentityRoleGuid>
{
    private readonly Guid _adminId = Guid.Parse("FBBCF7BF-8CBF-440F-939C-F67631109AA0");
    private readonly Guid _treasurerId = Guid.Parse("1FB64576-3F90-4CE9-8C3A-CFA13F587167");
    private readonly Guid _kitchenId = Guid.Parse("1D4D07E0-40AE-45CB-A4F6-397672F3605B");
    private readonly Guid _memberId = Guid.Parse("03A90DA5-ABF0-45E9-9B80-CDAC790B7105");
    private readonly Guid _interestedId = Guid.Parse("9DC6F0A7-7116-4820-8B76-4F20B73053EE");
    private readonly Guid _tripManager = Guid.Parse("2DDC9BEC-EB96-4CD5-94B4-DD0D99A19664");
    private readonly Guid _chairperson = Guid.Parse("8772DA16-FBBC-46AF-A52C-C83C90CA74C9");
    private readonly Guid _userManager = Guid.Parse("EF80D842-4A0F-4613-82EB-48A670BAC21D");
    private readonly Guid _fleetManager = Guid.Parse("F9493060-30B8-4BF3-A4BE-74630B0FA423");
    private readonly Guid _boatManager = Guid.Parse("668EF80A-9F57-4961-BEAC-540AD8003241");
    private readonly Guid _houseManager = Guid.Parse("AFD40E75-DCFE-4C46-9D4C-FF701F225179");
    private readonly Guid _mailingManager = Guid.Parse("6DC9442B-3279-46CC-95C0-1D4F8C3D31F0");
    private readonly Guid _wikiManager = Guid.Parse("02C2DFE3-7331-45F6-929B-F983273F2D74");

    public void Configure(EntityTypeBuilder<IdentityRoleGuid> builder)
    {
        builder.HasData(
            new IdentityRoleGuid
            {
                Id = _adminId,
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = "CFAD6F62-EEAA-4ECD-B847-0762C704EC45"
            },
            new IdentityRoleGuid
            {
                Id = _treasurerId,
                Name = Roles.Treasurer,
                NormalizedName = Roles.Treasurer.ToUpperInvariant(),
                ConcurrencyStamp = "6EE38FDC-5CE5-42FD-A5A5-168573DB2F86"
            },
            new IdentityRoleGuid
            {
                Id = _kitchenId,
                Name = Roles.Kitchen,
                NormalizedName = Roles.Kitchen.ToUpperInvariant(),
                ConcurrencyStamp = "995137CB-BD7B-4747-884E-A7467F3C0A3A"
            },
            new IdentityRoleGuid
            {
                Id = _memberId,
                Name = Roles.Member,
                NormalizedName = Roles.Member.ToUpperInvariant(),
                ConcurrencyStamp = "956824FE-2F13-4919-B2D2-0E60BECFCA12"
            },
            new IdentityRoleGuid
            {
                Id = _interestedId,
                Name = Roles.Interested,
                NormalizedName = Roles.Interested.ToUpperInvariant(),
                ConcurrencyStamp = "3096E774-529C-41B4-8CD3-355E0D2C930D"
            },
            new IdentityRoleGuid
            {
                Id = _tripManager,
                Name = Roles.TripManager,
                NormalizedName = Roles.TripManager.ToUpperInvariant(),
                ConcurrencyStamp = "F3BFC39C-70B7-4E91-A70B-131925290446"
            },
            new IdentityRoleGuid
            {
                Id = _chairperson,
                Name = Roles.Chairperson,
                NormalizedName = Roles.Chairperson.ToUpperInvariant(),
                ConcurrencyStamp = "49DD2CBF-AAC9-4015-9016-9E3ED25547DF"
            },
            new IdentityRoleGuid
            {
                Id = _userManager,
                Name = Roles.UserManager,
                NormalizedName = Roles.UserManager.ToUpperInvariant(),
                ConcurrencyStamp = "EA00C945-985C-42AD-B149-7D6FBCE9C279"
            },
            new IdentityRoleGuid
            {
                Id = _fleetManager,
                Name = Roles.FleetManager,
                NormalizedName = Roles.FleetManager.ToUpperInvariant(),
                ConcurrencyStamp = "D8FEF733-1C63-433F-916F-99B8645D1487"
            },
            new IdentityRoleGuid
            {
                Id = _boatManager,
                Name = Roles.BoatManager,
                NormalizedName = Roles.BoatManager.ToUpperInvariant(),
                ConcurrencyStamp = "4B04648D-DE82-4B0B-B014-3CE0BE5454FD"
            },
            new IdentityRoleGuid
            {
                Id = _houseManager,
                Name = Roles.HouseManager,
                NormalizedName = Roles.HouseManager.ToUpperInvariant(),
                ConcurrencyStamp = "4B04648D-DE82-4B0B-B014-3CE0BE5454FD"
            },
            new IdentityRoleGuid
            {
                Id = _mailingManager,
                Name = Roles.MailingManager,
                NormalizedName = Roles.MailingManager.ToUpperInvariant(),
                ConcurrencyStamp = "4B04648D-DE82-4B0B-B014-3CE0BE5454FD"
            },
            new IdentityRoleGuid
            {
                Id = _wikiManager,
                Name = Roles.WikiManager,
                NormalizedName = Roles.WikiManager.ToUpperInvariant(),
                ConcurrencyStamp = "4B04648D-DE82-4B0B-B014-3CE0BE5454FD"
            }
        );
    }
}
