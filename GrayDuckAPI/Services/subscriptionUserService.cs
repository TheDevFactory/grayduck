using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GrayDuck.Services
{
    public class subscriptionUserService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
 
        public subscriptionUserService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public string GenerateRandomAPIToken() {

            //var tokenBuilder = new StringBuilder();
            //RandomGenerator objRandom = new RandomGenerator();

            //// 10-Letters lower case   
            //tokenBuilder.Append(objRandom.RandomString(10, true));

            //// 4-Digits between 1000 and 9999  
            //tokenBuilder.Append(objRandom.RandomNumber(1000, 9999));

            //// 5-Letters upper case  
            //tokenBuilder.Append(objRandom.RandomString(5));
            //return tokenBuilder.ToString();


            //Use a Guid Instead.. used during the sign up process also
            return Guid.NewGuid().ToString();

        }

        // Generates a random password.  
        // 4-LowerCase + 4-Digits + 2-UpperCase  
        public async Task<string> GenerateRandomPassword()
        {

            try {
                var passwordBuilder = new StringBuilder();
                RandomGenerator objRandom = new RandomGenerator();

                // 4-Letters lower case   
                passwordBuilder.Append(objRandom.RandomString(4, true));

                // 4-Digits between 1000 and 9999  
                passwordBuilder.Append(objRandom.RandomNumber(1000, 9999));

                // 2-Letters upper case  
                passwordBuilder.Append(objRandom.RandomString(2));
                return passwordBuilder.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<DataTable> Get()
        {
            try {
                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                String strSubscriptionIds = "";

                //Build string with subscription Id's
                foreach (subscriptionSecurityModel objAuthSecurity in objAuthIdentity.authSecurity)
                {

                    if (objAuthSecurity.subscriptionUserView == true || objAuthSecurity.subscriptionManageAll == true) {

                        if (strSubscriptionIds == "")
                        {
                            strSubscriptionIds = "'" + objAuthSecurity.subscriptionId.ToString() + "'";
                        }
                        else
                        {
                            strSubscriptionIds = strSubscriptionIds + ",'" + objAuthSecurity.subscriptionId.ToString() + "'";
                        }

                    } 
                
                }

                //Read the database
                _dataTable = await _databaseManager.executeReader("SELECT subscriptionuser.id, subscriptionuser.email, subscriptionuser.mobile, subscriptionuser.apitoken, subscriptionuser.isactive, subscriptionuser.isdeleted, subscriptionuser.createdbyid, subscriptionuser.createdat, subscriptionuser.updatedat FROM public.subscriptionuser INNER JOIN public.subscriptionuseraccess ON subscriptionuser.id = subscriptionuseraccess.subscriptionuserid WHERE subscriptionuseraccess.subscriptionid in (" + strSubscriptionIds + "); ");

                return _dataTable;
            }        
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task <string> Get(Guid id)
        {

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                String strJSon;

                //Read the database
                _dataTable = await _databaseManager.executeReader("SELECT id, email, mobile, apitoken, isactive, isdeleted, createdbyid, createdat, updatedat FROM public.subscriptionuser where id='" + id + "';");

                //Convert to Json string
                strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                return strJSon;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<subscriptionUserModel> Create(subscriptionUserModel objNew)
        {
            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            subscriptionUserService objSubscriptionUser = new subscriptionUserService(objConfiguration, null);

            string strNewUserId = "";
            string strNewUserSecLinkId = "";
            string strExistingSubUserGuid = "";
            
            //string strNewUserPassword = await objSubscriptionUser.GenerateRandomPassword(); //Generate New Password
            string strNewUserAPIToken = objSubscriptionUser.GenerateRandomAPIToken(); //Generate New API Token

            bool bolProceed = false;
            string strProcessMessage = "";

            try
            {

                //Process to create a new subscription and return the subscription details...

                //0. Check for a duplicate Useremail.. as this is the key item.
                //   The Signup code will not ALLOW duplicate email addresses to create a subscription, or a duplicate user that already exists using that email address.
                //   We would rather grant access to an email address to a subscription
                //   This does slow down the sign up process but allows for control over the user and subscription base.

                strExistingSubUserGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.subscriptionuser where email='" + _databaseManager.sqlCheck(objNew.email.ToLower()) + "'");

                if (strExistingSubUserGuid == "")
                {
                    bolProceed = true;
                    strProcessMessage = "";
                }
                else
                {
                    bolProceed = false;
                    strProcessMessage = "Existing user account found for email address. Please use another email address.";
                }


                //If we are allowed to proceed the process can start
                if (bolProceed == true)
                {

                    //1. Create New user for subscription
                    strNewUserId = await _databaseManager.executeQuery("INSERT INTO public.subscriptionuser (email, password, mobile, apitoken, isactive, isdeleted, createdat, updatedat, createdbyid) VALUES ('" + _databaseManager.sqlCheck(objNew.email.ToLower()) + "','" + _databaseManager.sqlCheck(objNew.password) + "','" + _databaseManager.sqlCheck(objNew.mobile) + "','" + _databaseManager.sqlCheck(strNewUserAPIToken) + "',true,false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, '" + objNew.createdById + "') RETURNING Id;");

                    if (strNewUserId == "" || strNewUserId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new subscription user. " + strNewUserId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New subscription user created. ";
                    }


                    //2. Link New User and New security group
                    strNewUserSecLinkId = await _databaseManager.executeQuery("INSERT INTO public.subscriptionuseraccess (subscriptionid, subscriptionsecurityid, subscriptionuserid) VALUES ('" + objNew.subscriptionAccess[0].subscriptionId + "', '" + objNew.subscriptionAccess[0].subscriptionSecurityId + "', '" + strNewUserId + "') RETURNING Id;");

                    if (strNewUserSecLinkId == "" || strNewUserSecLinkId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new subscription user access link. " + strNewUserSecLinkId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New subscription user access granted. ";
                    }

                }


                //Verify that we can proceed and return the data back to the user
                if (bolProceed == true)
                {
                    objNew.Id = Guid.Parse(strNewUserId);
                    objNew.apiToken = strNewUserAPIToken;

                    //objNewSubscription.signupCode = "Success";
                    //objNewSubscription.signupMessage = strProcessMessage; ;
                }
                else
                {
                    objNew.apiToken = strProcessMessage;
                    
                    //objNewSubscription.signupCode = "Error";
                    //objNewSubscription.signupMessage = strProcessMessage;
                }


                return objNew;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //Two Methods (overload)
        public async Task <identityModel> AuthenticateUser(string email, Guid apitoken)
        {
            identityModel objUserIdentity = new identityModel();

            try
            {


                if (apitoken == null || apitoken == Guid.Empty)
                {

                    //If no valid token is found.. return 
                    objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                    objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                    objUserIdentity.authResult = "Invalid Auth Token provided."; //Error Message or Success Message
                    objUserIdentity.email = "";
                    //objUserIdentity.subscriptionId; //List of Subscriptions this user has access to

                }
                else
                {

                    databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                    DataTable _dataTable;
                    int intCount = 0;

                    //Read the database
                    _dataTable = await _databaseManager.executeReader("SELECT t1.*, t3.id as userid, t3.email, t3.apitoken, t3.isactive, t3.isdeleted FROM subscriptionsecurity t1 INNER JOIN subscriptionuseraccess t2 ON t1.subscriptionId = t2.subscriptionId INNER JOIN subscriptionuser t3 ON t2.subscriptionuserid=t3.id WHERE t3.apitoken='" + apitoken + "'");

                    if (_dataTable == null)
                    {

                        objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                        objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                        objUserIdentity.authResult = "Invalid Auth Token provided."; //Error Message or Success Message
                        objUserIdentity.email = "";

                    }
                    else
                    {

                        if (_dataTable.Rows.Count == 0)
                        {

                            objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                            objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                            objUserIdentity.authResult = "Invalid Auth Token provided."; //Error Message or Success Message
                            objUserIdentity.email = "";

                        }
                        else
                        {

                            //Create List Object
                            objUserIdentity.authSecurity = new List<subscriptionSecurityModel> { };

                            //We have data that is correct... so lets verify teh user exists, is active, not deleted and has access (return the model)
                            foreach (DataRow row in _dataTable.Rows)
                            {

                                if (row["isactive"] is false || row["isdeleted"] is true)
                                {

                                    objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                                    objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                                    objUserIdentity.authResult = "Inactive or Removed user account."; //Error Message or Success Message
                                    objUserIdentity.email = "";

                                }
                                else
                                {

                                    objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                                    objUserIdentity.authMessage = "SUCCESS"; //SUCCESS OR FAILED
                                    objUserIdentity.authResult = "Success, access granted via APIToken."; //Error Message or Success Message
                                    objUserIdentity.email = row["email"].ToString();
                                    objUserIdentity.authUserId = (Guid)row["userid"]; //Used to log activities

                                    //Add the new Security Object in here...
                                    objUserIdentity.authSecurity.Add(new subscriptionSecurityModel());

                                    //A user could be assigned to many Subscriptions... so we need to load all permissions
                                    //This is rare, one user will typically have access to only one subscription.
                                    //A user has permissions per subscription, so I could be an admin in one subscription and a normal user in another
                                    subscriptionSecurityModel objNewAuthSecurity = new subscriptionSecurityModel
                                    {
                                        Id = (Guid)row["id"],
                                        subscriptionId = (Guid)row["subscriptionid"],
                                        name = row["name"].ToString(),
                                        subscriptionManageAll = (bool)row["subscriptionManageAll"],
                                        serverlogView = (bool)row["serverlogView"],
                                        serverlogDelete = (bool)row["serverlogDelete"],
                                        subscriptionView = (bool)row["subscriptionView"],
                                        subscriptionAdd = (bool)row["subscriptionAdd"],
                                        subscriptionEdit = (bool)row["subscriptionEdit"],
                                        subscriptionDelete = (bool)row["subscriptionDelete"],
                                        subscriptionUserView = (bool)row["subscriptionUserView"],
                                        subscriptionUserAdd = (bool)row["subscriptionUserAdd"],
                                        subscriptionUserEdit = (bool)row["subscriptionUserEdit"],
                                        subscriptionUserDelete = (bool)row["subscriptionUserDelete"],
                                        accountView = (bool)row["accountView"],
                                        accountAdd = (bool)row["accountAdd"],
                                        accountEdit = (bool)row["accountEdit"],
                                        accountDelete = (bool)row["accountDelete"],
                                        contactView = (bool)row["contactView"],
                                        contactAdd = (bool)row["contactAdd"],
                                        contactEdit = (bool)row["contactEdit"],
                                        contactDelete = (bool)row["contactDelete"],
                                        activityView = (bool)row["activityView"],
                                        activityAdd = (bool)row["activityAdd"],
                                        activityEdit = (bool)row["activityEdit"],
                                        activityDelete = (bool)row["activityDelete"],
                                        activityTypesView = (bool)row["activityTypesView"],
                                        activityTypesAdd = (bool)row["activityTypesAdd"],
                                        activityTypesEdit = (bool)row["activityTypesEdit"],
                                        activityTypesDelete = (bool)row["activityTypesDelete"],
                                        scoringView = (bool)row["scoringView"],
                                        scoringAdd = (bool)row["scoringAdd"],
                                        scoringEdit = (bool)row["scoringEdit"],
                                        scoringDelete = (bool)row["scoringDelete"],
                                        customFieldsView = (bool)row["customFieldsView"],
                                        customFieldsAdd = (bool)row["customFieldsAdd"],
                                        customFieldsEdit = (bool)row["customFieldsEdit"],
                                        customFieldsDelete = (bool)row["customFieldsDelete"],
                                        auditlogView = (bool)row["auditlogView"],
                                        auditlogDelete = (bool)row["auditlogDelete"]
                                    };

                                    objUserIdentity.authSecurity[intCount] = objNewAuthSecurity;
                                    intCount += 1;

                                }

                            }

                        }

                    }

                }

                return objUserIdentity;

            }
            catch (Exception ex)
            {

                //Log Exception in Code (Example) To Rollbar
                Rollbar.RollbarLocator.RollbarInstance.Error(ex);

                //Create Object to return
                objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                objUserIdentity.authResult = "Auth exception, please try again later. " + ex.Message.ToString(); //Error Message or Success Message
                objUserIdentity.email = "";

                return objUserIdentity;
            }



        }

        public async Task <identityModel> AuthenticateUser(string email, string password)
        {
            identityModel objUserIdentity = new identityModel();

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                int intCount = 0;

                //Read the database
                _dataTable = await _databaseManager.executeReader("SELECT t1.*, t3.id as userid, t3.email, t3.password, t3.apitoken, t3.isactive, t3.isdeleted FROM subscriptionsecurity t1 INNER JOIN subscriptionuseraccess t2 ON t1.subscriptionId = t2.subscriptionId INNER JOIN subscriptionuser t3 ON t2.subscriptionuserid=t3.id WHERE t3.email='" + _databaseManager.sqlCheck(email) + "'");

                if (_dataTable == null)
                {

                    //objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                    objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                    objUserIdentity.authResult = "Invalid Auth Details provided."; //Error Message or Success Message
                    objUserIdentity.email = "";

                }
                else
                {

                    if (_dataTable.Rows.Count == 0)
                    {

                        //objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                        objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                        objUserIdentity.authResult = "Invalid Auth Details provided."; //Error Message or Success Message
                        objUserIdentity.email = "";

                    }
                    else
                    {

                        //Create List Object
                        objUserIdentity.authSecurity = new List<subscriptionSecurityModel> { };

                        //We have data that is correct... so lets verify teh user exists, is active, not deleted and has access (return the model)
                        foreach (DataRow row in _dataTable.Rows)
                        {

                            if (row["isactive"] is false || row["isdeleted"] is true)
                            {

                                //objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                                objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                                objUserIdentity.authResult = "Inactive or Removed user account."; //Error Message or Success Message
                                objUserIdentity.email = "";

                            }
                            else
                            {
                                //Compare Password received vs what we have on record for this email... no SQL Injection issues
                                if (row["password"].ToString() == password) {

                                    objUserIdentity.authToken = Guid.Parse(row["apitoken"].ToString()); //Return authToken we have stored for this user
                                    objUserIdentity.authMessage = "SUCCESS"; //SUCCESS OR FAILED
                                    objUserIdentity.authResult = "Success, access granted via login."; //Error Message or Success Message
                                    objUserIdentity.email = row["email"].ToString();
                                    objUserIdentity.authUserId = (Guid)row["userid"]; //Used to log activities

                                    //Add the new Security Object in here...
                                    objUserIdentity.authSecurity.Add(new subscriptionSecurityModel());

                                    //A user could be assigned to many Subscriptions... so we need to load all permissions
                                    //This is rare, one user will typically have access to only one subscription.
                                    //A user has permissions per subscription, so I could be an admin in one subscription and a normal user in another
                                    subscriptionSecurityModel objNewAuthSecurity = new subscriptionSecurityModel
                                    {
                                        Id = (Guid)row["id"],
                                        subscriptionId = (Guid)row["subscriptionid"],
                                        name = row["name"].ToString(),
                                        subscriptionManageAll = (bool)row["subscriptionManageAll"],
                                        serverlogView = (bool)row["serverlogView"],
                                        serverlogDelete = (bool)row["serverlogDelete"],
                                        subscriptionView = (bool)row["subscriptionView"],
                                        subscriptionAdd = (bool)row["subscriptionAdd"],
                                        subscriptionEdit = (bool)row["subscriptionEdit"],
                                        subscriptionDelete = (bool)row["subscriptionDelete"],
                                        subscriptionUserView = (bool)row["subscriptionUserView"],
                                        subscriptionUserAdd = (bool)row["subscriptionUserAdd"],
                                        subscriptionUserEdit = (bool)row["subscriptionUserEdit"],
                                        subscriptionUserDelete = (bool)row["subscriptionUserDelete"],
                                        accountView = (bool)row["accountView"],
                                        accountAdd = (bool)row["accountAdd"],
                                        accountEdit = (bool)row["accountEdit"],
                                        accountDelete = (bool)row["accountDelete"],
                                        contactView = (bool)row["contactView"],
                                        contactAdd = (bool)row["contactAdd"],
                                        contactEdit = (bool)row["contactEdit"],
                                        contactDelete = (bool)row["contactDelete"],
                                        activityView = (bool)row["activityView"],
                                        activityAdd = (bool)row["activityAdd"],
                                        activityEdit = (bool)row["activityEdit"],
                                        activityDelete = (bool)row["activityDelete"],
                                        activityTypesView = (bool)row["activityTypesView"],
                                        activityTypesAdd = (bool)row["activityTypesAdd"],
                                        activityTypesEdit = (bool)row["activityTypesEdit"],
                                        activityTypesDelete = (bool)row["activityTypesDelete"],
                                        scoringView = (bool)row["scoringView"],
                                        scoringAdd = (bool)row["scoringAdd"],
                                        scoringEdit = (bool)row["scoringEdit"],
                                        scoringDelete = (bool)row["scoringDelete"],
                                        customFieldsView = (bool)row["customFieldsView"],
                                        customFieldsAdd = (bool)row["customFieldsAdd"],
                                        customFieldsEdit = (bool)row["customFieldsEdit"],
                                        customFieldsDelete = (bool)row["customFieldsDelete"],
                                        auditlogView = (bool)row["auditlogView"],
                                        auditlogDelete = (bool)row["auditlogDelete"]
                                    };

                                    objUserIdentity.authSecurity[intCount] = objNewAuthSecurity;
                                    intCount += 1;

                                } else {

                                    //objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                                    objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                                    objUserIdentity.authResult = "Invalid Auth Details provided."; //Error Message or Success Message
                                    objUserIdentity.email = "";

                                }

                            }

                        }

                    }

                }

                return objUserIdentity;

            }
            catch (Exception ex)
            {
                //Log Exception in Code (Example) To Rollbar
                Rollbar.RollbarLocator.RollbarInstance.Error(ex);

                //objUserIdentity.authToken = apitoken; //Return authToken we have stored for this user
                objUserIdentity.authMessage = "FAILED"; //SUCCESS OR FAILED
                objUserIdentity.authResult = "Auth exception, please try again later. " + ex.Message.ToString(); //Error Message or Success Message
                objUserIdentity.email = "";

                return objUserIdentity;
            }



        }

    }

    public class subscriptionUserAccessService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public subscriptionUserAccessService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task <string> Get()
        {
            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                String strJSon;

                //Read the database
                _dataTable = await _databaseManager.executeReader("SELECT * FROM public.subscriptionuseraccess;");

                //Convert to Json string
                strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                return strJSon;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task <string> Get(string id)
        {

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                String strJSon;

                //Read the database
                _dataTable = await _databaseManager.executeReader("SELECT * FROM public.subscriptionuseraccess where id='" + id + "';");

                //Convert to Json string
                strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                return strJSon;

            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<subscriptionSignupModel> GenerateToken(string userId, string strCurrentAPIToken)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            subscriptionUserService objSubscriptionUser = new subscriptionUserService(objConfiguration, null);

            subscriptionSignupModel objNewSubscription = new subscriptionSignupModel();

            string strNewUserAPIToken = objSubscriptionUser.GenerateRandomAPIToken(); //Generate New API Token

            bool bolProceed = false;
            string strProcessMessage = "";
            long lngRecordsUpdated = 0;

            try
            {

                //Process to create a new subscription and return the subscription details...
                lngRecordsUpdated = await _databaseManager.executeNonQuery("UPDATE public.subscriptionuser SET apitoken='" + _databaseManager.sqlCheck(strNewUserAPIToken) + "', updatedat=CURRENT_TIMESTAMP WHERE id='" + _databaseManager.sqlCheck(userId) + "' and apitoken='" + _databaseManager.sqlCheck(strCurrentAPIToken) + "' ;");

                if (lngRecordsUpdated == 0)
                {
                    bolProceed = false;
                    strProcessMessage += "Unable to update profile with new token. ";
                }
                else
                {
                    bolProceed = true;
                    strProcessMessage += "New token created and profile updated. ";
                }



                //Verify that we can proceed and return the data back to the user
                if (bolProceed == true)
                {
                    objNewSubscription.Id = userId.ToString();
                    objNewSubscription.apitoken = strNewUserAPIToken;

                    objNewSubscription.signupCode = "Success";
                    objNewSubscription.signupMessage = strProcessMessage; ;
                }
                else
                {
                    objNewSubscription.Id = userId.ToString();
                    objNewSubscription.apitoken = strCurrentAPIToken;

                    objNewSubscription.signupCode = "Error";
                    objNewSubscription.signupMessage = strProcessMessage;
                }


                return objNewSubscription;

            }
            catch (Exception ex)
            {
                //Clear Data that will be sent back
                objNewSubscription.Id = userId.ToString();
                objNewSubscription.apitoken = strCurrentAPIToken;

                objNewSubscription.signupCode = "Error";
                objNewSubscription.signupMessage = strProcessMessage + ' ' + ex.Message;

                return objNewSubscription;
            }


        }

    }

    public class RandomGenerator
    {
        // Instantiate random number generator.  
        // It is better to keep a single Random instance 
        // and keep using Next on the same instance.  
        private readonly Random _random = new Random();

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        // Generates a random string with a given size.    
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):   
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }

    }
