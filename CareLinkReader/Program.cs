using CareLinkNetClient.Model;
using CareLinkNetClient;

namespace CareLinkReader
{
    internal class Program
    {
        private const string Username = "password";
        private const string Password = "username";
        private const string CountryCode = "pl";
        static void Main(string[] args)
        {
            callCareLinkClient(true, Username, Password, CountryCode, true, true, false, String.Empty, 2, 5);
        }

        private static void callCareLinkClient(bool verbose, String username, String password, String country, bool downloadSessionInfo, bool downloadData, bool anonymize, String folder, int repeat, int wait)
        {

            CareLinkClient client = null;
            RecentData recentData = null;

            client = new CareLinkClient(username, password, country);
            if (verbose) PrintLog("Client created!");

            if (client.login())
            {

                for (int i = 0; i < repeat; i++)
                {
                    if (verbose) PrintLog("Starting download, count:  " + (i + 1));
                    //Session info is requested
                    if (downloadSessionInfo)
                    {
                        writeJson(client.GetSessionUser(), folder, "user", anonymize, verbose);
                        writeJson(client.GetSessionProfile(), folder, "profile", anonymize, verbose);
                        writeJson(client.GetSessionCountrySettings(), folder, "country", anonymize, verbose);
                        writeJson(client.GetSessionMonitorData(), folder, "monitor", anonymize, verbose);
                    }
                    //Recent data is requested
                    if (downloadData)
                    {
                        try
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                recentData = client.GetRecentData();
                                //Auth error
                                if (client.getLastResponseCode() == 401)
                                {
                                    PrintLog("GetRecentData login error (response code 401). Trying again in 1 sec!");
                                    Thread.Sleep(1000);
                                }
                                //Get success
                                else if (client.getLastResponseCode() == 200)
                                {
                                    //Data OK
                                    if (client.getLastDataSuccess())
                                    {
                                        Console.WriteLine($"{DateTime.Now} Reading: {recentData.LastSg.Sg}  Trend: {recentData.LastSgTrend}  Base: {recentData.Basal.BasalRate} ");
                                        writeJson(recentData, folder, "data", anonymize, verbose);
                                        //Data error
                                    }
                                    else
                                    {
                                        PrintLog("Data exception: " + (client.getLastErrorMessage() == null ? "no details available" : client.getLastErrorMessage()));
                                    }
                                    //STOP!!!
                                    break;
                                }
                                else
                                {
                                    PrintLog("Error, response code: " + client.getLastResponseCode() + " Trying again in 1 sec!");
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    try
                    {
                        if (i < repeat - 1)
                        {
                            if (verbose) 
                                PrintLog("Waiting " + wait + " minutes before next download!");
                            Thread.Sleep(wait * 60000);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            else
            {
                PrintLog("Client login error! Response code: " + client.getLastResponseCode() + " Error message: " + client.getLastErrorMessage());
            }
        }

        protected static void writeJson(Object obj, String folder, String name, bool anonymize, bool verbose)
        {

            String content;

            //Anonymize data
            if (anonymize)
            {
                anonymizeData(obj);
            }
            // read obj
        }


        protected static void anonymizeData(Object obj)
        {

            User user;
            Profile profile;
            RecentData recentData;

            if (obj != null)
            {
                if (obj is User){
                    user = (User)obj;
                    user.AccountId = 99999999;
                    user.Id = user.AccountId.ToString();
                    user.LastName = "LastName";
                    user.FirstName = "FirstName";
                } else if (obj is Profile){
                    profile = (Profile)obj;
                    profile.Address = "Address";
                    profile.FirstName = "FirstName";
                    profile.LastName = "LastName";
                    profile.MiddleName = "MiddleName";
                    profile.DateOfBirth = "1900-01-01";
                    profile.City = "City";
                    profile.Email = "email@email.email";
                    profile.ParentFirstName = "ParentFirstName";
                    profile.ParentLastName = "ParentLastName";
                    profile.Phone = "+00-00-000000";
                    profile.PhoneLegacy = "+00-00-000000";
                    profile.PostalCode = "9999";
                    profile.PatientNickname = "Nickname";
                    profile.StateProvince = "State";
                    profile.Username = "Username";
                } else if (obj is RecentData){
                    recentData = (RecentData)obj;
                    recentData.FirstName = "FirstName";
                    recentData.LastName = "LastName";
                    recentData.MedicalDeviceSerialNumber = "SN9999999X";
                    recentData.ConduitSerialNumber = "XXXXXX-XXXX-XXXX-XXXX-9999-9999-9999-9999";
                }
            }
        }

        protected static void PrintLog(String logText)
        {
            Console.WriteLine(DateTime.Now + " " + logText);
        }
    }
}