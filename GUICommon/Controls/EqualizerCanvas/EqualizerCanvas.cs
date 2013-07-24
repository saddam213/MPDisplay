using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        private bool _rendering = false;
        private EQData[] _fftData = new EQData[100];

        const int _maxRangeValue = 255;
        private Pen _borderPen = null;
        private Pen _falloffPen = null;
        private int _ledCount = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIEqualizerSurface"/> class.
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
        public static readonly DependencyProperty BandBorderSizeProperty = DependencyProperty.Register("BandBorderSize", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(1, (d, e) => (d as EqualizerCanvas).SetBorderPen()));
        public static readonly DependencyProperty BandCornerRadiusProperty = DependencyProperty.Register("BandCornerRadius", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(2));
        public static readonly DependencyProperty BandBorderColorProperty = DependencyProperty.Register("BandBorderColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Silver, (d, e) => (d as EqualizerCanvas).SetBorderPen()));
        public static readonly DependencyProperty FallOffColorProperty = DependencyProperty.Register("FallOffColor", typeof(Brush), typeof(EqualizerCanvas), new PropertyMetadata(Brushes.Gray, (d, e) => (d as EqualizerCanvas).SetFalloffPen()));
        public static readonly DependencyProperty FalloffSpeedProperty = DependencyProperty.Register("FalloffSpeed", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(5));
        public static readonly DependencyProperty FallOffHeightProperty = DependencyProperty.Register("FallOffHeight", typeof(int), typeof(EqualizerCanvas), new PropertyMetadata(4, (d, e) => (d as EqualizerCanvas).SetFalloffPen()));
        public static readonly DependencyProperty ShowDummyDataProperty =  DependencyProperty.Register("ShowDummyData", typeof(bool), typeof(EqualizerCanvas), new PropertyMetadata(false, (d, e) => (d as EqualizerCanvas).SetDummyData()));


      
        

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
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the EQ data.
        /// </summary>
        /// <param name="arrayValue">The array value.</param>
        public void SetEQData(byte[] arrayValue)
        {
            if (!_rendering)
            {
                _rendering = true;
                lock (_fftData)
                {
                    if (_ledCount > 0)
                    {
                        for (int i = 0; i < BandCount; i++)
                        {
                            if (i < this._fftData.Length)
                            {
                                EQData pm = this._fftData[i];
                                pm.Value = (int)arrayValue[i];
                                if (pm.Falloff < pm.Value)
                                {
                                    pm.Falloff = pm.Value;
                                    pm.Speed = FalloffSpeed;
                                }

                                int nDecValue = _maxRangeValue / _ledCount;
                                if (pm.Value > 0)
                                {
                                    pm.Value -= (_ledCount > 1 ? nDecValue : (_maxRangeValue * 10) / 100);
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
                                this._fftData[i] = pm;
                            }
                        }
                    }
                }
                InvalidateVisual();
                _rendering = false;
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
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
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
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
                    default:
                        break;
                }
            }
            catch (Exception)
            {
              
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
            double ledWidth = ActualWidth / BandCount;
            double ledHeight = ledWidth / 2;
            double eqHeight = isDouble ? (ActualHeight / 2) - (ledHeight / 2) : (ActualHeight - ledHeight);
            double maxRange = isDouble ? _maxRangeValue / 2 : _maxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;
            double bandMinLimit = (eqHeight / maxRange) * minRange;
            double bandMedLimit = (eqHeight / maxRange) * medRange;

            _ledCount = (int)(ActualWidth / ledHeight);

            for (int bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                double bandX = bandNumber * ledWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                double vertValue = (eqHeight / maxRange) * dataValue;
                double falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);

                DrawRectangle(dc, FallOffColor, null, bandX, falloffValue, ledWidth, FallOffHeight, BandSpacing, 0);
                if (isDouble)
                {
                    DrawRectangle(dc, FallOffColor, null, bandX, ((eqHeight * 2) - falloffValue) + (ledHeight - FallOffHeight), ledWidth, FallOffHeight, BandSpacing, 0);
                }

                // Draw led's
                for (int led_Y = 0; led_Y < _ledCount; led_Y++)
                {
                    double locationY = (ledHeight * led_Y);
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
            int ledWidth = (int)(ActualWidth / BandCount);
            int ledHeight = ledWidth;
            double eqHeight = isDouble ? (ActualHeight / 2) - (ledHeight / 2) : (ActualHeight - ledHeight);
            double maxRange = isDouble ? _maxRangeValue / 2 : _maxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;
            double bandMinLimit = (eqHeight / maxRange) * minRange;
            double bandMedLimit = (eqHeight / maxRange) * medRange;

            _ledCount = (int)(ActualWidth / ledHeight);

            for (int bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                double bandX = bandNumber * ledWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                double vertValue = (eqHeight / maxRange) * dataValue;
                double falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);
                double offset = isDouble ? (ledHeight / 2) : ledHeight;


                DrawArc(dc, FallOffColor, SweepDirection.Clockwise, bandX, falloffValue + (ledHeight / 2), ledWidth, BandSpacing);
                if (isDouble)
                {
                    DrawArc(dc, FallOffColor, SweepDirection.Counterclockwise, bandX, ((eqHeight * 2) - falloffValue) + (ledHeight / 2), ledWidth, BandSpacing);
                }

                // Draw led's
                for (int led_Y = 0; led_Y < _ledCount; led_Y++)
                {
                    double locationY = (ledHeight * led_Y);
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
            double eqHeight = isDouble ? (ActualHeight / 2) : ActualHeight;
            double maxRange = isDouble ? _maxRangeValue / 2 : _maxRangeValue;
            double minRange = isDouble ? LowRangeValue / 2 : LowRangeValue;
            double medRange = isDouble ? MedRangeValue / 2 : MedRangeValue;

            double bandMinLimit = (eqHeight / maxRange) * minRange;
            double bandMedLimit = (eqHeight / maxRange) * medRange;
            double barWidth = ActualWidth / BandCount;
            _ledCount = (int)(ActualWidth / BandCount);

            for (int bandNumber = 0; bandNumber < BandCount; bandNumber++)
            {
                double bandX = bandNumber * barWidth;
                double dataValue = isDouble ? _fftData[bandNumber].Value / 2 : _fftData[bandNumber].Value;
                double falloffdataValue = isDouble ? _fftData[bandNumber].Falloff / 2 : _fftData[bandNumber].Falloff;
                double vertValue = (eqHeight / maxRange) * dataValue;
                double falloffValue = eqHeight - ((eqHeight / maxRange) * falloffdataValue);
                double loVal = Math.Max(eqHeight - bandMinLimit, eqHeight - vertValue);
                double medVal = Math.Max(eqHeight - bandMedLimit, eqHeight - vertValue);
                double hghVal = Math.Max(0, eqHeight - vertValue);

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
        private void DrawRectangle(DrawingContext dc, Brush color, Pen border, double x, double y, double width, double height, int spaceX, int spaceY)
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
        private void DrawRoundedRectangle(DrawingContext dc, Brush color, Pen border, double x, double y, double width, double height, int spaceX, int spaceY, int radius)
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
        private void DrawElipse(DrawingContext dc, Brush color, Pen border, double x, double y, double size, int space)
        {
            var rect = new Rect(x, y, size, size);
            rect.Inflate(-space, -space);
            double radius = rect.Width / 2;
            dc.DrawEllipse(color, border, new Point(rect.X + radius, rect.Y + radius), radius, radius);
        }

        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <param name="space">The space.</param>
        private void DrawArc(DrawingContext drawingContext, Brush brush, SweepDirection direction, double x, double y, double size, int space)
        {
            var rect = new Rect(x, y, size, size);
            rect.Inflate(-space, -space);
            // setup the geometry object
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
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
        /// Check If Value is within range
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="rangeMin">The minimum range</param>
        /// <param name="rangeMax">The Max range</param>
        /// <returns>If the value is beteen minimum and maximum</returns>
        private bool InRange(double value, double rangeMin, double rangeMax)
        {
            return (value >= rangeMin && value <= rangeMax);
        }

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
            else if (location <= medLimit)
            {
                return MedRangeColor;
            }
            return MaxRangeColor;
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
            byte[] meters1 = new byte[BandCount];
            Random rand = new Random();
            for (int i = 0; i < meters1.Length; i++)
            {
                int t = meters1.Length / 6;
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
                if (_dummyDataTimer != null)
                {
                    _dummyDataTimer.Stop();
                    _dummyDataTimer = null;
                }
            }
            else
            {
                if (_dummyDataTimer == null)
                {
                    _dummyDataTimer = new DispatcherTimer(DispatcherPriority.Background);
                    _dummyDataTimer.Interval = TimeSpan.FromMilliseconds(60);
                    _dummyDataTimer.Tick += (s, e) => ShowDummyEQData();
                    _dummyDataTimer.Start();
                }
            }
        }

        #endregion
    }

  
}
