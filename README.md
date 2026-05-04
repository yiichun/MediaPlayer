#MediaPlayer(多媒體播放器) #

這是一個用 C# WinForms 開發的輕量級播放器，核心使用 Windows Media Player (WMP) 元件。

🛠 主要功能 

1. 可播放 .mp4, .wmv, .avi, .mkv 等常見格式。

2. 檔案拖曳 (Drag & Drop)：實作 DragEnter 與 DragDrop 事件，支援從桌面或資料夾直接丟入檔案。

3. 時間同步：每 100ms 更新一次播放時間（格式：00:00 / 00:00）。

4. 手動跳轉：支援滑鼠點擊進度條直接切換播放位置。

5. 音量與靜音管理：支援 0-100% 音量微調。一鍵切換靜音，並連動按鈕圖示與音量條狀態。

6. 結束確認：關閉程式前會跳出 Dialog 確認。

⌨️操作說明

一開始執行畫面：未載入音檔之前，進度條、播放按鈕、暫停按鈕皆不能點擊。

<img width="1963" height="1428" alt="image" src="https://github.com/user-attachments/assets/bc70ef92-1d4b-42af-85cd-d866c4de5d09" />

按下瀏覽選擇影片或是拖曳影片後，即可控制進度條、播放按鈕、暫停按鈕和音量。

<img width="1972" height="1438" alt="image" src="https://github.com/user-attachments/assets/5ec265b8-a65e-4847-a5df-aa8bb3d35120" />

按下停止後，會恢復成一開始的執行畫面

<img width="1972" height="1447" alt="image" src="https://github.com/user-attachments/assets/2f11814f-989d-4c9e-b928-15512596d837" />

關閉視窗前會跳出通知。

<img width="1972" height="1419" alt="image" src="https://github.com/user-attachments/assets/ed4ab590-1973-4fba-bdab-cdf2f61901a5" />
