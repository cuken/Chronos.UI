using Chronos.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Chronos.UI
{
    /// <summary>
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        public List<WorkTask> _tasks = new List<WorkTask>();
        public WorkTask task = new WorkTask();

        public AddTaskWindow(List<WorkTask> Tasks)
        {
            InitializeComponent();
            _tasks = Tasks;
        }

        private void Tb_textName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Btn_Add_Click(sender, null);
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            string activityName = tb_textName.Text;
            if(_tasks.Any(x => x.Name.ToUpper() == activityName.ToUpper()))
            {
                //They've provided the same name as a name in our TaskList
            }
            else
            {
                task.Name = activityName;
                _tasks.Add(task);
                DialogResult = true;
                Close();
            }
        }
    }
}
