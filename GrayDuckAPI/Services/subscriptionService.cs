using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GrayDuck.Services
{
    public class subscriptionService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public subscriptionService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
 
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }


        public async Task <subscriptionSignupModel> SignUp(subscriptionModel objSignUp)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            subscriptionUserService objSubscriptionUser = new subscriptionUserService(objConfiguration,null);

            subscriptionSignupModel objNewSubscription = new subscriptionSignupModel();

            string strExistingUserGuid = "";
            string strExistingSubUserGuid = "";
            string strNewSubscritptionId = "";
            string strNewUserPassword = await objSubscriptionUser.GenerateRandomPassword(); //Generate New Password
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

                strExistingUserGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.subscription where email='" + _databaseManager.sqlCheck(objSignUp.email.ToLower()) + "' ");
                strExistingSubUserGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.subscriptionuser where email='" + _databaseManager.sqlCheck(objSignUp.email.ToLower()) + "'");

                if (strExistingUserGuid == "" && strExistingSubUserGuid == "") {
                    bolProceed = true;
                    strProcessMessage = "";
                } else {
                    bolProceed = false;
                    strProcessMessage = "Existing subscription / user account found for email address. Please use another email address or request access to the subscription.";
                }


                //If we are allowed to proceed the process can start
                if (bolProceed == true)
                {
                    //1. Create New subscription
                    strNewSubscritptionId = await _databaseManager.executeQuery("INSERT INTO public.subscription (externalid, name, email, mobile, officephone, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdat, updatedat) VALUES ('" + _databaseManager.sqlCheck(objSignUp.externalId) + "','" + _databaseManager.sqlCheck(objSignUp.name) + "','" + _databaseManager.sqlCheck(objSignUp.email.ToLower()) + "','" + _databaseManager.sqlCheck(objSignUp.mobile) + "','" + _databaseManager.sqlCheck(objSignUp.officePhone) + "','" + _databaseManager.sqlCheck(objSignUp.billingStreet) + "','" + _databaseManager.sqlCheck(objSignUp.billingCity) + "','" + _databaseManager.sqlCheck(objSignUp.billingState) + "','" + _databaseManager.sqlCheck(objSignUp.billingCountry) + "','" + _databaseManager.sqlCheck(objSignUp.billingPostCode) + "',true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;");

                    if (strNewSubscritptionId == "" || strNewSubscritptionId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new subscription. " + strNewSubscritptionId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New subscription created. ";
                    }


                    //Without a valid SubscriptonId we cannot continue
                    if (bolProceed == true)
                    {

                        //2. Create New user for subscription
                        string strNewUserId = "";

                        strNewUserId = await _databaseManager.executeQuery("INSERT INTO public.subscriptionuser (email, password, mobile, apitoken, isactive, isdeleted, createdat, updatedat) VALUES ('" + _databaseManager.sqlCheck(objSignUp.email.ToLower()) + "','" + _databaseManager.sqlCheck(strNewUserPassword) + "','" + _databaseManager.sqlCheck(objSignUp.mobile) + "','" + _databaseManager.sqlCheck(strNewUserAPIToken) + "',true,false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;");

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


                        //3. Create New security group for subscription
                        string strNewUserSecGroupId = "";
                        string strNewSecurityName = "Admin";
                        strNewUserSecGroupId = await _databaseManager.executeQuery("INSERT INTO public.subscriptionsecurity (subscriptionid, name, createdat, updatedat, subscriptionmanageall, subscriptionview, subscriptionadd, subscriptionedit, subscriptiondelete, subscriptionuserview, subscriptionuseradd, subscriptionuseredit, subscriptionuserdelete, accountview, accountadd, accountedit, accountdelete, contactview, contactadd, contactedit, contactdelete, activityview, activityadd, activityedit, activitydelete, scoringview, scoringadd, scoringedit, scoringdelete, customfieldsview, customfieldsadd, customfieldsedit, customfieldsdelete, serverlogview, serverlogdelete, activitytypesview, activitytypesadd, activitytypesedit, activitytypesdelete, auditlogview, auditlogdelete) VALUES ('" + _databaseManager.sqlCheck(strNewSubscritptionId) + "','" + strNewSecurityName + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true) RETURNING Id;");

                        if (strNewUserSecGroupId == "" || strNewUserSecGroupId.StartsWith("Error"))
                        {
                            bolProceed = false;
                            strProcessMessage += "Unable to create new subscription security. " + strNewUserSecGroupId;
                        }
                        else
                        {
                            bolProceed = true;
                            strProcessMessage += "New subscription security created. ";
                        }


                        //4. Link New User and New security group
                        string strNewUserSecLinkId = "";
                        strNewUserSecLinkId = await _databaseManager.executeQuery("INSERT INTO public.subscriptionuseraccess (subscriptionid, subscriptionsecurityid, subscriptionuserid) VALUES ('" + strNewSubscritptionId + "', '" + strNewUserSecGroupId + "', '" + strNewUserId + "') RETURNING Id;");

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
                    else
                    {
                        //Print out why the rest of the process could not be completed.
                        strProcessMessage += "No valid subscription Id obtained.";
                    }

                }
   

                //Verify that we can proceed and return the data back to the user
                if (bolProceed == true) {
                    objNewSubscription.Id = strNewSubscritptionId;
                    objNewSubscription.name = objSignUp.name;
                    objNewSubscription.externalId = objSignUp.externalId;
                    objNewSubscription.email = objSignUp.email;
                    objNewSubscription.password = strNewUserPassword;
                    objNewSubscription.apitoken = strNewUserAPIToken;

                    objNewSubscription.signupCode = "Success";
                    objNewSubscription.signupMessage = strProcessMessage; ;
                } else {
                    objNewSubscription.Id = strNewSubscritptionId;
                    objNewSubscription.name = objSignUp.name;
                    objNewSubscription.externalId = objSignUp.externalId;
                    objNewSubscription.email = objSignUp.email;
                    objNewSubscription.password = "";
                    objNewSubscription.apitoken = "";

                    objNewSubscription.signupCode = "Error";
                    objNewSubscription.signupMessage = strProcessMessage;
                }


                return objNewSubscription;

            }
            catch (Exception ex)
            {
                //Clear Data that will be sent back
                objNewSubscription.Id = "";
                objNewSubscription.email = objSignUp.email;
                objNewSubscription.password = "";
                objNewSubscription.apitoken = "";

                objNewSubscription.signupCode = "Error";
                objNewSubscription.signupMessage = strProcessMessage + ' ' + ex.Message;

                return objNewSubscription;
            }

            
        }

        public async Task <string> Get()
        {

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;
                String strJSon;

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //No Auth Sent, return empty JSon
                    strJSon = "[]";
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        //No Security found for User to Auth / Give permissions, return empty JSon
                        strJSon = "[]";
                    }
                    else
                    {

                        //Build Allowed Subscription List
                        string strSubscriptionIds = "";
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {
                            if (strSubscriptionIds == "")
                            {
                                strSubscriptionIds = "'" + row.subscriptionId.ToString() + "'";
                            }
                            else
                            {
                                strSubscriptionIds += ",'" + row.subscriptionId.ToString() + "'";
                            }
                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, externalid, name, email, mobile, officephone, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdbyid, createdat, updatedat FROM public.subscription WHERE id IN (" + strSubscriptionIds + ");");

                        //Convert to Json string
                        strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                    }
                }

                return strJSon;

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
                String strJSon = "[]";

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //No Auth Sent, return empty JSon
                    strJSon = "[]";
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        //No Security found for User to Auth / Give permissions, return empty JSon
                        strJSon = "[]";
                    }
                    else
                    {

                        //Build Allowed Subscription List
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {
                            if (id == (Guid)row.subscriptionId)
                            {
                                //Read the database
                                _dataTable = await _databaseManager.executeReader("SELECT id, externalid, name, email, mobile, officephone, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdbyid, createdat, updatedat FROM public.subscription where id='" + id + "';");

                                //Convert to Json string
                                strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                                break;
                            }
                        }

                    }
                }

                return strJSon;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task <subscriptionModel> Create(subscriptionModel objNew)
        {

            
            try
            {
                //Do Nothing for now.. use the SignUpOption only
                return objNew;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<long> Remove(Guid id)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            DataTable dtUsersForSubScription;
            DataTable dtUsersToSkip;
            long rowsAffected = 0;

            try
            {

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //No Auth Sent, return empty JSon
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        //No Security found for User to Auth / Give permissions, return empty JSon
                    }
                    else
                    {

                        //Build Allowed Subscription List
                        string strSubscriptionIds = "";
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {
                            if (strSubscriptionIds == "")
                            {
                                strSubscriptionIds = "'" + row.subscriptionId.ToString() + "'";
                            }
                            else
                            {
                                strSubscriptionIds += ",'" + row.subscriptionId.ToString() + "'";
                            }
                        }



                        //Deleting a Subscription Requires the Removal of all data (actual deletetion) of data for this subscription.
                        //Tables Affected (ALL)
                        //There is NO rollback / Undo option. This is a hard Delete of the subscription.

                        //account
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.account where subscriptionid='" + id + "';");

                        //accountcustomfield
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.accountcustomfield where subscriptionid='" + id + "';");

                        //activities
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.activities where subscriptionid='" + id + "';");

                        //activitiescustomfield
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.activitiescustomfield where subscriptionid='" + id + "';");

                        //activitiesscoring
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.activitiesscoring where subscriptionid='" + id + "';");

                        //activitiestypes
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.activitytypes where subscriptionid='" + id + "';");

                        //contact
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.contact where subscriptionid='" + id + "';");

                        //contactcustomfields
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.contactcustomfield where subscriptionid='" + id + "';");

                        //contactfile
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.contactfile where subscriptionid='" + id + "';");

                        //customfield
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.customfield where subscriptionid='" + id + "';");

                        //subscriptionsecurity
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.subscriptionsecurity where subscriptionid='" + id + "';");


                        //Get the UserId's to delete that are in this subscription (if they are shared then do NOT DELETE)
                        //This could slow down the process.. look to make performance improvements here in the future.
                        dtUsersToSkip = await _databaseManager.executeReader("select count(id),subscriptionuserid from public.subscriptionuseraccess group by subscriptionuserid order by count(id) DESC;");
                        dtUsersForSubScription = await _databaseManager.executeReader("select subscriptionuserid from public.subscriptionuseraccess where subscriptionid='" + id + "';");

                        //subscriptionuser
                        //Verify if the user is in another subscription... if they are do NOT delete the user.
                        try
                        {

                            if (dtUsersForSubScription != null && dtUsersToSkip != null)
                            {
                                foreach (DataRow dtRow in dtUsersForSubScription.Rows)
                                {

                                    //Find the userid in the UsersToShip datatable to see if they are link to multiple subscripstions or not.
                                    foreach (DataRow dtRowSkip in dtUsersToSkip.Rows)
                                    {

                                        //Find Matching Rows (user record rows)
                                        if ((Guid)dtRow["subscriptionuserid"] == (Guid)dtRowSkip["subscriptionuserid"]) {
                                                                                        
                                            if ((Int64)dtRowSkip["count"] > 1)
                                            {
                                                //If we find more than one.. then skip
                                            }
                                            else {
                                                //If only 1 exists then we can delete it

                                                //subscriptionuser
                                                //Execute Command
                                                rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.subscriptionuser where id='" + dtRow["subscriptionuserid"] + "';");

                                            }

                                            //Exit inner loop / for each only
                                            //This only breaks the current loop
                                            break;
                                        }


                                    }


                                }
                            }

                        } catch(Exception exUsers) {

                        }


                        //subscriptionuseraccess
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.subscriptionuseraccess where subscriptionid='" + id + "';");

                        //subscription
                        //Execute Command
                        rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.subscription where id='" + id + "';");

                    }
                }

                return rowsAffected;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
