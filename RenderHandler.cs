using System.Drawing;
using System.Windows.Forms;
//using Microsoft.VisualBasic;
using System.Collections;
using System;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
// '======================================
// RENDER HANDLER
// COPYRIGHT (C) 2015-2022 J.J.HIRNIAK
// '======================================
namespace Handlers {
    public class RenderHandler {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        public static int DPI() {
            int DPI_INT = 96;
            using (var bmp = new Bitmap(10, 10)) {
                var g = Graphics.FromImage(bmp);
                DPI_INT = Convert.ToInt32(g.DpiX);
            }
            return DPI_INT;
        }
        public static int REN_GetStandardSize(int Multiplier = 1) {
            var bmp = new Bitmap(10, 10);
            var g = Graphics.FromImage(bmp);
            int SZ_INT = 10;
            try {
                SZ_INT = Convert.ToInt32(g.MeasureString("W", new Font("Courier", 9)).Height * Multiplier);
            }
            catch {
            }
            g.Dispose();
            return SZ_INT;
        }
        public static int REN_GetTextSize(string Text, Font Fnt, bool UseUnit = true) {
            int SZ_INT = 0;
            using (var bmp = new Bitmap(10, 10)) {
                var g = Graphics.FromImage(bmp);

                try {
                    if (UseUnit == true)
                        SZ_INT = Convert.ToInt32(g.MeasureString("W", Fnt).Width);
                    else
                        SZ_INT = Convert.ToInt32(g.MeasureString(Text, Fnt).Width);
                }
                catch {
                }
                g.Dispose();
            }
            return SZ_INT;
        }
        public static Color AlphaChannelInsert(Color Input, int Alpha) {
            return Color.FromArgb(Alpha, Input.R, Input.G, Input.B);
        }
        public static Color DeterministicDarkenColor(Color Input, Color BackColor, int Alpha) {
            if (IsDark(BackColor) == true) {
                decimal AlphaReduce = Convert.ToDecimal((255 - Alpha) / (double)255);
                int AR = Convert.ToInt32(Math.Floor(Input.R * AlphaReduce));
                int AG = Convert.ToInt32(Math.Floor(Input.G * AlphaReduce));
                int AB = Convert.ToInt32(Math.Floor(Input.B * AlphaReduce));
                return Color.FromArgb(AR, AG, AB);
            }
            else {
                int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
                int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
                int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
                return Color.FromArgb(AR, AG, AB);
            }
        }
        public static Color DeterministicDarkenColorInverted(Color Input, Color BackColor, int Alpha) {
            if (IsDark(BackColor) == false) {
                decimal AlphaReduce = Convert.ToDecimal((255 - Alpha) / (double)255);
                int AR = Convert.ToInt32(Math.Floor(Input.R * AlphaReduce));
                int AG = Convert.ToInt32(Math.Floor(Input.G * AlphaReduce));
                int AB = Convert.ToInt32(Math.Floor(Input.B * AlphaReduce));
                return Color.FromArgb(AR, AG, AB);
            }
            else {
                int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
                int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
                int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
                return Color.FromArgb(AR, AG, AB);
            }
        }
        public static Color BlackDarkenColor(Color Input, int Alpha) {
            decimal AlphaReduce = Convert.ToDecimal((255 - Alpha) / (double)255);
            int AR = Convert.ToInt32(Math.Floor(Input.R * AlphaReduce));
            int AG = Convert.ToInt32(Math.Floor(Input.G * AlphaReduce));
            int AB = Convert.ToInt32(Math.Floor(Input.B * AlphaReduce));
            return Color.FromArgb(AR, AG, AB);
        }
        public static Color NormaliseDarkenColor(Color Input, int Alpha) {
            decimal AlphaReduce = Convert.ToDecimal(Alpha / (double)255);
            int AR = Convert.ToInt32(Math.Floor(Input.R * AlphaReduce));
            int AG = Convert.ToInt32(Math.Floor(Input.G * AlphaReduce));
            int AB = Convert.ToInt32(Math.Floor(Input.B * AlphaReduce));
            return Color.FromArgb(AR, AG, AB);
        }
        public static Color WhiteLightenColor(Color Input, int Alpha) {
            int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
            int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
            int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
            return Color.FromArgb(AR, AG, AB);
        }
        public static bool IsDark(Color InputColor) {
            // If InputColor.R <= 128 AndAlso InputColor.G <= 128 AndAlso InputColor.B <= 128 Then      'R(L),G(L),B(L)
            // Return True
            // ElseIf InputColor.R > 128 AndAlso InputColor.G <= 128 AndAlso InputColor.B <= 128 Then   'R(H),G(L),B(L)
            // Return True
            // ElseIf InputColor.R <= 128 AndAlso InputColor.G > 128 AndAlso InputColor.B <= 128 Then   'R(L),G(H),B(L)
            // Return True
            // ElseIf InputColor.R > 128 AndAlso InputColor.G > 128 AndAlso InputColor.B <= 128 Then    'R(H),G(H),B(L)
            // Return False
            // ElseIf InputColor.R <= 128 AndAlso InputColor.G <= 128 AndAlso InputColor.B > 128 Then   'R(L),G(L),B(H)
            // Return True
            // ElseIf InputColor.R > 128 AndAlso InputColor.G <= 128 AndAlso InputColor.B > 128 Then    'R(H),G(L),B(H)
            // Return False
            // ElseIf InputColor.R <= 128 AndAlso InputColor.G > 128 AndAlso InputColor.B > 128 Then    'R(L),G(H),B(H)
            // Return False
            // ElseIf InputColor.R > 128 AndAlso InputColor.G > 128 AndAlso InputColor.B > 128 Then     'R(H),G(H),B(H)
            // Return False
            // End If
            // Dim IsDarka As Boolean = (InputColor.R <= 128) Or _
            // (InputColor.G <= 128) Or _
            // (InputColor.B <= 128)
            // Return IsDarka
            if (ConvertRGBtoHSV(InputColor.R, InputColor.G, InputColor.B).B >= 50)
                return false;
            else
                return true;
        }
        public static bool IsDarkTheshold(Color InputColor, int Theshold) {
            var HSVA = ConvertRGBtoHSV(InputColor.R, InputColor.G, InputColor.B);
            if (ConvertRGBtoHSV(InputColor.R, InputColor.G, InputColor.B).B >= Theshold) {
                if (HSVA.H >= 0 && HSVA.H <= 20 || HSVA.H > 192 && HSVA.H <= 360) {
                    if (HSVA.S == 0)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return true;
        }
        public static Image InvertImageColors(Image Input, bool ColorCorrect = false, int Hue = 0) {
            Bitmap pic = new Bitmap(Input);
            for (int y = 0; (y <= (pic.Height - 1)); y++) {
                for (int x = 0; (x <= (pic.Width - 1)); x++) {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(inv.A, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    if (ColorCorrect == true) {
                        HSV Col = new HSV(inv);
                        Col.H = (Col.H + Hue) % 360;
                        inv = Col.ToColor();
                    }
                    pic.SetPixel(x, y, inv);
                }
            }
            return pic;
        }
        public static Image ChangeImageHUE(Image Input, int Hue) {
            try {
                int c_h = 0;
                int c_s = 0;
                int c_b = 0;
                if (Hue >= -180 && Hue <= 180)
                    c_h = Hue;
                else
                    c_h = 0;
                if (Input == null)
                    return null;
                Bitmap b = null;
                using (var imageAttr = new ImageAttributes()) {
                    var qm = new Imgx.QColorMatrix1();
                    qm.RotateHue(c_h);
                    imageAttr.SetColorMatrix(qm.ToColorMatrix());
                    b = new Bitmap(Input.Width, Input.Height);
                    using (var g = Graphics.FromImage(b)) {
                        var r = new Rectangle(0, 0, Input.Width, Input.Height);
                        g.DrawImage(Input, r, 0, 0, Input.Width, Input.Height, GraphicsUnit.Pixel, imageAttr);
                    }
                }
                if (b != null)
                    return b;
            }
            catch {
                return null;
            }
            return null;
        }
        public static Color HeatMapping(Color ColourLow, Color ColourMiddle, Color ColourHigh, ushort Postion) {
            if (Postion >= 256) {
                ushort POS_REL = Convert.ToUInt16(Postion - 256);
                int ELE_R = Convert.ToInt32((Convert.ToInt32(ColourHigh.R) - Convert.ToInt32(ColourMiddle.R)) / (double)255 * POS_REL + Convert.ToInt32(ColourMiddle.R));
                int ELE_G = Convert.ToInt32((Convert.ToInt32(ColourHigh.G) - Convert.ToInt32(ColourMiddle.G)) / (double)255 * POS_REL + Convert.ToInt32(ColourMiddle.G));
                int ELE_B = Convert.ToInt32((Convert.ToInt32(ColourHigh.B) - Convert.ToInt32(ColourMiddle.B)) / (double)255 * POS_REL + Convert.ToInt32(ColourMiddle.B));
                return Color.FromArgb(ELE_R, ELE_G, ELE_B);
            }
            else {
                int ELE_R = Convert.ToInt32((Convert.ToInt32(ColourMiddle.R) - Convert.ToInt32(ColourLow.R)) / (double)255 * Postion + Convert.ToInt32(ColourLow.R));
                int ELE_G = Convert.ToInt32((Convert.ToInt32(ColourMiddle.G) - Convert.ToInt32(ColourLow.G)) / (double)255 * Postion + Convert.ToInt32(ColourLow.G));
                int ELE_B = Convert.ToInt32((Convert.ToInt32(ColourMiddle.B) - Convert.ToInt32(ColourLow.B)) / (double)255 * Postion + Convert.ToInt32(ColourLow.B));
                return Color.FromArgb(ELE_R, ELE_G, ELE_B);
            }
        }
        public static Image ScaleImage(Image image, Size size, bool preserveAspectRatio = true) {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio) {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = Convert.ToSingle(size.Width) / Convert.ToSingle(originalWidth);
                float percentHeight = Convert.ToSingle(size.Height) / Convert.ToSingle(originalHeight);
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = Convert.ToInt32(originalWidth * percent);
                newHeight = Convert.ToInt32(originalHeight * percent);
            }
            else {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (var graphicsHandle = Graphics.FromImage(newImage)) {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        public static Image GrayScale(Image image) {
            try {
                Bitmap bitmap;
                bitmap = new Bitmap(image);
                int x, y;
                byte d;
                var loopTo = image.Width - 1;
                for (x = 0; x <= loopTo; x++) {
                    var loopTo1 = image.Height - 1;
                    for (y = 0; y <= loopTo1; y++) {
                        d = Convert.ToByte(Math.Round(bitmap.GetPixel(x, y).A * 0.299 + bitmap.GetPixel(x, y).R * 0.299 + bitmap.GetPixel(x, y).G * 0.587 + bitmap.GetPixel(x, y).B * 0.114));
                        bitmap.SetPixel(x, y, Color.FromArgb(d, d, d, d));
                    }
                }
                return bitmap;
            }
            catch {
                return null;
            }
        }
        public static Image ColorCorrection(Bitmap image, int Hue, int Saturation, int Brightness) {
            try {
                int c_h = 0;
                int c_s = 0;
                int c_b = 0;
                if (Hue >= -180 && Hue <= 180)
                    c_h = Hue;
                else
                    c_h = 0;
                if (Saturation >= -100 && Saturation <= 300)
                    c_s = Saturation;
                else
                    c_s = 0;
                if (Brightness >= -100 && Brightness <= 100)
                    c_b = Brightness;
                else
                    c_b = 0;
                if (image == null)
                    return null;
                Bitmap b = null;
                using (var imageAttr = new ImageAttributes()) {
                    var qm = new Imgx.QColorMatrix1();
                    qm.RotateHue(c_h);
                    qm.SetSaturation2(c_s / 100.0F);
                    qm.SetBrightness(c_b / 100.0F);
                    imageAttr.SetColorMatrix(qm.ToColorMatrix());
                    b = new Bitmap(image.Width, image.Height);
                    using (var g = Graphics.FromImage(b)) {
                        var r = new Rectangle(0, 0, image.Width, image.Height);
                        g.DrawImage(image, r, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
                    }
                }
                if (b != null)
                    return b;
            }
            catch {
                return null;
            }
            return null;
        }
        public static Image ShearImage(Bitmap image, float red, float green, float blue, ShearEnums Shear_Channel) {
            try {
                if (image == null)
                    return null;
                Bitmap b = null;
                using (var imageAttr = new ImageAttributes()) {
                    var qm = new Imgx.QColorMatrix1();
                    if (Shear_Channel == (int)ShearEnums.SHEAR_RED)
                        qm.ShearRed(green, blue);
                    else if ((int)Shear_Channel == (int)ShearEnums.SHEAR_GREEN)
                        qm.ShearGreen(red, blue);
                    else if ((int)Shear_Channel == (int)ShearEnums.SHEAR_BLUE)
                        qm.ShearBlue(red, green);
                    else if ((int)Shear_Channel == (int)ShearEnums.SHEAR_ALL) {
                        qm.ShearRed(green, blue);
                        qm.ShearGreen(red, blue);
                        qm.ShearBlue(red, green);
                    }
                    b = new Bitmap(image.Width, image.Height);
                    using (var g = Graphics.FromImage(b)) {
                        var r = new Rectangle(0, 0, image.Width, image.Height);
                        g.DrawImage(image, r, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
                    }
                }
                if (b != null)
                    return b;
            }
            catch {
                return null;
            }
            return null;
        }
        public static HSV ConvertRGBtoHSV(int R, int G, int B) {
            // '# Normalize the RGB values by scaling them to be between 0 and 1
            decimal red = Convert.ToDecimal(R / (double)255);
            decimal green = Convert.ToDecimal(G / (double)255);
            decimal blue = Convert.ToDecimal(B / (double)255);
            decimal minValue = Math.Min(red, Math.Min(green, blue));
            decimal maxValue = Math.Max(red, Math.Max(green, blue));
            decimal delta = maxValue - minValue;
            var h = default(decimal);
            decimal s;
            decimal v = maxValue;
            // '# Calculate the hue (in degrees of a circle, between 0 and 360)
            switch (maxValue) {
                case var @case when @case == red: {
                        if (green >= blue) {
                            if (delta == 0)
                                h = 0;
                            else
                                h = 60 * (green - blue) / delta;
                        }
                        else if (green < blue)
                            h = 60 * (green - blue) / delta + 360;
                        break;
                    }

                case var case1 when case1 == green: {
                        h = 60 * (blue - red) / delta + 120;
                        break;
                    }

                case var case2 when case2 == blue: {
                        h = 60 * (red - green) / delta + 240;
                        break;
                    }
            }
            // '# Calculate the saturation (between 0 and 1)
            if (maxValue == 0)
                s = 0;
            else
                s = 1M - minValue / maxValue;
            // '# Scale the saturation and value to a percentage between 0 and 100
            s *= 100;
            v *= 100;
            // '# Return a color in the new color space
            var HSV_VAL = new HSV();
            {
                var withBlock = HSV_VAL;
                withBlock.H = Convert.ToSingle(h);
                withBlock.S = Convert.ToSingle(s);
                withBlock.B = Convert.ToSingle(v);
            }
            return HSV_VAL;
        }
        public static Color GetPixelColor(int x, int y) {
            var hdc = GetWindowDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            Color color;
            ReleaseDC(IntPtr.Zero, hdc);
            // MsgBox(pixel)
            color = Color.FromArgb(Convert.ToInt32(Convert.ToInt32(pixel & (long)0xFF)), Convert.ToInt32(Convert.ToInt32(pixel & (long)0xFF00) >> 8), Convert.ToInt32(Convert.ToInt32(pixel & (long)0xFF0000) >> 16));
            return color;
        }
        public static Color ConverttHSVtoRGB(int a, double h, double s, double b) {
            if (0 > a | 255 < a)
                throw new ArgumentOutOfRangeException("a", a, "Invalid Alpha Value");
            if (0.0F > h | 360.0F < h)
                throw new ArgumentOutOfRangeException("h", h, "Invalid Hue Value");
            if (0.0F > s | 1.0F < s)
                throw new ArgumentOutOfRangeException("s", s, "Invalid Saturation Value");
            if (0.0F > b | 1.0F < b)
                throw new ArgumentOutOfRangeException("b", b, "Invalid Brightness Value");
            if (0 == s)
                return Color.FromArgb(a, int.Parse(Convert.ToString(b * 255)), int.Parse(Convert.ToString(b * 255)), int.Parse(Convert.ToString(b * 255)));
            double fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;
            if (0.5 < b) {
                fMax = b - b * s + s;
                fMin = b + b * s - s;
            }
            else {
                fMax = b + b * s;
                fMin = b - b * s;
            }
            iSextant = int.Parse(Convert.ToString(Math.Floor(h / 60.0F)));
            if (300.0F <= h)
                h -= 360.0F;
            h /= 60.0F;
            h -= 2.0F * double.Parse(Convert.ToString(Math.Floor((iSextant + 1.0F) % 6.0F / 2.0F)));
            if (0 == iSextant % 2)
                fMid = h * (fMax - fMin) + fMin;
            else
                fMid = fMin - h * (fMax - fMin);
            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);
            switch (iSextant) {
                case 1: {
                        return Color.FromArgb(a, iMid, iMax, iMin);
                    }

                case 2: {
                        return Color.FromArgb(a, iMin, iMax, iMid);
                    }

                case 3: {
                        return Color.FromArgb(a, iMin, iMid, iMax);
                    }

                case 4: {
                        return Color.FromArgb(a, iMid, iMin, iMax);
                    }

                case 5: {
                        return Color.FromArgb(a, iMax, iMin, iMid);
                    }

                default: {
                        return Color.FromArgb(a, iMax, iMid, iMin);
                    }
            }
        }
        public static Image GausianBlurImage(Image image, bool alphaEdgesOnly, Size blurSize) {
            int PixelY;
            int PixelX;
            var bmp = (Bitmap)image;
            var loopTo = bmp.Width - 1;
            for (PixelY = 0; PixelY <= loopTo; PixelY++) {
                var loopTo1 = bmp.Height - 1;
                for (PixelX = 0; PixelX <= loopTo1; PixelX++) {
                    if (!alphaEdgesOnly)
                        bmp.SetPixel(PixelX, PixelY, AverageImageColor(image, blurSize, bmp.PhysicalDimension, PixelX, PixelY));
                    else if (bmp.GetPixel(PixelX, PixelY).A
                                            != 255)
                        bmp.SetPixel(PixelX, PixelY, AverageImageColor(image, blurSize, bmp.PhysicalDimension, PixelX, PixelY));
                    Application.DoEvents();
                }
            }
            return (Image)bmp.Clone();
            // bmp.Dispose();
        }
        private static Color AverageImageColor(Image image, Size Size, SizeF imageSize, int PixelX, int Pixely) {
            var pixels = new ArrayList();
            int x;
            int y;
            var bmp = (Bitmap)image;
            var loopTo = PixelX + Convert.ToInt32(Size.Width / (double)2);
            for (x = PixelX - Convert.ToInt32(Size.Width / (double)2); x <= loopTo; x++) {
                var loopTo1 = Pixely + Convert.ToInt32(Size.Height / (double)2);
                for (y = Pixely - Convert.ToInt32(Size.Height / (double)2); y <= loopTo1; y++) {
                    if (x > 0 & x < imageSize.Width & y > 0 & y < imageSize.Height)
                        pixels.Add(bmp.GetPixel(x, y));
                }
            }
            //Color thisColor = Color.Black;
            int alpha = 0;
            int red = 0;
            int green = 0;
            int blue = 0;
            foreach (Color thisColor in pixels) {
                alpha += (int)thisColor.A;
                red += (int)thisColor.R;
                green += (int)thisColor.G;
                blue += (int)thisColor.B;
            }
            return Color.FromArgb(Convert.ToInt32(alpha / (double)pixels.Count), Convert.ToInt32(red / (double)pixels.Count), Convert.ToInt32(green / (double)pixels.Count), Convert.ToInt32(blue / (double)pixels.Count));
        }
        public static Image DrawText(string Text, Font Font, Color ForeColor, Size FrameSize, Point Position) {
            var bmp = new Bitmap(FrameSize.Width, FrameSize.Height);
            var g = Graphics.FromImage(bmp);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            using (Brush br = new SolidBrush(ForeColor)) {
                var stringsize = new SizeF(g.MeasureString(Text, Font));
                var centrepos = new Point(Convert.ToInt32(Position.X - stringsize.Width / 2), Convert.ToInt32(Position.Y - stringsize.Height / 2));
                g.DrawString(Text, Font, br, centrepos);
            }
            g.ResetClip();
            g.SmoothingMode = SmoothingMode.HighSpeed;
            return bmp;
            //bmp.Dispose();
            //g.Dispose();
        }
        public static Point CentrePoint(Size FrameSize, CentreEnums Centre) {
            int half_x = Convert.ToInt32(FrameSize.Width / (double)2);
            int half_y = Convert.ToInt32(FrameSize.Height / (double)2);
            if ((int)Centre == (int)CentreEnums.CENTRE_BOTH)
                return new Point(half_x, half_y);
            else if (Centre == (int)CentreEnums.CENTRE_HORIZONTAL)
                return new Point(half_x, FrameSize.Height);
            else if ((int)Centre == (int)CentreEnums.CENTRE_VERTICAL)
                return new Point(FrameSize.Width, half_y);
            return default(Point);
        }
        public enum ShearEnums {
            /// <summary>
            /// Shear red Channel
            /// </summary>
            SHEAR_RED = 0,
            /// <summary>
            /// Shear green Channel
            /// </summary>
            SHEAR_GREEN = 1,
            /// <summary>
            /// Shear blue Channel
            /// </summary>
            SHEAR_BLUE = 2,
            /// <summary>
            /// Shear all Channel
            /// </summary>
            SHEAR_ALL = 3
        }
        public enum CentreEnums {
            /// <summary>
            /// Centre Horizontal
            /// </summary>
            CENTRE_HORIZONTAL = 0,
            /// <summary>
            /// Centre Vertical
            /// </summary>
            CENTRE_VERTICAL = 1,
            /// <summary>
            /// Centre Both
            /// </summary>
            CENTRE_BOTH = 2
        }
    }
    namespace Imgx {
        public class QColorMatrix1 {
            private const int MatrixLength = 5;
            private float[,] m = new float[5, 5];
            private const float rad = 0.0174532925199F;//PI/180
            public static float lumR = 0.3086F;
            public static float lumG = 0.6094F;
            public static float lumB = 0.082F;
            private static QColorMatrix1 preHue = new QColorMatrix1();
            private static QColorMatrix1 postHue = new QColorMatrix1();
            private static bool initialized = false;
            public enum MatrixOrder {
                MatrixOrderPrepend = 0,
                MatrixOrderAppend = 1
            }
            public QColorMatrix1() {
                Reset();
            }
            public QColorMatrix1(float[,] m) {
                if (m == null) {
                    Reset();
                    return;
                }
                Copy(m);
            }
            public QColorMatrix1(float[][] m) {
                FromJaggedMatrix(m);
            }
            public QColorMatrix1(QColorMatrix1 qm) {
                Copy(qm);
            }
            public QColorMatrix1(ColorMatrix cm) {
                FromColorMatrix(cm);
            }
            public float[,] Matrix {
                get {
                    return m;
                }
            }
            public void FromJaggedMatrix(float[][] m) {
                Reset();
                if (m == null)
                    return;
                for (int i = 0, loopTo = m.Length - 1; i <= loopTo; i++) {
                    if (m[i] == null)
                        throw new ArgumentException();
                    for (int j = 0, loopTo1 = m[i].Length - 1; j <= loopTo1; j++)
                        this.m[i, j] = m[i][j];
                }
            }
            public float[][] ToJaggedMatrix() {
                var t = new float[5][];
                for (int i = 0, loopTo = t.Length - 1; i <= loopTo; i++) {
                    t[i] = new float[5];
                    for (int j = 0, loopTo1 = t[i].Length - 1; j <= loopTo1; j++)
                        t[i][j] = m[i, j];
                }
                return t;
            }
            public void FromColorMatrix(ColorMatrix cm) {
                if (cm == null) {
                    Reset();
                    return;
                }
                for (int i = 0; i <= MatrixLength - 1; i++) {
                    for (int j = 0; j <= MatrixLength - 1; j++)
                        m[i, j] = cm[i, j];
                }
            }
            public ColorMatrix ToColorMatrix() {
                var cm = new ColorMatrix();
                for (int i = 0; i <= MatrixLength - 1; i++) {
                    for (int j = 0; j <= MatrixLength - 1; j++)
                        cm[i, j] = m[i, j];
                }
                return cm;
            }
            public void Reset() {
                for (int i = 0; i <= MatrixLength - 1; i++) {
                    for (int j = 0; j <= MatrixLength - 1; j++)
                        m[i, j] = i == j ? 1.0F : 0.0F;
                }
            }
            public float[] TransformVector(float[] v) {
                return TransformVector(v, false);
            }
            public static float[] Color2Vector(Color c) {
                if (c == default(Color))
                    return null;
                var p = new float[4];
                p[0] = Convert.ToSingle(c.R);
                p[1] = Convert.ToSingle(c.G);
                p[2] = Convert.ToSingle(c.B);
                p[3] = Convert.ToSingle(c.A);
                return p;
            }
            public static Color Vector2Color(float[] p) {
                if (p == null || p.Length < 4)
                    throw new ArgumentException();
                return Color.FromArgb(Convert.ToInt32(Math.Truncate(p[3])), Convert.ToInt32(Math.Truncate(p[0])), Convert.ToInt32(Math.Truncate(p[1])), Convert.ToInt32(Math.Truncate(p[2])));
            }
            public float[] TransformVector(float[] v, bool normalize) {
                if (v == null || v.Length < 4)
                    throw new ArgumentException();
                var temp = new float[4];
                for (int x = 0; x <= 3; x++) {
                    temp[x] = 255.0F * m[4, x];
                    for (int y = 0; y <= 3; y++)
                        temp[x] += v[y] * m[y, x];
                }
                for (int x = 0; x <= 3; x++) {
                    v[x] = temp[x];
                    if (normalize) {
                        if (v[x] < 0)
                            v[x] = 0.0F;
                        else if (v[x] > 255.0F)
                            v[x] = 255.0F;
                    }
                }
                return v;
            }
            public Color[] TransformColors(Color[] colors) {
                if (colors == null)
                    return null;
                for (int i = 0, loopTo = colors.Length - 1; i <= loopTo; i++)
                    colors[i] = Vector2Color(TransformVector(Color2Vector(colors[i]), true));
                return colors;
            }
            public void Multiply(QColorMatrix1 matrix) {
                Multiply(matrix, MatrixOrder.MatrixOrderPrepend);
            }
            public void Multiply(QColorMatrix1 matrix, MatrixOrder order) {
                if (matrix == null)
                    throw new ArgumentException();
                float[,] a = null;
                float[,] b = null;
                if ((int)order == (int)MatrixOrder.MatrixOrderAppend) {
                    a = matrix.m;
                    b = m;
                }
                else {
                    a = m;
                    b = matrix.m;
                }
                var temp = new float[5, 5];
                for (int y = 0; y <= MatrixLength - 1; y++) {
                    for (int x = 0; x <= MatrixLength - 1; x++) {
                        float t = 0;
                        for (int i = 0; i <= MatrixLength - 1; i++)
                            t += b[y, i] * a[i, x];
                        temp[y, x] = t;
                    }
                }
                for (int y = 0; y <= MatrixLength - 1; y++) {
                    for (int x = 0; x <= MatrixLength - 1; x++)
                        m[y, x] = temp[y, x];
                }
            }
            public void Scale(float scaleRed, float scaleGreen, float scaleBlue, float scaleOpacity) {
                Scale(scaleRed, scaleGreen, scaleBlue, scaleOpacity, MatrixOrder.MatrixOrderPrepend);
            }
            public void Scale(float scaleRed, float scaleGreen, float scaleBlue, float scaleOpacity, MatrixOrder order) {
                var qm = new QColorMatrix1();
                qm.m[0, 0] = scaleRed;
                qm.m[1, 1] = scaleGreen;
                qm.m[2, 2] = scaleBlue;
                qm.m[3, 3] = scaleOpacity;
                Multiply(qm, order);
            }
            public void ScaleColors(float scale) {
                ScaleColors(scale, MatrixOrder.MatrixOrderPrepend);
            }
            public void ScaleColors(float scale__1, MatrixOrder order) {
                Scale(scale__1, scale__1, scale__1, 1.0F, order);
            }
            public void ScaleOpacity(float scaleOpacity__1) {
                ScaleOpacity(scaleOpacity__1, MatrixOrder.MatrixOrderPrepend);
            }
            public void ScaleOpacity(float scaleOpacity__1, MatrixOrder order) {
                Scale(1.0F, 1.0F, 1.0F, scaleOpacity__1, order);
            }
            public void Translate(float offsetRed, float offsetGreen, float offsetBlue, float offsetOpacity) {
                Translate(offsetRed, offsetGreen, offsetBlue, offsetOpacity, MatrixOrder.MatrixOrderPrepend);
            }
            public void Translate(float offsetRed, float offsetGreen, float offsetBlue, float offsetOpacity, MatrixOrder order) {
                var qm = new QColorMatrix1();
                qm.m[4, 0] = offsetRed;
                qm.m[4, 1] = offsetGreen;
                qm.m[4, 2] = offsetBlue;
                qm.m[4, 3] = offsetOpacity;
                Multiply(qm, order);
            }
            public void TranslateColors(float offset) {
                TranslateColors(offset, MatrixOrder.MatrixOrderPrepend);
            }
            public void TranslateColors(float offset, MatrixOrder order) {
                Translate(offset, offset, offset, 0.0F, order);
            }
            public void TranslateOpacity(float offsetOpacity) {
                TranslateOpacity(offsetOpacity, MatrixOrder.MatrixOrderPrepend);
            }
            public void TranslateOpacity(float offsetOpacity, MatrixOrder order) {
                Translate(0.0F, 0.0F, 0.0F, offsetOpacity, order);
            }
            // Rotate the matrix around one of the color axes. The color of the rotation
            // axis is unchanged, the other two colors are rotated in color space.
            // The angle phi is in degrees (-180.0f... 180.0f).
            public void RotateRed(float phi) {
                RotateRed(phi, MatrixOrder.MatrixOrderPrepend);
            }
            public void RotateGreen(float phi) {
                RotateGreen(phi, MatrixOrder.MatrixOrderPrepend);
            }
            public void RotateBlue(float phi) {
                RotateBlue(phi, MatrixOrder.MatrixOrderPrepend);
            }
            public void RotateRed(float phi, MatrixOrder order) {
                RotateColor(phi, 2, 1, order);
            }
            public void RotateGreen(float phi, MatrixOrder order) {
                RotateColor(phi, 0, 2, order);
            }
            public void RotateBlue(float phi, MatrixOrder order) {
                RotateColor(phi, 1, 0, order);
            }
            public void ShearRed(float green, float blue) {
                ShearRed(green, blue, MatrixOrder.MatrixOrderPrepend);
            }
            public void ShearGreen(float red, float blue) {
                ShearGreen(red, blue, MatrixOrder.MatrixOrderPrepend);
            }
            public void ShearBlue(float red, float green) {
                ShearBlue(red, green, MatrixOrder.MatrixOrderPrepend);
            }
            public void ShearRed(float green, float blue, MatrixOrder order) {
                ShearColor(0, 1, green, 2, blue, order);
            }
            public void ShearGreen(float red, float blue, MatrixOrder order) {
                ShearColor(1, 0, red, 2, blue, order);
            }
            public void ShearBlue(float red, float green, MatrixOrder order) {
                ShearColor(2, 0, red, 1, green, order);
            }
            public void SetSaturation(float saturation) {
                SetSaturation(saturation, MatrixOrder.MatrixOrderPrepend);
            }
            public void SetSaturation(float saturation, MatrixOrder order) {
                float satCompl = 1.0F - saturation;
                float satComplR = lumR * satCompl;
                float satComplG = lumG * satCompl;
                float satComplB = lumB * satCompl;
                var tm = new float[,] { { satComplR + saturation, satComplR, satComplR, 0.0F, 0.0F }, { satComplG, satComplG + saturation, satComplG, 0.0F, 0.0F }, { satComplB, satComplB, satComplB + saturation, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F, 1.0F, 0.0F }, { 0.0F, 0.0F, 0.0F, 0.0F, 1.0F } };
                var qm = new QColorMatrix1(tm);
                Multiply(qm, order);
            }
            public void RotateHue(float phi) {
                InitHue();
                Multiply(preHue, MatrixOrder.MatrixOrderAppend);
                RotateBlue(phi, MatrixOrder.MatrixOrderAppend);
                Multiply(postHue, MatrixOrder.MatrixOrderAppend);
            }
            public void SetContrast(float scale) {
                ScaleColors(scale);
            }
            public void SetBrightness(float offset) {
                TranslateColors(offset, MatrixOrder.MatrixOrderAppend);
            }
            public void SetSaturation2(float saturation) {
                SetSaturation(saturation, MatrixOrder.MatrixOrderAppend);
            }
            private static void InitHue() {
                const float greenRotation = 35.0F;
                if (!initialized) {
                    initialized = true;
                    preHue.RotateRed(45.0F);
                    preHue.RotateGreen(-greenRotation, MatrixOrder.MatrixOrderAppend);
                    var lum = new float[] { lumR, lumG, lumB, 1.0F };
                    preHue.TransformVector(lum);
                    float red = lum[0] / lum[2];
                    float green = lum[1] / lum[2];
                    preHue.ShearBlue(red, green, MatrixOrder.MatrixOrderAppend);
                    postHue.ShearBlue(-red, -green);
                    postHue.RotateGreen(greenRotation, MatrixOrder.MatrixOrderAppend);
                    postHue.RotateRed(-45.0F, MatrixOrder.MatrixOrderAppend);
                }
            }
            private void RotateColor(float phi, int x, int y, MatrixOrder order) {
                phi *= rad;
                var qm = new QColorMatrix1();
                var argtarget = qm.m[y, y];
                qm.m[x, x] = InlineAssignHelper(ref argtarget, Convert.ToSingle(Math.Cos(phi)));
                float s = Convert.ToSingle(Math.Sin(phi));
                qm.m[y, x] = s;
                qm.m[x, y] = -s;
                Multiply(qm, order);
            }
            private void ShearColor(int x, int y1, float d1, int y2, float d2, MatrixOrder order) {
                var qm = new QColorMatrix1();
                qm.m[y1, x] = d1;
                qm.m[y2, x] = d2;
                Multiply(qm, order);
            }
            private void Copy(QColorMatrix1 qm) {
                if (qm == null) {
                    Reset();
                    return;
                }
                Copy(qm.m);
            }
            private void Copy(float[,] m) {
                if (m == null || m.Length != this.m.Length)
                    throw new ArgumentException();
                Array.Copy(m, this.m, m.Length);
            }
            private static T InlineAssignHelper<T>(ref T target, T value) {
                target = value;
                return value;
            }
        }
    }
    public class HSV {
        int a = 255;
        private float h;
        private float s;
        private float b;
        public int A {
            get { return a; }
            set {
                if (value > 255) { a = 255; }
                else if (value < 0) { a = 0; }
                else { a = value; }
            }
        }
        public float H {
            get { return h; }
            set {
                if (value > 360.0F) { h = 360.0F; }
                else if (value < 0) { h = 0; }
                else { h = value; }
            }
        }
        public float S {
            get { return s; }
            set {
                if (value > 1.0F) { s = 1.0F; }
                else if (value < 0) { s = 0; }
                else { s = value; }
            }
        }
        public float B {
            get { return b; }
            set {
                if (value > 1.0F) { b = 1.0F; }
                else if (value < 0) { b = 0; }
                else { b = value; }
            }
        }
        public HSV(HSL Input) {
            H = Input.H;
            b = Input.L + (Input.S * Math.Min(Input.L, 1 - Input.L));
            if (b == 0) {
                s = 0;
            }
            else {
                S = 2 - (2 * Input.L / b);
            }
        }
        public HSV(Color Input) {
            ConvertFromColor(Input);
        }
        public HSV(float h, float s, float b) {
            H = h;
            S = s;
            B = b;
            A = 255;
        }
        public HSV(int a, float h, float s, float b) {
            H = h;
            S = s;
            B = b;
            A = a;
        }
        public HSV(YDbDr Input) {
            ConvertFromColor(Input.ToColor());
        }
        public HSV(YCoCg Input) {
            ConvertFromColor(Input.ToColor());
        }
        public HSV(YUV Input) {
            ConvertFromColor(Input.ToColor());
        }
        private void ConvertFromColor(Color Input) {
            a = Input.A;
            float red = (float)(Input.R / (double)255);
            float green = (float)(Input.G / (double)255);
            float blue = (float)(Input.B / (double)255);
            float minValue = Math.Min(red, Math.Min(green, blue));
            float maxValue = Math.Max(red, Math.Max(green, blue));
            float delta = maxValue - minValue;
            h = 0;
            b = maxValue;
            switch (maxValue) {
                case var @case when @case == red: {
                        if (green >= blue) {
                            if (delta == 0)
                                h = 0.0f;
                            else
                                h = 60.0f * (green - blue) / delta;
                        }
                        else if (green < blue)
                            h = 60.0f * (green - blue) / delta + 360.0f;
                        break;
                    }
                case var case1 when case1 == green: {
                        h = 60.0f * (blue - red) / delta + 120.0f;
                        break;
                    }
                case var case2 when case2 == blue: {
                        h = 60.0f * (red - green) / delta + 240.0f;
                        break;
                    }
            }
            if (maxValue == 0) {
                s = 0.0f;
            }
            else {
                s = (1.0f - minValue / maxValue);
            }
        }
        public HSV() { }
        public Color ToColor() {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);
            int val = 0;
            val = (int)(b * 255.0f);
            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - s));
            int q = Convert.ToInt32(val * (1 - f * s));
            int t = Convert.ToInt32(val * (1 - (1 - f) * s));

            if (hi == 0)
                return Color.FromArgb(a, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(a, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(a, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(a, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(a, t, p, v);
            else
                return Color.FromArgb(a, v, p, q);
        }
        public HSL ToHSL() {
            float temp_l = b - (b * s / 2.0f);
            float temp_s = (b - temp_l) / Math.Min(temp_l, 1 - temp_l);
            if ((temp_l == 0) || (temp_l == 1)) { temp_s = 0; }
            HSL Output = new HSL(a, h, temp_s, temp_l);
            return Output;
        }
        public YUV ToYUV() {
            return new YUV(this);
        }
        public YDbDr ToYDbDr() {
            return new YDbDr(this);
        }
        public YCoCg ToYCoCg() {
            return new YCoCg(this);
        }
        public override string ToString() {
            return "{H: " + H.ToString() + ", S: " + S.ToString() + ", V: " + B.ToString() + "}";
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class HSL {
        int a = 255;
        private float h;
        private float s;
        private float l;
        public int A {
            get { return a; }
            set {
                if (value > 255) { a = 255; }
                else if (value < 0) { a = 0; }
                else { a = value; }
            }
        }
        public float H {
            get { return h; }
            set {
                if (value > 360.0F) { h = 360.0F; }
                else if (value < 0) { h = 0; }
                else { h = value; }
            }
        }
        public float S {
            get { return s; }
            set {
                if (value > 1.0F) { s = 1.0F; }
                else if (value < 0) { s = 0; }
                else { s = value; }
            }
        }
        public float L {
            get { return l; }
            set {
                if (value > 1.0F) { l = 1.0F; }
                else if (value < 0) { l = 0; }
                else { l = value; }
            }
        }
        public HSL(HSV Input) {
            H = Input.H;
            L = Input.B - (Input.B * Input.S / 2.0f);
            if ((L == 0) || (L == 1)) {
                s = 0;
            }
            else {
                S = (Input.B - l) / Math.Min(l, 1 - l);
            }
        }
        public HSL(Color Input) {
            ConvertFromColor(Input);
        }
        public HSL(float h, float s, float l) {
            H = h;
            S = s;
            L = l;
            A = 255;
        }
        public HSL(int a, float h, float s, float l) {
            H = h;
            S = s;
            L = l;
            A = a;
        }
        public HSL(YDbDr Input) {
            ConvertFromColor(Input.ToColor());
        }
        public HSL(YCoCg Input) {
            ConvertFromColor(Input.ToColor());
        }
        public HSL(YUV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public HSL() { }
        private void ConvertFromColor(Color Input) {
            a = Input.A;
            float red = (float)(Input.R / (double)255);
            float green = (float)(Input.G / (double)255);
            float blue = (float)(Input.B / (double)255);
            float minValue = Math.Min(red, Math.Min(green, blue));
            float maxValue = Math.Max(red, Math.Max(green, blue));
            float delta = maxValue - minValue;
            float h = 0;
            float s_temp;
            float v = maxValue;
            switch (maxValue) {
                case var @case when @case == red: {
                        if (green >= blue) {
                            if (delta == 0)
                                h = 0;
                            else
                                h = 60.0f * (green - blue) / delta;
                        }
                        else if (green < blue)
                            h = 60.0f * (green - blue) / delta + 360.0f;
                        break;
                    }
                case var case1 when case1 == green: {
                        h = 60.0f * (blue - red) / delta + 120.0f;
                        break;
                    }
                case var case2 when case2 == blue: {
                        h = 60.0f * (red - green) / delta + 240.0f;
                        break;
                    }
            }
            if (maxValue == 0) {
                s_temp = 0;
            }
            else {
                s_temp = 1.0f - minValue / maxValue;
            }
            l = v - (v * s_temp / 2.0f);
            if ((l == 0) || (l == 1)) {
                s = 0;
            }
            else {
                s = (v - l) / Math.Min(l, 1 - l);
            }
            H = h;
        }
        public Color ToColor() {
            float b = l + s * Math.Min(l, 1 - l);
            float s_temp = 2 - (2 * l / b);
            if (b == 0) { s_temp = 0; }
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);
            int val = 0;
            val = (int)(b * 255.0f);
            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - s_temp));
            int q = Convert.ToInt32(val * (1 - f * s_temp));
            int t = Convert.ToInt32(val * (1 - (1 - f) * s_temp));

            if (hi == 0)
                return Color.FromArgb(a, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(a, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(a, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(a, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(a, t, p, v);
            else
                return Color.FromArgb(a, v, p, q);
        }
        public HSV ToHSV() {
            float temp_v = l + s * Math.Min(l, 1 - l);
            float temp_s = 2 - (2 * l / temp_v);
            if (temp_v == 0) { temp_s = 0; }
            HSV Output = new HSV(a, h, temp_s, temp_v);
            return Output;
        }
        public YUV ToYUV() {
            return new YUV(this);
        }
        public YDbDr ToYDbDr() {
            return new YDbDr(this);
        }
        public YCoCg ToYCoCg() {
            return new YCoCg(this);
        }
        public override string ToString() {
            return "{H: " + H.ToString() + ", S: " + S.ToString() + ", L: " + L.ToString() + "}";
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class YUV {
        int a = 255;
        double y = 0;
        double u = 0;
        double v = 0;
        YUVSystem YUVSystem = YUVSystem.BT470;
        public YUVSystem System {
            get { return YUVSystem; }
            set {
                YUVSystem = value;
            }
        }
        const float UMax = 0.436f;
        const float VMax = 0.615f;
        public int A {
            get { return a; }
            set {
                if (value > 255) { a = 255; }
                else if (value < 0) { a = 0; }
                else { a = value; }
            }
        }
        public double Y {
            get { return y; }
            set {
                if (value > 1.0F) { y = 1.0F; }
                else if (value < 0) { y = 0; }
                else { y = value; }
            }
        }
        public double U {
            get { return u; }
            set {
                if (value > UMax) { u = UMax; }
                else if (value < -UMax) { u = -UMax; }
                else { u = value; }
            }
        }
        public double V {
            get { return v; }
            set {
                if (value > UMax) { v = VMax; }
                else if (value < -UMax) { v = -VMax; }
                else { v = value; }
            }
        }
        public YUV() { }
        public YUV(double y, double u, double v) {
            Y = y;
            U = u;
            V = v;
        }
        public YUV(double y, double u, double v, YUVSystem System) {
            this.System = System;
            Y = y;
            U = u;
            V = v;
        }
        public YUV(int a, double y, double u, double v, YUVSystem System) {
            this.System = System;
            A = a;
            Y = y;
            U = u;
            V = v;
        }
        public YUV(int a, double y, double u, double v) {
            A = a;
            Y = y;
            U = u;
            V = v;
        }
        public YUV(Color Input) {
            ConvertFromColor(Input);
        }
        public YUV(Color Input, YUVSystem System) {
            this.System = System;
            ConvertFromColor(Input);
        }
        public YUV(HSV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YUV(HSV Input, YUVSystem System) {
            this.System = System;
            ConvertFromColor(Input.ToColor());
        }
        public YUV(HSL Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YUV(HSL Input, YUVSystem System) {
            this.System = System;
            ConvertFromColor(Input.ToColor());
        }
        public YUV(YDbDr Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YUV(YDbDr Input, YUVSystem System) {
            this.System = System;
            ConvertFromColor(Input.ToColor());
        }
        public YUV(YCoCg Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YUV(YCoCg Input, YUVSystem System) {
            this.System = System;
            ConvertFromColor(Input.ToColor());
        }
        private void ConvertFromColor(Color Input) {
            float rp = (float)Input.R / 255.0f;
            float gp = (float)Input.G / 255.0f;
            float bp = (float)Input.B / 255.0f;
            switch (YUVSystem) {
                case YUVSystem.BT470:
                    A = Input.A;
                    Y = (double)(0.299f * rp + 0.587f * gp + 0.114f * bp);
                    U = (double)(-0.14713f * rp + -0.28886 * gp + 0.436f * bp);
                    V = (double)(0.615f * rp + -0.51499f * gp + -0.10001f * bp);
                    break;
                case YUVSystem.BT709:
                    A = Input.A;
                    Y = (double)(0.2126f * rp + 0.7152f * gp + 0.0722f * bp);
                    U = (double)(-0.09991f * rp + -0.33609 * gp + 0.436f * bp);
                    V = (double)(0.615f * rp + -0.51861f * gp + -0.05639f * bp);
                    break;
            }
        }
        private Color ConvertToColor() {
            double tempR = 0;
            double tempG = 0;
            double tempB = 0;
            switch (YUVSystem) {
                case YUVSystem.BT470:
                    tempR = (double)(y + 1.13983f * v);
                    tempG = (double)(y + -0.39465 * u + -0.58060f * v);
                    tempB = (double)(y + 2.03211f * u);
                    break;
                case YUVSystem.BT709:
                    tempR = (double)(y + 1.28033f * v);
                    tempG = (double)(y + -0.21482 * u + -0.38059f * v);
                    tempB = (double)(y + 2.12798f * u);
                    break;
            }
            if (tempR < 0) { tempR = 0; }
            if (tempG < 0) { tempG = 0; }
            if (tempB < 0) { tempB = 0; }
            return Color.FromArgb(A, (int)(tempR * 255), (int)(tempG * 255), (int)(tempB * 255));
        }
        public Color ToColor() {
            return ConvertToColor();
        }
        public HSV ToHSV() {
            return new HSV(ConvertToColor());
        }
        public HSL ToHSL() {
            return new HSL(ConvertToColor());
        }
        public YDbDr ToYDbDr() {
            return new YDbDr(ConvertToColor());
        }
        public YCoCg ToYCoCg() {
            return new YCoCg(ConvertToColor());
        }
        public override string ToString() {
            return "{Y: " + Y.ToString() + ", U: " + U.ToString() + ", V: " + V.ToString() + "}";
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class YDbDr {
        int a = 255;
        double y = 0;
        double db = 0;
        double dr = 0;
        const float Max = 1.333f;
        public int A {
            get { return a; }
            set {
                if (value > 255) { a = 255; }
                else if (value < 0) { a = 0; }
                else { a = value; }
            }
        }
        public double Y {
            get { return y; }
            set {
                if (value > 1.0F) { y = 1.0F; }
                else if (value < 0) { y = 0; }
                else { y = value; }
            }
        }
        public double Db {
            get { return db; }
            set {
                if (value > Max) { db = Max; }
                else if (value < -Max) { db = -Max; }
                else { db = value; }
            }
        }
        public double Dr {
            get { return dr; }
            set {
                if (value > Max) { dr = Max; }
                else if (value < -Max) { dr = -Max; }
                else { dr = value; }
            }
        }
        public YDbDr() { }
        public YDbDr(double y, double db, double dr) {
            Y = y;
            Db = db;
            Dr = dr;
        }
        public YDbDr(int a, double y, double db, double dr) {
            A = a;
            Y = y;
            Db = db;
            Dr = dr;
        }
        public YDbDr(Color Input) {
            ConvertFromColor(Input);
        }
        public YDbDr(HSV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YDbDr(HSL Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YDbDr(YUV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YDbDr(YCoCg Input) {
            ConvertFromColor(Input.ToColor());
        }
        private void ConvertFromColor(Color Input) {
            float rp = (float)Input.R / 255.0f;
            float gp = (float)Input.G / 255.0f;
            float bp = (float)Input.B / 255.0f;
            A = Input.A;
            Y = (double)(0.299f * rp + 0.587f * gp + 0.114f * bp);
            Db = (double)(-0.45f * rp + -0.883 * gp + 1.33f * bp);
            Dr = (double)(-1.333f * rp + 1.116f * gp + 0.217f * bp);
        }
        private Color ConvertToColor() {
            double tempR = 0;
            double tempG = 0;
            double tempB = 0;
            tempR = (double)(y + 0.000092303716148f * db + -0.525912630661865f * dr);
            tempG = (double)(y + -0.129132898890509f * db + 0.267899328207599f * dr);
            tempB = (double)(y + 0.664679059978955f * db + -0.000079202543533f * dr);
            if (tempR < 0) { tempR = 0; }
            if (tempG < 0) { tempG = 0; }
            if (tempB < 0) { tempB = 0; }
            return Color.FromArgb(A, (int)(tempR * 255), (int)(tempG * 255), (int)(tempB * 255));
        }
        public Color ToColor() {
            return ConvertToColor();
        }
        public HSV ToHSV() {
            return new HSV(ConvertToColor());
        }
        public HSL ToHSL() {
            return new HSL(ConvertToColor());
        }
        public YUV ToYUV() {
            return new YUV(ConvertToColor());
        }
        public YCoCg ToYCoCg() {
            return new YCoCg(ConvertToColor());
        }
        public override string ToString() {
            return "{Y: " + Y.ToString() + ", Db: " + db.ToString() + ", Dr: " + dr.ToString() + "}";
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class YCoCg {
        int a = 255;
        double y = 0;
        double co = 0;
        double cg = 0;
        const float Max = 0.5f;
        public int A {
            get { return a; }
            set {
                if (value > 255) { a = 255; }
                else if (value < 0) { a = 0; }
                else { a = value; }
            }
        }
        public double Y {
            get { return y; }
            set {
                if (value > 1.0F) { y = 1.0F; }
                else if (value < 0) { y = 0; }
                else { y = value; }
            }
        }
        public double Co {
            get { return co; }
            set {
                if (value > Max) { co = Max; }
                else if (value < -Max) { co = -Max; }
                else { co = value; }
            }
        }
        public double Cg {
            get { return cg; }
            set {
                if (value > Max) { cg = Max; }
                else if (value < -Max) { cg = -Max; }
                else { cg = value; }
            }
        }
        public YCoCg() { }
        public YCoCg(double y, double co, double cg) {
            Y = y;
            Co = co;
            Cg = cg;
        }
        public YCoCg(int a, double y, double co, double cg) {
            A = a;
            Y = y;
            Co = co;
            Cg = cg;
        }
        public YCoCg(Color Input) {
            ConvertFromColor(Input);
        }
        public YCoCg(HSV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YCoCg(HSL Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YCoCg(YUV Input) {
            ConvertFromColor(Input.ToColor());
        }
        public YCoCg(YDbDr Input) {
            ConvertFromColor(Input.ToColor());
        }
        private void ConvertFromColor(Color Input) {
            float rp = (float)Input.R / 255.0f;
            float gp = (float)Input.G / 255.0f;
            float bp = (float)Input.B / 255.0f;
            A = Input.A;
            Y = (double)(0.25f * rp + 0.5f * gp + 0.25f * bp);
            Co = (double)(0.5f * rp  + -0.5f * bp);
            Cg = (double)(-0.25f * rp + 0.5 * gp + -0.25 * bp);
        }
        private Color ConvertToColor() {
            double tempR = 0;
            double tempG = 0;
            double tempB = 0;
            double tmp = Y - Cg;
            tempR = tmp + Co;
            tempG = Y + Cg;
            tempB = tmp - Co; 
            if (tempR < 0) { tempR = 0; }
            if (tempG < 0) { tempG = 0; }
            if (tempB < 0) { tempB = 0; }
            return Color.FromArgb(A, (int)(tempR * 255), (int)(tempG * 255), (int)(tempB * 255));
        }
        public Color ToColor() {
            return ConvertToColor();
        }
        public HSV ToHSV() {
            return new HSV(ConvertToColor());
        }
        public HSL ToHSL() {
            return new HSL(ConvertToColor());
        }
        public YUV ToYUV() {
            return new YUV(ConvertToColor());
        }
        public YDbDr ToYDbDr() {
            return new YDbDr(ConvertToColor());
        }
        public override string ToString() {
            return "{Y: " + Y.ToString() + ", Co: " + co.ToString() + ", Cg: " + cg.ToString() + "}";
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public enum YUVSystem {
        BT470 = 0X00,
        BT709 = 0x01
    }
}
