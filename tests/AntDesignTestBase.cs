﻿using System;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AntDesign.Tests
{
    public class AntDesignTestBase : IDisposable
    {
        public TestContext Context { get; }
        public TestNavigationManager NavigationManager { get; }

        public AntDesignTestBase()
        {
            Context = new TestContext();
            NavigationManager = new TestNavigationManager();

            Context.Services.AddScoped<NavigationManager>(sp => NavigationManager);
            Context.Services.AddAntDesign();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
