// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static CommunityToolkit.Labs.WinUI.TokenView;

namespace TokenViewExperiment.Samples;

/// <summary>
/// Sample of the basic properties of the TokenView
/// </summary>
[ToolkitSampleMultiChoiceOption("LayoutOrientation", "Horizontal", "Wrapped", Title = "Orientation")]
[ToolkitSampleMultiChoiceOption("SelectionMode", "Single", "Multiple", Title = "Selection mode")]
[ToolkitSample(id: nameof(TokenViewBasicSample), "TokenView", description: $"A sample for showing how to create and use a {nameof(TokenView)} control.")]
public sealed partial class TokenViewBasicSample : Page
{
    public TokenViewBasicSample()
    {
        this.InitializeComponent();
    }

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static TokenViewOrientation ConvertStringToTokenViewOrientation(string orientation) => orientation switch
    {
        "Horizontal" => TokenViewOrientation.Horizontal,
        "Wrapped" => TokenViewOrientation.Wrapped,
        _ => throw new System.NotImplementedException(),
    };

    public static ListViewSelectionMode ConvertStringToListViewSelectionMode(string mode) => mode switch
    {
        "Single" => ListViewSelectionMode.Single,
        "Multiple" => ListViewSelectionMode.Multiple,
        _ => throw new System.NotImplementedException(),
    };
}
