using FluentAssertions;
using PathFinder.sdk.Models;
using PathFinderWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PathFinderWeb.Client.Test
{
    public class StateCacheServiceTests
    {
        [Fact]
        public void GivenSingleState_WhenCached_Retrieve()
        {
            // Arrange
            var stateCacheService = new StateCacheService();
            var state = new KeyValue("key1", "value1");

            // Act
            stateCacheService.Count.Should().Be(0);
            KeyValue saveState = stateCacheService.GetOrCreate(() => new KeyValue("key1", "value1"));

            // Assert
            stateCacheService.Count.Should().Be(1);
            KeyValue test = (KeyValue)stateCacheService.Select(x => x.Value).First();
            (test == state).Should().BeTrue();

            stateCacheService.TryGetValue<KeyValue>(out KeyValue value2).Should().BeTrue();

            KeyValue saveState2 = stateCacheService.GetOrCreate(() => new KeyValue("key2", "value2"));
            (saveState2 == state).Should().BeTrue();
        }

        [Fact]
        public void GivenGenericState_WhenCached_Retrieve()
        {
            // Arrange
            var stateCacheService = new StateCacheService();

            // Act
            stateCacheService.Count.Should().Be(0);
            Envelope<KeyValue> saveState = stateCacheService.GetOrCreate(() => new Envelope<KeyValue>("first", new KeyValue("key1", "value1")));

            // Assert
            stateCacheService.Count.Should().Be(1);
            Envelope<KeyValue> test = (Envelope<KeyValue>)stateCacheService.Select(x => x.Value).First();

            stateCacheService.TryGetValue<Envelope<KeyValue>>(out Envelope<KeyValue> value2).Should().BeTrue();

            value2.Name.Should().Be("first");
            value2.Value.Key.Should().Be("key1");
            value2.Value.Value.Should().Be("value1");
        }

        [Fact]
        public void GivenTwoState_WhenCached_Retrieve()
        {
            // Arrange
            var stateCacheService = new StateCacheService();

            // Act
            stateCacheService.Count.Should().Be(0);
            KeyValue saveState = stateCacheService.GetOrCreate(() => new KeyValue("key1", "value1"));
            Envelope<KeyValue> saveState2 = stateCacheService.GetOrCreate(() => new Envelope<KeyValue>("first", new KeyValue("key1", "value1")));

            // Assert
            stateCacheService.Count.Should().Be(2);
            stateCacheService.TryGetValue<KeyValue>(out KeyValue value2).Should().BeTrue();
            (new KeyValue("key1", "value1") == saveState).Should().BeTrue();

            stateCacheService.TryGetValue<Envelope<KeyValue>>(out Envelope<KeyValue> value3).Should().BeTrue();
            value3.Name.Should().Be("first");
            value3.Value.Key.Should().Be("key1");
            value3.Value.Value.Should().Be("value1");
        }

        [Fact]
        public void GivenTwoGenericState_WhenCached_Retrieve()
        {
            // Arrange
            var stateCacheService = new StateCacheService();

            // Act / Assert
            Envelope<KeyValuePair<string, string>> saveState = stateCacheService.GetOrCreate(() => new Envelope<KeyValuePair<string, string>>("first", new KeyValuePair<string, string>("key1", "value1")));

            stateCacheService.TryGetValue(out Envelope<KeyValuePair<string, string>> value1).Should().BeTrue();
            value1.Name.Should().Be("first");
            value1.Value.Key.Should().Be("key1");
            value1.Value.Value.Should().Be("value1");

            // Act / Assert
            Envelope<KeyValue> saveState2 = stateCacheService.GetOrCreate(() => new Envelope<KeyValue>("second", new KeyValue("key2", "value2")));

            stateCacheService.TryGetValue(out Envelope<KeyValue> value2).Should().BeTrue();
            value2.Name.Should().Be("second");
            value2.Value.Key.Should().Be("key2");
            value2.Value.Value.Should().Be("value2");
        }

        private class Envelope<T>
        {
            public Envelope(string name, T value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }
            public T Value { get; set; }
        }
    }
}
