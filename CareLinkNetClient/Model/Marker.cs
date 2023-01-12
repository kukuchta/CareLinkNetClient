namespace CareLinkNetClient.Model;

public class Marker
{
    public static string MARKER_TYPE_MEAL = "MEAL";
    public static string MARKER_TYPE_CALIBRATION = "CALIBRATION";
    public static string MARKER_TYPE_BG_READING = "BG_READING";
    public static string MARKER_TYPE_INSULIN = "INSULIN";
    public static string MARKER_TYPE_AUTO_BASAL = "AUTO_BASAL_DELIVERY";
    public static string MARKER_TYPE_AUTO_MODE_STATUS = "AUTO_MODE_STATUS";

    public string ActivationType = string.Empty;
    public int Amount;
    public bool AutoModeOn;
    public float BolusAmount;
    public string BolusType = string.Empty;
    public bool CalibrationSuccess;
    public bool Completed;
    public DateTime Datetime;
    public float DeliveredExtendedAmount;
    public float DeliveredFastAmount;
    public int EffectiveDuration;
    public int Index;
    public string Kind = string.Empty;
    public int ProgrammedDuration;
    public float ProgrammedExtendedAmount;
    public float ProgrammedFastAmount;
    public int RelativeOffset;
    public string Type = string.Empty;
    public int Value;
    public int Version;
}