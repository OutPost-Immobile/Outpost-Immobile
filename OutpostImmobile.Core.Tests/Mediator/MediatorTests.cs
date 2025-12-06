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
    public async Task SendAsyncShouldResolveAndInvokeHandler()
    {
        var request = new TestRequest();
        var expectedResponse = "Success";
        
        var registry = new HandlerRegistry();
        registry.Register(typeof(TestRequest), typeof(ITestHandler));
        
        var mockHandler = new Mock<ITestHandler>();
        mockHandler
            .Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ITestHandler)))
            .Returns(mockHandler.Object);

        var mediator = new Core.Mediator.Internal.Mediator(mockServiceProvider.Object, registry);
        
        var result = await mediator.Send(request);
        
        Assert.That(result, Is.EqualTo(expectedResponse));
        
        mockHandler.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void SendAsyncShouldThrowWhenHandlerNotRegistered()
    {
        var emptyRegistry = new HandlerRegistry();
        var mockProvider = new Mock<IServiceProvider>();
        var mediator = new Core.Mediator.Internal.Mediator(mockProvider.Object, emptyRegistry);
        
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
        {
            await mediator.Send(new TestRequest());
        });
    }
}