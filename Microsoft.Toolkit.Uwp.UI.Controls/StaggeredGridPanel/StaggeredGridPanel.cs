using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    public class StaggeredGridPanel : Panel
    {
        /// <summary>
        /// Identifies the <see cref="DesiredItemSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredItemSizeProperty =
            DependencyProperty.Register(nameof(DesiredItemSize), typeof(double), typeof(StaggeredGridPanel), new PropertyMetadata(200d, (d, e) => (d as StaggeredGridPanel)?.InvalidateMeasure()));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StaggeredGridPanel), new PropertyMetadata(Orientation.Vertical, (d, e) => (d as StaggeredGridPanel)?.InvalidateMeasure()));

        /// <summary>
        /// Initializes a new instance of the <see cref="StaggeredGridPanel"/> class.
        /// </summary>
        public StaggeredGridPanel()
        {
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Orientation == Orientation.Vertical)
            {
                return ArrangeVertical(finalSize);
            }
            else
            {
                return ArrangeHorizontal(finalSize);
            }
        }

        private Size ArrangeVertical(Size finalSize)
        {
            int cols = (int)Math.Floor(finalSize.Width / DesiredItemSize);
            var w = finalSize.Width / cols;
            List<double> colHeight = new List<double>(cols);
            for (int i = 0; i < cols; i++)
            {
                colHeight.Add(0);
            }

            foreach (var child in Children)
            {
                // place item in the use the shortest column
                var col = colHeight.IndexOf(colHeight.Min());
                double x = w * col;
                double y = colHeight[col];
                child.Arrange(new Rect(x, y, w, child.DesiredSize.Height));
                colHeight[col] += child.DesiredSize.Height;
            }

            return new Size(finalSize.Width, colHeight.Max());
        }

        private Size ArrangeHorizontal(Size finalSize)
        {
            finalSize = new Size(finalSize.Width, DesiredItemSize);
            double x = 0;
            double y = 0;
            foreach (var child in Children)
            {
                if (x + child.DesiredSize.Width > finalSize.Width)
                {
                    x = 0;
                    y += DesiredItemSize;
                }

                child.Arrange(new Rect(x, y, child.DesiredSize.Width, DesiredItemSize));
                x += child.DesiredSize.Width;
            }

            return new Size(finalSize.Width, y + DesiredItemSize);
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Vertical)
            {
                return MeasureVertical(availableSize);
            }
            else
            {
                return MeasureHorizontal(availableSize);
            }
        }

        private Size MeasureVertical(Size availableSize)
        {
            int cols = (int)Math.Floor(availableSize.Width / DesiredItemSize);
            var w = availableSize.Width / cols;
            availableSize = new Size(w, Math.Min(availableSize.Height, double.MaxValue));
            List<double> colHeight = new List<double>(cols);
            for (int i = 0; i < cols; i++)
            {
                colHeight.Add(0);
            }

            foreach (var child in Children)
            {
                child.Measure(availableSize);

                // use the shortest column
                var col = colHeight.IndexOf(colHeight.Min());
                colHeight[col] += child.DesiredSize.Height;
            }

            return new Size(availableSize.Width, colHeight.Max());
        }

        private Size MeasureHorizontal(Size availableSize)
        {
            availableSize = new Size(Math.Min(availableSize.Width, double.MaxValue), DesiredItemSize);

            double rowWidth = double.MaxValue;
            double height = 0;
            foreach (var child in Children)
            {
                child.Measure(availableSize);
                rowWidth += child.DesiredSize.Width;
                if (rowWidth > availableSize.Width)
                {
                    rowWidth = child.DesiredSize.Width;
                    height += DesiredItemSize;
                }
            }

            return new Size(availableSize.Width, height);
        }

        /// <summary>
        /// Gets or sets the desired items size. If the orientation is Vertical,
        /// this referes to the width, and for horizontal the height.
        /// </summary>
        public double DesiredItemSize
        {
            get { return (double)GetValue(DesiredItemSizeProperty); }
            set { SetValue(DesiredItemSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction the items are staggered
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}
