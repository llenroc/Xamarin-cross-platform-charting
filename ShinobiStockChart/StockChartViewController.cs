using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShinobiCharts;
using System.Net;
using System.Linq;
using ShinobiStockChart.Utilities;
using ShinobiStockChart.Presenter;

namespace ShinobiStockChart
{
    public partial class StockChartViewController : UIViewController, StockChartPresenter.View
    {
        #region View implementation

        public string ChartTitle {
            set {
                if (symbolLabel != null) {
                    symbolLabel.Text = value;
                } else {
                    _chartTitle = value;
                }
            }
        }

        public void UpdateChartWithData (List<ChartDataPoint> data)
        {
            _chartDataSource.DataPoints = data.Select(dp => new SChartDataPoint () {
                                                    XValue = dp.XValue.ToNSDate (),
                                                    YValue = new NSNumber (dp.YValue)
                                                })
                                                .Cast<SChartData>()
                                                .ToList(); 
            _chart.ReloadData ();
            _chart.RedrawChart ();

            progressIndicatorView.Hidden = true;
            chartHostView.Hidden = false;
        }

        public event EventHandler<MovingAverageRequestedEventArgs> MovingAverageRequested = delegate { };

        public void UpdateChartWithMovingAverage (List<ChartDataPoint> data)
        {
            _chartDataSource.MovingAverageDataPoints = data.Select (dp => new SChartDataPoint () {
                XValue = dp.XValue.ToNSDate (),
                YValue = new NSNumber (dp.YValue)
            })
                .Cast<SChartData> ()
                .ToList ();
            _chart.ReloadData ();
            _chart.RedrawChart ();
        }

        #endregion

        private ShinobiChart _chart;

        private StockChartDataSource _chartDataSource;

        private string _chartTitle;

        public StockChartViewController (StockChartPresenter presenter) : base ("StockChartViewController", null)
        {
            Title = presenter.Title;
            presenter.SetView (this);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            if (_chartTitle != null) {
                symbolLabel.Text = _chartTitle;
            }

            // create the chart and add to the view      
            _chart = new ShinobiChart (chartHostView.Bounds);
			_chart.LicenseKey = @"<PUT YOUR LICENSE KEY HERE>";
      
            // set the datasource
            _chartDataSource = new StockChartDataSource ();
            _chart.DataSource = _chartDataSource;
      
            _chart.Theme = new SChartMidnightTheme ();
            View.BackgroundColor = _chart.Theme.ChartStyle.BackgroundColor;
 
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
            chartHostView.InsertSubview (_chart, 0);

            // Wire up the moving average button
            btnCreateMovingAverage.TouchUpInside += (sender, e) => {
                movingAveragePeriod.ResignFirstResponder ();
                var maPeriod = int.Parse (movingAveragePeriod.Text);
                MovingAverageRequested(this, new MovingAverageRequestedEventArgs (maPeriod));
            };
        }

        private void ConfigureAxis (SChartAxis axis)
        {
            axis.EnableGesturePanning = true;
            axis.EnableGestureZooming = true;
            axis.EnableMomentumPanning = true;
            axis.EnableMomentumZooming = true;
        }


        private class StockChartDataSource : SChartDataSource
        {
            private List<SChartData> _dataPoints;
            private List<SChartData> _movingAverageDataPoints;

            public StockChartDataSource ()
            {
                _dataPoints = new List<SChartData> ();      
            }

            public List<SChartData> DataPoints {
                set {
                    _dataPoints = value;
                }
            }

            public List<SChartData> MovingAverageDataPoints {
                set {
                    _movingAverageDataPoints = value;
                }
            }

            public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int seriesIndex)
            {
                // no-op
                return null;
            }

            protected override SChartData[] GetDataPoints (ShinobiChart chart, int seriesIndex)
            {
                if (seriesIndex == 0) {
                    return _dataPoints.ToArray ();
                } else {
                    return _movingAverageDataPoints.ToArray ();
                }
            }

            public override int GetNumberOfSeries (ShinobiChart chart)
            {
                if(_movingAverageDataPoints != null) {
                    return 2;
                } else {
                    return 1;
                }
            }

            public override int GetNumberOfDataPoints (ShinobiChart chart, int seriesIndex)
            {
                if(seriesIndex == 0) {
                    return _dataPoints.Count;
                } else {
                    return _movingAverageDataPoints.Count;
                }
            }

            public override SChartSeries GetSeries (ShinobiChart chart, int index)
            {
                var lineSeries = new SChartLineSeries ();
         
                if (index == 0) {
                    lineSeries.Style.LineColor = UIColor.FromRGB (166, 166, 166);
                    lineSeries.Style.AreaColor = UIColor.FromRGB (16, 99, 123);
                    lineSeries.Style.AreaColorLowGradient = UIColor.FromRGB (0, 0, 41);
                    lineSeries.Style.ShowFill = true;
                    lineSeries.CrosshairEnabled = true;
                } else {
                    lineSeries.Style.LineColor = UIColor.Red;
                    lineSeries.Style.LineWidth = 2.0;
                    lineSeries.Style.ShowFill = false;
                    lineSeries.CrosshairEnabled = false;
                }
        
                return lineSeries;
            }
        }
    }
}


