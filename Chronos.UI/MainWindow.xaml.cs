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
        private Button _addTaskButton = new Button();
        private WorkTask activeTask = null;
        private DispatcherTimer _clockTimer;
        private BackgroundWorker backgroundWorker;
        private BrushConverter _bc = new BrushConverter();

        public MainWindow()
        {
            InitializeComponent();
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
                Label lb = (Label)LogicalTreeHelper.FindLogicalNode(sp_main, $"lb_{ workTask.Guid}");                
                TimeSpan diff = DateTime.Now - workTask.Activites[workTask.Activites.Count - 1].StartTime;
                lb.Content = diff.ToString(@"hh\:mm\:ss");
            }
        }

        #region UIActions

        private void AddTaskToStackPanel(WorkTask task)
        {
            var background = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));
            background.Opacity = 0.90;            

            WrapPanel panel = new WrapPanel
            {
                Width = sp_main.Width - 10,
                Height = 25,
                Background = background,
                Margin = new Thickness(5)
            };

            Label taskName = new Label
            {
                Content = task.Name,
                Width = 250,
                Foreground = (Brush)_bc.ConvertFrom("#FFF"),
            };

            LinearGradientBrush verticalGradient = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            verticalGradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, 117, 206), 0.0));
            verticalGradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, 72, 130), 1.0));

            Button taskButton = new Button
            {
                Name = $"btn_{task.Guid}",
                Foreground = (Brush)_bc.ConvertFrom("#FFF"),
                Background = verticalGradient,
                Width = 75,
                Height = 20,
                Content = "Start",
                Margin = new Thickness(0,1.5,0,0)
            };

            Label taskDuration = new Label
            {
                Name = $"lb_{task.Guid}",
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
                Name = $"sp_{task.Guid}"
            };

            sp.Children.Add(taskName);            
            sp.Children.Add(taskDuration);
            sp.Children.Add(taskButton);
            sp.Children.Add(taskComment);
            panel.Children.Add(sp);
            sp_main.Children.Add(panel);

            //sp.Children.Add(taskName);
            //sp.Children.Add(taskDuration);
            //sp.Children.Add(taskButton);
            //sp.Children.Add(taskComment);
            //sp_main.Children.Add(sp);
        }

        private void HandleTaskButtonStartClick(WorkTask workTask, object sender, RoutedEventArgs e)
        {          
            if (activeTask != workTask)
            {
                if(activeTask != null)
                {
                    activeTask.Activites[activeTask.Activites.Count - 1].StopTime = DateTime.Now;
                    activeTask.ComputeTotalDuration(activeTask.Activites.Count - 1);
                    Button oldbtn = (Button)LogicalTreeHelper.FindLogicalNode(sp_main, $"btn_{ workTask.Guid}");
                    oldbtn.Content = "Start";                    
                }

                activeTask = workTask;
                Button newbtn = (Button)LogicalTreeHelper.FindLogicalNode(sp_main, $"btn_{ workTask.Guid}");
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
                Button btn = (Button)LogicalTreeHelper.FindLogicalNode(sp_main, $"btn_{ workTask.Guid}");
                btn.Content = "Start";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow newTask = new AddTaskWindow(_tasks);
            bool? dr = newTask.ShowDialog();

            if (dr ?? false) // <- This shits' crazy. (dr!=null && dr)
                _tasks = newTask._tasks;
            AddTaskToStackPanel(newTask.task);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
        }
    }
    #endregion
}
