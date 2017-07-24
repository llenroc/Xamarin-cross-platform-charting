# Cross-Platform Charting App using ShinobiControls

This project demonstrates an approach to building cross-platform apps using
Xamarin and ShinobiControls.

## Overview

The project uses the MVP pattern to allow extensive code sharing between the
Android and iOS solutions. The shared code is in a portable class library (PCL)
and there are 2 additional projects in the solution for the Android and iOS
apps.

## Getting Started

### Shinobi Trial Keys

This project has been built using the trial versions of **shinobicontrols** for iOS
and for Android - available from the 
[ShinobiControls website](https://www.shinobicontrols.com).

Since we're using the trial versions you'll be emailed trial keys for each
version when you download the trials.

There are 2 files which require trial keys - `StockChartViewController.cs` for
the iOS project:

    ShinobiCharts.TrialKey = @"<PUT YOUR TRIAL KEY HERE>";

And `StockChartActivity.cs` in the Android project:

    _chart.SetTrialKey ("<PUT YOUR TRIAL KEY HERE>");


### Minimum Versions

The iOS app has been designed to work with iOS7, and the Android with api-level
> 12 (it has been tested with Android 4.0.4, Ice Cream Sandwich (API Level 15)).


