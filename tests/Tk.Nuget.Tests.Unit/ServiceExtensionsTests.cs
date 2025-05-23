﻿using Shouldly;
using Microsoft.Extensions.DependencyInjection;

namespace Tk.Nuget.Tests.Unit
{
    public class ServiceExtensionsTests
    {
        [Fact]
        public void AddNugetClient_InjectionMade()
        {
            var col = new ServiceCollection();

            var col2 = col.AddNugetClient();

            var xs = col2.Where(sd => sd.ServiceType == typeof(INugetClient));

            xs.ShouldNotBeEmpty();
        }
    }
}
