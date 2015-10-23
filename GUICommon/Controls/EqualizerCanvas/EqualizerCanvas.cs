using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MPDisplay.Common.Controls
{
    internal struct EQData
    {
        public int Value;
        public int Falloff;
        public int Speed;
    }

    public enum EQStyle
    {
        SingleRectangle = 0,
        SingleRoundedRectangle = 1,
        SingleCircle = 2,
        SingleBar = 3,
        DoubleRectangle = 4,
        DoubleRoundedRectangle = 5,
        DoubleCircle = 6,
        DoubleBar = 7
    }

    public class EqualizerCanvas : Canvas
    {
        #region Vars

        private bool _rendering;
        private EQData[] _fftData = new EQData[100];

        const int MaxRangeValue = 255;
        private Pen _borderPen;
        private Pen _falloffPen;
        private int _ledCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the GUIEqualizerSurface class.
        /// </summary>
        public EqualizerCanvas()
        {
            UseLayoutRounding = true;
        }

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty EQStyleProperty = DependencyProperty.Register("EQStyle", typeof(EQStyle), typeof(EqualizerCanvas), new PropertyMetadata(EQStyle.SingleBar));
        public static readonly DependencyProperty LowRangeValueProperty = DependencyProperty.Register("LowRangeValue", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(85));
        public static readonly DependencyProperty MedRangeValueProperty = DependencyProperty.Register("MedRangeValue", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(170));
        public static readonly DependencyProperty LowRangeColorProperty = DependencyProperty.Register("LowRangeColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Green));
        public static readonly DependencyProperty MedRangeColorProperty = DependencyProperty.Register("MedRangeColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Yellow));
        public static readonly DependencyProperty MaxRangeColorProperty = DependencyProperty.Register("MaxRangeColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty BandCountProperty = DependencyProperty.Register("BandCount", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(30));
        public static readonly DependencyProperty BandSpacingProperty = DependencyProperty.Register("BandSpacing", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(2));
        public static readonly DependencyProperty EQChannelProperty = DependencyProperty.Register("EQChannel", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(0));
        public static readonly DependencyProperty BandBorderSizeProperty = DependencyProperty.Register("BandBorderSize", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(1, (d, e) => ((EqualizerCanvas) d).SetBorderPen()));
        public static readonly DependencyProperty BandCornerRadiusProperty = DependencyProperty.Register("BandCornerRadius", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(2));
        public static readonly DependencyProperty BandBorderColorProperty = DependencyProperty.Register("BandBorderColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Silver, (d, e) => ((EqualizerCanvas) d).SetBorderPen()));
        public static readonly DependencyProperty FallOffColorProperty = DependencyProperty.Register("FallOffColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Gray, (d, e) => ((EqualizerCanvas) d).SetFalloffPen()));
        public static readonly DependencyProperty FalloffSpeedProperty = DependencyProperty.Register("FalloffSpeed", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(5));
        public static readonly DependencyProperty FallOffHeightProperty = DependencyProperty.Register("FallOffHeight", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(4, (d, e) => ((EqualizerCanvas) d).SetFalloffPen()));
        public static readonly DependencyProperty ShowDummyDataProperty = DependencyProperty.Register("ShowDummyData", typeof(bool), typeof(EqualizerCanvas), new PropertyMetadata(false, (d, e) => ((EqualizerCanvas) d).SetDummyData()));

        #endregion

        #region Properties
         
        public EQStyle EQStyle
        {
            get { return (EQStyle)GetValue(EQStyleProperty); }
            set { SetValue(EQStyleProperty, value); }
        }

        public int LowRangeValue
        {
            get { return (int)GetValue(LowRangeValueProperty); }
            set { SetValue(LowRangeValueProperty, value); }
        }

        public int MedRangeValue
        {
            get { return (int)GetValue(MedRangeValueProperty); }
            set { SetValue(MedRangeValueProperty, value); }
        }

        public Brush LowRangeColor
        {
            get { return (Brush)GetValue(LowRangeColorProperty); }
            set { SetValue(LowRangeColorProperty, value); }
        }

        public Brush MedRangeColor
        {
            get { return (Brush)GetValue(MedRangeColorProperty); }
            set { SetValue(MedRangeColorProperty, value); }
        }

        public Brush MaxRangeColor
        {
            get { return (Brush)GetValue(MaxRangeColorProperty); }
            set { SetValue(MaxRangeColorProperty, value); }
        }

        public int BandCount
        {
            get { return (int)GetValue(BandCountProperty); }
            set { SetValue(BandCountProperty, value); }
        }

        public int BandSpacing
        {
            get { return (int)GetValue(BandSpacingProperty); }
            set { SetValue(BandSpacingProperty, value); }
        }

        public int EQChannel
        {
            get { return (int)GetValue(EQChannelProperty); }
            set { SetValue(EQChannelProperty, value); }
        }

        public int BandBorderSize
        {
            get { return (int)GetValue(BandBorderSizeProperty); }
            set { SetValue(BandBorderSizeProperty, value); }
        }

        public int BandCornerRadius
        {
            get { return (int)GetValue(BandCornerRadiusProperty); }
            set { SetValue(BandCornerRadiusProperty, value); }
        }

        public Brush BandBorderColor
        {
            get { return (Brush)GetValue(BandBorderColorProperty); }
            set { SetValue(BandBorderColorProperty, value); }
        }

        public Brush FallOffColor
        {
            get { return (Brush)GetValue(FallOffColorProperty); }
            set { SetValue(FallOffColorProperty, value); }
        }

        public int FalloffSpeed
        {
            get { return (int)GetValue(FalloffSpeedProperty); }
            set { SetValue(FalloffSpeedProperty, value); }
        }

        public int FallOffHeight
        {
            get { return (int)GetValue(FallOffHeightProperty); }
            set { SetValue(FallOffHeightProperty, value); }
        }

        public bool ShowDummyData
        {
            get { return (bool)GetValue(ShowDummyDataProperty); }
            set { SetValue(ShowDummyDataProperty, value); }
        }
     
        #endregion

        #region Methods

        /// <summary>
        /// Switches the style.
        /// </summary>
        public void SwitchStyle()
        {
            switch (EQStyle)
            {
                case EQStyle.SingleBar:
                    EQStyle = EQStyle.DoubleBar;
                    break;
                case EQStyle.DoubleBar:
                    EQStyle = EQStyle.SingleRectangle;
                    break;
                case EQStyle.SingleRectangle:
                    EQStyle = EQStyle.DoubleRectangle;
                    break;
                case EQStyle.DoubleRectangle:
                    EQStyle = EQStyle.SingleRoundedRectangle;
                    break;
                case EQStyle.SingleRoundedRectangle:
                    EQStyle = EQStyle.DoubleRoundedRectangle;
                    break;
                case EQStyle.DoubleRoundedRectangle:
                    EQStyle = EQStyle.SingleCircle;
                    break;
                case EQStyle.SingleCircle:
                    EQStyle = EQStyle.DoubleCircle;
                    break;
                case EQStyle.DoubleCircle:
                    EQStyle = EQStyle.SingleBar;
                    break;
            }
        }

        /// <summary>
        /// Sets the EQ data.
        /// </summary>
        /// <param name="arrayValue">The array value.</param>
        public void SetEQData(byte[] arrayValue)
        {
            if (_rendering) return;

            _rendering = true;
            lock (_fftData)
            {
                if (_ledCount > 0)
                {
                    for (var i = 0; i < BandCount; i++)            // array conatains interleaved values for 2 channels
                    {
                        if (i >= _fftData.Length || i >= arrayValue.Length/2) continue;
                        var pm = _fftData[i];
                        pm.Value = arrayValue[2*i+EQChannel]; // select data according to channel of this canvas
                        if (pm.Falloff < pm.Value)
                        {
                            pm.Falloff = pm.Value;
                            pm.Speed = FalloffSpeed;
                        }

                        var nDecValue = MaxRangeValue / _ledCount;
                        if (pm.Value > 0)
                        {
                            pm.Value -= (_ledCount > 1 ? nDecValue : (MaxRangeValue * 10) / 100);
                            if (pm.Value < 0)
                            {
                                pm.Value = 0;
                            }
                        }
                        if (pm.Speed > 0)
                        {
                            pm.Speed -= 1;
                        }
                        if (pm.Speed == 0 && pm.Falloff > 0)
                        {
                            pm.Falloff -= (_ledCount > 1 ? nDecValue >> 1 : 5);
                            if (pm.Falloff < 0)
                            {
                                pm.Falloff = 0;
                            }
                        }
                        _fftData[i] = pm;
                    }
                }
            }
            InvalidateVisual();
            _rendering = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SwitchStyle();
        }

        #endregion

        #region Render

        /// <summary>
        /// Draws the content of a <see cref="T:System.Windows.Media.DrawingContext"/> object during the render pass of a <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </summary>
        /// <param name="dc">The <see cref="T:System.Windows.Media.DrawingContext"/> object to draw.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            try
            {
                switch (EQStyle)
                {
                    case EQStyle.SingleRectangle:
                        DrawRectangleLeds(dc, false, false);
                        break;
                    case EQStyle.DoubleRectangle:
                        DrawRectangleLeds(dc, true, false);
                        break;
                    case EQStyle.SingleRoundedRectangle:
                        DrawRectangleLeds(dc, false, true);
                        break;
                    case EQStyle.DoubleRoundedRectangle:
                        DrawRectangleLeds(dc, true, true);
                        break;
                    case EQStyle.SingleCircle:
                        DrawCircleLeds(dc, false);
                        break;
                    case EQStyle.DoubleCircle:
                        DrawCircleLeds(dc, true);
                        break;
                    case EQStyle.SingleBar:
                        DrawBar(dc, false);
                        break;
                    case EQStyle.DoubleBar:
                        DrawBar(dc, true);
                        break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Draws the rectangle leds.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="isDouble">if set to <c>true</c> [is double].</param>
        /// <param name="isRounded">if set to <c>true</c> [is rounded].</param>
        private void DrawRectangleLeds(DrawingContext dc, bool isDouble, bool isRounded)
        {
            var ledWidth = ActualWidth / BandCount;
            var ledHeight = ledWidth / 2;
            var eqHeight = isDouble ? (ActualHeight / 2) - (ledHeight / 2) : (ActualHeight - ledHeight);
            double maxRange = isDouble ? MaxRangeValue / 2 : MaxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;
            var bandMinLimit = (eqHeight / maxRange) * minRange;
            var bandMedLimit = (eqHeight / maxRange) * medRange;

            _ledCount = (int)(ActualWidth / ledHeight);

            for (var bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                var bandX = bandNumber * ledWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                var vertValue = (eqHeight / maxRange) * dataValue;
                var falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);

                DrawRectangle(dc, FallOffColor, null, bandX, falloffValue, ledWidth, FallOffHeight, BandSpacing, 0);
                if (isDouble)
                {
                    DrawRectangle(dc, FallOffColor, null, bandX, ((eqHeight * 2) - falloffValue) + (ledHeight - FallOffHeight), ledWidth, FallOffHeight, BandSpacing, 0);
                }

                // Draw led's
                for (var ledY = 0; ledY < _ledCount; ledY++)
                {
                    var locationY = (ledHeight * ledY);
                    if (vertValue > locationY)
                    {
                        var color = GetRangeColor(locationY, bandMinLimit, bandMedLimit);

                        if (isRounded)
                        {
                            DrawRoundedRectangle(dc, color, _borderPen, bandX, eqHeight - locationY, ledWidth, ledHeight, BandSpacing, BandSpacing, BandCornerRadius);
                            if (isDouble)
                            {
                                DrawRoundedRectangle(dc, color, _borderPen, bandX, eqHeight + locationY, ledWidth, ledHeight, BandSpacing, BandSpacing, BandCornerRadius);
                            }
                        }
                        else
                        {
                            DrawRectangle(dc, color, _borderPen, bandX, eqHeight - locationY, ledWidth, ledHeight, BandSpacing, BandSpacing);
                            if (isDouble)
                            {
                                DrawRectangle(dc, color, _borderPen, bandX, eqHeight + locationY, ledWidth, ledHeight, BandSpacing, BandSpacing);
                            }
                        }
                        continue;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Draws the circle leds.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="isDouble">if set to <c>true</c> [is double].</param>
        private void DrawCircleLeds(DrawingContext dc, bool isDouble)
        {
            var ledWidth = (int)(ActualWidth / BandCount);
            var ledHeight = ledWidth;
            var eqHeight = isDouble ? (ActualHeight / 2) - ((double)ledHeight / 2) : (ActualHeight - ledHeight);
            double maxRange = isDouble ? MaxRangeValue / 2 : MaxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;
            var bandMinLimit = (eqHeight / maxRange) * minRange;
            var bandMedLimit = (eqHeight / maxRange) * medRange;

            _ledCount = (int)(ActualWidth / ledHeight);

            for (var bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                double bandX = bandNumber * ledWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                var vertValue = (eqHeight / maxRange) * dataValue;
                var falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);


                DrawArc(dc, SweepDirection.Clockwise, bandX, falloffValue + ((double)ledHeight / 2), ledWidth, BandSpacing);
                if (isDouble)
                {
                    DrawArc(dc, SweepDirection.Counterclockwise, bandX, ((eqHeight * 2) - falloffValue) + ((double)ledHeight / 2), ledWidth, BandSpacing);
                }

                // Draw led's
                for (var ledY = 0; ledY < _ledCount; ledY++)
                {
                    double locationY = (ledHeight * ledY);
                    if (vertValue > locationY)
                    {

                        var color = GetRangeColor(locationY, bandMinLimit, bandMedLimit);
                        DrawElipse(dc, color, _borderPen, bandX, eqHeight - locationY, ledWidth, BandSpacing);
                        if (isDouble)
                        {
                            DrawElipse(dc, color, _borderPen, bandX, eqHeight + locationY, ledWidth, BandSpacing);
                        }
                        continue;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Draws the bar.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="isDouble">if set to <c>true</c> [is double].</param>
        private void DrawBar(DrawingContext dc, bool isDouble)
        {
            var eqHeight = isDouble ? (ActualHeight / 2) : ActualHeight;
            double maxRange = isDouble ? MaxRangeValue / 2 : MaxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;

            var bandMinLimit = (eqHeight / maxRange) * minRange;
            var bandMedLimit = (eqHeight / maxRange) * medRange;
            var barWidth = ActualWidth / BandCount;
            _ledCount = (int)(ActualWidth / BandCount);

            for (var bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                var bandX = bandNumber * barWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                var vertValue = (eqHeight / maxRange) * dataValue;
                var falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);
                var loVal = Math.Max(eqHeight - bandMinLimit, eqHeight - vertValue);
                var medVal = Math.Max(eqHeight - bandMedLimit, eqHeight - vertValue);
                var hghVal = Math.Max(0, eqHeight - vertValue);

                DrawRectangle(dc, FallOffColor, null, bandX, falloffValue, barWidth, FallOffHeight, BandSpacing, 0);
                if (isDouble)
                {
                    DrawRectangle(dc, FallOffColor, null, bandX, (eqHeight * 2) - falloffValue, barWidth, FallOffHeight, BandSpacing, 0);
                }

                if (hghVal < (eqHeight - bandMedLimit))
                {
                    DrawRectangle(dc, MaxRangeColor, null, bandX, hghVal, barWidth, (eqHeight - bandMedLimit) - hghVal, BandSpacing, 0);
                    if (isDouble)
                    {
                        DrawRectangle(dc, MaxRangeColor, null, bandX, eqHeight + bandMedLimit, barWidth, (eqHeight - bandMedLimit) - hghVal, BandSpacing, 0);
                    }
                }
                if (medVal < (eqHeight - bandMinLimit))
                {
                    DrawRectangle(dc, MedRangeColor, null, bandX, medVal, barWidth, (eqHeight - bandMinLimit) - medVal, BandSpacing, 0);
                    if (isDouble)
                    {
                        DrawRectangle(dc, MedRangeColor, null, bandX, eqHeight + bandMinLimit, barWidth, (eqHeight - bandMinLimit) - medVal, BandSpacing, 0);
                    }
                }
                if (loVal < eqHeight)
                {
                    DrawRectangle(dc, LowRangeColor, null, bandX, loVal, barWidth, eqHeight - loVal, BandSpacing, 0);
                    if (isDouble)
                    {
                        DrawRectangle(dc, LowRangeColor, null, bandX, eqHeight, barWidth, eqHeight - loVal, BandSpacing, 0);
                    }
                }

                DrawRectangle(dc, null, _borderPen, bandX, hghVal, barWidth, eqHeight - hghVal, BandSpacing, 0);
                if (isDouble)
                {
                    DrawRectangle(dc, null, _borderPen, bandX, eqHeight, barWidth, eqHeight - hghVal, BandSpacing, 0);
                }
            }
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="color">The color.</param>
        /// <param name="border">The border.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="spaceX">The space X.</param>
        /// <param name="spaceY">The space Y.</param>
        private static void DrawRectangle(DrawingContext dc, Brush color, Pen border, double x, double y, double width, double height, int spaceX, int spaceY)
        {
            var rect = new Rect(x, y, width, height);
            rect.Inflate(-spaceX, -spaceY);
            dc.DrawRectangle(color, border, rect);
        }

        /// <summary>
        /// Draws the rounded rectangle.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="color">The color.</param>
        /// <param name="border">The border.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="spaceX">The space X.</param>
        /// <param name="spaceY">The space Y.</param>
        /// <param name="radius">The radius.</param>
        private static void DrawRoundedRectangle(DrawingContext dc, Brush color, Pen border, double x, double y, double width, double height, int spaceX, int spaceY, int radius)
        {
            var rect = new Rect(x, y, width, height);
            rect.Inflate(-spaceX, -spaceY);
            dc.DrawRoundedRectangle(color, border, rect, radius, radius);
        }

        /// <summary>
        /// Draws the elipse.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="color">The color.</param>
        /// <param name="border">The border.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <param name="space">The space.</param>
        private static void DrawElipse(DrawingContext dc, Brush color, Pen border, double x, double y, double size, int space)
        {
            var rect = new Rect(x, y, size, size);
            rect.Inflate(-space, -space);
            var radius = rect.Width / 2;
            dc.DrawEllipse(color, border, new Point(rect.X + radius, rect.Y + radius), radius, radius);
        }

        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <param name="space">The space.</param>
        private void DrawArc(DrawingContext drawingContext, SweepDirection direction, double x, double y, double size, int space)
        {
            var rect = new Rect(x, y, size, size);
            rect.Inflate(-space, -space);
            // setup the geometry object
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = new Point(rect.X, rect.Y);

            // add the arc to the geometry
            figure.Segments.Add(new ArcSegment(new Point(rect.X + rect.Width, rect.Y), new Size(rect.Width / 2, rect.Width / 2), 0, false, direction, true));

            // draw the arc
            drawingContext.DrawGeometry(null, _falloffPen, geometry);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the color of the range.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="minLimit">The min limit.</param>
        /// <param name="medLimit">The med limit.</param>
        /// <returns></returns>
        private Brush GetRangeColor(double location, double minLimit, double medLimit)
        {
            if (location <= minLimit)
            {
                return LowRangeColor;
            }
            return location <= medLimit ? MedRangeColor : MaxRangeColor;
        }

        /// <summary>
        /// Sets the border pen.
        /// </summary>
        private void SetBorderPen()
        {
            _borderPen = new Pen(BandBorderColor, BandBorderSize);
        }

        /// <summary>
        /// Sets the falloff pen.
        /// </summary>
        private void SetFalloffPen()
        {
            _falloffPen = new Pen(FallOffColor, FallOffHeight);
        }

        /// <summary>
        /// Gets the dummy EQ data.
        /// </summary>
        /// <returns></returns>
        public void ShowDummyEQData()
        {
            var meters1 = new byte[BandCount];
            var rand = new Random();
            for (var i = 0; i < meters1.Length; i++)
            {
                var t = meters1.Length / 6;
                if (i < t)
                {
                    meters1[i] = (byte)rand.Next(30, 100);
                }
                else if (i >= t && i < t * 2)
                {
                    meters1[i] = (byte)rand.Next(100, 160);
                }
                else if (i >= t && i < t * 3)
                {
                    meters1[i] = (byte)rand.Next(160, 255);
                }
                else if (i >= t && i < t * 4)
                {
                    meters1[i] = (byte)rand.Next(80, 120);
                }
                else if (i >= t && i < t * 5)
                {
                    meters1[i] = (byte)rand.Next(30, 60);
                }
                else
                {
                    meters1[i] = (byte)rand.Next(0, 30);
                }
            }
            SetEQData(meters1);
        }

        private DispatcherTimer _dummyDataTimer;

        private void SetDummyData()
        {
            if (ShowDummyData == false)
            {
                if (_dummyDataTimer == null) return;

                _dummyDataTimer.Stop();
                _dummyDataTimer = null;
            }
            else
            {
                if (_dummyDataTimer != null) return;

                _dummyDataTimer = new DispatcherTimer(DispatcherPriority.Background)
                {
                    Interval = TimeSpan.FromMilliseconds(60)
                };
                _dummyDataTimer.Tick += (s, e) => ShowDummyEQData();
                _dummyDataTimer.Start();
            }
        }

        #endregion
    }

  
}
