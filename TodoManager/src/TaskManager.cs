using System;
using System.Collections.Generic;
using System.Xml;

namespace TodoManager.src
{
    class TaskManager
    {
        private const String confName = "taskList.xml";
        private List<TaskItem> taskList;
        private static TaskManager instance = null;

        private TaskManager()
        {
            //xmlの読み込み
            try
            {
                using (System.IO.TextReader reader = new System.IO.StreamReader(confName))
                {
                    
                    XmlDocument xmlData = new XmlDocument();
                    xmlData.PreserveWhitespace = true;
                    xmlData.LoadXml(reader.ReadToEnd());
                    XmlNodeReader xmlReader = new XmlNodeReader(xmlData.DocumentElement);
                    this.taskList = (List<TaskItem>)new System.Xml.Serialization.XmlSerializer(typeof(List<TaskItem>)).Deserialize(xmlReader);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                //ファイルがなければ新規
                this.taskList = new List<TaskItem>();
            }
        }

        //シングルトンインスタンスの取得
        public static TaskManager getInstance()
        {
            if (instance == null)
            {
                instance = new TaskManager();
            }
            return instance;
        } 
        //タスクの追加
        public void addTask(TaskItem item)
        {
            this.taskList.Add(item);
            this.taskList.Sort(delegate (TaskItem a, TaskItem b) { return a.Deadline.CompareTo(b.Deadline); });
        }
        //削除
        public void deleteTaskById(long id)
        {
            for (int i = 0; i < this.taskList.Count; i++)
            {
                TaskItem task = this.taskList[i];

                if(task.Id == id)
                {
                    this.taskList.Remove(task);
                    break;
                }
            }
        }
        //完了
        public void completeTaskById(long id)
        {
            for(int i = 0; i < this.taskList.Count; i++)
            {
                TaskItem task = this.taskList[i];

                if (task.Id == id)
                {
                    if (task.RepeatType == REPEAT_TYPE.none)
                    {
                        this.taskList.Remove(task);
                    }
                    else if (task.RepeatType == REPEAT_TYPE.day)
                    {
                        DateTime date = task.Deadline.AddDays(1);
                        //明日まで更新
                        while (DateTime.Now.Date > date.Date) date = date.AddDays(1);
                        task.Deadline = date;
                        this.taskList.Sort(delegate (TaskItem a, TaskItem b) { return a.Deadline.CompareTo(b.Deadline); });

                    }
                    else if (task.RepeatType == REPEAT_TYPE.week)
                    {
                        DateTime date = task.Deadline.AddDays(7);
                        //明日まで更新
                        while (DateTime.Now.Date > date.Date) date = date.AddDays(7);
                        task.Deadline = date;
                        this.taskList.Sort(delegate (TaskItem a, TaskItem b) { return a.Deadline.CompareTo(b.Deadline); });

                    }
                    break;
                }
            }
        }
        //取得
        public TaskItem getTaskItemById(long id)
        {
            TaskItem item = null;
            for (int i = 0; i < this.taskList.Count; i++)
            {
                if (id == this.taskList[i].Id)
                {
                    item = this.taskList[i];
                    break;
                }
            }
            return item;
        }
        //変更
        public void editTaskItemById(long id,String task,DateTime deadline,REPEAT_TYPE type)
        {
            TaskItem taskItem = this.getTaskItemById(id);
            
            if(taskItem != null)
            {
                taskItem.Task = task;
                taskItem.Deadline = deadline;
                taskItem.RepeatType = type;
                this.taskList.Sort(delegate (TaskItem a, TaskItem b) { return a.Deadline.CompareTo(b.Deadline); });
            }
        }
        //リストを取得
        public List<TaskItem> getTaskList()
        {
            return this.taskList;
        }


        // タスクをファイルに保存
        public void saveTaskList()
        {
            using (System.IO.TextWriter writer = new System.IO.StreamWriter(confName))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<TaskItem>));
                serializer.Serialize(writer, this.taskList);
            }
        }

    }

    
}
