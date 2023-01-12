namespace CareLinkNetClient.Model;

public class CountrySettings
{
    public string BgUnits = string.Empty;
    public string BlePereodicDataEndpoint = string.Empty;
    public string CarbDefaultUnit = string.Empty;
    public long CarbExchangeRatioDefault;
    public string CarbohydrateUnitsDefault = string.Empty;
    public bool CpMobileAppAvailable;
    public string DefaultCountryName = string.Empty;
    public string DefaultDevice = string.Empty;
    public string DefaultLanguage = string.Empty;
    public string DialCode = string.Empty;
    public string FirstDayOfWeek = string.Empty;
    public string GlucoseUnitsDefault = string.Empty;
    public List<Language> Languages = new();
    public int LegalAge;
    public string MediaHost = string.Empty;
    public MfaRules Mfa = new();
    public string Name = string.Empty;
    public NumberFormat NumberFormat = new();
    public PostalInfo Postal = new();
    public string RecordSeparator = string.Empty;
    public string Region = string.Empty;
    public ReportDateFormat ReportDateFormat = new();
    public string ShortDateFormat = string.Empty;
    public string ShortTimeFormat = string.Empty;
    public bool SmsSendingAllowed;
    public List<SupportedReport> SupportedReports = new();
    public string TechDays = string.Empty;
    public string TechHours = string.Empty;
    public string TechSupport = string.Empty;
    public string TimeFormat = string.Empty;
    public string TimeUnitsDefault = string.Empty;
    public bool UploaderAllowed;
}