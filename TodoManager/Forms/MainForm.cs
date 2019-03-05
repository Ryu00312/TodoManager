using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TodoManager.src;
using TodoManager.Control;

namespace TodoManager
{
    /// <summary>
    /// メイン画面
    /// </summary>
    public partial class MainForm : Form
    {
        TaskEditView taskEditView;
        SettingView settingView;

        public MainForm()
        {
            InitializeComponent();
            this.taskEditView = new TaskEditView();
            this.taskEditView.okEvent += okButton_Click;
            this.taskEditView.cancelEvent += cancelButton_Click;

            this.settingView = new SettingView();
            this.settingView.okEvent += setting_okButton_Click;
            this.settingView.cancelEvent += setting_cancelButton_Click;

            System.Drawing.Size formSize = this.DisplayRectangle.Size;

            this.buttonToolTip.SetToolTip(this.addButton, "新規タスクの登録");
            this.buttonToolTip.SetToolTip(this.settingButton, "アプリケーション設定");

            this.settingButton.Location = new System.Drawing.Point(formSize.Width - this.settingButton.Width - 10, this.settingButton.Location.Y);

            this.mainPanel.SetRowSpan(this.taskListPanel, 2);
            this.mainPanel.Size = new System.Drawing.Size(formSize.Width - 4, formSize.Height - this.mainPanel.Location.Y - 2);

            //テーブルの更新
            refreshTaskTable();
            

        }
        
        ///メイン画面にデイビューを表示
        private void addDateView(DateTime date)
        {
            DeadlineLabel deadlineLabel = new DeadlineLabel(date);
            this.taskListPanel.Controls.Add(deadlineLabel);
        }
        /// <summary>
        /// メイン画面にタスクビューを表示
        /// </summary>
        /// <param name="task"></param>
        private void addTaskView(TaskItem task)
        {
            TaskView view = new TaskView(task);
            view.completeButton_Click += complete;
            view.editButton_Click += edit;
            view.deleteButton_Click += delete;
            //ビューのコントロールを追加
            this.taskListPanel.Controls.Add(view);
            //リストに追加
            TaskViewManager.getInstance().addTaskView(view);
        }


        ///タスク追加ボタンのクリックイベント
        private void addButton_Click(object sender,EventArgs e)
        {
            //タスク編集(MODE＝New)を表示
            showTaskEditView(EDIT_MODE.New);
            //タスク一覧をロック
            TaskViewManager.getInstance().hideAllMenuContent();
        }

        ///タスク追加画面のOKボタンクリックイベント
        private void okButton_Click(object sender,EventArgs e)
        {
            TaskManager manager = TaskManager.getInstance();
            long id = this.taskEditView.ID;

            if (id<0)
            {
                TaskItem task = new TaskItem(this.taskEditView.Task, this.taskEditView.Deadline, this.taskEditView.RepeatType);
                manager.addTask(task);
            }
            else
            {
                manager.editTaskItemById(id, this.taskEditView.Task, this.taskEditView.Deadline, this.taskEditView.RepeatType);
            }
            manager.saveTaskList();
            //編集ビューを非表示
            hideTaskEditView();
            //テーブル更新
            refreshTaskTable();
        }
        //タスク編集画面のキャンセルボタンクリックイベント
        private void cancelButton_Click(object sender,EventArgs e)
        {
            //タスク編集画面を非表示
            hideTaskEditView();
        }

        //完了
        private void complete(object sender)
        {
            TaskView taskView = (TaskView)sender;
            TaskManager manager = TaskManager.getInstance();
            //たすくIDを元にタスク完了処理
            manager.completeTaskById(taskView.getTaskItemId());
            //タスクリスト保存
            manager.saveTaskList();
            //テーブル更新
            refreshTaskTable();

        }

        //編集
        private void edit(object sender)
        {
            //編集するタスクからIDを取得=>IDからタスクを取得
            TaskItem taskItem = TaskManager.getInstance().getTaskItemById(((TaskView)sender).getTaskItemId());
            if(taskItem != null)
            {
                this.taskEditView.reflectTaskItem(taskItem);
            }
            showTaskEditView(EDIT_MODE.Edit);
        }

        //削除
        private void delete(object sender)
        {
            TaskView taskView = (TaskView)sender;
            TaskManager manager = TaskManager.getInstance();

            manager.deleteTaskById(taskView.getTaskItemId());

            manager.saveTaskList();
            refreshTaskTable();
        }
 
        //編集ビュー
        private void showTaskEditView(EDIT_MODE mode)
        {
            this.mainPanel.SetRowSpan(this.taskListPanel, 1);
            this.mainPanel.SetRow(this.taskListPanel, 1);

            this.mainPanel.Controls.Add(this.taskEditView);
            this.mainPanel.SetRow(this.taskEditView, 0);

            this.settingButton.Enabled = false;
            this.settingButton.BackgroundImage = Properties.Resources.setting_disable;

            this.addButton.Enabled = false;
            this.addButton.BackgroundImage = Properties.Resources.plus_disable;

            this.taskEditView.setEditMode(mode);

            //タスク一覧(taskViewManager)のロック
            TaskViewManager.getInstance().setTaskViewLock(true);

        }
        //編集ビューを非表示
        private void hideTaskEditView()
        {
            this.mainPanel.Controls.Remove(this.taskEditView);
            this.mainPanel.SetRow(this.taskListPanel, 0);
            this.mainPanel.SetRowSpan(this.taskListPanel, 2);

            this.settingButton.Enabled = true;
            this.settingButton.BackgroundImage = Properties.Resources.setting_enable;

            this.addButton.Enabled = true;
            this.addButton.BackgroundImage = Properties.Resources.plus_enable;

            //タスク一覧(taskViewManager)のロックを解除
            TaskViewManager.getInstance().setTaskViewLock(false);

            
        }

        //設定ボタンクリックイベント
        private void settingButton_Click(object sender,EventArgs e)
        {
            showSettingView();
        }

        //設定画面の表示
        private void showSettingView()
        {
            this.mainPanel.SetRowSpan(this.taskListPanel, 1);
            this.mainPanel.SetRow(this.taskListPanel, 1);

            this.mainPanel.Controls.Add(this.settingView);
            this.mainPanel.SetRow(this.settingView, 0);

            this.settingButton.Enabled = false;
            this.settingButton.BackgroundImage = Properties.Resources.setting_disable;

            this.addButton.Enabled = false;
            this.addButton.BackgroundImage = Properties.Resources.plus_disable;

            TaskViewManager.getInstance().hideAllMenuContent();
            
            //タスク一覧のロック
            TaskViewManager.getInstance().setTaskViewLock(true);
        }
        
            //設定画面の非表示
        private void hideSettingView()
        {
            this.mainPanel.Controls.Remove(this.settingView);
            this.mainPanel.SetRow(this.taskListPanel, 0);
            this.mainPanel.SetRowSpan(this.taskListPanel, 2);

            this.settingButton.Enabled = true;
            this.settingButton.BackgroundImage = Properties.Resources.setting_enable;

            this.addButton.Enabled = true;
            this.addButton.BackgroundImage = Properties.Resources.plus_enable;

            //タスク一覧のロック解除
            TaskViewManager.getInstance().setTaskViewLock(false);
        }
        
            //設定のOKボタンクリックイベント
        private void setting_okButton_Click(object sender, EventArgs e)
        {
            SettingEventArgs args = (SettingEventArgs)e;

            hideSettingView();

            if (args.changeTaskNum)
            {
                this.refreshTaskTable();
            }

            this.settingView.setFontSize(Properties.Settings.Default.fontSize);
            this.taskEditView.setFontSize(Properties.Settings.Default.fontSize);

            for (int i = 0; i < this.taskListPanel.Controls.Count; i++)
            {
                System.Windows.Forms.Control control = this.taskListPanel.GetControlFromPosition(0, i);

                if (control is DeadlineLabel)
                {
                    ((DeadlineLabel)control).setFontSize(Properties.Settings.Default.fontSize);
                }
                else if (control is TaskView)
                {
                    ((TaskView)control).setFontSize(Properties.Settings.Default.fontSize);
                }
            }

            this.taskListPanel.AutoScroll = false;
            this.taskListPanel.AutoScroll = true;
        }
        //設定のキャンセルボタンクリックイベント
        private void setting_cancelButton_Click(object sender,EventArgs e)
        {
            hideSettingView();
        }

        //タスク一覧更新
        private void refreshTaskTable()
        {
            TaskManager manager = TaskManager.getInstance();
            List<TaskItem> taskList = manager.getTaskList();

            this.taskListPanel.Controls.Clear();
            TaskViewManager.getInstance().clear();

            DateTime date = new DateTime();
            int max = Properties.Settings.Default.taskNum;

            for (int i = 0; i < taskList.Count && i < max; i++)
            {
                TaskItem task = taskList[i];

                if (task.Deadline.Date != date)
                {
                    addDateView(task.Deadline.Date);
                    date = task.Deadline.Date;
                }
                addTaskView(task);
            }

            this.taskListPanel.AutoScroll = false;
            this.taskListPanel.AutoScroll = true;
        }
        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.taskListPanel.Controls.Count; i++)
            {
                System.Windows.Forms.Control control = this.taskListPanel.GetControlFromPosition(0, i);
                if (control is DeadlineLabel)
                {
                    ((DeadlineLabel)control).refreshRemainDays();
                }

            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

    }
}
