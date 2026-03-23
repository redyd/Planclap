using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.Tests.Common;

/// <summary>
///     Classe générée par IA
/// </summary>
public class PageNotifierTests
{
    [Test]
    public void Should_InvokeSubscribedHandler_When_PublishIsCalled_Given_HandlerForEventTypeExists()
    {
        // Arrange
        var sut = new PageNotifier();
        TestEventA? received = null;

        sut.Subscribe<TestEventA>(e => received = e);

        var evt = new TestEventA { Value = 42 };

        // Act
        sut.Publish(evt);

        // Assert
        Assert.That(received, Is.Not.Null);
        Assert.That(received!.Value, Is.EqualTo(42));
    }

    [Test]
    public void Should_NotInvokeHandler_When_PublishIsCalled_Given_NoHandlerForEventTypeExists()
    {
        // Arrange
        var sut = new PageNotifier();
        var invoked = false;

        // We subscribe to a different event type
        sut.Subscribe<TestEventB>(_ => invoked = true);

        var evt = new TestEventA { Value = 10 };

        // Act
        sut.Publish(evt);

        // Assert
        Assert.That(invoked, Is.False);
    }

    [Test]
    public void Should_InvokeAllSubscribedHandlers_When_PublishIsCalled_Given_MultipleHandlersForSameType()
    {
        // Arrange
        var sut = new PageNotifier();
        var callCount = 0;

        sut.Subscribe<TestEventA>(_ => callCount++);
        sut.Subscribe<TestEventA>(_ => callCount++);

        var evt = new TestEventA();

        // Act
        sut.Publish(evt);

        // Assert
        Assert.That(callCount, Is.EqualTo(2));
    }

    [Test]
    public void Should_KeepHandlersForDifferentTypesSeparate_When_PublishIsCalled_Given_MultipleEventTypes()
    {
        // Arrange
        var sut = new PageNotifier();
        var invokedA = false;
        var invokedB = false;

        sut.Subscribe<TestEventA>(_ => invokedA = true);
        sut.Subscribe<TestEventB>(_ => invokedB = true);

        // Act
        sut.Publish(new TestEventA());

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(invokedA, Is.True);
            Assert.That(invokedB, Is.False);
        }
    }

    [Test]
    public void Should_AddNewListForEventType_When_SubscribeIsCalled_Given_FirstSubscriptionForType()
    {
        // Arrange
        var sut = new PageNotifier();

        // Act
        sut.Subscribe<TestEventA>(_ => { });

        // Assert
        // We check behavior by publishing
        var invoked = false;
        sut.Subscribe<TestEventA>(_ => invoked = true);
        sut.Publish(new TestEventA());

        Assert.That(invoked, Is.True);
    }

    private class TestEventA : EventArgs
    {
        public int Value { get; set; }
    }

    private class TestEventB : EventArgs
    {
        public string Text { get; set; } = string.Empty;
    }
}
