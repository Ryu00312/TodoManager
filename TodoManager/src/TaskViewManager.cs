using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoManager.Control;

namespace TodoManager.src
{
    class TaskViewManager
    {
        private List<TaskView> taskViewList;
        private static TaskViewManager instance = null;

        private TaskViewManager()
        {
            taskViewList = new List<TaskView>();
        }

        public static TaskViewManager getInstance()
        {
            if (instance == null)
            {
                instance = new TaskViewManager();
            }
            return instance;
        }

        public void addTaskView(TaskView view)
        {
            this.taskViewList.Add(view);
        }
        //タスクのメニューを非表示
        public void hideAllMenuContent()
        {
            foreach (TaskView view in this.taskViewList)
            {
                view.hideOptionContent();
            }
        }
        //タスクのビューのロック
        public void setTaskViewLock(bool value)
        {
            foreach (TaskView view in this.taskViewList)
            {
                view.setLock(value);
            }
        }
        //指定したタスクのメニューだけ表示(他のメニューを閉じる)
        public void indicateShowMenu(TaskView src)
        {
            for (int i = 0; i < this.taskViewList.Count; i++)
            {
                TaskView view = this.taskViewList[i];
                if(view != src)
                {
                    view.hideOptionContent();
                }
            }
        }
        public void clear()
        {
            this.taskViewList.Clear();
        }
    }
}
