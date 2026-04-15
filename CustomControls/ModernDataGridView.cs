using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.Theme;

namespace FaceIDApp.CustomControls
{
    /// <summary>
    /// DataGridView hiện đại — Fluent Design: light header, 48px rows, row hover, empty state.
    /// </summary>
    public class ModernDataGridView : DataGridView
    {
        private int _hoveredRowIndex = -1;
        private string _emptyMessage = "Không có dữ liệu";

        public string EmptyMessage
        {
            get => _emptyMessage;
            set { _emptyMessage = value; Invalidate(); }
        }

        public ModernDataGridView()
        {
            DoubleBuffered = true;
            DesignSystem.ApplyDataGridViewStyle(this);
            MultiSelect = false;
            StandardTab = true;

            // Fluent selection colors
            DefaultCellStyle.SelectionBackColor = Color.FromArgb(212, 228, 255);
            DefaultCellStyle.SelectionForeColor = DesignSystem.TextPrimary;
            RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(212, 228, 255);
            RowHeadersDefaultCellStyle.SelectionForeColor = DesignSystem.TextPrimary;
            RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(212, 228, 255);
            RowsDefaultCellStyle.SelectionForeColor = DesignSystem.TextPrimary;
            AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(212, 228, 255);
            AlternatingRowsDefaultCellStyle.SelectionForeColor = DesignSystem.TextPrimary;

            CellMouseEnter += OnCellMouseEnter;
            CellMouseLeave += OnCellMouseLeave;
        }

        private void OnCellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < Rows.Count)
            {
                _hoveredRowIndex = e.RowIndex;
                InvalidateRow(e.RowIndex);
            }
        }

        private void OnCellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (_hoveredRowIndex >= 0 && _hoveredRowIndex < Rows.Count)
            {
                int old = _hoveredRowIndex;
                _hoveredRowIndex = -1;
                InvalidateRow(old);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Apply rounded corners clip
            int radius = DesignSystem.BorderRadiusLg;
            if (Width > radius && Height > radius)
            {
                var rect = new Rectangle(0, 0, Width, Height);
                using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                {
                    Region = new Region(path);
                }
            }
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            base.OnCellFormatting(e);

            if (e.RowIndex >= 0 && e.RowIndex == _hoveredRowIndex && !Rows[e.RowIndex].Selected)
            {
                e.CellStyle.BackColor = Color.FromArgb(235, 240, 255); // Subtle accent hover
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Empty state
            if (Rows.Count == 0 || (Rows.Count == 1 && Rows[0].IsNewRow))
            {
                var g = e.Graphics;
                DesignSystem.ApplyHighQualityRendering(g);

                int headerH = ColumnHeadersVisible ? ColumnHeadersHeight : 0;
                var contentRect = new Rectangle(0, headerH, Width, Height - headerH);

                // Icon
                using (var brush = new SolidBrush(DesignSystem.TextMuted))
                {
                    var iconFont = new Font("Segoe UI", DesignSystem.Scale(40), GraphicsUnit.Pixel);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    var iconRect = new RectangleF(contentRect.X, contentRect.Y + contentRect.Height / 2f - DesignSystem.Scale(44), contentRect.Width, DesignSystem.Scale(55));
                    g.DrawString("📋", iconFont, brush, iconRect, sf);
                    iconFont.Dispose();
                }

                // Message
                using (var brush = new SolidBrush(DesignSystem.TextMuted))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    var msgRect = new RectangleF(contentRect.X, contentRect.Y + contentRect.Height / 2f + DesignSystem.Scale(14), contentRect.Width, DesignSystem.Scale(30));
                    g.DrawString(_emptyMessage, DesignSystem.Data, brush, msgRect, sf);
                }
            }
        }
    }
}
