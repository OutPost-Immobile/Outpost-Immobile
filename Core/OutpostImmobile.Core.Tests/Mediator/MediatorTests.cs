using Microsoft.Extensions.DependencyInjection;
using Moq;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Mediator.Internal;

namespace OutpostImmobile.Core.Tests.Mediator;

[TestFixture]
public class MediatorTests
{
    public class TestRequest : IRequest<TestRequest, string> { }

    public interface ITestHandler : IRequestHandler<TestRequest, string> { }

    [Test]
    public void SendAsyncShouldResolveAndInvokeHandler()
    {
        var request = new TestRequest();
        var expectedResponse = "Success";

        var registry = new HandlerRegistry();
        registry.Register(typeof(TestRequest), typeof(ITestHandler));

        var mockHandler = new Mock<ITestHandler>();
        mockHandler
            .Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .Returns(expectedResponse);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ITestHandler)))
            .Returns(mockHandler.Object);

        var mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory
            .Setup(f => f.CreateScope())
            .Returns(mockScope.Object);

        var mediator = new Core.Mediator.Internal.Mediator(mockScopeFactory.Object, registry);

        var result = mediator.Send<TestRequest, string>(request);

        Assert.That(result, Is.EqualTo(expectedResponse));

        mockHandler.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void SendAsyncShouldThrowWhenHandlerNotRegistered()
    {
        var emptyRegistry = new HandlerRegistry();
        var scopeFactory = new Mock<IServiceScopeFactory>().Object;

        var mediator = new Core.Mediator.Internal.Mediator(scopeFactory, emptyRegistry);

        Assert.Throws<InvalidOperationException>(() =>
        {
            mediator.Send<TestRequest, string>(new TestRequest());
        });
    }
}