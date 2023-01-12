namespace CareLinkNetClient.Model;

public class PumpBannerState {

    public static string STATE_DUAL_BOLUS = "DUAL_BOLUS";
    public static string STATE_SQUARE_BOLUS = "SQUARE_BOLUS";
    public static string STATE_LOAD_RESERVOIR = "LOAD_RESERVOIR";
    public static string STATE_SUSPENDED_ON_LOW = "SUSPENDED_ON_LOW";
    public static string STATE_SUSPENDED_BEFORE_LOW = "SUSPENDED_BEFORE_LOW";
    public static string STATE_DELIVERY_SUSPEND = "DELIVERY_SUSPEND";
    public static string STATE_BG_REQUIRED = "BG_REQUIRED";
    public static string STATE_PROCESSING_BG = "PROCESSING_BG";
    public static string STATE_WAIT_TO_ENTER_BG = "WAIT_TO_ENTER_BG";
    public static string STATE_TEMP_TARGET = "TEMP_TARGET";
    public static string STATE_TEMP_BASAL = "TEMP_BASAL";
    public static string STATE_NO_DELIVERY = "NO_DELIVERY";
    public static string STATE_CALIBRATION_REQUIRED = "CALIBRATION_REQUIRED";

    public string Type = string.Empty;
    public int TimeRemaining;

}
