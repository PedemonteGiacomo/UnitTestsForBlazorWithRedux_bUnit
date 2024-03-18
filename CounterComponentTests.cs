using BlazorWithRedux.Pages;
using BlazorWithRedux.Store.Counter.Actions;
using BlazorWithRedux.Store.Counter.Reducers;
using BlazorWithRedux.Store.Counter.State;
using BlazorWithRedux.Store.Counter.Feature;
using BlazorWithRedux;
using Bunit;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Fluxor.Blazor.Web;
using FluentAssertions;

namespace UnitTestsForBlazorWithRedux_bUnit
{
    public class CounterComponentTests : TestContext
    {

        [Fact]
        public void simpleTest()
        {
            Services.AddFluxor(options => options
                .ScanAssemblies(typeof(Counter).Assembly)); // this can be putted in a fixture

            // Act
            var component = RenderComponent<Counter>();

            // Assert
            Assert.Equal("Current count: 0", component.Find("#currentCount").TextContent);
            // the component is find correctly and the text is correct, so the test will pass
            component.Instance.IncrementCount();

            var state = Services.GetRequiredService<IState<UndoableCounterState>>();

            component.Render(); // we need to re-render the component to see the changes

            Assert.Equal(1, component.Instance.currentCount); // The current count should be 1
            Assert.Equal("Current count: 1", component.Find("#currentCount").TextContent);

            component.Find("#increment").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(2, component.Instance.currentCount); // The current count should be 2
            Assert.Equal("Current count: 2", component.Find("#currentCount").TextContent);
            // using fluent assertion
            component.Instance.currentCount.Should().Be(2);
            // by getting the state from the services we can check if the state was updated
            state = Services.GetRequiredService<IState<UndoableCounterState>>();
            state.Value.Present.Count.Should().Be(2);
        }

        // now performing also some undo actions after performing the increment
        [Fact]
        public void complexTestWithUndoRedo()
        {
            Services.AddFluxor(options => options
                           .ScanAssemblies(typeof(Counter).Assembly)); // this can be putted in a fixture

            // Act
            var component = RenderComponent<Counter>();

            // Assert
            Assert.Equal("Current count: 0", component.Find("#currentCount").TextContent);
            // the component is find correctly and the text is correct, so the test will pass
            component.Instance.IncrementCount();

            component.Render(); // we need to re-render the component to see the changes

            Assert.Equal(1, component.Instance.currentCount); // The current count should be 1
            Assert.Equal("Current count: 1", component.Find("#currentCount").TextContent);

            component.Find("#increment").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(2, component.Instance.currentCount); // The current count should be 2
            Assert.Equal("Current count: 2", component.Find("#currentCount").TextContent);

            component.Find("#undoAction").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(1, component.Instance.currentCount); // The current count should be 1
            Assert.Equal("Current count: 1", component.Find("#currentCount").TextContent);

            component.Find("#undoAction").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(0, component.Instance.currentCount); // The current count should be 0
            Assert.Equal("Current count: 0", component.Find("#currentCount").TextContent);

            // now redo the action
            component.Find("#redoAction").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(1, component.Instance.currentCount); // The current count should be 1
            Assert.Equal("Current count: 1", component.Find("#currentCount").TextContent);

            // using fluent assertion
            component.Instance.currentCount.Should().Be(1);

            // now undo all the actions
            component.Find("#undoAllAction").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(0, component.Instance.currentCount); // The current count should be 0
            Assert.Equal("Current count: 0", component.Find("#currentCount").TextContent);

            // now redo all the actions
            component.Find("#redoAllAction").Click();
            component.Render(); // we need to re-render the component to see the changes
            Assert.Equal(2, component.Instance.currentCount); // The current count should be 2
            Assert.Equal("Current count: 2", component.Find("#currentCount").TextContent);
        }
    }
}
