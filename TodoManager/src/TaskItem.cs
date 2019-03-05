using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager.src
{
    public enum REPEAT_TYPE
    {
        none =0,
        day,
        week
    }
    ///タスクが持つ情報
     public class TaskItem
    {
        private String task;
        private DateTime deadline;
        private REPEAT_TYPE repeatType;
        private long id;

        public TaskItem()
        {
        }

        public TaskItem(String task, DateTime deadline, REPEAT_TYPE type)
        {
            this.task = task;
            this.deadline = deadline;
            this.repeatType = type;
            this.id = DateTime.Now.ToFileTimeUtc();
        }
        

        public REPEAT_TYPE RepeatType { get => repeatType; set => repeatType = value; }
        public DateTime Deadline { get => deadline; set => deadline = value; }
        public string Task { get => task; set => task = value; }
        public long Id { get => id; set => id = value; }
    }
}
