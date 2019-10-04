using Chronos.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Chronos.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // #TODO: Load the previous time sheet and instaniate this stuff;

        private List<WorkTask> _tasks = new List<WorkTask>();
        private StackPanel _sp = new StackPanel();
        private Button _addTaskButton = new Button();
        private WorkTask activeTask = null;
        private DispatcherTimer _clockTimer;
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            BuildInitialForm();
            SetupClock();
        }

        private void SetupClock()
        {
            _clockTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _clockTimer.Tick += _clockTimer_Tick;
            _clockTimer.Start();
        }

        private void _clockTimer_Tick(object sender, EventArgs e)
        {
            if (activeTask != null)
            {
                WorkTask workTask = _tasks.Where(x => x.Name == activeTask.Name).FirstOrDefault();
                Label lb = (Label)LogicalTreeHelper.FindLogicalNode(_sp, $"lb_{ workTask.Name}");                
                TimeSpan diff = DateTime.Now - workTask.Activites[workTask.Activites.Count - 1].StartTime;
                lb.Content = diff.ToString(@"hh\:mm\:ss");
            }
        }

        private void BuildInitialForm()
        {
            SetupAddTaskButton();
            MainGrid.Children.Add(_addTaskButton);
            MainGrid.Children.Add(_sp);
        }

        #region UISetup
        private void SetupAddTaskButton()
        {
            _addTaskButton.HorizontalAlignment = HorizontalAlignment.Right;
            _addTaskButton.Content = "AddTask";
            _addTaskButton.Click += _addTaskButton_Click;
        }

        private void SetupReportButton()
        {
            //addReportButton.HorizontalAlignment = HorizontalAlignment.Right;
            //addReportButton.VerticalAlignment = VerticalAlignment.Bottom;
            //addReportButton.Content = "AddTask";
            //addReportButton.Click += _addReportButton_Click;
        }


        #endregion

        #region UIActions

        private void _addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow newTask = new AddTaskWindow(_tasks);
            bool? dr = newTask.ShowDialog();

            if (dr ?? false) // <- This shits' crazy. (dr!=null && dr)
                _tasks = newTask._tasks;
            AddTaskToStackPanel(newTask.task);
        }

        private void AddTaskToStackPanel(WorkTask task)
        {
            Label taskName = new Label
            {
                Content = task.Name
            };

            Button taskButton = new Button
            {
                Name = $"btn_{task.Name}",
                Content = "Start"
            };

            Label taskDuration = new Label
            {
                Name = $"lb_{task.Name}",
                Content = task.TotalDuration.ToString(@"hh\:mm\:ss")
            };

            Label taskComment = new Label
            {
                Content = "Insert comment here"
            };

            taskButton.Click += (sender, e) => HandleTaskButtonStartClick(task, sender, e);

            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Name = $"sp_{task.Name}"
            };

            sp.Children.Add(taskName);
            sp.Children.Add(taskDuration);
            sp.Children.Add(taskButton);
            sp.Children.Add(taskComment);
            _sp.Children.Add(sp);
        }

        private void HandleTaskButtonStartClick(WorkTask workTask, object sender, RoutedEventArgs e)
        {          
            if (activeTask != workTask)
            {
                if(activeTask != null)
                {

                    activeTask.Activites[activeTask.Activites.Count - 1].StopTime = DateTime.Now;
                    activeTask.ComputeTotalDuration(activeTask.Activites.Count - 1);
                    Button oldbtn = (Button)LogicalTreeHelper.FindLogicalNode(_sp, $"btn_{ workTask.Name}");
                    //Button btn = (Button)_sp.FindName($"btn_{workTask.Name}");
                    oldbtn.Content = "Start";                    
                }

                activeTask = workTask;
                Button newbtn = (Button)LogicalTreeHelper.FindLogicalNode(_sp, $"btn_{ workTask.Name}");
                newbtn.Content = "Stop";
                workTask.Activites.Add(new Activity
                {
                    StartTime = DateTime.Now,
                });
            }
            else
            {
                //They clicked stopped;
                activeTask = null;
                workTask.Activites[workTask.Activites.Count - 1].StopTime = DateTime.Now;
                workTask.ComputeTotalDuration(workTask.Activites.Count - 1);
                Button btn = (Button)LogicalTreeHelper.FindLogicalNode(_sp, $"btn_{ workTask.Name}");
                btn.Content = "Start";
            }
        }

        private void HandleTaskButtonStopClick(WorkTask workTask, object sender, RoutedEventArgs e)
        {
            activeTask = null;
            activeTask.Activites[activeTask.Activites.Count - 1].StopTime = DateTime.Now;
            Button btn = (Button)LogicalTreeHelper.FindLogicalNode(_sp, $"btn_{ workTask.Name}");
            btn.Content = "Start";
        }

    }
    #endregion
}
