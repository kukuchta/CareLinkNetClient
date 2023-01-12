namespace CareLinkNetClient.Model;

public class ActiveNotification
{
    public bool AlertSilenced;
    public DateTime Datetime;
    public int FaultId;
    public string Guid = string.Empty;
    public int InstanceId;
    public string MessageId = string.Empty;
    public string PnpId = string.Empty;
    public string PumpDeliverySuspendState = string.Empty;
    public int RelativeOffset;
    public string Type = string.Empty;
}