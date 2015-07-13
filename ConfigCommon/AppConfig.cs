using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ConfigCommon
{
    public class AppConfig
    {
        public static string DefaultConnectionString
        {
            get { return GetConnectionString("DefaultConnection"); }
        }

        public static string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public static string ReadSetting(string key)
        {
            return ReadSetting(key, false);
        }

        public static string ReadSetting(string key, bool OnErrorRaiseException)
        {
            string result = "";
            try
            {
                result = ConfigurationManager.AppSettings[key] ?? "Not Found";
                if ((result == "Not Found") && OnErrorRaiseException)
                { throw new Exception("Key (" + key + ") not found in app settings"); }
            }
            catch (ConfigurationErrorsException)
            {
                throw new ConfigurationErrorsException("Error reading app settings");
                Console.WriteLine("Error reading app settings");
            }


            return result;
        }

        public static void ReadAllSettings()
        {
            throw new NotImplementedException();

            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            // http://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager.appsettings.aspx
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
