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
using Skateboard3Server.Blaze.Tickets;
using Skateboard3Server.Data;
using Xunit;

namespace Skateboard3Server.Blaze.Tests.Handlers.Authentication;

public class LoginHandlerFacts
{
    private static readonly Fixture Fixture = new();
    private readonly DbContextOptions<Skateboard3Context> _dbContextOptions;

    public LoginHandlerFacts()
    {
        // Build DbContextOptions
        _dbContextOptions = new DbContextOptionsBuilder<Skateboard3Context>()
            .UseInMemoryDatabase("Skateboard3")
            .Options;
    }

    [Fact]
    public async Task Handle_new_user_created()
    {
        var dbContext = new Skateboard3Context(_dbContextOptions);
        var clientContext = Substitute.For<ClientContext>();
        var notificationHandler = Substitute.For<IBlazeNotificationHandler>();
        var ticketParser = Substitute.For<IPs3TicketParser>();
        var ticketValidator = Substitute.For<IPs3TicketValidator>();
        var userSessionManager = Substitute.For<IUserSessionManager>();
        var clientManager = Substitute.For<IClientManager>();

        var ticket = Fixture.Create<Ps3Ticket>();

        ticketParser.ParseTicket(Arg.Any<byte[]>()).Returns(ticket);
        ticketValidator.ValidateTicket(Arg.Any<Ps3Ticket>()).Returns(true);

        var request = new LoginRequest
        {
            Email = "dummy@example.com",
            Ticket = new byte[] { }
        };

        //Act
        var handler = new LoginHandler(dbContext, clientContext, notificationHandler, ticketParser, ticketValidator,
            userSessionManager,
            clientManager);

        var _ = await handler.Handle(request, CancellationToken.None);

        //Assert
        dbContext.Users.Should().HaveCount(1);
    }

    //TODO test blob
    //TODO test the rest
}