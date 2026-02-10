// ScreenShotLite, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// ScreenShotLite.Main
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using ScreenShotLite;

public class Main : Form
{
    private delegate bool EnumWindowsProc(nint hWnd, nint lParam);

    private struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;
    }

    private sealed class WindowItem
    {
        public nint Hwnd { get; }

        public string Title { get; }

        public WindowItem(nint hWnd, string title)
        {
            Hwnd = hWnd;
            Title = title;
        }

        public override string ToString()
        {
            return $"{Title} (0x{((IntPtr)Hwnd).ToInt64():X})";
        }
    }

    private sealed class ScreenItem
    {
        public int Index { get; }

        public Rectangle Bounds { get; }

        public string Name { get; }

        public ScreenItem(int index, Screen s)
        {
            Index = index;
            Bounds = s.Bounds;
            Name = s.DeviceName;
        }

        public override string ToString()
        {
            return $"显示器 {Index}: {Name} {Bounds.Width}x{Bounds.Height} @ {Bounds.Location}";
        }
    }

    private sealed class SelectionForm : Form
    {
        private Point _start;

        private Point _current;

        private bool _selecting;
        private readonly Image? _backgroundImage;

        public Rectangle SelectedRectangle { get; private set; }

        // backgroundImage: if provided, used as frozen background
        public SelectionForm(Image? backgroundImage = null)
        {
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = GetVirtualScreenBounds();
            _backgroundImage = backgroundImage;

            if (_backgroundImage != null)
            {
                BackgroundImage = _backgroundImage;
                BackgroundImageLayout = ImageLayout.None;
                Opacity = 1.0;
            }
            else
            {
                BackColor = Color.Black;
                Opacity = 0.25;
            }

            TopMost = true;
            ShowInTaskbar = false;
            Cursor = Cursors.Cross;

            MouseDown += SelectionForm_MouseDown;
            MouseMove += SelectionForm_MouseMove;
            MouseUp += SelectionForm_MouseUp;
            KeyDown += SelectionForm_KeyDown;
        }

        private void SelectionForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                base.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void SelectionForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 进入拖拽选择模式
                _selecting = true;
                _start = e.Location;
                _current = e.Location;
                SelectedRectangle = Rectangle.Empty;
            }
        }

        private void SelectionForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_selecting)
            {
                _current = e.Location;
                Invalidate();
            }
        }

        private void SelectionForm_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _selecting = false;
                Rectangle r = GetRectangle(_start, _current);
                r.Offset(Bounds.Location);

                // normal behavior: use dragged rectangle

                SelectedRectangle = r;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_selecting)
            {
                Rectangle r = GetRectangle(_start, _current);
                using Pen pen = new Pen(Color.Red, 2f);
                e.Graphics.DrawRectangle(pen, r);
            }
        }

        private static Rectangle GetRectangle(Point a, Point b)
        {
            return new Rectangle(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _backgroundImage?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }

    private System.Timers.Timer? _autoTimer;

    private NotifyIcon? _notifyIcon;

    private ContextMenuStrip? _notifyMenu;

    private ContextMenuStrip? _logContextMenu;

    private string? _lastNotificationFile;

    private bool _hiddenToTray = false;

    private bool _selectionInProgress = false;

    private readonly object _selectionLock = new object();

    private const int HOTKEY_ID = 36864;

    private const uint MOD_ALT = 1u;

    private const uint MOD_CONTROL = 2u;

    private const uint MOD_SHIFT = 4u;

    private const uint MOD_WIN = 8u;

    private const int WM_HOTKEY = 786;

    private uint _currentHotkeyModifiers = 6u;

    private Keys _currentHotkeyKey = Keys.S;

    private const uint SRCCOPY = 13369376u;

    private const uint KEYEVENTF_KEYUP = 2u;

    private const byte VK_SNAPSHOT = 44;

    private const byte VK_MENU = 18;

    private const int SW_RESTORE = 9;

    private IContainer components = null;

    private Label Title;

    private Label SaveLocationLabel;

    private Panel Panel;

    private CheckBox checkBox2;

    private CheckBox checkBox1;

    private Button button2;

    private Button button1;

    private TextBox textBox1;

    private TextBox textBox2;

    private NumericUpDown numericUpDown1;

    private ComboBox comboBox1;

    private CheckBox checkBox4;

    private CheckBox checkBox3;

    private Button button3;

    private CheckBox checkBox5;

    private RadioButton radioButton3;

    private RadioButton radioButton2;

    private RadioButton radioButton1;

    private ComboBox comboBox2;

    private CheckBox checkBox6;

    private NumericUpDown numericUpDown2;

    private RichTextBox richTextBox1;

    private Label label1;

    private Panel panel1;

    private Button button4;

    private Button button8;

    private Button button7;

    private CheckBox checkBox7;

    private Label label3;

    private Label label2;

    private Button button6;

    private Button button5;
    private CheckBox checkBox9;
    private CheckBox checkBox8;

    public Main()
    {
        InitializeComponent();
        button1.Click += Button1_Click;
        button2.Click += Button2_Click;
        button3.Click += Button3_Click;
        button4.Click += Button4_Click;
        button5.Click += Button5_Click;
        button6.Click += Button6_Click;
        comboBox1.DropDown += delegate
        {
            PopulateTopLevelWindows();
        };
        comboBox2.DropDown += delegate
        {
            PopulateScreens();
        };
        textBox2.KeyDown += TextBox2_KeyDown;
        checkBox4.CheckedChanged += CheckBox4_CheckedChanged;
        base.FormClosing += Form1_FormClosing;
        base.HandleCreated += Form1_HandleCreated;
        base.HandleDestroyed += Form1_HandleDestroyed;
        base.Activated += Form1_Activated;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        if (string.IsNullOrWhiteSpace(textBox1.Text))
        {
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
        textBox2.Text = HotkeyToString(_currentHotkeyModifiers, _currentHotkeyKey);
        try
        {
            _notifyMenu = new ContextMenuStrip();
            ToolStripMenuItem miCapture = new ToolStripMenuItem("截图");
            miCapture.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        Invoke(delegate
                        {
                            try
                            {
                                string text = CaptureAndSave(forceSingle: true);
                                if (!string.IsNullOrEmpty(text))
                                {
                                    Log("托盘截图已保存：" + text);
                                }
                            }
                            catch (Exception value)
                            {
                                Log($"托盘截图失败：{value}");
                            }
                        });
                    }
                    catch (Exception ex2)
                    {
                        Exception ex3 = ex2;
                        Exception ex4 = ex3;
                        try
                        {
                            Invoke(delegate
                            {
                                Log($"启动托盘截图任务失败：{ex4}");
                            });
                        }
                        catch
                        {
                        }
                    }
                });
            };
            ToolStripMenuItem miShow = new ToolStripMenuItem("打开界面");
            miShow.Click += delegate
            {
                try
                {
                    _hiddenToTray = false;
                    if (!base.Visible || base.WindowState == FormWindowState.Minimized)
                    {
                        Show();
                        base.WindowState = FormWindowState.Normal;
                    }
                    BringToFront();
                    Activate();
                }
                catch
                {
                }
            };
            _notifyMenu.Items.Add(miCapture);
            _notifyMenu.Items.Add(miShow);
            ToolStripMenuItem miExit = new ToolStripMenuItem("关闭");
            miExit.Click += delegate
            {
                try
                {
                    Application.Exit();
                }
                catch
                {
                    try
                    {
                        Environment.Exit(0);
                    }
                    catch
                    {
                    }
                }
            };
            _notifyMenu.Items.Add(new ToolStripSeparator());
            _notifyMenu.Items.Add(miExit);
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = base.Icon;
            _notifyIcon.Text = "ScreenShotLite";
            _notifyIcon.ContextMenuStrip = _notifyMenu;
            _notifyIcon.Visible = true;
            _notifyIcon.BalloonTipClicked += delegate
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(_lastNotificationFile) && File.Exists(_lastNotificationFile))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = _lastNotificationFile,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex2)
                {
                    try
                    {
                        Log("打开通知文件失败：" + ex2.Message);
                    }
                    catch
                    {
                    }
                }
            };
            _notifyIcon.DoubleClick += delegate
            {
                try
                {
                    _hiddenToTray = false;
                    if (!base.Visible || base.WindowState == FormWindowState.Minimized)
                    {
                        Show();
                        base.WindowState = FormWindowState.Normal;
                    }
                    BringToFront();
                    Activate();
                }
                catch
                {
                }
            };
            base.Resize += delegate
            {
                try
                {
                    if (base.WindowState == FormWindowState.Minimized && _hiddenToTray)
                    {
                        Hide();
                        Log("已最小化到系统托盘。双击图标或右击选择“打开界面”以恢复。");
                    }
                }
                catch
                {
                }
            };
            _logContextMenu = new ContextMenuStrip();
            ToolStripMenuItem miClearLog = new ToolStripMenuItem("清除日志");
            miClearLog.Click += delegate
            {
                try
                {
                    richTextBox1.Clear();
                    Log("日志已清除。");
                }
                catch (Exception value)
                {
                    Log($"清除日志失败：{value}");
                }
            };
            _logContextMenu.Items.Add(miClearLog);
            richTextBox1.ContextMenuStrip = _logContextMenu;
        }
        catch (Exception ex)
        {
            Log("初始化托盘图标失败：" + ex.Message);
        }
    }

    private void Form1_Activated(object? sender, EventArgs e)
    {
        PopulateTopLevelWindows();
        PopulateScreens();
    }

    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        try
        {
            if (!base.IsDisposed)
            {
                BeginInvoke(PopulateScreens);
            }
        }
        catch
        {
        }
    }

    private void Form1_HandleCreated(object? sender, EventArgs e)
    {
        if (checkBox4.Checked)
        {
            RegisterCurrentHotkey();
        }
    }

    private void Form1_HandleDestroyed(object? sender, EventArgs e)
    {
        UnregisterHotKey(base.Handle, 36864);
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _autoTimer?.Stop();
        _autoTimer?.Dispose();
        UnregisterHotKey(base.Handle, 36864);
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        try
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
        catch
        {
        }
        try
        {
            _notifyMenu?.Dispose();
            _notifyMenu = null;
        }
        catch
        {
        }
        try
        {
            _logContextMenu?.Dispose();
            _logContextMenu = null;
        }
        catch
        {
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 786 && ((IntPtr)m.WParam).ToInt32() == 36864)
        {
            OnHotKeyPressed();
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    private void CheckBox4_CheckedChanged(object? sender, EventArgs e)
    {
        if (checkBox4.Checked)
        {
            RegisterCurrentHotkey();
            Log("已启用热键：" + textBox2.Text);
        }
        else
        {
            UnregisterHotKey(base.Handle, 36864);
            Log("已禁用热键。");
        }
    }

    private void TextBox2_KeyDown(object? sender, KeyEventArgs e)
    {
        e.SuppressKeyPress = true;
        e.Handled = true;
        if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu && e.KeyCode != Keys.LWin && e.KeyCode != Keys.RWin)
        {
            uint mods = 0u;
            if (e.Control)
            {
                mods |= 2;
            }
            if (e.Alt)
            {
                mods |= 1;
            }
            if (e.Shift)
            {
                mods |= 4;
            }
            if ((Control.ModifierKeys & Keys.LWin) == Keys.LWin || (Control.ModifierKeys & Keys.RWin) == Keys.RWin)
            {
                mods |= 8;
            }
            _currentHotkeyModifiers = mods;
            _currentHotkeyKey = e.KeyCode;
            textBox2.Text = HotkeyToString(_currentHotkeyModifiers, _currentHotkeyKey);
            if (checkBox4.Checked)
            {
                RegisterCurrentHotkey();
                Log("热键已更新为：" + textBox2.Text);
            }
        }
    }

    private void RegisterCurrentHotkey()
    {
        try
        {
            UnregisterHotKey(base.Handle, 36864);
            if (!RegisterHotKey(base.Handle, 36864, _currentHotkeyModifiers, _currentHotkeyKey))
            {
                Log("注册热键失败（可能被其他程序占用）。");
                MessageBox.Show(this, "注册热键失败（可能被其他程序占用）。", "热键注册失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        catch (Exception value)
        {
            Log($"注册热键异常：{value}");
        }
    }

    private static string HotkeyToString(uint mods, Keys key)
    {
        List<string> parts = new List<string>();
        if ((mods & 2) != 0)
        {
            parts.Add("Ctrl");
        }
        if ((mods & 1) != 0)
        {
            parts.Add("Alt");
        }
        if ((mods & 4) != 0)
        {
            parts.Add("Shift");
        }
        if ((mods & 8) != 0)
        {
            parts.Add("Win");
        }
        parts.Add(key.ToString());
        return string.Join("+", parts);
    }

    private void OnHotKeyPressed()
    {
        try
        {
            if (!_hiddenToTray && (!base.Visible || base.WindowState == FormWindowState.Minimized))
            {
                Show();
                base.WindowState = FormWindowState.Normal;
                BringToFront();
                Activate();
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            Log("热键触发：开始截图。");
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    Invoke(delegate
                    {
                        try
                        {
                            string text = CaptureAndSave(forceSingle: true);
                            if (!string.IsNullOrEmpty(text))
                            {
                                Log("热键截图已保存：" + text);
                            }
                        }
                        catch (Exception value2)
                        {
                            Log($"热键截图失败：{value2}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    Exception ex3 = ex2;
                    Invoke(delegate
                    {
                        Log($"启动热键截图任务失败：{ex3}");
                    });
                }
            });
        }
        catch (Exception value)
        {
            Log($"处理热键失败：{value}");
        }
    }

    private void Button1_Click(object? sender, EventArgs e)
    {
        using FolderBrowserDialog dlg = new FolderBrowserDialog();
        if (!string.IsNullOrWhiteSpace(textBox1.Text) && Directory.Exists(textBox1.Text))
        {
            dlg.SelectedPath = textBox1.Text;
        }
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            textBox1.Text = dlg.SelectedPath;
            Log("已选择保存目录：" + dlg.SelectedPath);
        }
    }

    private void Button2_Click(object? sender, EventArgs e)
    {
        string path = textBox1.Text;
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            MessageBox.Show(this, "保存目录不存在，请先选择有效目录。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "无法打开目录：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            Log($"打开目录失败：{ex}");
        }
    }

    private void Button3_Click(object? sender, EventArgs e)
    {
        if (checkBox6.Checked)
        {
            StartAutoCapture();
        }
        else
        {
            StartSingleCapture();
        }
    }

    private void StartSingleCapture()
    {
        int delaySeconds = (checkBox3.Checked ? ((int)numericUpDown1.Value) : 0);
        if (delaySeconds > 0)
        {
            Log($"开始倒计时：{delaySeconds} 秒...");
        }
        ThreadPool.QueueUserWorkItem(delegate
        {
            try
            {
                if (delaySeconds > 0)
                {
                    Thread.Sleep(delaySeconds * 1000);
                }
                Invoke(delegate
                {
                    try
                    {
                        string text = CaptureAndSave();
                        if (!string.IsNullOrEmpty(text))
                        {
                            Log("截图已保存：" + text);
                        }
                    }
                    catch (Exception value)
                    {
                        Log($"截图失败：{value}");
                    }
                });
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                Exception ex3 = ex2;
                Invoke(delegate
                {
                    Log($"启动截图任务失败：{ex3}");
                });
            }
        });
    }

    private void StartAutoCapture()
    {
        int intervalSeconds = (int)numericUpDown2.Value;
        if (intervalSeconds <= 0)
        {
            MessageBox.Show(this, "请设置大于 0 的自动截图间隔。", "无效间隔", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }
        if (_autoTimer != null)
        {
            _autoTimer.Stop();
            _autoTimer.Dispose();
            _autoTimer = null;
        }
        _autoTimer = new System.Timers.Timer(intervalSeconds * 1000);
        _autoTimer.Elapsed += AutoTimer_Elapsed;
        _autoTimer.AutoReset = true;
        _autoTimer.Start();
        Log($"已开始自动截图：间隔 {intervalSeconds} 秒。再次点击可停止。");
        button3.Click -= Button3_Click;
        EventHandler stopHandler = null;
        stopHandler = delegate
        {
            _autoTimer?.Stop();
            _autoTimer?.Dispose();
            _autoTimer = null;
            Log("已停止自动截图。");
            button3.Click -= stopHandler;
            button3.Click += Button3_Click;
        };
        button3.Click += stopHandler;
    }

    private void AutoTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            lock (_selectionLock)
            {
                if (_selectionInProgress)
                {
                    try
                    {
                        BeginInvoke(delegate
                        {
                            Log("上一次框选未完成，跳过自动截图。");
                        });
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            Invoke(delegate
            {
                try
                {
                    string text = CaptureAndSave();
                    if (!string.IsNullOrEmpty(text))
                    {
                        Log("自动截图已保存：" + text);
                    }
                }
                catch (Exception value)
                {
                    Log($"自动截图失败：{value}");
                }
            });
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private string? CaptureAndSave(bool forceSingle = false)
    {
        string folder = textBox1.Text;
        if (string.IsNullOrWhiteSpace(folder))
        {
            folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            textBox1.Text = folder;
        }
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        bool wasVisible = base.Visible;
        if (checkBox1.Checked)
        {
            base.Visible = false;
            Application.DoEvents();
            Thread.Sleep(200);
        }
        Bitmap bmp = null;
        nint usedWindow = IntPtr.Zero;
        Rectangle? usedScreenBounds = null;
        try
        {
            if (radioButton2.Checked)
            {
                Rectangle bounds = ((!(comboBox2.SelectedItem is ScreenItem sitem)) ? GetVirtualScreenBounds() : sitem.Bounds);
                usedScreenBounds = bounds;
                bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                using Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
            }
            else if (radioButton1.Checked)
            {
                nint hWnd = IntPtr.Zero;
                hWnd = ((!(comboBox1.SelectedItem is WindowItem witem)) ? GetForegroundWindow() : witem.Hwnd);
                Rectangle rect = GetWindowRectOrEmpty(hWnd);
                if (rect.Width <= 0 || rect.Height <= 0 || hWnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("无法获取目标窗口句柄或尺寸。");
                }
                usedWindow = hWnd;
                bmp = TryCaptureWindowRobust(hWnd, rect);
                if (bmp == null)
                {
                    throw new InvalidOperationException("窗口捕获失败（所有方法均未成功）。");
                }
            }
            else
            {
                bool isAuto = _autoTimer != null && _autoTimer.Enabled;
                if (isAuto)
                {
                    lock (_selectionLock)
                    {
                        if (_selectionInProgress)
                        {
                            Log("上一次框选未完成，跳过此次自动框选。");
                            return null;
                        }
                        _selectionInProgress = true;
                    }
                }
                try
                {
                    Image? bg = null;
                    // 如果勾选了 checkBox9（截图时使画面静止），先截取虚拟屏幕作为背景
                    try
                    {
                        if (checkBox9 != null && checkBox9.Checked)
                        {
                            var vb = GetVirtualScreenBounds();
                            bg = new Bitmap(vb.Width, vb.Height, PixelFormat.Format32bppArgb);
                            using (var gg = Graphics.FromImage(bg))
                            {
                                gg.CopyFromScreen(vb.Location, Point.Empty, vb.Size, CopyPixelOperation.SourceCopy);
                            }
                        }
                    }
                    catch { bg?.Dispose(); bg = null; }

                    using SelectionForm sel = new SelectionForm(bg);
                    if (sel.ShowDialog(this) == DialogResult.OK)
                    {
                        Rectangle r = sel.SelectedRectangle;
                        if (r.Width <= 0 || r.Height <= 0)
                        {
                            throw new InvalidOperationException("未选择有效区域。");
                        }
                        bmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
                        using Graphics g2 = Graphics.FromImage(bmp);
                        g2.CopyFromScreen(r.Location, Point.Empty, r.Size, CopyPixelOperation.SourceCopy);
                    }
                    else
                    {
                        Log("框选取消。");
                    }
                }
                finally
                {
                    if (isAuto)
                    {
                        lock (_selectionLock)
                        {
                            _selectionInProgress = false;
                        }
                    }
                }
            }
            if (bmp == null)
            {
                Log("未生成截图。");
                return null;
            }
            string filename = Path.Combine(folder, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");
            bmp.Save(filename, ImageFormat.Png);
            _lastNotificationFile = filename;
            if (checkBox2.Checked)
            {
                try
                {
                    Clipboard.SetImage(bmp);
                    Log("已复制到剪切板。");
                }
                catch (Exception ex)
                {
                    Log("复制到剪切板失败：" + ex.Message);
                }
            }
            if (checkBox5.Checked)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filename,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex2)
                {
                    Log("打开图片失败：" + ex2.Message);
                }
            }
            try
            {
                if (checkBox8.Checked && _notifyIcon != null && !base.IsDisposed && base.IsHandleCreated)
                {
                    BeginInvoke(delegate
                    {
                        try
                        {
                            _notifyIcon.ShowBalloonTip(3000, "截图已保存", Path.GetFileName(filename), ToolTipIcon.Info);
                        }
                        catch
                        {
                        }
                    });
                }
            }
            catch
            {
            }
            try
            {
                if (usedWindow != IntPtr.Zero)
                {
                    SelectWindowInCombo(usedWindow);
                }
                if (usedScreenBounds.HasValue)
                {
                    SelectScreenInCombo(usedScreenBounds.Value);
                }
            }
            catch (Exception ex3)
            {
                Log("更新 ComboBox 选择失败：" + ex3.Message);
            }
            return filename;
        }
        finally
        {
            bmp?.Dispose();
            if (checkBox1.Checked && wasVisible)
            {
                base.Visible = true;
            }
        }
    }

    private void Button6_Click(object? sender, EventArgs e)
    {
        try
        {
            if (base.Visible && base.WindowState != FormWindowState.Minimized)
            {
                Hide();
                base.WindowState = FormWindowState.Minimized;
                _hiddenToTray = true;
                Log("已隐藏到托盘。右击托盘图标或双击以打开界面。");
            }
        }
        catch (Exception value)
        {
            Log($"隐藏到托盘失败：{value}");
        }
    }

    private void Button5_Click(object? sender, EventArgs e)
    {
        try
        {
            richTextBox1.Clear();
            Log("日志已清除。");
        }
        catch (Exception value)
        {
            Log($"清除日志失败：{value}");
        }
    }

    private void SelectWindowInCombo(nint hWnd)
    {
        if (hWnd == IntPtr.Zero)
        {
            return;
        }
        for (int i = 0; i < comboBox1.Items.Count; i++)
        {
            if (comboBox1.Items[i] is WindowItem wi && wi.Hwnd == hWnd)
            {
                comboBox1.SelectedIndex = i;
                return;
            }
        }
        int len = GetWindowTextLength(hWnd);
        string title;
        if (len > 0)
        {
            StringBuilder sb = new StringBuilder(len + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            title = sb.ToString();
        }
        else
        {
            title = $"窗口 0x{((IntPtr)hWnd).ToInt64():X}";
        }
        WindowItem item = new WindowItem(hWnd, title);
        comboBox1.Items.Insert(0, item);
        comboBox1.SelectedIndex = 0;
    }

    private void SelectScreenInCombo(Rectangle bounds)
    {
        for (int i = 0; i < comboBox2.Items.Count; i++)
        {
            if (comboBox2.Items[i] is ScreenItem si && si.Bounds == bounds)
            {
                comboBox2.SelectedIndex = i;
                return;
            }
        }
        PopulateScreens();
        for (int j = 0; j < comboBox2.Items.Count; j++)
        {
            if (comboBox2.Items[j] is ScreenItem si2 && si2.Bounds == bounds)
            {
                comboBox2.SelectedIndex = j;
                return;
            }
        }
        if (comboBox2.Items.Count > 0)
        {
            comboBox2.SelectedIndex = 0;
        }
    }

    private Bitmap? TryCaptureWindowRobust(nint hWnd, Rectangle rect)
    {
        Bitmap bmp = null;
        bmp = TryPrintWindow(hWnd, rect, 2);
        if (bmp != null && !IsMostlyBlank(bmp))
        {
            return bmp;
        }
        bmp?.Dispose();
        bmp = TryPrintWindow(hWnd, rect, 0);
        if (bmp != null && !IsMostlyBlank(bmp))
        {
            return bmp;
        }
        bmp?.Dispose();
        try
        {
            Bitmap bt = TryBitBltWindow(hWnd, rect);
            if (bt != null && !IsMostlyBlank(bt))
            {
                return bt;
            }
            bt?.Dispose();
        }
        catch (Exception ex)
        {
            Log("BitBlt 捕获异常：" + ex.Message);
        }
        try
        {
            Bitmap altBmp = TryAltPrintScreenCapture(hWnd, rect);
            if (altBmp != null && !IsMostlyBlank(altBmp))
            {
                return altBmp;
            }
            altBmp?.Dispose();
        }
        catch (Exception ex2)
        {
            Log("Alt+PrtSc 捕获异常：" + ex2.Message);
        }
        return null;
    }

    private Bitmap? TryPrintWindow(nint hWnd, Rectangle rect, int flags)
    {
        Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        try
        {
            using Graphics g = Graphics.FromImage(bmp);
            nint hdc = g.GetHdc();
            try
            {
                bool ok = PrintWindow(hWnd, hdc, flags);
                g.ReleaseHdc(hdc);
                if (!ok)
                {
                    return bmp;
                }
                return bmp;
            }
            catch
            {
                try
                {
                    g.ReleaseHdc(hdc);
                }
                catch
                {
                }
                return bmp;
            }
        }
        catch
        {
            bmp.Dispose();
            return null;
        }
    }

    private Bitmap? TryBitBltWindow(nint hWnd, Rectangle rect)
    {
        nint hWndDC = IntPtr.Zero;
        nint hMemDC = IntPtr.Zero;
        nint hBitmap = IntPtr.Zero;
        nint hOld = IntPtr.Zero;
        try
        {
            hWndDC = GetWindowDC(hWnd);
            if (hWndDC == IntPtr.Zero)
            {
                return null;
            }
            hMemDC = CreateCompatibleDC(hWndDC);
            if (hMemDC == IntPtr.Zero)
            {
                return null;
            }
            hBitmap = CreateCompatibleBitmap(hWndDC, rect.Width, rect.Height);
            if (hBitmap == IntPtr.Zero)
            {
                return null;
            }
            hOld = SelectObject(hMemDC, hBitmap);
            bool success = BitBlt(hMemDC, 0, 0, rect.Width, rect.Height, hWndDC, rect.Left - rect.Left, rect.Top - rect.Top, 13369376u);
            SelectObject(hMemDC, hOld);
            if (!success)
            {
                return null;
            }
            return Image.FromHbitmap(hBitmap);
        }
        finally
        {
            if (hBitmap != IntPtr.Zero)
            {
                DeleteObject(hBitmap);
            }
            if (hMemDC != IntPtr.Zero)
            {
                DeleteDC(hMemDC);
            }
            if (hWndDC != IntPtr.Zero)
            {
                ReleaseDC(hWnd, hWndDC);
            }
        }
    }

    private Bitmap? TryAltPrintScreenCapture(nint hWnd, Rectangle rect)
    {
        if (hWnd == IntPtr.Zero)
        {
            return null;
        }
        nint originalForeground = GetForegroundWindow();
        try
        {
            ShowWindow(hWnd, 9);
            SetForegroundWindow(hWnd);
            Application.DoEvents();
            Thread.Sleep(150);
            keybd_event(18, 0, 0u, UIntPtr.Zero);
            keybd_event(44, 0, 0u, UIntPtr.Zero);
            keybd_event(44, 0, 2u, UIntPtr.Zero);
            keybd_event(18, 0, 2u, UIntPtr.Zero);
            Thread.Sleep(200);
            if (Clipboard.ContainsImage())
            {
                Image img = Clipboard.GetImage();
                if (img == null)
                {
                    return null;
                }
                try
                {
                    Bitmap bmp = new Bitmap(img);
                    if (bmp.Width == rect.Width && bmp.Height == rect.Height)
                    {
                        return bmp;
                    }
                    if (bmp.Width >= rect.Width && bmp.Height >= rect.Height)
                    {
                        int x = Math.Max(0, (bmp.Width - rect.Width) / 2);
                        int y = Math.Max(0, (bmp.Height - rect.Height) / 2);
                        Bitmap crop = bmp.Clone(new Rectangle(x, y, rect.Width, rect.Height), bmp.PixelFormat);
                        bmp.Dispose();
                        return crop;
                    }
                    return bmp;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        finally
        {
            if (originalForeground != IntPtr.Zero)
            {
                try
                {
                    SetForegroundWindow(originalForeground);
                }
                catch
                {
                }
            }
        }
    }

    private static bool IsMostlyBlank(Bitmap bmp)
    {
        try
        {
            List<Point> points = new List<Point>
            {
                new Point(bmp.Width / 2, bmp.Height / 2),
                new Point(1, 1),
                new Point(Math.Max(0, bmp.Width - 2), 1),
                new Point(1, Math.Max(0, bmp.Height - 2)),
                new Point(Math.Max(0, bmp.Width - 2), Math.Max(0, bmp.Height - 2))
            };
            int whiteCount = 0;
            foreach (Point p in points)
            {
                if (p.X >= 0 && p.X < bmp.Width && p.Y >= 0 && p.Y < bmp.Height)
                {
                    Color c = bmp.GetPixel(p.X, p.Y);
                    if (c.A == 0)
                    {
                        whiteCount++;
                    }
                    else if (c.R > 240 && c.G > 240 && c.B > 240)
                    {
                        whiteCount++;
                    }
                }
            }
            return whiteCount >= Math.Min(points.Count, 3);
        }
        catch
        {
            return false;
        }
    }

    private void Button4_Click(object? sender, EventArgs e)
    {
        using SaveFileDialog sfd = new SaveFileDialog
        {
            Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
            FileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
            Title = "保存日志"
        };
        if (sfd.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                File.WriteAllText(sfd.FileName, richTextBox1.Text);
                Log("日志已保存到：" + sfd.FileName);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "保存日志失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Log($"保存日志失败：{ex}");
                return;
            }
        }
    }

    private void Log(string text)
    {
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}";
        richTextBox1.AppendText(line + Environment.NewLine);
        richTextBox1.SelectionStart = richTextBox1.TextLength;
        richTextBox1.ScrollToCaret();
    }

    private void PopulateTopLevelWindows()
    {
        try
        {
            comboBox1.Items.Clear();
            EnumWindows(delegate (nint hWnd, nint lParam)
            {
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }
                int windowTextLength = GetWindowTextLength(hWnd);
                if (windowTextLength == 0)
                {
                    return true;
                }
                StringBuilder stringBuilder = new StringBuilder(windowTextLength + 1);
                GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);
                string text = stringBuilder.ToString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return true;
                }
                comboBox1.Items.Add(new WindowItem(hWnd, text));
                return true;
            }, IntPtr.Zero);
            if (comboBox1.Items.Count > 0 && comboBox1.SelectedIndex < 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            Log("刷新窗口列表失败：" + ex.Message);
        }
    }

    private void PopulateScreens()
    {
        try
        {
            comboBox2.Items.Clear();
            Screen[] screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                comboBox2.Items.Add(new ScreenItem(i, screens[i]));
            }
            for (int j = 0; j < comboBox2.Items.Count; j++)
            {
                if (comboBox2.Items[j] is ScreenItem si && si.Bounds == Screen.PrimaryScreen.Bounds)
                {
                    comboBox2.SelectedIndex = j;
                    break;
                }
            }
            if (comboBox2.SelectedIndex < 0 && comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            Log("刷新显示器列表失败：" + ex.Message);
        }
    }

    private static Rectangle GetWindowRectOrEmpty(nint hWnd)
    {
        if (hWnd == IntPtr.Zero)
        {
            return Rectangle.Empty;
        }
        if (!GetWindowRect(hWnd, out var r))
        {
            return Rectangle.Empty;
        }
        return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
    }

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, Keys vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(nint hwnd, nint hdcBlt, int nFlags);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(nint hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hWnd);

    [DllImport("user32.dll")]
    private static extern nint GetWindowDC(nint hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleBitmap(nint hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(nint hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, nint hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    private static Rectangle GetVirtualScreenBounds()
    {
        int left = SystemInformation.VirtualScreen.Left;
        int top = SystemInformation.VirtualScreen.Top;
        int width = SystemInformation.VirtualScreen.Width;
        int height = SystemInformation.VirtualScreen.Height;
        return new Rectangle(left, top, width, height);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        Title = new Label();
        SaveLocationLabel = new Label();
        Panel = new Panel();
        checkBox9 = new CheckBox();
        checkBox8 = new CheckBox();
        button6 = new Button();
        label3 = new Label();
        label2 = new Label();
        checkBox7 = new CheckBox();
        button8 = new Button();
        button7 = new Button();
        checkBox6 = new CheckBox();
        numericUpDown2 = new NumericUpDown();
        checkBox5 = new CheckBox();
        radioButton3 = new RadioButton();
        radioButton2 = new RadioButton();
        radioButton1 = new RadioButton();
        comboBox2 = new ComboBox();
        comboBox1 = new ComboBox();
        checkBox4 = new CheckBox();
        checkBox3 = new CheckBox();
        button3 = new Button();
        textBox2 = new TextBox();
        numericUpDown1 = new NumericUpDown();
        checkBox2 = new CheckBox();
        checkBox1 = new CheckBox();
        button2 = new Button();
        button1 = new Button();
        textBox1 = new TextBox();
        richTextBox1 = new RichTextBox();
        label1 = new Label();
        panel1 = new Panel();
        button5 = new Button();
        button4 = new Button();
        Panel.SuspendLayout();
        ((ISupportInitialize)numericUpDown2).BeginInit();
        ((ISupportInitialize)numericUpDown1).BeginInit();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // Title
        // 
        Title.AutoSize = true;
        Title.Font = new Font("Noto Sans SC", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Title.Location = new Point(19, 13);
        Title.Margin = new Padding(5, 0, 5, 0);
        Title.Name = "Title";
        Title.Size = new Size(226, 42);
        Title.TabIndex = 0;
        Title.Text = "ScreenShotLite";
        // 
        // SaveLocationLabel
        // 
        SaveLocationLabel.AutoSize = true;
        SaveLocationLabel.Location = new Point(5, 8);
        SaveLocationLabel.Margin = new Padding(5, 0, 5, 0);
        SaveLocationLabel.Name = "SaveLocationLabel";
        SaveLocationLabel.Size = new Size(86, 24);
        SaveLocationLabel.TabIndex = 1;
        SaveLocationLabel.Text = "保存位置:";
        // 
        // Panel
        // 
        Panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        Panel.Controls.Add(checkBox9);
        Panel.Controls.Add(checkBox8);
        Panel.Controls.Add(button6);
        Panel.Controls.Add(label3);
        Panel.Controls.Add(label2);
        Panel.Controls.Add(checkBox7);
        Panel.Controls.Add(button8);
        Panel.Controls.Add(button7);
        Panel.Controls.Add(checkBox6);
        Panel.Controls.Add(numericUpDown2);
        Panel.Controls.Add(checkBox5);
        Panel.Controls.Add(radioButton3);
        Panel.Controls.Add(radioButton2);
        Panel.Controls.Add(radioButton1);
        Panel.Controls.Add(comboBox2);
        Panel.Controls.Add(comboBox1);
        Panel.Controls.Add(checkBox4);
        Panel.Controls.Add(checkBox3);
        Panel.Controls.Add(button3);
        Panel.Controls.Add(textBox2);
        Panel.Controls.Add(numericUpDown1);
        Panel.Controls.Add(checkBox2);
        Panel.Controls.Add(checkBox1);
        Panel.Controls.Add(button2);
        Panel.Controls.Add(button1);
        Panel.Controls.Add(textBox1);
        Panel.Controls.Add(SaveLocationLabel);
        Panel.Location = new Point(19, 55);
        Panel.Margin = new Padding(5, 4, 5, 4);
        Panel.Name = "Panel";
        Panel.Size = new Size(1097, 380);
        Panel.TabIndex = 2;
        // 
        // checkBox9
        // 
        checkBox9.AutoSize = true;
        checkBox9.Location = new Point(135, 293);
        checkBox9.Name = "checkBox9";
        checkBox9.Size = new Size(180, 28);
        checkBox9.TabIndex = 36;
        checkBox9.Text = "截图时使画面静止";
        checkBox9.UseVisualStyleBackColor = true;
        // 
        // checkBox8
        // 
        checkBox8.AutoSize = true;
        checkBox8.Location = new Point(580, 45);
        checkBox8.Name = "checkBox8";
        checkBox8.Size = new Size(162, 28);
        checkBox8.TabIndex = 35;
        checkBox8.Text = "截图后显示通知";
        checkBox8.UseVisualStyleBackColor = true;
        // 
        // button6
        // 
        button6.Location = new Point(133, 330);
        button6.Margin = new Padding(5, 4, 5, 4);
        button6.Name = "button6";
        button6.Size = new Size(118, 32);
        button6.TabIndex = 34;
        button6.Text = "隐藏在托盘";
        button6.UseVisualStyleBackColor = true;
        // 
        // label3
        // 
        label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        label3.AutoSize = true;
        label3.Location = new Point(927, 335);
        label3.Margin = new Padding(5, 0, 5, 0);
        label3.Name = "label3";
        label3.Size = new Size(161, 24);
        label3.TabIndex = 33;
        label3.Text = "v1.2 ©CEllOMiKA";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(271, 335);
        label2.Margin = new Padding(5, 0, 5, 0);
        label2.Name = "label2";
        label2.Size = new Size(418, 24);
        label2.TabIndex = 32;
        label2.Text = "自动截图时再按一次截图停止(实在不行三大金刚键)";
        // 
        // checkBox7
        // 
        checkBox7.AutoSize = true;
        checkBox7.Location = new Point(135, 209);
        checkBox7.Margin = new Padding(5, 4, 5, 4);
        checkBox7.Name = "checkBox7";
        checkBox7.Size = new Size(190, 28);
        checkBox7.TabIndex = 31;
        checkBox7.Text = "使用Alt+PrtSc截图";
        checkBox7.UseVisualStyleBackColor = true;
        // 
        // button8
        // 
        button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button8.Location = new Point(979, 253);
        button8.Margin = new Padding(5, 4, 5, 4);
        button8.Name = "button8";
        button8.Size = new Size(118, 32);
        button8.TabIndex = 30;
        button8.Text = "刷新";
        button8.UseVisualStyleBackColor = true;
        // 
        // button7
        // 
        button7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button7.Location = new Point(979, 208);
        button7.Margin = new Padding(5, 4, 5, 4);
        button7.Name = "button7";
        button7.Size = new Size(118, 32);
        button7.TabIndex = 29;
        button7.Text = "刷新";
        button7.UseVisualStyleBackColor = true;
        // 
        // checkBox6
        // 
        checkBox6.AutoSize = true;
        checkBox6.Location = new Point(5, 127);
        checkBox6.Margin = new Padding(5, 4, 5, 4);
        checkBox6.Name = "checkBox6";
        checkBox6.Size = new Size(148, 28);
        checkBox6.TabIndex = 26;
        checkBox6.Text = "自动截图间隔:";
        checkBox6.UseVisualStyleBackColor = true;
        // 
        // numericUpDown2
        // 
        numericUpDown2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        numericUpDown2.Location = new Point(174, 124);
        numericUpDown2.Margin = new Padding(5, 4, 5, 4);
        numericUpDown2.Name = "numericUpDown2";
        numericUpDown2.Size = new Size(922, 30);
        numericUpDown2.TabIndex = 25;
        // 
        // checkBox5
        // 
        checkBox5.AutoSize = true;
        checkBox5.Location = new Point(410, 45);
        checkBox5.Margin = new Padding(5, 4, 5, 4);
        checkBox5.Name = "checkBox5";
        checkBox5.Size = new Size(162, 28);
        checkBox5.TabIndex = 24;
        checkBox5.Text = "截图后打开图片";
        checkBox5.UseVisualStyleBackColor = true;
        // 
        // radioButton3
        // 
        radioButton3.AutoSize = true;
        radioButton3.Checked = true;
        radioButton3.Location = new Point(5, 292);
        radioButton3.Margin = new Padding(5, 4, 5, 4);
        radioButton3.Name = "radioButton3";
        radioButton3.Size = new Size(107, 28);
        radioButton3.TabIndex = 23;
        radioButton3.TabStop = true;
        radioButton3.Text = "框选截图";
        radioButton3.UseVisualStyleBackColor = true;
        // 
        // radioButton2
        // 
        radioButton2.AutoSize = true;
        radioButton2.Location = new Point(5, 251);
        radioButton2.Margin = new Padding(5, 4, 5, 4);
        radioButton2.Name = "radioButton2";
        radioButton2.Size = new Size(111, 28);
        radioButton2.TabIndex = 22;
        radioButton2.Text = "屏幕截图:";
        radioButton2.UseVisualStyleBackColor = true;
        // 
        // radioButton1
        // 
        radioButton1.AutoSize = true;
        radioButton1.Location = new Point(5, 208);
        radioButton1.Margin = new Padding(5, 4, 5, 4);
        radioButton1.Name = "radioButton1";
        radioButton1.Size = new Size(111, 28);
        radioButton1.TabIndex = 21;
        radioButton1.Text = "窗口截图:";
        radioButton1.UseVisualStyleBackColor = true;
        // 
        // comboBox2
        // 
        comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboBox2.FormattingEnabled = true;
        comboBox2.Location = new Point(137, 250);
        comboBox2.Margin = new Padding(5, 4, 5, 4);
        comboBox2.Name = "comboBox2";
        comboBox2.Size = new Size(831, 32);
        comboBox2.TabIndex = 20;
        // 
        // comboBox1
        // 
        comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboBox1.FormattingEnabled = true;
        comboBox1.Location = new Point(346, 206);
        comboBox1.Margin = new Padding(5, 4, 5, 4);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(622, 32);
        comboBox1.TabIndex = 17;
        // 
        // checkBox4
        // 
        checkBox4.AutoSize = true;
        checkBox4.Location = new Point(5, 168);
        checkBox4.Margin = new Padding(5, 4, 5, 4);
        checkBox4.Name = "checkBox4";
        checkBox4.Size = new Size(112, 28);
        checkBox4.TabIndex = 14;
        checkBox4.Text = "热键捕捉:";
        checkBox4.UseVisualStyleBackColor = true;
        // 
        // checkBox3
        // 
        checkBox3.AutoSize = true;
        checkBox3.Location = new Point(5, 86);
        checkBox3.Margin = new Padding(5, 4, 5, 4);
        checkBox3.Name = "checkBox3";
        checkBox3.Size = new Size(130, 28);
        checkBox3.TabIndex = 13;
        checkBox3.Text = "截图倒计时:";
        checkBox3.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        button3.Location = new Point(5, 330);
        button3.Margin = new Padding(5, 4, 5, 4);
        button3.Name = "button3";
        button3.Size = new Size(118, 32);
        button3.TabIndex = 12;
        button3.Text = "截图";
        button3.UseVisualStyleBackColor = true;
        // 
        // textBox2
        // 
        textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox2.Location = new Point(137, 165);
        textBox2.Margin = new Padding(5, 4, 5, 4);
        textBox2.Name = "textBox2";
        textBox2.Size = new Size(958, 30);
        textBox2.TabIndex = 11;
        // 
        // numericUpDown1
        // 
        numericUpDown1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        numericUpDown1.Location = new Point(156, 83);
        numericUpDown1.Margin = new Padding(5, 4, 5, 4);
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(941, 30);
        numericUpDown1.TabIndex = 9;
        // 
        // checkBox2
        // 
        checkBox2.AutoSize = true;
        checkBox2.Location = new Point(189, 45);
        checkBox2.Margin = new Padding(5, 4, 5, 4);
        checkBox2.Name = "checkBox2";
        checkBox2.Size = new Size(198, 28);
        checkBox2.TabIndex = 6;
        checkBox2.Text = "截图后复制到剪切板";
        checkBox2.UseVisualStyleBackColor = true;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Location = new Point(5, 45);
        checkBox1.Margin = new Padding(5, 4, 5, 4);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(162, 28);
        checkBox1.TabIndex = 5;
        checkBox1.Text = "截图时隐藏窗口";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button2.Location = new Point(979, 4);
        button2.Margin = new Padding(5, 4, 5, 4);
        button2.Name = "button2";
        button2.Size = new Size(118, 32);
        button2.TabIndex = 4;
        button2.Text = "打开";
        button2.UseVisualStyleBackColor = true;
        // 
        // button1
        // 
        button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button1.Location = new Point(852, 4);
        button1.Margin = new Padding(5, 4, 5, 4);
        button1.Name = "button1";
        button1.Size = new Size(118, 32);
        button1.TabIndex = 3;
        button1.Text = "选择";
        button1.UseVisualStyleBackColor = true;
        // 
        // textBox1
        // 
        textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox1.Location = new Point(107, 4);
        textBox1.Margin = new Padding(5, 4, 5, 4);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(724, 30);
        textBox1.TabIndex = 2;
        // 
        // richTextBox1
        // 
        richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        richTextBox1.Location = new Point(5, 37);
        richTextBox1.Margin = new Padding(5, 4, 5, 4);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new Size(1085, 286);
        richTextBox1.TabIndex = 3;
        richTextBox1.Text = "";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(5, 8);
        label1.Margin = new Padding(5, 0, 5, 0);
        label1.Name = "label1";
        label1.Size = new Size(50, 24);
        label1.TabIndex = 4;
        label1.Text = "日志:";
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        panel1.Controls.Add(button5);
        panel1.Controls.Add(button4);
        panel1.Controls.Add(richTextBox1);
        panel1.Controls.Add(label1);
        panel1.Location = new Point(19, 443);
        panel1.Margin = new Padding(5, 4, 5, 4);
        panel1.Name = "panel1";
        panel1.Size = new Size(1097, 333);
        panel1.TabIndex = 5;
        // 
        // button5
        // 
        button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button5.Location = new Point(846, 4);
        button5.Margin = new Padding(5, 4, 5, 4);
        button5.Name = "button5";
        button5.Size = new Size(118, 32);
        button5.TabIndex = 34;
        button5.Text = "清除日志";
        button5.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button4.Location = new Point(974, 4);
        button4.Margin = new Padding(5, 4, 5, 4);
        button4.Name = "button4";
        button4.Size = new Size(118, 32);
        button4.TabIndex = 5;
        button4.Text = "保存日志";
        button4.UseVisualStyleBackColor = true;
        // 
        // Main
        // 
        AutoScaleDimensions = new SizeF(11F, 24F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1136, 785);
        Controls.Add(panel1);
        Controls.Add(Panel);
        Controls.Add(Title);
        Margin = new Padding(5, 4, 5, 4);
        Name = "Main";
        Text = "ScreenShotLite";
        Panel.ResumeLayout(false);
        Panel.PerformLayout();
        ((ISupportInitialize)numericUpDown2).EndInit();
        ((ISupportInitialize)numericUpDown1).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
