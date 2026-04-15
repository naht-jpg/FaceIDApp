using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FaceIDApp.CustomControls;
using FaceIDApp.Theme;

namespace FaceIDApp.UserControls
{
    public partial class UCFaceRegistration : UserControl
    {
        private PictureBox picCamera;
        private Panel pnlCameraCard;
        private Panel pnlImageGrid;
        private RoundedButton btnCapture, btnStartCam, btnRegister, btnClear;
        private ModernComboBox cboEmployee;
        private StatusBadge badgeStatus;
        private Panel pnlProgress;
        private List<Image> capturedImages = new List<Image>();
        private int maxImages = 5;
        private Timer _glowTimer;
        private float _glowPhase;
        private bool _cameraActive;

        public UCFaceRegistration()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            SuspendLayout();
            BackColor = DesignSystem.Background;
            Padding = new Padding(DesignSystem.PaddingOuter);
            AutoScroll = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // ─── Title ─────────────────────────────────────────────
            var pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(50)
            };
            pnlTitle.Paint += (s, e) =>
            {
                DesignSystem.ApplyHighQualityRendering(e.Graphics);
                DesignSystem.DrawSectionTitle(e.Graphics, "Đăng Ký Khuôn Mặt (FaceID)", 0, 0, 50);
            };

            // ─── Top Row: Employee + Camera + Capture ──────────────
            var pnlTopRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(66),
                Padding = new Padding(0, 0, 0, DesignSystem.Scale(10))
            };

            // Employee selection card
            var pnlSelectCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(DesignSystem.CardPadding, DesignSystem.Scale(10), DesignSystem.CardPadding, DesignSystem.Scale(10))
            };
            pnlSelectCard.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlSelectCard.Width - 1, pnlSelectCard.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            var lblSelectTitle = new Label
            {
                Text = "Chọn nhân viên:",
                Font = DesignSystem.Label,
                ForeColor = DesignSystem.TextSecondary,
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(0, DesignSystem.Scale(6), DesignSystem.Scale(8), 0)
            };

            cboEmployee = new ModernComboBox
            {
                Size = new Size(DesignSystem.Scale(220), DesignSystem.InputHeight),
                Dock = DockStyle.Left
            };
            cboEmployee.Items.AddRange(new[] { "NV001 - Nguyễn Văn An", "NV002 - Trần Thị Bình", "NV003 - Lê Văn Cường" });
            if (cboEmployee.Items.Count > 0) cboEmployee.SelectedIndex = 0;

            var spacer1 = new Panel { Dock = DockStyle.Left, Width = DesignSystem.Scale(12) };

            badgeStatus = new StatusBadge
            {
                Text = "Chưa bắt đầu",
                Type = BadgeType.Neutral,
                ShowDot = true,
                Size = new Size(DesignSystem.Scale(130), DesignSystem.Scale(26)),
                Dock = DockStyle.Right
            };

            pnlSelectCard.Controls.Add(badgeStatus);
            pnlSelectCard.Controls.Add(spacer1);
            pnlSelectCard.Controls.Add(cboEmployee);
            pnlSelectCard.Controls.Add(lblSelectTitle);
            pnlTopRow.Controls.Add(pnlSelectCard);

            // ─── Camera + Image Grid ───────────────────────────────
            var pnlMiddle = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(340)
            };

            // Camera card
            int camW = DesignSystem.Scale(420);
            pnlCameraCard = new Panel
            {
                Dock = DockStyle.Left,
                Width = camW,
                Padding = new Padding(DesignSystem.Scale(3))
            };
            pnlCameraCard.Paint += PnlCameraCard_Paint;

            picCamera = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(20, 20, 30)
            };
            picCamera.Paint += PicCamera_Paint;
            pnlCameraCard.Controls.Add(picCamera);

            _glowTimer = new Timer { Interval = 30 };
            _glowTimer.Tick += (s, e) =>
            {
                _glowPhase += 0.06f;
                if (_glowPhase > 2 * Math.PI) _glowPhase -= (float)(2 * Math.PI);
                pnlCameraCard.Invalidate();
            };

            // Right column: image grid + buttons
            var pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(DesignSystem.SectionSpacing, 0, 0, 0)
            };

            var pnlRightCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(DesignSystem.CardPadding)
            };
            pnlRightCard.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlRightCard.Width - 1, pnlRightCard.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            // Image grid title
            var lblGridTitle = new Label
            {
                Text = "Ảnh đã chụp (0/" + maxImages + ")",
                Font = DesignSystem.TitleSection,
                ForeColor = DesignSystem.TextPrimary,
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(30)
            };

            // Image grid
            pnlImageGrid = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, DesignSystem.Scale(8), 0, 0)
            };

            // Placeholders
            for (int i = 0; i < maxImages; i++)
            {
                var placeholder = CreateImageSlot(i);
                pnlImageGrid.Controls.Add(placeholder);
            }

            // Buttons
            var pnlBtns = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = DesignSystem.Scale(52),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, DesignSystem.Scale(8), 0, 0)
            };

            btnStartCam = new RoundedButton
            {
                Text = "Mở Camera",
                ButtonIcon = "▶",
                Variant = ButtonVariant.Primary,
                Size = new Size(DesignSystem.Scale(130), DesignSystem.ButtonHeight),
                Margin = new Padding(0, 0, DesignSystem.Scale(8), 0)
            };
            btnStartCam.Click += (s, e) => StartCamera();

            btnCapture = new RoundedButton
            {
                Text = "Chụp",
                ButtonIcon = "📸",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(100), DesignSystem.ButtonHeight),
                Enabled = false,
                Margin = new Padding(0, 0, DesignSystem.Scale(8), 0)
            };
            btnCapture.Click += (s, e) => CaptureImage();

            btnClear = new RoundedButton
            {
                Text = "Xóa",
                Variant = ButtonVariant.Outline,
                Size = new Size(DesignSystem.Scale(80), DesignSystem.ButtonHeight),
                Margin = new Padding(0, 0, DesignSystem.Scale(8), 0)
            };
            btnClear.Click += (s, e) => ClearImages();

            pnlBtns.Controls.AddRange(new Control[] { btnStartCam, btnCapture, btnClear });

            pnlRightCard.Controls.Add(pnlImageGrid);
            pnlRightCard.Controls.Add(lblGridTitle);
            pnlRightCard.Controls.Add(pnlBtns);

            pnlRight.Controls.Add(pnlRightCard);
            pnlMiddle.Controls.Add(pnlRight);
            pnlMiddle.Controls.Add(pnlCameraCard);

            // ─── Progress Section ──────────────────────────────────
            var pnlProgressSection = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(100),
                Padding = new Padding(0, DesignSystem.SectionSpacing, 0, 0)
            };

            var pnlProgressCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(DesignSystem.CardPadding)
            };
            pnlProgressCard.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, pnlProgressCard.Width - 1, pnlProgressCard.Height - 1);
                DesignSystem.DrawElevatedSurface(e.Graphics, rect, DesignSystem.BorderRadiusLg);
            };

            pnlProgress = new Panel
            {
                Dock = DockStyle.Top,
                Height = DesignSystem.Scale(30)
            };
            pnlProgress.Paint += PnlProgress_Paint;

            btnRegister = new RoundedButton
            {
                Text = "Đăng ký FaceID",
                ButtonIcon = "✓",
                Variant = ButtonVariant.Success,
                Size = new Size(DesignSystem.Scale(180), DesignSystem.ButtonHeight),
                Enabled = false,
                Dock = DockStyle.Bottom
            };
            btnRegister.Click += (s, e) =>
            {
                badgeStatus.SetStatus("Đã đăng ký!", BadgeType.Success);
                MessageBox.Show("Đăng ký FaceID thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pnlProgressCard.Controls.Add(pnlProgress);
            pnlProgressCard.Controls.Add(btnRegister);
            pnlProgressSection.Controls.Add(pnlProgressCard);

            // Build order
            Controls.Add(pnlProgressSection);
            Controls.Add(pnlMiddle);
            Controls.Add(pnlTopRow);
            Controls.Add(pnlTitle);

            Disposed += (s, e) =>
            {
                _glowTimer?.Stop();
                _glowTimer?.Dispose();
            };
            ResumeLayout(false);
        }

        private Panel CreateImageSlot(int index)
        {
            int slotSize = DesignSystem.Scale(80);
            var slot = new Panel
            {
                Size = new Size(slotSize, slotSize),
                Margin = new Padding(0, 0, DesignSystem.Scale(8), DesignSystem.Scale(8)),
                BackColor = Color.Transparent
            };

            slot.Paint += (s, e) =>
            {
                var g = e.Graphics;
                DesignSystem.ApplyHighQualityRendering(g);

                var rect = new Rectangle(0, 0, slotSize - 1, slotSize - 1);
                int radius = DesignSystem.BorderRadius;

                bool hasImage = index < capturedImages.Count;
                if (hasImage)
                {
                    // Fill with image (clipped to rounded rect)
                    using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                    {
                        g.SetClip(path);
                        g.DrawImage(capturedImages[index], rect);
                        g.ResetClip();
                        using (var border = new Pen(DesignSystem.Accent, DesignSystem.Scale(1.5f)))
                            g.DrawPath(border, path);
                    }

                    // Checkmark
                    int checkSize = DesignSystem.Scale(18);
                    var checkRect = new Rectangle(rect.Right - checkSize - 2, 2, checkSize, checkSize);
                    using (var bg = new SolidBrush(DesignSystem.Success))
                        g.FillEllipse(bg, checkRect);
                    using (var brush = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString("✓", DesignSystem.Small, brush, checkRect, sf);
                    }
                }
                else
                {
                    // Empty placeholder
                    using (var path = DesignSystem.CreateRoundedRect(rect, radius))
                    {
                        using (var fill = new SolidBrush(DesignSystem.SurfaceAlt))
                            g.FillPath(fill, path);
                        using (var border = new Pen(DesignSystem.BorderLight, 1f) { DashStyle = DashStyle.Dash })
                            g.DrawPath(border, path);
                    }

                    using (var brush = new SolidBrush(DesignSystem.TextMuted))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString($"{index + 1}", DesignSystem.Label, brush, rect, sf);
                    }
                }
            };

            return slot;
        }

        private void PnlProgress_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            int total = maxImages;
            int captured = capturedImages.Count;
            float progress = (float)captured / total;

            int barH = DesignSystem.Scale(10);
            int barY = (pnlProgress.Height - barH) / 2;
            var bgRect = new Rectangle(0, barY, pnlProgress.Width - 1, barH);

            // Background
            using (var path = DesignSystem.CreateRoundedRect(bgRect, barH / 2))
            using (var brush = new SolidBrush(Color.FromArgb(20, DesignSystem.Accent)))
                g.FillPath(brush, path);

            // Fill
            if (progress > 0)
            {
                int fillW = Math.Max(DesignSystem.Scale(10), (int)(bgRect.Width * progress));
                var fillRect = new Rectangle(bgRect.X, bgRect.Y, fillW, bgRect.Height);
                var fillColor = captured >= total ? DesignSystem.Success : DesignSystem.Accent;

                using (var path = DesignSystem.CreateRoundedRect(fillRect, barH / 2))
                using (var brush = DesignSystem.CreateHorizontalGradient(fillRect, fillColor, DesignSystem.AccentLight))
                    g.FillPath(brush, path);
            }

            // Label
            string label = $"{captured}/{total} ảnh đã chụp";
            using (var brush = new SolidBrush(DesignSystem.TextSecondary))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(label, DesignSystem.Small, brush, new RectangleF(0, 0, pnlProgress.Width, pnlProgress.Height), sf);
            }
        }

        private void PnlCameraCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            int radius = DesignSystem.BorderRadiusLg;
            var rect = new Rectangle(0, 0, pnlCameraCard.Width - 1, pnlCameraCard.Height - 1);

            DesignSystem.DrawFluentShadow(g, rect, radius);

            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var bg = new SolidBrush(Color.FromArgb(20, 20, 30)))
                g.FillPath(bg, path);

            if (_cameraActive)
            {
                float glowIntensity = (float)(Math.Sin(_glowPhase) * 0.3 + 0.5);
                int alpha = (int)(60 * glowIntensity);
                for (int i = 3; i >= 1; i--)
                {
                    var glowRect = new Rectangle(rect.X - i, rect.Y - i, rect.Width + i * 2, rect.Height + i * 2);
                    using (var pen = new Pen(Color.FromArgb(alpha / i, DesignSystem.Accent), 1.5f))
                    using (var path = DesignSystem.CreateRoundedRect(glowRect, radius + i))
                        g.DrawPath(pen, path);
                }
            }

            Color borderColor = _cameraActive ? Color.FromArgb(90, DesignSystem.Accent) : Color.FromArgb(40, DesignSystem.Border);
            using (var path = DesignSystem.CreateRoundedRect(rect, radius))
            using (var pen = new Pen(borderColor, 1.5f))
                g.DrawPath(pen, path);
        }

        private void PicCamera_Paint(object sender, PaintEventArgs e)
        {
            if (picCamera.Image != null) return;
            var g = e.Graphics;
            DesignSystem.ApplyHighQualityRendering(g);

            g.Clear(Color.FromArgb(20, 20, 30));

            using (var brush = new SolidBrush(Color.FromArgb(80, DesignSystem.TextMuted)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var iconFont = new Font("Segoe UI", DesignSystem.Scale(48), GraphicsUnit.Pixel);
                g.DrawString("📷", iconFont, brush, picCamera.ClientRectangle, sf);
                iconFont.Dispose();
            }

            using (var brush = new SolidBrush(Color.FromArgb(100, DesignSystem.TextMuted)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var textRect = new RectangleF(0, picCamera.Height / 2f + DesignSystem.Scale(30), picCamera.Width, DesignSystem.Scale(28));
                g.DrawString("Nhấn [Mở Camera] để bắt đầu", DesignSystem.Data, brush, textRect, sf);
            }
        }

        private void StartCamera()
        {
            _cameraActive = true;
            btnStartCam.Enabled = false;
            btnCapture.Enabled = true;
            badgeStatus.SetStatus("Đang chụp...", BadgeType.Info);
            _glowTimer.Start();
            pnlCameraCard.Invalidate();
        }

        private void CaptureImage()
        {
            if (capturedImages.Count >= maxImages)
            {
                MessageBox.Show("Đã đủ ảnh!", "Thông báo");
                return;
            }

            // Capture from camera (placeholder using generated image)
            var bmp = new Bitmap(DesignSystem.Scale(80), DesignSystem.Scale(80));
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(50, 60, 80));
                using (var brush = new SolidBrush(Color.FromArgb(150, DesignSystem.Accent)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("👤", new Font("Segoe UI", 30, GraphicsUnit.Pixel), brush, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
                }
            }

            capturedImages.Add(bmp);

            // Refresh image slots
            for (int i = 0; i < pnlImageGrid.Controls.Count; i++)
                pnlImageGrid.Controls[i].Invalidate();

            pnlProgress.Invalidate();

            // Update grid title
            foreach (Control c in pnlImageGrid.Parent.Controls)
            {
                if (c is Label lbl && lbl.Text.Contains("Ảnh đã chụp"))
                {
                    lbl.Text = $"Ảnh đã chụp ({capturedImages.Count}/{maxImages})";
                    break;
                }
            }

            if (capturedImages.Count >= maxImages)
            {
                btnCapture.Enabled = false;
                btnRegister.Enabled = true;
                badgeStatus.SetStatus("Đủ ảnh — Sẵn sàng đăng ký", BadgeType.Success);
            }
        }

        private void ClearImages()
        {
            foreach (var img in capturedImages) img.Dispose();
            capturedImages.Clear();

            for (int i = 0; i < pnlImageGrid.Controls.Count; i++)
                pnlImageGrid.Controls[i].Invalidate();

            pnlProgress.Invalidate();
            btnRegister.Enabled = false;
            if (_cameraActive) btnCapture.Enabled = true;
            badgeStatus.SetStatus(_cameraActive ? "Đang chụp..." : "Chưa bắt đầu", _cameraActive ? BadgeType.Info : BadgeType.Neutral);

            foreach (Control c in pnlImageGrid.Parent.Controls)
            {
                if (c is Label lbl && lbl.Text.Contains("Ảnh đã chụp"))
                {
                    lbl.Text = $"Ảnh đã chụp (0/{maxImages})";
                    break;
                }
            }
        }
    }
}
