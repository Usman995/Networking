using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcw.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        public User()
        {
            // empty constructor
        }

        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public enum Role
        {
            User,
            Admin
        }
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        public static User CreateUser(string pUsername)
        {
            // Create a new user with a string given as a parameter then create a new GUID which is saved as a string
            // to the db as the ApiKey. This method must return the APIKey or the user object so that the server can pass the key
            // back to the client


            return null;
        }


        public static bool CheckUser(string pApiKey)
        {
            // chech the apiKey of this with any user in database
            if (pApiKey == "000")
                return true;
            else return false;
        }

        public static bool CheckUser(string pApiKey, string pUserName)
        {
            // check if the user exists with this username and this apikey, if any of them is wrong return false, else true;
            return true;
        }

        public static bool DeleteUser(string pApiKey)
        {
            // delete the user from the db with the given ApiKey
            return true;
        }

        public static User ReturnUser(string pApiKey)
        {
            // checking if the user exists and returns it or returns null

            return null;
        }
        #endregion
    }


}