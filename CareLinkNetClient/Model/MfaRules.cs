namespace CareLinkNetClient.Model;

public class MfaRules
{
    public int CodeValidityDuration;
    public string FromDate = string.Empty;
    public int GracePeriod;
    public int MaxAttempts;
    public int RememberPeriod;
    public string Status = string.Empty;
}