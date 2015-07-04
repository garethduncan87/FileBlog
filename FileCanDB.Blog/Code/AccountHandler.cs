using FileCanDB;
using FileCanDB.Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace FileCanDB.Blog.Code
{
    public class AccountHandler
    {
        private FileCanDB<UserModel> FileCanDbUser;
        
        private string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        private string UserAccountDatabaseEncryptionPassword = ConfigurationManager.AppSettings["UserAccountDatabaseEncryptionPassword"];
        private string UserAccountDatabaseEncryptionSalt = ConfigurationManager.AppSettings["UserAccountDatabaseEncryptionSalt"];
        private string UserAccountDatabaseEncryptionPasswordHash;
        public AccountHandler()
        {
            UserAccountDatabaseEncryptionPasswordHash = Encryption.GetHash(UserAccountDatabaseEncryptionPassword, GetBytes(UserAccountDatabaseEncryptionSalt));
            FileCanDbUser = new FileCanDB<UserModel>(DatabaseLocation, "Admin", "Users", false, UserAccountDatabaseEncryptionPasswordHash);
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = FileCanDbUser.ReadList(0, 1000).Select(x => x.Data).ToList();
            return users;
        }

        public bool CreateUser(UserModel user)
        {
            string salt = Encryption.GetSalt(40);
            user.Salt = salt;
            user.LoginCount = 0;
            user.Password = Encryption.GetHash(user.Password, GetBytes(user.Salt));
            user.EnabledDate = DateTime.Now;
            FileCanDbUser.Insert(user.Username, user);
            return true;
        }

        public UserModel Login(string username, string password, out string message)
        {
            UserModel user = FileCanDbUser.Read(username).Data;
            if (user.EnabledDate > DateTime.Now)
            {
                message = "Locked";
                return null;
            }
            if (user.Password == Encryption.GetHash(username, GetBytes(user.Salt)))
            {
                message = "Successful";
                return user;
            }
            user.LoginCount = user.LoginCount + 1;
            if (user.LoginCount == 3)
            {
                user.EnabledDate = DateTime.Now.AddMinutes(15);
            }
            FileCanDbUser.Update(username, user);
            message = "Failed";
            return null;
        }
    }
}