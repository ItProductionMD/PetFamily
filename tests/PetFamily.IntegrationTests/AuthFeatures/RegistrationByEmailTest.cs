using Account.Application.UserManagement.Commands.RegisterByEmail;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.WebApplicationFactory;

namespace PetFamily.IntegrationTests.AuthFeatures;

public class RegistrationByEmailTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<RegisterByEmailCommand>(factory)
{
    [Fact]
    public async Task Should_register_user_successfully()
    {
        RegisterByEmailCommand cmd = new
        (
            "unique@gmail.com",
            "uniqueLogin",
            "uniquePassword-1234",
            "+390",
            "1000002",
            []
        );


    }
}
