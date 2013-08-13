using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using UserSettingsLib;
using EncryptionLib;
using ErrorLoggingLib;
using ApplicationDataClass;
using System.Threading;


namespace EmailServicesLib
{
    public partial class EmailServices
    {


        public class EMAIL_SETTINGS
        {
            public const string userNameSetting = "user name";
            public const string outBoundServerSetting = "outbound server";
            public const string fromAddressSetting = "from address";
            public const string adminAddressSetting = "admin address";
            public const string passwordSetting = "password";

            bool _EmailDataLoaded = false;

            string _userName;
            string _outBoundServer;
            string _fromAddress;
            string _adminAddress;
            string _passwordEnc;
            string _password;

            object singleton;

            public EMAIL_SETTINGS()
            {
                singleton = new object();
            }

            public void LoadSettings()
            {
                // load the settings

                _userName = UserSettings.Get(userNameSetting);
                _outBoundServer = UserSettings.Get(outBoundServerSetting);
                _fromAddress = UserSettings.Get(fromAddressSetting);
                _adminAddress = UserSettings.Get(adminAddressSetting);
                _passwordEnc = UserSettings.Get(passwordSetting);
                _password = null;
                if (_passwordEnc != null && _userName != null)
                {
                    _password = decryptPassword(_passwordEnc, _userName);
                }

                if (_userName == null || _outBoundServer == null || _fromAddress == null || _adminAddress == null || _password == null)
                {
                    _EmailDataLoaded = false;
                    return;
                }

                _EmailDataLoaded = true;
            }

            public bool EmailDataLoaded
            { get { lock (singleton) { return _EmailDataLoaded; } } set { lock (singleton) { } } }

            public string UserName
            {
                get { lock (singleton) { return _userName; } }
                set { lock (singleton) { _userName = value; UserSettings.Set(userNameSetting, value); } }
            }

            public string OutBoundServer
            {
                get { lock (singleton) { return _outBoundServer; } }
                set { lock (singleton) { _outBoundServer = value; UserSettings.Set(outBoundServerSetting, value); } }
            }

            public string FromAddress
            {
                get { lock (singleton) { return _fromAddress; } }
                set { lock (singleton) { _fromAddress = value; UserSettings.Set(fromAddressSetting, value); } }
            }

            public string AdminAddress
            {
                get { lock (singleton) { return _adminAddress; } }
                set { lock (singleton) { _adminAddress = value; UserSettings.Set(adminAddressSetting, value); } }
            }

            public string Password
            {
                get
                {
                    lock (singleton)
                    {
                        return _password;
                    }
                }
                set { lock (singleton) { UserSettings.Set(passwordSetting, encryptPassword(value, _userName)); } }
            }

            byte[] salt = { 4, 1, 2, 3, 4, 1, 2, 3, 4 };

            public string encryptPassword(string passwordInClear, string keyString)
            {
                string encryptedPassword;

                Encryption encrypter = new Encryption(Encryption.EncryptionTypes.DES);

                encrypter.Password = keyString;

                encrypter.Salt = salt;

                encryptedPassword = encrypter.Encrypt(passwordInClear);

                return (encryptedPassword);

            }


            public string decryptPassword(string passwordEncrypted, string keyString)
            {
                string decryptedPassword;

                if (passwordEncrypted == null) return (null);
                if (passwordEncrypted.Length < 1) return (null);
                if (keyString == null) return (null);
                if (keyString.Length < 1) return (null);

                Encryption decrypter = new Encryption(Encryption.EncryptionTypes.DES);

                decrypter.Salt = salt;

                decrypter.Password = keyString;

                decryptedPassword = decrypter.Decrypt(passwordEncrypted);

                return (decryptedPassword);

            }


        }


    }

}