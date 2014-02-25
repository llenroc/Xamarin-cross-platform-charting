using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShinobiCharts;
using System.Net;
using System.Linq;
using ShinobiStockChart.Core.Utilities;
using ShinobiStockChart.Core.Presenter;
using ShinobiStockChart.Core.Model;
using ShinobiStockChart.Touch.Utilities;

namespace ShinobiStockChart.Touch.Views
{
    public partial class StockChartViewController : UIViewController, StockChartPresenter.View
    {
        #region View implementation

        public string ChartTitle {
            set {
                if(_chart != null) {
                    _chart.Title = value;
                } else {
                    _chartTitle = value;
                }
            }
        }

        public void UpdateChartWithData (List<ChartDataPoint> data)
        {
            Console.WriteLine ("UpdateChartWithData requested");

            progressIndicatorView.Hidden = true;
            chartHostView.Hidden = false;
        }

        #endregion

        private ShinobiChart _chart;
        private string _chartTitle;

        public StockChartViewController (StockChartPresenter presenter) : base ("StockChartViewController", null)
        {
            Title = presenter.Title;
            presenter.SetView (this);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // create the chart and add to the view      
            _chart = new ShinobiChart (chartHostView.Bounds);
            _chart.LicenseKey = "<PUT YOUR LICENSE KEY HERE";
            if(_chartTitle != null) {
                _chart.Title = _chartTitle;
            }

            // set the datasource
 
            // add a couple of axes
            _chart.XAxis = new SChartDateTimeAxis ();
            _chart.YAxis = new SChartNumberAxis ();
            ConfigureAxis (_chart.XAxis);
            ConfigureAxis (_chart.YAxis);
      
            // add a fancy border to the loading indicato
            progressIndicatorView.Layer.MasksToBounds = false;
            progressIndicatorView.Layer.CornerRadius = 10;
            progressIndicatorView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            progressIndicatorView.Layer.ShadowOpacity = 1.0f;
            progressIndicatorView.Layer.ShadowRadius = 6.0f;
            progressIndicatorView.Layer.ShadowOffset = new SizeF (0f, 3f);
      
            chartHostView.Hidden = true;
            //chartHostView.InsertSubview (_chart, 0);

            // Add a nav bar button to add a trend line
            NavigationItem.SetRightBarButtonItem (
                new UIBarButtonItem (UIBarButtonSystemItem.Compose, (sender, e) => {
                    // Present an alert view
                    var alertView = new UIAlertView ("Moving Average",
                                        "Set the period of the moving average",
                                        null,
                                        "OK",
                                        new string[] { "Cancel" });
                    alertView.Clicked += (alertSender, button) => {
                        if(button.ButtonIndex == 0) {
                            Console.WriteLine ("Moving Average Requested (window {0})", int.Parse (alertView.GetTextField (0).Text));
                        }
                    };
                    alertView.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
                    alertView.GetTextField (0).Placeholder = "Moving Average Period";
                    alertView.GetTextField (0).KeyboardType = UIKeyboardType.NumberPad;
                    alertView.Show ();
                })
                , true);
        }

        private void ConfigureAxis (SChartAxis axis)
        {
            axis.EnableGesturePanning = true;
            axis.EnableGestureZooming = true;
            axis.EnableMomentumPanning = true;
            axis.EnableMomentumZooming = true;
            axis.Style.MajorGridLineStyle.ShowMajorGridLines = false;
        }

    }
}


