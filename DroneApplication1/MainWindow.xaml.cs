using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextBox = System.Windows.Forms.TextBox;

namespace DroneApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            RegularServiceListView.SelectionChanged += RegularServiceListView_SelectionChanged;
            ExpressServiceListView.SelectionChanged += ExpressServiceListView_SelectionChanged;
        }

        List<Drone> FinishedList = new List<Drone>();
        Queue<Drone> RegularService = new Queue<Drone>();
        Queue<Drone> ExpressService = new Queue<Drone>();

        private void ClearTextBoxes()
        {
            nametxt.Clear();
            modeltxt.Clear();
            problemtxt.Clear();
            costtxt.Clear();
        }

        private void costtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if input is a digit or decimal point
            if (!char.IsControl(e.Text[0]) && !char.IsDigit(e.Text[0]) && (e.Text[0] != '.'))
            {
                e.Handled = true;
            }

            // Check if input is already a decimal point
            if ((e.Text[0] == '.') && (((TextBox)sender).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(serviceTag.Text, out int value))
            {
                serviceTag.Text = (value + 10).ToString();
            }
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(serviceTag.Text, out int value))
            {
                serviceTag.Text = (value - 10).ToString();
            }
        }

        private System.Windows.Controls.TextBox GetServiceTag()
        {
            return serviceTag;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {


            // Get values from textboxes
            string clientName = nametxt.Text;
            string droneModel = modeltxt.Text;
            string serviceProblem = problemtxt.Text;
            double serviceCost = Convert.ToDouble(costtxt.Text);

            // Get service priority from radio buttons
            string servicePriority = GetServicePriority();
            int serviceTag1 = int.Parse(serviceTag.Text);
            // Check if the service tag is within the valid range
            if ( serviceTag1 < 100 || serviceTag1 > 900)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Service Tag should be between 100 and 900");
                return;
            }
            // Increment service tag
            IncrementServiceTag();

            
            // Create new Drone object with input values
            Drone newDrone = new Drone(serviceTag1, clientName, droneModel, serviceProblem, serviceCost);

            // Add new service item to appropriate queue based on priority
            if (servicePriority == "Regular")
            {
                RegularService.Enqueue(newDrone);
                DisplayRegularService();
            }
            else if (servicePriority == "Express")
            {
                // Increase service cost by 15% for express service
                serviceCost *= 1.15;
                newDrone.ServiceCost = serviceCost;
                ExpressService.Enqueue(newDrone);
                DisplayExpressService();
            }

            // Clear textboxes
            ClearTextBoxes();
        }

        // GetServicePriority custom method
        private string GetServicePriority()
        {
            if (Regular.IsChecked == true)
            {
                return "Regular";
            }
            else if (Express.IsChecked == true)
            {
                return "Express";
            }
            else
            {
                return "";
            }
        }



        // IncrementServiceTag custom method
        private void IncrementServiceTag()
        {
            // Get current service tag value
            int currentValue = int.Parse(serviceTag.Text);

            // Increment and set new value
            serviceTag.Text = (currentValue + 10).ToString();
        }


        
        private void DisplayRegularService()
        {
            // Clear list view items
            RegularServiceListView.Items.Clear();

            // Add service items to list view
            foreach (Drone drone in RegularService)
            {
                // Create an object of a class that contains properties corresponding to each column in the ListView
                var item = new
                {
                    serviceTag = drone.ServiceTag.ToString(),
                    Names = drone.ClientName,
                    Models = drone.DroneModel,
                    Costs = drone.ServiceCost.ToString("C"),
                    Problem = drone.ServiceProblem
                };

                // Add the object to the list 
                RegularServiceListView.Items.Add(item);
            }
        }



        private void DisplayExpressService()
        {
            ExpressServiceListView.Items.Clear();
            // Clear list view items
            foreach (Drone drone in ExpressService)
            {
                // Create an object of a class that contains properties corresponding to each column in the ListView
                var item = new
                {
                    serviceTag = drone.ServiceTag.ToString(),
                    Names = drone.ClientName,
                    Models = drone.DroneModel,
                    Costs = drone.ServiceCost.ToString("C"),
                    Problem = drone.ServiceProblem
                };

                // Add the object to the list 
                ExpressServiceListView.Items.Add(item);
            }
        }

        private System.Windows.Controls.ListView GetRegularServiceListView()
        {
            return RegularServiceListView;
        }

        private void RegularServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (RegularServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                var selectedItem = (dynamic)RegularServiceListView.SelectedItems[0];

                // Get the client name and service problem from the selected item
                string clientName = selectedItem.Names;
                string serviceProblem = selectedItem.Problem;

                // Display the client name and service problem in the related textboxes
                nametxt.Text = clientName;
                problemtxt.Text = serviceProblem;
            }
        }





        private void ExpressServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (ExpressServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                var selectedItem = (dynamic)ExpressServiceListView.SelectedItems[0];

                // Get the client name and service problem from the selected item
                string clientName = selectedItem.Names;
                string serviceProblem = selectedItem.Problem;

                // Display the client name and service problem in the related textboxes
                nametxt.Text = clientName;
                problemtxt.Text = serviceProblem;
            }


        }

        
        
            private void Delete_Click(object sender, RoutedEventArgs e)
            {
                if (RegularServiceListView.SelectedItems.Count > 0)
                {
                    // Get the selected item
                    var selectedItem = RegularServiceListView.SelectedItem as Drone;

                    // Dequeue the corresponding drone from the RegularService queue
                    Drone completedDrone = RegularService.Dequeue();

                    // Add the completed drone to the FinishedList
                    FinishedList.Add(completedDrone);

                    // Remove the selected item from the Regular ListView
                    RegularServiceListView.Items.Remove(selectedItem);

                    // Display the finished drone in the FinishedListBox
                    
                listBox1.Items.Add($"Client Name: {completedDrone.ClientName} - Service Cost: {completedDrone.ServiceCost.ToString("C")}");
                // Update the list view
                DisplayRegularService();
            }
                if (ExpressServiceListView.SelectedItems.Count > 0)
                {
                    // Get the selected item
                    var selectedItem = ExpressServiceListView.SelectedItem as Drone;

                    // Dequeue the corresponding drone from the RegularService queue
                    Drone completedDrone = ExpressService.Dequeue();

                    // Add the completed drone to the FinishedList
                    FinishedList.Add(completedDrone);

                    // Remove the selected item from the express ListView
                    ExpressServiceListView.Items.Remove(selectedItem);

                    // Display the finished drone in the FinishedListBox
                    listBox1.Items.Add($"Client Name: {completedDrone.ClientName} - Service Cost: {completedDrone.ServiceCost.ToString("C")}");
                // Update the list view
                DisplayExpressService();
            }

  


                // Clear the textboxes
                ClearTextBoxes();
            }


        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // Get the selected item and remove it from the listbox
                string selectedService = listBox1.SelectedItem.ToString();
                listBox1.Items.Remove(selectedService);

                // Find and remove the same service from the FinishedList
                foreach (Drone drone in FinishedList)
                {
                    if (drone.ToString() == selectedService)
                    {
                        FinishedList.Remove(drone);
                        break;
                    }
                }
            }
        }

        private void serviceTag_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the entered text is a number
            if (!int.TryParse(e.Text, out int result))
            {
                e.Handled = true; // Ignore non-numeric input
                return;
            }

            // Check if the entered number exceeds the maximum value
            int currentValue = int.Parse(serviceTag.Text + e.Text);
            if (currentValue > 900)
            {
                e.Handled = true; // Ignore input that exceeds the limit
            }
        }
    }
}