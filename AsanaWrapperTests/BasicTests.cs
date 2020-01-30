using System;
using Xunit;
using AsanaAPI;
using System.Collections.Generic;

namespace AsanaWrapperTests
{
    public class BasicTests
    {
        [Fact]
        public void CanConnect()
        {
            var sut = new AsanaWorkSpace("Test");

            sut.AddTask($"test task {DateTime.UtcNow.Ticks.ToString()}", "dummy text", new List<string>() { "Etsy Orders (Test)" }, DateTime.Now.AddDays(5));

        }
    }
}
