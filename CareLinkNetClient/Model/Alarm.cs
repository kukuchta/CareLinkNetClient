namespace CareLinkNetClient.Model;

public class Alarm
{
    public int Code;
    public DateTime Datetime;
    public bool Flash;
    public int InstanceId;
    public string Kind = string.Empty;
    public string MessageId = string.Empty;
    public bool PumpDeliverySuspendState;
    public string ReferenceGuid = string.Empty;
    public int Sg;
    public string Type = string.Empty;
    public long Version;
}