// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
#if WINAPPSDK
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinRT.Interop;
#endif


// A control to show a Fluent titlebar

namespace CommunityToolkit.App.Shared.Controls;

[TemplatePart(Name = LeftPaddingColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = RightPaddingColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = BackButtonColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = IconColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = LeftPaddingColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = LeftDragColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = RightDragColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = SearchColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = TitleColumn, Type = typeof(ColumnDefinition))]
[TemplatePart(Name = TitleTextBlock, Type = typeof(TextBlock))]
[TemplatePart(Name = AppTitleBar, Type = typeof(Grid))]
[TemplateVisualState(Name = "Visible", GroupName = "BackButtonStates")]
[TemplateVisualState(Name = "Collapsed", GroupName = "BackButtonStates")]
[TemplatePart(Name = PartIconPresenter, Type = typeof(Button))]
[TemplatePart(Name = PartDragRegionPresenter, Type = typeof(Grid))]
public sealed partial class TitleBar : Control
{
    internal const string LeftPaddingColumn = "MenuColumn";
    internal const string RightPaddingColumn = "RightPaddingColumn";
    internal const string AppTitleBar = "AppTitleBar";
    internal const string LeftDragColumn = "LeftDragColumn";
    internal const string RightDragColumn = "RightDragColumn";
    internal const string TitleColumn = "TitleColumn";
    internal const string BackButtonColumn = "BackButtonColumn";
    internal const string SearchColumn = "SearchColumn";
    internal const string IconColumn = "IconColumn";
    private const string PartDragRegionPresenter = "PART_DragRegion";
    private const string TitleTextBlock = "PART_TitleText";

    
    private const string PartIconPresenter = "PART_BackButton";
    private Button? _backButton;
    private Grid? _dragRegion;
    private TitleBar? _titleBar;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata(default(string)));

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TitleBar), new PropertyMetadata(default(ImageSource)));

    public bool IsBackButtonVisible
    {
        get => (bool)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(TitleBar), new PropertyMetadata(default(bool), IsBackButtonVisibleChanged));

    public event EventHandler<RoutedEventArgs>? BackButtonClick;


    public TitleBar()
    {
        this.DefaultStyleKey = typeof(TitleBar);
        #if WINAPPSDK
        Loaded += this.TitleBar_Loaded;
        SizeChanged += this.TitleBar_SizeChanged;
#endif
    }



    protected override void OnApplyTemplate()
    {
        Update();
        _titleBar = (TitleBar)this;
        _backButton = (Button)_titleBar.GetTemplateChild(PartIconPresenter);
        _dragRegion = (Grid)_titleBar.GetTemplateChild(PartDragRegionPresenter);
        _backButton.Click += _backButton_Click;
        base.OnApplyTemplate();
    }

    private void _backButton_Click(object sender, RoutedEventArgs e)
    {
        OnBackButtonClicked();
    }

    private void OnBackButtonClicked()
    {
        BackButtonClick?.Invoke(this, new RoutedEventArgs());
    }

    private static void IsBackButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((TitleBar)d).Update();
    }

    private void Update()
    {
        VisualStateManager.GoToState(this, IsBackButtonVisible ? "Visible" : "Collapsed", true);
#if WINAPPSDK
        SetDragRegionForCustomTitleBar();
#endif
    }

    private void SetTitleBar()
    {
#if WINAPPSDK && !HAS_UNO
    SetDragRegionForCustomTitleBar();
#endif
#if WINDOWS_UWP && !HAS_UNO
        Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        Window.Current.SetTitleBar(_dragRegion);
        // NOT SUPPORTED IN UNO WASM
#endif
    }

    private void TitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        SetTitleBar();
    }




#if WINAPPSDK
    private void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetDragRegionForCustomTitleBar();
    }

    private void SetDragRegionForCustomTitleBar()
    {
        Window window = App.currentWindow;
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(wndId);

        var titleBar = appWindow.TitleBar;
        titleBar.ExtendsContentIntoTitleBar = true;

        double scaleAdjustment = GetScaleAdjustment(window);

        if (GetTemplateChild(RightPaddingColumn) is ColumnDefinition rightPaddingColumn)
        {
            rightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);

            if (GetTemplateChild(LeftPaddingColumn) is ColumnDefinition leftPaddingColumn)
            {
                leftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                if (GetTemplateChild(IconColumn) is ColumnDefinition iconColumn)
                {
                    if (GetTemplateChild(TitleColumn) is ColumnDefinition titleColumn)
                    {
                        if (GetTemplateChild(LeftDragColumn) is ColumnDefinition leftDragColumn)
                        {
                            if (GetTemplateChild(SearchColumn) is ColumnDefinition searchColumn)
                            {
                                if (GetTemplateChild(RightDragColumn) is ColumnDefinition rightDragColumn)
                                {
                                    if (GetTemplateChild(TitleTextBlock) is FrameworkElement titleTextBlock)
                                    {

                                        List<Windows.Graphics.RectInt32> dragRectsList = new();

                                        if (GetTemplateChild(AppTitleBar) is FrameworkElement appTitleBar)
                                        {
                                            Windows.Graphics.RectInt32 dragRectL;
                                            dragRectL.X = (int)((leftPaddingColumn.ActualWidth) * scaleAdjustment);
                                            dragRectL.Y = 0;
                                            dragRectL.Height = (int)(appTitleBar.ActualHeight * scaleAdjustment);
                                            dragRectL.Width = (int)((iconColumn.ActualWidth
                                                                    + titleColumn.ActualWidth
                                                                    + leftDragColumn.ActualWidth) * scaleAdjustment);
                                            dragRectsList.Add(dragRectL);

                                            Windows.Graphics.RectInt32 dragRectR;
                                            dragRectR.X = (int)((leftPaddingColumn.ActualWidth
                                                                + iconColumn.ActualWidth
                                                                + titleTextBlock.ActualWidth
                                                                + leftDragColumn.ActualWidth
                                                                + searchColumn.ActualWidth) * scaleAdjustment);
                                            dragRectR.Y = 0;
                                            dragRectR.Height = (int)(appTitleBar.ActualHeight * scaleAdjustment);
                                            dragRectR.Width = (int)(rightDragColumn.ActualWidth * scaleAdjustment);
                                            dragRectsList.Add(dragRectR);

                                            Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                                            appWindow.TitleBar.SetDragRectangles(dragRects);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

      [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

    private double GetScaleAdjustment(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
        if (result != 0)
        {
            throw new Exception("Could not get DPI for monitor.");
        }

        uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
        return scaleFactorPercent / 100.0;
    }
#endif
}
