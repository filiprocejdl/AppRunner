using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TaskSheduler
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string taskName = "Test";
        public MainWindow()
        {
            InitializeComponent();

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Does something";
                td.Principal.LogonType = TaskLogonType.InteractiveToken;

                // Create a trigger that fires 1 minute from now and then every 15 minutes for the
                // next 7 days.
                TimeTrigger tTrigger = (TimeTrigger)td.Triggers.Add(new TimeTrigger());
                tTrigger.StartBoundary = DateTime.Now + TimeSpan.FromMinutes(1);
                tTrigger.EndBoundary = DateTime.Today + TimeSpan.FromDays(7);
                tTrigger.ExecutionTimeLimit = TimeSpan.FromSeconds(15);
                tTrigger.Id = "Time test";
                tTrigger.Repetition.Duration = TimeSpan.FromMinutes(10);
                tTrigger.Repetition.Interval = TimeSpan.FromMinutes(2);
                tTrigger.Repetition.StopAtDurationEnd = true;
                //test
                // Add an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction("notepad.exe", "c:\\test.log", null));

                // Register the task in the root folder

                ts.RootFolder.RegisterTaskDefinition(taskName, td);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (TaskService ts = new TaskService())
            {
                Microsoft.Win32.TaskScheduler.Task t = ts.GetTask(taskName);
                if (t == null) return;

                // In some cases you need Administrator permission

                /*
                // Check to make sure account privileges allow task deletion
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                
                    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                    throw new Exception($"Cannot delete task with your current identity '{identity.Name}' permissions level." +
                    "You likely need to run this application 'as administrator' even if you are using an administrator account.");
                */

                // Remove the task we just created
                ts.RootFolder.DeleteTask(taskName);
            }
        }

       
    }
}
