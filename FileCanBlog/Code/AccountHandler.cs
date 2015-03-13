﻿using Duncan.FileCanDB;
using FileCanBlog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace FileCanBlog.Code
{
    public class AccountHandler
    {
        private IFileCanDB<UserModel> FileCanDbUser;
        
        private string DatabaseLocation = ConfigurationManager.AppSettings["DatabaseLocation"];
        private string UserAccountDatabaseEncryptionPassword = ConfigurationManager.AppSettings["UserAccountDatabaseEncryptionPassword"];
        private string UserAccountDatabaseEncryptionSalt = ConfigurationManager.AppSettings["UserAccountDatabaseEncryptionSalt"];
        private string UserAccountDatabaseEncryptionPasswordHash;
        public AccountHandler()
        {
            FileCanDbUser = new FileCanDB<UserModel>(DatabaseLocation, "Admin", "Users", StorageType.encrypted, false);
            UserAccountDatabaseEncryptionPasswordHash = Encryption.GetHash(UserAccountDatabaseEncryptionPassword, GetBytes(UserAccountDatabaseEncryptionSalt));
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = FileCanDbUser.GetPackets(0, 1000, UserAccountDatabaseEncryptionPasswordHash).Select(x => x.Data).ToList();
            return users;
        }

        public bool CreateUser(UserModel user)
        {
            string salt = Encryption.GetSalt(40);
            user.Salt = salt;
            user.LoginCount = 0;
            user.Password = Encryption.GetHash(user.Password, GetBytes(user.Salt));
            user.EnabledDate = DateTime.Now;
            FileCanDbUser.InsertPacket(user.Username, user, UserAccountDatabaseEncryptionPasswordHash);
            return true;
        }

        public UserModel Login(string username, string password, out string message)
        {
            UserModel user = FileCanDbUser.GetPacket(username, UserAccountDatabaseEncryptionPasswordHash).Data;
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
            FileCanDbUser.UpdatePacket(username, user);
            message = "Failed";
            return null;
        }
    }
}