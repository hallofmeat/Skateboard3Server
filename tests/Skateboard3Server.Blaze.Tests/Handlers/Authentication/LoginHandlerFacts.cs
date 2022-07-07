using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Skateboard3Server.Blaze.Handlers.Authentication;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Common.Decoders;
using Skateboard3Server.Common.Models;
using Skateboard3Server.Data;
using Xunit;

namespace Skateboard3Server.Blaze.Tests.Handlers.Authentication;

public class LoginHandlerFacts
{
    private readonly DbContextOptions<Skateboard3Context> _dbContextOptions;

    private static readonly Fixture Fixture = new();

    public LoginHandlerFacts()
    {
        // Build DbContextOptions
        _dbContextOptions = new DbContextOptionsBuilder<Skateboard3Context>()
            .UseInMemoryDatabase(databaseName: "Skateboard3")
            .Options;
    }

    [Fact]
    public async Task Handle_new_user_created()
    {
        var dbContext = new Skateboard3Context(_dbContextOptions);
        var clientContext = Substitute.For<ClientContext>();
        var notificationHandler = Substitute.For<IBlazeNotificationHandler>();
        var ticketDecoder = Substitute.For<IPs3TicketDecoder>();
        var userSessionManager = Substitute.For<IUserSessionManager>();
        var clientManager = Substitute.For<IClientManager>();

        var ticket = Fixture.Create<Ps3Ticket>();

        ticketDecoder.DecodeTicket(Arg.Any<byte[]>()).Returns(ticket);

        var request = new LoginRequest
        {
            Email = "dummy@example.com",
            Ticket = new byte[] { }
        };

        //Act
        var handler = new LoginHandler(dbContext, clientContext, notificationHandler, ticketDecoder, userSessionManager,
            clientManager);

        var _ = await handler.Handle(request, CancellationToken.None);

        //Assert
        dbContext.Users.Should().HaveCount(1);
    }

    //TODO test blob
    //TODO test the rest

}