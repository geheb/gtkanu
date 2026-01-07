using Dapper;
using GtKanu.Infrastructure.Database;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace GtKanu.Infrastructure.Worker;

internal sealed class MySqlMigration
{
    private readonly string _connectionStringMySql;
    private readonly AppDbContext _appDbContext;

    public MySqlMigration(AppDbContext appDbContext, IConfiguration configuration)
    {
        _connectionStringMySql = configuration.GetConnectionString("MySql")!;
        _appDbContext = appDbContext;
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        var connection = new MySqlConnection(_connectionStringMySql);
        await connection.OpenAsync(cancellationToken);

        await InsertIdentities(connection, cancellationToken);
        await InsertVehicles(connection, cancellationToken);
        await InsertClubhouseBookings(connection, cancellationToken);
        await InsertTrips(connection, cancellationToken);
        await InsertTryouts(connection, cancellationToken);
        await InsertInvoices(connection, cancellationToken);
        await InsertFoodBookings(connection, cancellationToken);
        await InsertMailings(connection, cancellationToken);
    }

    private async Task InsertMailings(MySqlConnection connection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.Mailings.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var mailings = await connection.QueryAsync("select Id,Created,Updated,CanSendToAllMembers,OtherRecipients,ReplyAddress,Subject,HtmlBody,IsClosed,EmailCount,IsYoungPeople from mailings");
        foreach (var m in mailings)
        {
            byte[] id = m.Id;

            var entity = new Mailing
            {
                Id = new Guid(id),
                Created = new DateTimeOffset(m.Created, TimeSpan.Zero),
                Updated = m.Updated is null ? null : new DateTimeOffset(m.Updated, TimeSpan.Zero),
                CanSendToAllMembers = m.CanSendToAllMembers,
                OtherRecipients = m.OtherRecipients,
                ReplyAddress = m.ReplyAddress,
                Subject = m.Subject,
                HtmlBody = m.HtmlBody,
                IsClosed = m.IsClosed,
                EmailCount = m.EmailCount,
                IsYoungPeople = m.IsYoungPeople,
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);

        var myMailings = await connection.QueryAsync("select Id,Created,Updated,MailingId,UserId,HasRead from my_mailings");

        foreach (var m in myMailings)
        {
            byte[] id = m.Id;
            byte[] mailingId = m.MailingId;
            byte[] userId = m.UserId;

            var entity = new MyMailing
            {
                Id = new Guid(id),
                Created = new DateTimeOffset(m.Created, TimeSpan.Zero),
                Updated = m.Updated is null ? null : new DateTimeOffset(m.Updated, TimeSpan.Zero),
                MailingId = new Guid(mailingId),
                UserId = new Guid(userId),
                HasRead = m.HasRead,
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertFoodBookings(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.FoodBookings.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var foodLists = await mySqlConnection.QueryAsync("select Id,Name,ValidFrom from food_lists");
        foreach (var l in foodLists)
        {
            byte[] id = l.Id;

            var entity = new FoodList
            {
                Id = new Guid(id),
                Name = l.Name,
                ValidFrom = new DateTimeOffset(l.ValidFrom, TimeSpan.Zero)
            };

            _appDbContext.Add(entity);
        }

        var foods = await mySqlConnection.QueryAsync("select Id,FoodListId,Name,Price,Type from foods");
        foreach (var f in foods)
        {
            byte[] id = f.Id;
            byte[] foodListId = f.FoodListId;

            var entity = new Food
            {
                Id = new Guid(id),
                FoodListId = new Guid(foodListId),
                Name = f.Name,
                Price = f.Price,
                Type = f.Type,
            };

            _appDbContext.Add(entity);
        }

        var bookings = await mySqlConnection.QueryAsync("select Id,UserId,FoodId,Status,Count,BookedOn,CancelledOn,InvoiceId from bookings");
        foreach (var b in bookings)
        {
            byte[] id = b.Id;
            byte[] userId = b.UserId;
            byte[] foodId = b.FoodId;
            byte[] invoiceId = b.InvoiceId;

            var entity = new FoodBooking
            {
                Id = new Guid(id),
                UserId = new Guid(userId),
                FoodId = new Guid(foodId),
                Status = b.Status,
                Count = b.Count,
                BookedOn = new DateTimeOffset(b.BookedOn, TimeSpan.Zero),
                CancelledOn = b.CancelledOn is null ? null : new DateTimeOffset(b.CancelledOn, TimeSpan.Zero),
                InvoiceId = invoiceId is null ? null : new Guid(invoiceId),
            };

            _appDbContext.Add(entity);

        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertInvoices(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.FoodInvoices.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var periods = await mySqlConnection.QueryAsync("select Id,Description,`From`,`To` from invoice_periods");
        foreach (var p in periods)
        {
            byte[] id = p.Id;

            var entity = new FoodInvoicePeriod
            {
                Id = new Guid(id),
                Description = p.Description,
                From = new DateTimeOffset(p.From, TimeSpan.Zero),
                To = new DateTimeOffset(p.To, TimeSpan.Zero),
            };

            _appDbContext.Add(entity);
        }

        var invoices = await mySqlConnection.QueryAsync("select Id,UserId,CreatedOn,Total,Status,PaidOn,InvoicePeriodId from invoices");
        foreach (var i in invoices)
        {
            byte[] id = i.Id;
            byte[] userId = i.UserId;
            byte[] invoicePeriodId = i.InvoicePeriodId;

            var entity = new FoodInvoice
            {
                Id = new Guid(id),
                UserId = new Guid(userId),
                CreatedOn = new DateTimeOffset(i.CreatedOn, TimeSpan.Zero),
                Total = i.Total,
                Status = i.Status,
                PaidOn = i.PaidOn is null ? null : new DateTimeOffset(i.PaidOn, TimeSpan.Zero),
                InvoicePeriodId = invoicePeriodId is null ? null : new Guid(invoicePeriodId),
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertTryouts(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.Tryouts.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var tryouts = await mySqlConnection.QueryAsync("select Id,Type,Date,UserId,MaxBookings,BookingStart,BookingEnd,Description from tryouts");
        foreach (var t in tryouts)
        {
            byte[] id = t.Id;
            byte[] userId = t.UserId;

            var entity = new Tryout
            {
                Id = new Guid(id),
                Type = t.Type,
                Date = new DateTimeOffset(t.Date, TimeSpan.Zero),
                UserId = new Guid(userId),
                MaxBookings = t.MaxBookings,
                BookingStart = t.BookingStart is null ? null : new DateTimeOffset(t.BookingStart, TimeSpan.Zero),
                BookingEnd = t.BookingEnd is null ? null : new DateTimeOffset(t.BookingEnd, TimeSpan.Zero),
                Description = t.Description,
            };

            _appDbContext.Add(entity);
        }

        var bookings = await mySqlConnection.QueryAsync("select Id,TryoutId,UserId,Name,BookedOn,ConfirmedOn,CancelledOn from tryout_bookings");
        foreach (var b in bookings)
        {
            byte[] id = b.Id;
            byte[] tryoutId = b.TryoutId;
            byte[] userId = b.UserId;

            var entity = new TryoutBooking
            {
                Id = new Guid(id),
                TryoutId = new Guid(tryoutId),
                UserId = new Guid(userId),
                Name = b.Name,
                BookedOn = new DateTimeOffset((DateTime)b.BookedOn, TimeSpan.Zero),
                ConfirmedOn = b.ConfirmedOn is null ? null : new DateTimeOffset(b.ConfirmedOn, TimeSpan.Zero),
                CancelledOn = b.CancelledOn is null ? null : new DateTimeOffset(b.CancelledOn, TimeSpan.Zero),
            };

            _appDbContext.Add(entity);
        }

        var chats = await mySqlConnection.QueryAsync("select Id,TryoutId,UserId,CreatedOn,Message from tryout_chats");
        foreach (var c in chats)
        {
            byte[] id = c.Id;
            byte[] tryoutId = c.TryoutId;
            byte[] userId = c.UserId;

            var entity = new TryoutChat
            {
                Id = new Guid(id),
                TryoutId = new Guid(tryoutId),
                UserId = new Guid(userId),
                CreatedOn = new DateTimeOffset(c.CreatedOn, TimeSpan.Zero),
                Message = c.Message,
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertTrips(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.Trips.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var trips = await mySqlConnection.QueryAsync("select Id,Start,End,UserId,Target,MaxBookings,BookingStart,BookingEnd,Description,Categories,IsPublic from trips");
        foreach (var t in trips)
        {
            byte[] id = t.Id;
            byte[] userId = t.UserId;

            var entity = new Trip
            {
                Id = new Guid(id),
                Start = new DateTimeOffset(t.Start, TimeSpan.Zero),
                End = new DateTimeOffset(t.End, TimeSpan.Zero),
                UserId = new Guid(userId),
                Target = t.Target,
                MaxBookings = t.MaxBookings,
                BookingStart = t.BookingStart is null ? null : new DateTimeOffset(t.BookingStart, TimeSpan.Zero),
                BookingEnd = t.BookingEnd is null ? null : new DateTimeOffset(t.BookingEnd, TimeSpan.Zero),
                Description = t.Description,
                Categories = t.Categories,
                IsPublic = t.IsPublic,
            };

            _appDbContext.Add(entity);
        }

        var bookings = await mySqlConnection.QueryAsync("select Id,TripId,UserId,Name,BookedOn,ConfirmedOn,CancelledOn from trip_bookings");
        foreach (var b in bookings)
        {
            byte[] id = b.Id;
            byte[] tripId = b.TripId;
            byte[] userId = b.UserId;

            var entity = new TripBooking
            {
                Id = new Guid(id),
                TripId = new Guid(tripId),
                UserId = new Guid(userId),
                Name = b.Name,
                BookedOn = new DateTimeOffset(b.BookedOn, TimeSpan.Zero),
                ConfirmedOn = b.ConfirmedOn is null ? null : new DateTimeOffset(b.ConfirmedOn, TimeSpan.Zero),
                CancelledOn = b.CancelledOn is null ? null : new DateTimeOffset(b.CancelledOn, TimeSpan.Zero),
            };

            _appDbContext.Add(entity);
        }

        var chats = await mySqlConnection.QueryAsync("select Id,TripId,UserId,CreatedOn,Message from trip_chats");
        foreach (var c in chats)
        {
            byte[] id = c.Id;
            byte[] tripId = c.TripId;
            byte[] userId = c.UserId;

            var entity = new TripChat
            {
                Id = new Guid(id),
                TripId = new Guid(tripId),
                UserId = new Guid(userId),
                CreatedOn = new DateTimeOffset(c.CreatedOn, TimeSpan.Zero),
                Message = c.Message,
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertClubhouseBookings(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.ClubhouseBookings.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var bookings = await mySqlConnection.QueryAsync("select Id,Start,End,Title,Description from clubhouse_bookings");
        foreach (var b in bookings)
        {
            byte[] id = b.Id;

            var entity = new ClubhouseBooking
            {
                Id = new Guid(id),
                Start = new DateTimeOffset(b.Start, TimeSpan.Zero),
                End = new DateTimeOffset(b.End, TimeSpan.Zero),
                Title = b.Title,
                Description = b.Description,
            };

            await _appDbContext.AddAsync(entity, cancellationToken);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertVehicles(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.Vehicles.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var vehicles = await mySqlConnection.QueryAsync("select Id,Name,Identifier,IsInUse from vehicles");
        foreach (var v in vehicles)
        {
            byte[] id = v.Id;
            var entity = new Vehicle
            {
                Id = new Guid(id),
                Name = v.Name,
                Identifier = v.Identifier,
                IsInUse = v.IsInUse
            };

            _appDbContext.Add(entity);
        }

        var bookings = await mySqlConnection.QueryAsync("select Id,VehicleId,UserId,Start,End,BookedOn,ConfirmedOn,CancelledOn,Purpose from vehicle_bookings");
        foreach (var b in bookings)
        {
            byte[] id = b.Id;
            byte[] vehicleId = b.VehicleId;
            byte[] userId = b.UserId;

            var entity = new VehicleBooking
            {
                Id = new Guid(id),
                VehicleId = new Guid(vehicleId),
                UserId = new Guid(userId),
                Start = new DateTimeOffset(b.Start, TimeSpan.Zero),
                End = new DateTimeOffset(b.End, TimeSpan.Zero),
                BookedOn = new DateTimeOffset(b.BookedOn, TimeSpan.Zero),
                ConfirmedOn = b.ConfirmedOn is not null ? new DateTimeOffset(b.ConfirmedOn, TimeSpan.Zero) : null,
                CancelledOn = b.CancelledOn is not null ? new DateTimeOffset(b.CancelledOn, TimeSpan.Zero) : null,
                Purpose = b.Purpose,
            };

            _appDbContext.Add(entity);
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertIdentities(MySqlConnection mySqlConnection, CancellationToken cancellationToken)
    {
        var count = await _appDbContext.IdentityUsers.CountAsync(cancellationToken);
        if (count > 0)
        {
            return;
        }

        var users = await mySqlConnection.QueryAsync("select Id,Name,LastLogin,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,DebtorNumber,AddressNumber,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LeftOn,AuthenticatorKey,Mailings from users");
        foreach (var u in users)
        {
            byte[] id = u.Id;

            var entity = new IdentityUserGuid
            {
                Id = new Guid(id),
                UserName = new Guid(id).ToString("N"),
                NormalizedUserName = new Guid(id).ToString("N").ToUpperInvariant(),
                Name = u.Name,
                LastLogin = u.LastLogin is not null ? new DateTimeOffset(u.LastLogin, TimeSpan.Zero) : null,
                Email = u.Email,
                NormalizedEmail = u.NormalizedEmail,
                EmailConfirmed = u.EmailConfirmed,
                PasswordHash = u.PasswordHash,
                SecurityStamp = u.SecurityStamp,
                ConcurrencyStamp = u.ConcurrencyStamp,
                LockoutEnabled = true,
                DebtorNumber = u.DebtorNumber,
                AddressNumber = u.AddressNumber,
                PhoneNumber = u.PhoneNumber,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                TwoFactorEnabled = u.TwoFactorEnabled,
                LeftOn = u.LeftOn is not null ? new DateTimeOffset(u.LeftOn, TimeSpan.Zero) : null,
                AuthenticatorKey = u.AuthenticatorKey,
                Mailings = u.Mailings
            };

            var userRoles = new List<IdentityUserRoleGuid>();

            var roles = await mySqlConnection.QueryAsync<byte[]>("select RoleId from user_roles where UserId=@id", new { id });
            foreach (var r in roles)
            {
                userRoles.Add(new()
                {
                    UserId = new Guid(id),
                    RoleId = new Guid(r)
                });
            }

            entity.UserRoles = userRoles;

            _appDbContext.Add(entity);

            var tokens = await mySqlConnection.QueryAsync("select LoginProvider,Name,Value from user_tokens where UserId=@id", new { id });
            foreach (var t in tokens)
            {
                var token = new IdentityUserTokenGuid
                {
                    UserId = new Guid(id),
                    LoginProvider = t.LoginProvider,
                    Name = t.Name,
                    Value = t.Value
                };

                _appDbContext.Add(token);
            }

            if (entity.TwoFactorEnabled)
            {
                var claim = new IdentityUserClaimGuid
                {
                    UserId = new Guid(id),
                    ClaimType = UserClaims.TwoFactorClaim.Type,
                    ClaimValue = UserClaims.TwoFactorClaim.Value
                };

                _appDbContext.Add(claim);
            }
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}