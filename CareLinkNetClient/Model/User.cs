namespace CareLinkNetClient.Model;

public class User {

    public static string ROLE_CARE_PARTNER_US = "CARE_PARTNER";
    public static string ROLE_CARE_PARTNER_OUS = "CARE_PARTNER_OUS";
    public static string ROLE_CARE_PATIENT_US = "CARE_PARTNER";
    public static string ROLE_CARE_PATIENT_OUS = "CARE_PARTNER_OUS";    
    public static string USER_ROLE_CARE_PARTNER = "carepartner";
    public static string USER_ROLE_PATIENT = "patient";

    public DateTime LoginDateUtc;
    public string Id = string.Empty;
    public string Country = string.Empty;
    public string Language = string.Empty;
    public string LastName = string.Empty;
    public string FirstName = string.Empty;
    public int AccountId;
    public string Role = string.Empty;
    public string CpRegistrationStatus = string.Empty;
    public string AccountSuspended = string.Empty;
    public bool NeedToReconsent;
    public bool MfaRequired;
    public bool MfaEnabled;

    public bool IsCarePartner() {
        return(this.Role.Equals(ROLE_CARE_PARTNER_US) || this.Role.Equals(ROLE_CARE_PARTNER_OUS));
    }

    public string GetUserRole() {
        if (IsCarePartner())
        {
            return USER_ROLE_CARE_PARTNER;
        }

        return USER_ROLE_PATIENT;
    }
}
