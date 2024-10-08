﻿using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.ComponentModel;

namespace Boxy_Core.Utilities
{
    /// <summary>
    /// Possible options for cut line sizes.
    /// </summary>
    public enum CutLineSizes
    {
        [Description("Small")]
        Small,

        [Description("Medium")]
        Medium,

        [Description("Large")]
        Large,

        [Description("Extra Large")]
        QuiteLarge,

        [Description("Line-Colossus")]
        Colossal,

        [Description("A Line to surpass Metal Gear")]
        ALineToSurpassMetalGear,
    }

    /// <summary>
    /// Draws regular rectangular images on a <see cref="PdfPage"/>.
    /// </summary>
    public class CardImageDrawer 
    {
        /// <summary>
        /// Creates a new instance of <see cref="CardImageDrawer"/>.
        /// </summary>
        public CardImageDrawer(PdfPage page, double scalingPercent, bool hasCutLines, CutLineSizes cutLineSize, XKnownColor cutLineColor)
        {
            ScalingPercent = scalingPercent;
            HasCutLines = hasCutLines;
            CutLineSize = cutLineSize;
            CutLineColor = cutLineColor;
            Gfx = XGraphics.FromPdfPage(page);

            // Set Gutter
            GutterThickness = HasCutLines ? CutLineSize.ToPointSize() : 0;

            // Set some properties other methods will need to use.
            PointsPerInch = page.Width.Point / page.Width.Inch;
            Margin = 0.25 * PointsPerInch;
            UseableX = page.Width - 2 * Margin;
            UseableY = page.Height - 2 * Margin;

            // MTG cards are 3.48 x 2.49 inches or 63 x 88 mm, then slightly scaled down to fit better in card sleeves.
            CardSize = new XSize(2.49 * PointsPerInch * ScalingPercent / 100 * 0.99, 3.48 * PointsPerInch * ScalingPercent / 100 * 0.99);

            // Predict the number of cards per row and cards per column.
            Rows = (int)((UseableY - GutterThickness) / (CardSize.Height + GutterThickness));
            Columns = (int)((UseableX - GutterThickness) / (CardSize.Width + GutterThickness));
            ImagesPerPage = Rows * Columns;

            // Calculate how much to shift all images to center everything on the page. Helps with printers which have trouble printing near the edge.
            double usedY = GutterThickness + Rows * (CardSize.Height + GutterThickness);
            double usedX = GutterThickness + Columns * (CardSize.Width + GutterThickness);

            // Half of what is not used inside the margins
            VerticalCenteringOffset = (UseableY - usedY) / 2.0;
            HorizontalCenteringOffset = (UseableX - usedX) / 2.0;
        }

        private double ScalingPercent { get; }

        private bool HasCutLines { get; }

        private CutLineSizes CutLineSize { get; }

        private XKnownColor CutLineColor { get; }

        private XSize CardSize { get; }

        private XGraphics Gfx { get; }

        private double GutterThickness { get; }

        private double PointsPerInch { get; }

        private double Margin { get; }

        private double UseableX { get; }

        private double UseableY { get; }

        private double VerticalCenteringOffset { get; }

        private double HorizontalCenteringOffset { get; }

        private int ImagesDrawn { get; set; }

        private bool IsDrawing { get; set; }

        /// <summary>
        /// Number of rows of images able to be drawn on a page.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Number of columns of images able to be drawn on a page.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Total number of images that can be drawn on each page (Rows * Columns).
        /// </summary>
        public int ImagesPerPage { get; }

        public async Task DrawImage(XImage image, int row, int column)
        {
            while (IsDrawing)
            {
                await Task.Delay(1);
            }

            IsDrawing = true;
            var gutterPen = new XPen(XColor.FromKnownColor(CutLineColor), GutterThickness);
            XRect imagePlacement = await Task.Run(() => GetCardPlacement(row, column));

            if (HasCutLines)
            {
                var verticalLinePlacement = new XRect(
                    new XPoint(imagePlacement.Left - GutterThickness / 2, imagePlacement.Top - GutterThickness),
                    new XPoint(imagePlacement.Right + GutterThickness / 2, imagePlacement.Bottom + GutterThickness));

                var horizontalLinePlacement = new XRect(
                    new XPoint(imagePlacement.Left - GutterThickness, imagePlacement.Top - GutterThickness / 2),
                    new XPoint(imagePlacement.Right + GutterThickness, imagePlacement.Bottom + GutterThickness / 2));

                await Task.Run(() => Gfx.DrawLine(gutterPen, horizontalLinePlacement.TopLeft, horizontalLinePlacement.TopRight));
                await Task.Run(() => Gfx.DrawLine(gutterPen, verticalLinePlacement.TopRight, verticalLinePlacement.BottomRight));
                await Task.Run(() => Gfx.DrawLine(gutterPen, horizontalLinePlacement.BottomRight, horizontalLinePlacement.BottomLeft));
                await Task.Run(() => Gfx.DrawLine(gutterPen, verticalLinePlacement.BottomLeft, verticalLinePlacement.TopLeft));
            }

            if (ImagesDrawn >= ImagesPerPage)
            {
                throw new InvalidOperationException($"Attempted to draw {ImagesDrawn + 1} to a page with a maximum of {ImagesPerPage} images possible.");
            }

            await Task.Run(() => Gfx.DrawImage(image, imagePlacement));
            ImagesDrawn += 1;

            IsDrawing = false;
        }

        private XRect GetCardPlacement(int row, int column)
        {
            if (row > Rows)
            {
                throw new InvalidOperationException($"Attempted to place an image in row {row} when the max number of rows was {Rows}");
            }

            if (column > Columns)
            {
                throw new InvalidOperationException($"Attempted to place an image in column {column} when the max number of columns was {Columns}");
            }

            // Calculate the position of the top left corner of the image.
            double xPos = HorizontalCenteringOffset + Margin + GutterThickness + column * (CardSize.Width + GutterThickness);
            double yPos = VerticalCenteringOffset + Margin + GutterThickness + row * (CardSize.Height + GutterThickness);
            var position = new XPoint(xPos, yPos);
            var imagePlacement = new XRect(position, CardSize);
            return imagePlacement;
        }
    }
}
