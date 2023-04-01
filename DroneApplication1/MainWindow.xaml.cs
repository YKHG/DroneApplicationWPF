using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
            RegularServiceListView.ItemSelectionChanged += RegularServiceListView_ItemSelectionChanged;
            ExpressServiceListView.ItemSelectionChanged += ExpressServiceListView_ItemSelectionChanged;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;

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
        private void costtxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if input is a digit or decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Check if input is already a decimal point
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            // Get values from textboxes
            string clientName = nametxt.Text;
            string droneModel = modeltxt.Text;
            string serviceProblem = problemtxt.Text;
            double serviceCost = Convert.ToDouble(costtxt.Text);
            int serviceTag1 = (int)serviceTag.Value;

            // Get service priority from radio buttons
            string servicePriority = GetServicePriority();

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
            if (Regular.Checked)
            {
                return "Regular";
            }
            else if (Express.Checked)
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
            int currentValue = (int)serviceTag.Value;

            // Increment and set new value
            serviceTag.Value = currentValue + 10;
        }

        // DisplayRegularService custom method
        private void DisplayRegularService()
        {
            // Clear list view items
            RegularServiceListView.Items.Clear();

            // Add service items to list view
            foreach (Drone drone in RegularService)
            {
                ListViewItem item = new ListViewItem(drone.ServiceTag.ToString());
                item.SubItems.Add(drone.ClientName);
                item.SubItems.Add(drone.DroneModel);
                item.SubItems.Add(drone.ServiceCost.ToString("C"));
                item.SubItems.Add(drone.ServiceProblem);
                RegularServiceListView.Items.Add(item);
            }
        }
        private void DisplayExpressService()
        {
            // Clear list view items
            ExpressServiceListView.Items.Clear();

            // Add service items to list view
            foreach (Drone drone in ExpressService)
            {
                ListViewItem item = new ListViewItem(drone.ServiceTag.ToString());
                item.SubItems.Add(drone.ClientName);
                item.SubItems.Add(drone.DroneModel);
                item.SubItems.Add(drone.ServiceCost.ToString("C"));
                item.SubItems.Add(drone.ServiceProblem);
                ExpressServiceListView.Items.Add(item);
            }
        }
        private void RegularServiceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (RegularServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                ListViewItem selectedItem = RegularServiceListView.SelectedItems[0];

                // Get the client name and service problem from the selected item
                string clientName = selectedItem.SubItems[1].Text;
                string serviceProblem = selectedItem.SubItems[4].Text;

                // Display the client name and service problem in the related textboxes
                nametxt.Text = clientName;
                problemtxt.Text = serviceProblem;
            }
        }




        private void ExpressServiceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (ExpressServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                ListViewItem selectedItem = ExpressServiceListView.SelectedItems[0];

                // Get the client name and service problem from the selected item
                string clientName = selectedItem.SubItems[1].Text;
                string serviceProblem = selectedItem.SubItems[4].Text;

                // Display the client name and service problem in the related textboxes
                nametxt.Text = clientName;
                problemtxt.Text = serviceProblem;
            }


        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (RegularServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                ListViewItem selectedItem = RegularServiceListView.SelectedItems[0];

                // Get the service tag from the selected item
                int serviceTag = int.Parse(selectedItem.SubItems[0].Text);

                // Dequeue the corresponding drone from the RegularService queue
                Drone completedDrone = RegularService.Dequeue();

                // Add the completed drone to the FinishedList
                FinishedList.Add(completedDrone);

                // Remove the selected item from the Regular ListView
                RegularServiceListView.Items.Remove(selectedItem);

                // Display the finished drone in the FinishedListBox
                listBox1.Items.Add($"Client Name: {completedDrone.ClientName} - Service Cost: {completedDrone.ServiceCost.ToString("C")}");
            }
            if (ExpressServiceListView.SelectedItems.Count > 0)
            {
                // Get the selected item
                ListViewItem selectedItem = ExpressServiceListView.SelectedItems[0];

                // Get the service tag from the selected item
                int serviceTag = int.Parse(selectedItem.SubItems[0].Text);

                // Dequeue the corresponding drone from the RegularService queue
                Drone completedDrone = ExpressService.Dequeue();

                // Add the completed drone to the FinishedList
                FinishedList.Add(completedDrone);

                // Remove the selected item from the express ListView
                ExpressServiceListView.Items.Remove(selectedItem);

                // Display the finished drone in the FinishedListBox
                listBox1.Items.Add($"Client Name: {completedDrone.ClientName} - Service Cost: {completedDrone.ServiceCost.ToString("C")}");

            }

            // Update the list view
            DisplayRegularService();

            // Clear the textboxes
            ClearTextBoxes();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
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
    }
}