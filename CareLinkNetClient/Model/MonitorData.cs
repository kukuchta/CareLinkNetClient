namespace CareLinkNetClient.Model;

public class MonitorData
{
    public string DeviceFamily = string.Empty;

    public bool IsBle()
    {
        return DeviceFamily.Contains("BLE");
    }
}