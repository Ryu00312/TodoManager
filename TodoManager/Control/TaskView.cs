using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TodoManager.src;

namespace TodoManager.Control
{
    public partial class TaskView : UserControl
    {

        private Label timeLabel;
        private Label taskLabel;
        private long ID;
        private bool showOption;
        private bool Lock;

        public delegate void optionButtonEventHandler(object sender);
        public event optionButtonEventHandler completeButton_Click;
        public event optionButtonEventHandler editButton_Click;
        public event optionButtonEventHandler deleteButton_Click;

        public TaskView(TaskItem taskItem)
        {
            InitializeComponent();

            this.showOption = false;
            this.Lock = false;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Margin = new Padding(0, 0, 0, 3);

        
            this.timeLabel = new Label();
            this.timeLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.timeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.timeLabel.Margin = new Padding(0);
            this.timeLabel.Padding = new Padding(0, 0, 0, 0);
            //ラベルにクリックイベントを追加
            this.timeLabel.Click += Task_Click;

            this.taskLabel = new Label();
            this.taskLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.taskLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.taskLabel.Margin = new Padding(0);
            this.taskLabel.Padding = new Padding(0, 0, 0, 0);
            //ラベルにクリックイベントを追加
            this.taskLabel.Click += Task_Click;


            this.mainPanel.Controls.Add(this.timeLabel);
            this.mainPanel.Controls.Add(this.taskLabel);

            //フォント設定
            setFontSize(Properties.Settings.Default.fontSize);

            //タスクのセット
            setTaskItem(taskItem);
        }

        public void setFontSize(int size)
        {
            Font font = new Font("Meiryo UI", size);

            this.timeLabel.Font = font;
            this.taskLabel.Font = font;

            this.mainPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, font.Height * 4);

            int line = this.taskLabel.Text.Count(c => c == '\n') + 1;

            if (line < 2) line = 2;
            this.Height = (int)(this.taskLabel.Font.GetHeight() * (line + 1));
        }

        public long getTaskItemId()
        {
            return this.ID;
        }
        //タスクセット
        public void setTaskItem(TaskItem taskItem)
        {
            this.ID = taskItem.Id;
            int hour = taskItem.Deadline.TimeOfDay.Hours;
            int minute = taskItem.Deadline.TimeOfDay.Minutes;
            this.timeLabel.Text = hour.ToString("D2") + ":" + minute.ToString("D2");

            this.taskLabel.Text = taskItem.Task;

            int line = taskItem.Task.Count(c => c == '\n') + 1;
            this.Height = (int)(this.taskLabel.Font.GetHeight() * (line + 2));

            switch (taskItem.RepeatType)
            {
                case REPEAT_TYPE.none:
                    break;
                case REPEAT_TYPE.day:
                    this.timeLabel.Text += "\n(毎日)";
                    break;
                case REPEAT_TYPE.week:
                    this.timeLabel.Text += "\n(毎週)";
                    break;
                default:
                    break;
            }
        }
        
        //タスクビュー表示
        private void showTaskContent()
        {
            this.mainPanel.Controls.Clear();
            this.mainPanel.Controls.Add(this.timeLabel);
            this.mainPanel.Controls.Add(this.taskLabel);
            this.showOption = false;
        }
        
        //タスクのオプション表示
        private void showOptionContent()
        {
            if (this.Lock)
            {
                return;
            }
            this.mainPanel.Controls.Clear();
            TaskOptionPanel opt = createOptionPanel();
            this.mainPanel.Controls.Add(opt, 0, 0);
            this.mainPanel.SetColumnSpan(opt, this.mainPanel.ColumnCount);
            this.showOption = true;
            //指定タスク以外のオプションを非表示に
            TaskViewManager.getInstance().indicateShowMenu(this);
        }
        //メニュー＝＞タスクビューに切り替え
        public void hideOptionContent()
        {
            if (this.showOption == true)
            {
                this.showTaskContent();
            }
        }

        //タスクビューロック
        public void setLock(bool value)
        {
            this.Lock = value;
        }
        //タスクパネルクリックイベント
        private void Task_Click(object sender,EventArgs e)
        {
            this.showOptionContent();
        }

        //
        //オプションクリックイベント
        //
        
        //完了ボタン
        private void completeButton_ClickEvent()
        {
            this.showTaskContent();
            completeButton_Click(this);
        } 
        //戻る
        private void returnButton_ClickEvent()
        {
            this.showTaskContent();
        }
        //編集
        private void editButton_ClickEvent()
        {
            this.showTaskContent();
            editButton_Click(this);
        }
        //削除
        private void deleteButton_ClickEvent()
        {
            this.showTaskContent();
            deleteButton_Click(this);
        }

        private TaskOptionPanel createOptionPanel()
        {
            TaskOptionPanel option = new TaskOptionPanel();
            option.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            option.Margin = new Padding(0);
            option.completeEvent += completeButton_ClickEvent;
            option.editEvent += editButton_ClickEvent;
            option.returnEvent += returnButton_ClickEvent;
            option.deleteEvent += deleteButton_ClickEvent;

            return option;
        }

    }
}
