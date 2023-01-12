namespace CareLinkNetClient.Model;

public class ClearedNotification
{
    public bool AlertSilenced;
    public DateTime Datetime;
    public int FaultId;
    public string Guid = string.Empty;
    public int InstanceId;
    public string MessageId = string.Empty;
    public string PnpId = string.Empty;
    public string PumpDeliverySuspendState = string.Empty;
    public string ReferenceGuid = string.Empty;
    public int RelativeOffset;
    public DateTime TriggeredDateTime;
    public string Type = string.Empty;
}