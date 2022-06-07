// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Labs.WinUI;

#if !WINAPPSDK
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#endif

namespace Expander.Tests
{
    [TestClass]
    public class ExampleExpanderTestClass
    {
        [TestMethod]
        public void SimpleExampleTest()
        {
            var systemUnderTest = new CommunityToolkit.Labs.WinUI.Expander();
            Assert.IsNotNull(systemUnderTest);
        }
    }
}