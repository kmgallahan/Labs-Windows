// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemSeparator = Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;

namespace CommunityToolkit.Labs.Shared.Helpers;

public static class NavigationViewHelper
{
    public static IEnumerable<NavigationViewItem> GenerateNavItemTree(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        // Make categories
        var categoryData = GenerateCategoryNavItems(sampleMetadata);

        foreach (var navData in categoryData)
        {
            // Make subcategories
            var subcategoryData = GenerateSubcategoryNavItems(navData.SampleMetadata ?? Enumerable.Empty<ToolkitFrontMatter>());

            foreach (var subcategoryItemData in subcategoryData)
            {
                // Make samples
                var sampleNavigationItems = GenerateSampleNavItems(subcategoryItemData.SampleMetadata ?? Enumerable.Empty<ToolkitFrontMatter>());
                subcategoryItemData.NavItem.MenuItems.Add(new NavigationViewItemSeparator());
                foreach (var item in sampleNavigationItems)
                {
                    // Add sample to subcategory
                    subcategoryItemData.NavItem.MenuItems.Add(item);
                }

                // Add subcategory to category
       
                navData.NavItem.MenuItems.Add(subcategoryItemData.NavItem);
            }

            // Return category
            yield return navData.NavItem;
        }
    }

    private static IEnumerable<NavigationViewItem> GenerateSampleNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        foreach (var metadata in sampleMetadata)
        {
            yield return new NavigationViewItem
            {
                Content = metadata.Title,
                Icon = new BitmapIcon() { ShowAsMonochrome = false, UriSource = new Uri("ms-appx:///Assets/Images/ConnectedAnimation.png") }, // TO DO: This is probably a property we need to add to ToolkitFrontMatter?
                Tag = metadata,
            };
        }
    }

    private static IEnumerable<GroupNavigationItemData> GenerateSubcategoryNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        var samplesBySubcategory = sampleMetadata.GroupBy(x => x.Subcategory);

        foreach (var subcategoryGroup in samplesBySubcategory)
        {
            yield return new GroupNavigationItemData(new NavigationViewItem
            {
                Content = subcategoryGroup.Key,
                SelectsOnInvoked = false,
                Style = (Style)App.Current.Resources["SubcategoryNavigationViewItemStyle"],
            }, subcategoryGroup.ToArray());
        }
    }

    private static IEnumerable<GroupNavigationItemData> GenerateCategoryNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        var samplesByCategory = sampleMetadata.GroupBy(x => x.Category);

        foreach (var categoryGroup in samplesByCategory)
        {
            yield return new GroupNavigationItemData(new NavigationViewItem
            {
                Content = categoryGroup.Key,
                Icon = new SymbolIcon() { Symbol = Symbol.Keyboard }, // TO DO: Helper that checks what icon belongs to what Category enum
                SelectsOnInvoked = false,
            }, categoryGroup.ToArray());
        }
    }

    /// <param name="NavItem">A navigation item to contain items in this group.</param>
    /// <param name="SampleMetadata">The samples that belong under <see cref="NavItem"/>.</param>
    private record GroupNavigationItemData(NavigationViewItem NavItem, IEnumerable<ToolkitFrontMatter> SampleMetadata);

}