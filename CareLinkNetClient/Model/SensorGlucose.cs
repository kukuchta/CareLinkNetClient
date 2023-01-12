namespace CareLinkNetClient.Model;

public class SensorGlucose {

    public int Sg;
    public DateTime Datetime;
    public bool TimeChange;
    public string Kind = string.Empty;
    public int Version;
    public string SensorState = string.Empty;
    public int RelativeOffset;

    public string ToS() {
        string dt;
        if (Datetime == null)
        {
            dt = "";
        }
        else
        {
            dt = Datetime.ToString();
        }
        return dt + " "  + Sg;
    }

}
