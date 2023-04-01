// Represents a drone object with properties for client name, drone model, service problem, service cost, and service tag
using System.Collections.Generic;
using System.Globalization;
using DroneApplication1;

public class Drone
{
    // Private fields to store property values
    private string clientName;
    private string droneModel;
    private string serviceProblem;
    private double serviceCost;
    private int serviceTag;

    public Drone(int serviceTag, string clientName, string droneModel, string serviceProblem, double serviceCost)
    {
        this.serviceTag = serviceTag;
        this.clientName = clientName;
        this.droneModel = droneModel;
        this.serviceProblem = serviceProblem;
        this.serviceCost = serviceCost;
    }

    // Public properties to access and set private fields with additional formatting using helper methods
    public string ClientName
    {
        get { return clientName; }
        set { clientName = ToTitleCase(value); }
    }

    public string DroneModel
    {
        get { return droneModel; }
        set { droneModel = value; }
    }

    public string ServiceProblem
    {
        get { return serviceProblem; }
        set { serviceProblem = ToSentenceCase(value); }
    }

    public double ServiceCost
    {
        get { return serviceCost; }
        set { serviceCost = value; }
    }

    public int ServiceTag
    {
        get { return serviceTag; }
        set { serviceTag = value; }
    }

    // Public method to display drone information with formatted client name and service cost
    public string Display()
    {
        return $"{ClientName} - ${ServiceCost:F2}";
    }

    // Private helper method to convert input string to title case
    private string ToTitleCase(string input)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

    // Private helper method to convert input string to sentence case
    private string ToSentenceCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}

// Static class to represent service queues for drones
public static class ServiceQueues
{
    // List to store finished drone objects
    public static List<Drone> FinishedList { get; set; } = new List<Drone>();

    // Queue to store drone objects waiting for regular service
    public static Queue<Drone> RegularService { get; set; } = new Queue<Drone>();

    // Queue to store drone objects waiting for express service
    public static Queue<Drone> ExpressService { get; set; } = new Queue<Drone>();
}
