using System.Reflection;
using Moq;
using OutpostImmobile.Api.Controllers;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Maczkopats.Commands;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OutpostImmobile.Core.Tests.Controllers;

[TestFixture]
public class MaczkopatControllerTests
{
    private static MethodInfo GetAddLogAsyncMethod()
    {
        var method = typeof(MaczkopatController)
            .GetMethod("AddLogAsync", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.That(method, Is.Not.Null, "AddLogAsync method not found.");
        return method!;
    }

    [Test]
    [Order(1)]
    public async Task AddLogAsync_ReturnsCreated_WhenMediatorSucceeds()
    {
        // jeśli
        var maczkopatId = Guid.NewGuid();
        var request = new AddLogRequest
        {
            MaczkopatId = maczkopatId,
            LogType = MaczkopatEventLogType.LockerOpened
        };

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(
                It.Is<MaczkopatAddLogCommand>(cmd =>
                    cmd.MaczkopatId == maczkopatId &&
                    cmd.LogType == request.LogType),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var method = GetAddLogAsyncMethod();

        // gdy
        var task = (Task)method.Invoke(null, new object[] { mediator.Object, request })!;
        await task;

        var result = task.GetType().GetProperty("Result")!.GetValue(task);

        // wtedy
        Assert.That(result, Is.InstanceOf<Results<Created, BadRequest>>());
        
        var resultProperty = result.GetType().GetProperty("Result");
        var actualResult = resultProperty?.GetValue(result);
    
        Assert.That(actualResult, Is.InstanceOf<Created>());

        mediator.Verify(
            m => m.Send(It.IsAny<MaczkopatAddLogCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    [Order(2)]
    public void AddLogAsync_ThrowsException_WhenMediatorFails()
    {
        // jeśli
        var request = new AddLogRequest
        {
            MaczkopatId = Guid.NewGuid(),
            LogType = MaczkopatEventLogType.Error
        };

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(
                It.IsAny<MaczkopatAddLogCommand>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Simulated failure"));

        var method = GetAddLogAsyncMethod();

        // gdy / wtedy
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var task = (Task)method.Invoke(null, new object[] { mediator.Object, request })!;
            await task;
        });
    }
}
