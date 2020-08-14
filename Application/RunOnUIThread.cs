using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    public class RunOnUIThread
    {        
        public static void Run(Action action)
        {
            TaskRunner.addAction(action);
        }
    }

    public static class TaskRunner
    {
        private static List<Action> actions = new List<Action>();

        private static Object lockObj = new object();

        public static void addAction(Action action)
        {
            lock(lockObj)
            {
                actions.Add(action);
            }
        }
       
        public static void RunTasks()
        {
            List<Action> taskCopy = new List<Action>();
            lock (lockObj)
            {
                if (actions == null) return;
                foreach (Action action in actions)
                    taskCopy.Add((Action)action.Clone());

                actions.Clear();
            }
            
            foreach (Action action in taskCopy)
                action();
        }

    }

}
