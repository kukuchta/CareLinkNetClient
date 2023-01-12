namespace CareLinkNetClient.Model;

public class RecentData {

    public static string DEVICE_FAMILY_GUARDIAN = "GUARDIAN";
    public static string DEVICE_FAMILY_NGP = "NGP";

    //sensorState
    public static string SENSOR_STATE_CALIBRATION_REQUIRED = "CALIBRATION_REQUIRED";
    public static string SENSOR_STATE_NO_DATA_FROM_PUMP = "NO_DATA_FROM_PUMP";
    public static string SENSOR_STATE_WAIT_TO_CALIBRATE = "WAIT_TO_CALIBRATE";
    public static string SENSOR_STATE_DO_NOT_CALIBRATE = "DO_NOT_CALIBRATE";
    public static string SENSOR_STATE_CALIBRATING = "CALIBRATING";
    public static string SENSOR_STATE_NO_ERROR_MESSAGE = "NO_ERROR_MESSAGE";
    public static string SENSOR_STATE_WARM_UP = "WARM_UP";
    public static string SENSOR_STATE_CHANGE_SENSOR = "CHANGE_SENSOR";
    public static string SENSOR_STATE_NORMAL = "NORMAL";
    public static string SENSOR_STATE_UNKNOWN = "UNKNOWN";

    //systemStatusMessage
    public static string SYSTEM_STATUS_BLUETOOTH_OFF = "BLUETOOTH_OFF";
    public static string SYSTEM_STATUS_UPDATING = "UPDATING";
    public static string SYSTEM_STATUS_RECONNECTING_TO_PUMP = "RECONNECTING_TO_PUMP";
    public static string SYSTEM_STATUS_PUMP_PAIRING_LOST = "PUMP_PAIRING_LOST";
    public static string SYSTEM_STATUS_LOST_PUMP_SIGNAL = "LOST_PUMP_SIGNAL";
    public static string SYSTEM_STATUS_PUMP_NOT_PAIRED = "PUMP_NOT_PAIRED";
    public static string SYSTEM_STATUS_SENSOR_OFF = "SENSOR_OFF";
    public static string SYSTEM_STATUS_NO_ERROR_MESSAGE = "NO_ERROR_MESSAGE";
    public static string SYSTEM_STATUS_CALIBRATION_REQUIRED = "CALIBRATION_REQUIRED";

    public string GetDeviceFamily(){
        return MedicalDeviceFamily;
    }
    public bool IsGm(){
        return GetDeviceFamily().Equals(DEVICE_FAMILY_GUARDIAN);
    }
    public bool IsNgp(){
        return GetDeviceFamily().Equals(DEVICE_FAMILY_NGP);
    }

    public long LastSensorTs;
    public DateTime MedicalDeviceTimeAsString;
    public DateTime LastSensorTsAsString;
    public string Kind = string.Empty;
    public int Version;
    public string PumpModelNumber = string.Empty;
    public long CurrentServerTime;
    public long LastConduitTime;
    public long LastConduitUpdateServerTime;
    public long LastMedicalDeviceDataUpdateServerTime;
    public string FirstName = string.Empty;
    public string LastName = string.Empty;
    public string ConduitSerialNumber = string.Empty;
    public int ConduitBatteryLevel;
    public string ConduitBatteryStatus = string.Empty;
    public bool ConduitInRange;
    public bool ConduitMedicalDeviceInRange;
    public bool ConduitSensorInRange;
    public string MedicalDeviceFamily = string.Empty;
    public string SensorState = string.Empty;
    public string MedicalDeviceSerialNumber = string.Empty;
    public long MedicalDeviceTime;
    public DateTime sMedicalDeviceTime;
    public int ReservoirLevelPercent;
    public int ReservoirAmount;
    public float ReservoirRemainingUnits;
    public int MedicalDeviceBatteryLevelPercent;
    public int SensorDurationHours;
    public int TimeToNextCalibHours;
    public string CalibStatus = string.Empty;
    public string BgUnits = string.Empty;
    public string TimeFormat = string.Empty;
    public long LastSensorTime;
    public DateTime sLastSensorTime;
    public bool MedicalDeviceSuspended;
    public string LastSgTrend = string.Empty;
    public SensorGlucose LastSg = new();
    public Alarm LastAlarm = new();
    public ActiveInsulin ActiveInsulin = new();
    public List<SensorGlucose> Sgs = new();
    public List<Limit> Limits = new();
    public List<Marker> Markers = new();
    public NotificationHistory NotificationHistory = new();
    public TherapyAlgorithmState TherapyAlgorithmState = new();
    public List<PumpBannerState> PumpBannerState = new();
    public Basal Basal = new();
    public string SystemStatusMessage = string.Empty;
    public int AverageSg;
    public int BelowHypoLimit;
    public int AboveHyperLimit;
    public int TimeInRange;
    public bool PumpCommunicationState;
    public bool GstCommunicationState;
    public int GstBatteryLevel;
    public DateTime LastConduitDateTime;
    public float MaxAutoBasalRate;
    public float MaxBolusAmount;
    public int SensorDurationMinutes;
    public int TimeToNextCalibrationMinutes;
    public string ClientTimeZoneName = string.Empty;
    public int SgBelowLimit;
    public float AverageSgFloat;
    public bool CalFreeSensor;
    public bool FinalCalibration;
}
