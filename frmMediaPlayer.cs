using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MediaPlayer
{
    public partial class frmMediaPlayer : Form
    {
        private bool isDragging = false;

        public frmMediaPlayer()
        {
            InitializeComponent();
            ResetUI();
            timer1.Interval = 100;
            this.AllowDrop = true;
        }

        private void ResetUI()
        {
            btnPlay.Enabled = false;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            trackBar1.Value = 0;
            trackBar1.Enabled = false;
            tkbVolume.Value = 50;
            wmpVideo.settings.volume = tkbVolume.Value;
        }

        private void StartPlaying(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            // 1. 設定影片路徑並播放
            wmpVideo.URL = filePath;
            wmpVideo.Ctlcontrols.play();

            // 2. 啟動計時器以更新進度條
            timer1.Start();

            // 3. 啟用控制按鈕
            btnPlay.Enabled = true;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
            trackBar1.Enabled = true;

            // 4. 更新視窗標題
            this.Text = "正在播放: " + System.IO.Path.GetFileName(filePath);
        }

        // 1. 當檔案拖到視窗上方時
        private void frmMediaPlayer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // 顯示增加圖示
        }

        // 2. 當檔案放開時
        private void frmMediaPlayer_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                // 使用封裝好的播放邏輯
                StartPlaying(files[0]);
            }
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "影片檔案|*.mp4;*.wmv;*.avi;*.mkv|所有檔案|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 使用封裝好的播放邏輯
                StartPlaying(ofd.FileName);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            wmpVideo.Ctlcontrols.play();
            timer1.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            wmpVideo.Ctlcontrols.pause();
            timer1.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            wmpVideo.Ctlcontrols.stop();
            timer1.Stop();
            ResetUI();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (wmpVideo.currentMedia != null && wmpVideo.currentMedia.duration > 0)
            {
                // 動態設定 Maximum (秒數 * 10 提高精度)
                trackBar1.Maximum = (int)(wmpVideo.currentMedia.duration * 10);

                // 重點：當使用者「不在拉動」滑塊時，才由計時器自動更新位置
                if (!isDragging)
                {
                    int currentPos = (int)(wmpVideo.Ctlcontrols.currentPosition * 10);
                    if (currentPos > trackBar1.Maximum) currentPos = trackBar1.Maximum;
                    trackBar1.Value = currentPos;
                }

                // 時間文字同步 (直接讀取 WMP 的字串最準)
                string curTime = wmpVideo.Ctlcontrols.currentPositionString;
                string totalTime = wmpVideo.currentMedia.durationString;

                if (!string.IsNullOrEmpty(curTime) && !string.IsNullOrEmpty(totalTime))
                {
                    lblTime.Text = $"{curTime} / {totalTime}";
                }

                // 播放結束後的處理
                if (wmpVideo.playState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    timer1.Stop();
                    ResetUI();
                }
            }
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (wmpVideo.currentMedia == null || wmpVideo.currentMedia.duration == 0)
            {
                isDragging = false;
                return;
            }
            isDragging = true;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            if (wmpVideo.currentMedia != null)
            {
                // 核心修正：只在放開滑鼠時執行一次跳轉，避免解碼器卡死
                wmpVideo.Ctlcontrols.currentPosition = (double)trackBar1.Value / 10.0;
            }
            isDragging = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // 這裡不要去動 wmpVideo.Ctlcontrols.currentPosition！
            // 只需要更新畫面上的時間文字，讓使用者知道現在拉到哪了
            if (wmpVideo.currentMedia != null)
            {
                double tempPos = (double)trackBar1.Value / 10.0;
                // 我們手動算出格式化時間，避免頻繁調用 WMP 組件
                TimeSpan ts = TimeSpan.FromSeconds(tempPos);
                lblTime.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2} / {wmpVideo.currentMedia.durationString}";
            }
        }

        private void frmMediaPlayer_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }

        private void frmMediaPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("確定要結束播放並關閉程式嗎？", "確認關閉",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // 取消關閉動作
            }
        }

        private void tkbVolume_Scroll(object sender, EventArgs e)
        {
            wmpVideo.settings.volume = tkbVolume.Value;
            lblVolume.Text = $"音量: {tkbVolume.Value}%";
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            wmpVideo.settings.mute = !wmpVideo.settings.mute;

            // 根據狀態改變按鈕文字，讓使用者知道現在的情況
            if (wmpVideo.settings.mute)
            {
                btnMute.Text = "🔊";
                tkbVolume.Enabled = false;
            }
            else
            {
                btnMute.Text = "🔇";
                tkbVolume.Enabled = true;
            }
        }
    }
}
