using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GrayDuck.Services
{
    public class contactService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public contactService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<DataTable> Search(string firstname, string lastname, string mobile, string email, string customfield1, string customfieldvalue1, string customfield2, string customfieldvalue2)
        {
            DataTable _dataTable = new DataTable();

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //Nothing to do
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        ////No Security found for User to Auth / Give permissions, return empty JSon
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

                        //Create WHERE clause with the search data received
                        string strWhereClause = "";

                        if (!string.IsNullOrEmpty(firstname)) 
                        {
                            if (string.IsNullOrEmpty(strWhereClause)) {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and firstname='" + _databaseManager.sqlCheck(firstname) + "' ";
                            } else {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and firstname='" + _databaseManager.sqlCheck(firstname) + "' ";
                            }                            
                        }

                        if (!string.IsNullOrEmpty(lastname)) 
                        {
                            if (string.IsNullOrEmpty(strWhereClause))
                            {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and lastname='" + _databaseManager.sqlCheck(lastname) + "' ";
                            }
                            else
                            {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and lastname='" + _databaseManager.sqlCheck(lastname) + "' ";
                            }
                        }

                        if (!string.IsNullOrEmpty(mobile))
                        {
                            if (string.IsNullOrEmpty(strWhereClause))
                            {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and mobile='" + _databaseManager.sqlCheck(mobile) + "' ";
                            }
                            else
                            {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and mobile='" + _databaseManager.sqlCheck(mobile) + "' ";
                            }
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            if (string.IsNullOrEmpty(strWhereClause))
                            {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and email='" + _databaseManager.sqlCheck(email) + "' ";
                            }
                            else
                            {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and email='" + _databaseManager.sqlCheck(email) + "' ";
                            }
                        }

                        if (!string.IsNullOrEmpty(customfield1))
                        {
                            if (string.IsNullOrEmpty(strWhereClause))
                            {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and id IN(select contactid from public.contactcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and fieldname='" + _databaseManager.sqlCheck(customfield1) + "' and fieldvalue='" + _databaseManager.sqlCheck(customfieldvalue1) + "' GROUP BY contactid) ";
                            }
                            else
                            {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and id IN(select contactid from public.contactcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and fieldname='" + _databaseManager.sqlCheck(customfield2) + "' and fieldvalue='" + _databaseManager.sqlCheck(customfieldvalue1) + "' GROUP BY contactid) ";
                            }
                        }

                        if (!string.IsNullOrEmpty(customfield2))
                        {
                            if (string.IsNullOrEmpty(strWhereClause))
                            {
                                strWhereClause += " subscriptionid IN (" + strSubscriptionIds + ") and id IN(select contactid from public.contactcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and fieldname='" + _databaseManager.sqlCheck(customfield2) + "' and fieldvalue='" + _databaseManager.sqlCheck(customfieldvalue2) + "' GROUP BY contactid) ";
                            }
                            else
                            {
                                strWhereClause += " OR subscriptionid IN (" + strSubscriptionIds + ") and id IN(select contactid from public.contactcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and fieldname='" + _databaseManager.sqlCheck(customfield2) + "' and fieldvalue='" + _databaseManager.sqlCheck(customfieldvalue2) + "' GROUP BY contactid) ";
                            }
                        }


                        //Sample Query to Find Data

                        //select* from public.contact
                        //where subscriptionid='2c436422-3a7b-4a9a-b6ed-754dff80d95c' and firstname = 'nussm'
                        //OR subscriptionid = '2c436422-3a7b-4a9a-b6ed-754dff80d95c' and lastname = 'nuddts'
                        //OR subscriptionid = '2c436422-3a7b-4a9a-b6ed-754dff80d95c' and email = 'test@gmail.com'
                        //OR subscriptionid = '2c436422-3a7b-4a9a-b6ed-754dff80d95c' and mobile = '27835959588'
                        //OR subscriptionid = '2c436422-3a7b-4a9a-b6ed-754dff80d95c' and id
                        //IN(
                        //select contactid from public.contactcustomfield
                        //where subscriptionid='2c436422-3a7b-4a9a-b6ed-754dff80d95c' and fieldname = 'Student Number' and fieldvalue = '12345'
                        //OR subscriptionid = '2c436422-3a7b-4a9a-b6ed-754dff80d95c' and fieldname = 'ID Number' and fieldvalue = '1212131515059'
                        //GROUP BY contactid
                        //);


                        //Read the database
                        //_dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.contact WHERE subscriptionid IN (" + strSubscriptionIds + ") " + strWhereClause + " ORDER BY createdat DESC;");

                        //Using Custom Query Option
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.contact WHERE " + strWhereClause + " ORDER BY createdat DESC;");

                    }
                }

                //return strJSon;
                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DataTable> Get()
        {
            DataTable _dataTable = new DataTable();
            
            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //Nothing to do
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        ////No Security found for User to Auth / Give permissions, return empty JSon
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.contact WHERE subscriptionid IN (" + strSubscriptionIds + ") ORDER BY createdat DESC LIMIT 1000;");

                    }
                }

                //return strJSon;
                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DataTable> Get(int limit, string orderby)
        {
            DataTable _dataTable = new DataTable();

            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //Nothing to do
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        ////No Security found for User to Auth / Give permissions, return empty JSon
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

                        //Default to 100 if no limit set
                        if (limit == 0) {
                            limit = 100;
                        }
                        ////If the Limit is over 5000 set it to a max of 5000
                        //if (limit > 5000) {
                        //    limit = 5000;
                        //}

                        //Set default sorting to createdat DESC
                        //If not apply what the user sent
                        if (orderby == "") {
                            orderby = "createdat DESC";
                        }

                        //Read Database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.contact WHERE subscriptionid IN (" + strSubscriptionIds + ") ORDER BY " + orderby + " LIMIT " + limit + ";");
                    }
                }

                //return strJSon;
                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DataTable> Get(Guid id)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            DataTable _dataTable = new DataTable();

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

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.contact WHERE subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "';");

                    }
                }

                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<contactModel> Create(contactModel objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            contactModel objNewContact =  new contactModel();

            string strExistingContactEmailGuid = "";
            string strExistingContactMobileGuid = "";
            string strNewContactId = "";

            bool bolProceed = false;
            string strProcessMessage = "";

            try
            {

                //Process to create new contact for subscription


                //See if the contact exists for either the email address or mobile number
                if (String.IsNullOrEmpty(objNew.email)) {
                
                } else {
                    strExistingContactEmailGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.contact where subscriptionId='" + objNew.subscriptionId + "' and email='" + _databaseManager.sqlCheck(objNew.email.ToLower()) + "' ");
                }
                if (String.IsNullOrEmpty(objNew.mobile)) {
                
                } else {
                    strExistingContactMobileGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.contact where subscriptionId='" + objNew.subscriptionId + "' and mobile='" + _databaseManager.sqlCheck(objNew.mobile.ToLower()) + "'");
                }


                if (strExistingContactEmailGuid == "" && strExistingContactMobileGuid == "")
                {
                    bolProceed = true;
                    strProcessMessage = "";
                }
                else
                {
                    bolProceed = false;
                    strProcessMessage = "Existing contact found for this email address / mobile number.";

                    //Return Existing Contact Id
                    //Email Address wins over mobile number - always
                    if (String.IsNullOrEmpty(strExistingContactEmailGuid))
                    {
                        if (String.IsNullOrEmpty(strExistingContactMobileGuid)) {
                        
                        } else {
                            objNewContact = objNew;
                            objNewContact.Id = Guid.Parse(strExistingContactMobileGuid);
                        }
                    }
                    else {
                        objNewContact = objNew;
                        objNewContact.Id = Guid.Parse(strExistingContactEmailGuid);
                    }

                    //Existing Contact Found so we can build and Update statement - If any fields are sent here
                    //Build fields to update
                    //await _databaseManager.executeNonQuery("UPDATE public.contact SET  where id=" + Guid.Parse(strExistingContactEmailGuid));

                }


                //If we are allowed to proceed the process can start
                if (bolProceed == true)
                {
                    //1. Create New contact
                    strNewContactId = await _databaseManager.executeQuery("INSERT INTO public.contact (subscriptionid, accountid, externalid, firstname, middlename, lastname, salutation, gender, email, mobile, officephone, homephone, donotcall, donotcallreason, donotcallat, unsubscribed, unsubscribedreason, unsubscribedat, companyname, companytitle, companyrole, companydepartment, companyindustry, companysiccode, companywebsite, contactscore, contactsource, contactstatus, contactrating, isactive, createdbyid, createdat, updatedat, assignedtoid) VALUES ('" + objNew.subscriptionId + "', '" + objNew.accountId + "','" + _databaseManager.sqlCheck(objNew.externalId) + "','" + _databaseManager.sqlCheck(objNew.firstName) + "','" + _databaseManager.sqlCheck(objNew.middelName) + "','" + _databaseManager.sqlCheck(objNew.lastName) + "','" + _databaseManager.sqlCheck(objNew.saluation) + "','" + _databaseManager.sqlCheck(objNew.gender) + "','" + _databaseManager.sqlCheck(objNew.email.ToLower()) + "','" + _databaseManager.sqlCheck(objNew.mobile) + "','" + _databaseManager.sqlCheck(objNew.officePhone) + "','" + _databaseManager.sqlCheck(objNew.homePhone) + "'," + objNew.doNotCall + ",'" + _databaseManager.sqlCheck(objNew.doNotCallReason) + "', CURRENT_TIMESTAMP, " + objNew.unsubscribed + ", '" + _databaseManager.sqlCheck(objNew.unsubscribedReason) + "', CURRENT_TIMESTAMP, '" + _databaseManager.sqlCheck(objNew.companyName) + "', '" + _databaseManager.sqlCheck(objNew.companyTitle) + "', '" + _databaseManager.sqlCheck(objNew.companyRole) + "', '" + _databaseManager.sqlCheck(objNew.companyDepartment) + "', '" + _databaseManager.sqlCheck(objNew.companyIndustry) + "', '" + _databaseManager.sqlCheck(objNew.companySICCode) + "', '" + _databaseManager.sqlCheck(objNew.companyWebsite) + "', " + objNew.contactScore + ",'" + _databaseManager.sqlCheck(objNew.contactSource) + "', '" + _databaseManager.sqlCheck(objNew.contactStatus) + "', " + objNew.contactRating + ", " + objNew.isActive + ", '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, '" + objNew.assignedToId + "') RETURNING Id;");

                    if (strNewContactId == "" || strNewContactId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new contact. " + strNewContactId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New contact created. ";
                        objNewContact = objNew;
                        objNewContact.Id = Guid.Parse(strNewContactId);
                        objNewContact.createdById = objAuthIdentity.authUserId;

                        //If we have a new contact we can insert the custom field values
                        if (objNew.customFields is null) {
                            //No Custom Fields Found    
                        } else {
                            //Custom Fields Found
                            foreach (contactcustomfieldModel objField in objNew.customFields)
                            {

                                await _databaseManager.executeQuery("INSERT INTO public.contactcustomfield (subscriptionid, customfieldid, contactid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objNew.subscriptionId + "', '" + objField.Id + "', '" + strNewContactId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                            }
                        }

                    }
                }

                return objNewContact;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public void UpdateStatus(Guid id, string Status, Guid subscriptionId)
        {
            databaseSettings _databaseManager = new databaseSettings(objConfiguration);

            try
            {

                //Update the status filed only on a contact record
                _databaseManager.executeNonQuery("UPDATE public.contact set contactstatus='" + Status + "' where id='" + id + "' and subscriptionid='" + subscriptionId + "'");

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<long> Remove(Guid id)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
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



                        //Execute Command

                        //Get all activities custom fields for the activity we want to delete
                        //activitiescustomfield
                        DataTable dtContactActivities;
                        dtContactActivities = await _databaseManager.executeReader("select id, subscriptionid from public.activities where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + id + "';");

                        try
                        {

                            if (dtContactActivities != null )
                            {
                                foreach (DataRow dtRow in dtContactActivities.Rows)
                                {

                                    //Execute Command
                                    rowsAffected += await _databaseManager.executeNonQuery("DELETE FROM public.activitiescustomfield where activityid='" + dtRow["id"] + "' and subscriptionid IN (" + strSubscriptionIds + ");");

                                }
                            }

                        }
                        catch (Exception exActivities)
                        {

                        }

                        //activities
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.activities where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + id + "';");

                        //contactcustomfield
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.contactcustomfield where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + id + "';");

                        //contactfile
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.contactfile where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + id + "';");

                        //Delete the contact record
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.contact where subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "';");

                    }
                }

                return rowsAffected;

            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public async Task<long> CalculateContactScore(Guid id, Guid activityId, Guid activityscoringId) {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);

            long newScoreValue = 0;
            string currentContactScoreValue = "0";
            string activityScoringValue = "0";

            try
            {

                //Recalculate Contact Score based on the data we have available for this contact record
                //We look at the new activityId that was received to calculate the score from here on forward.
                //Speeds things up vs going over all activities for this contact to create score
                //Also allows a system reset of a contact score and then it starts over... re-scoring of contacts from a new date.


                //1 - Get Existing contact score
                currentContactScoreValue = await _databaseManager.executeScalarReturn("SELECT contactscore FROM public.contact WHERE id='" + id + "';");
                if (currentContactScoreValue == "") {
                    currentContactScoreValue = "0";
                }

                //2 - Get Activity scoring info to use in calculation
                activityScoringValue = await _databaseManager.executeScalarReturn("SELECT score FROM public.activitiesscoring WHERE id='" + activityscoringId + "';");
                if (activityScoringValue == "")
                {
                    activityScoringValue = "0";
                }

                //Calculate New Score
                newScoreValue = long.Parse(currentContactScoreValue) + long.Parse(activityScoringValue);

                //3 - Take Activity Scoring and apply score to contact
                //    Update Contact Record with the new Score
                _databaseManager.executeNonQuery("UPDATE public.contact SET contactscore=" + newScoreValue + " WHERE id='" + id + "'");

                //Return New Score Value if Needed
                return newScoreValue;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }

    public class contactCustomFieldsService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public contactCustomFieldsService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
 
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<string> Get(Guid contactid)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, contactid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.contactcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + contactid + "';");

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

        public async Task<contactcustomfieldModel[]> Create(contactcustomfieldModel[] objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            DataTable _dataTable;
            string strNewFieldValue = "";

            try
            {

                //Process to create or update customfield values
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


                        string strQueryString = "";

                        //Build Cache of Existing Fields
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {

                            //0 - Query for Existing Fields
                            //    This ensures that you cannot try to add values for subscriptions you have no access to
                            foreach (contactcustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {
                                    //Build Query Stringt to Execute
                                    if (strQueryString == "") {
                                        strQueryString = " subscriptionid=" + objField.subscriptionId + " and contactid='" + objField.contactId + "' and customfieldid='" + objField.Id + "' ";
                                    } else { 
                                        strQueryString += " OR subscriptionid=" + objField.subscriptionId + " and contactid='" + objField.contactId + "' and customfieldid='" + objField.Id + "' ";
                                    }
                                }
                                else
                                {
                                    //Not found so do nothing
                                }

                            }

                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, contactid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.contactcustomfield WHERE " + strQueryString +" ;");




                        //Build Allowed Subscription List
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {

                            //1 - Create Custom Field for each object we receive
                            //    This ensures that you cannot try to add values for subscriptions you have no access to
                            foreach (contactcustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId) {

                                    //Before we INSERT see if we need to do an UPDATE if the field already exists for this contact
                                    DataRow[] foundRows = _dataTable.Select("customfieldid='" + objField.Id + "' AND contactid='" + objField.contactId + "' AND subscriptionid='" + objField.subscriptionId + "'");
                                    if (foundRows.Length == 0)
                                    {
                                        //Found so it is allowed, insert data
                                        strNewFieldValue = await _databaseManager.executeQuery("INSERT INTO public.contactcustomfield (subscriptionid, customfieldid, contactid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objField.subscriptionId + "', '" + objField.Id + "', '" + objField.contactId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                                        //Update Object
                                        objField.Id = Guid.Parse(strNewFieldValue);
                                        objField.createdById = objAuthIdentity.authUserId;
                                    }
                                    else
                                    {
                                        //Existing Record found so, update data
                                        strNewFieldValue = await _databaseManager.executeQuery("UPDATE public.contactcustomfield SET fieldvalue='" + _databaseManager.sqlCheck(objField.fieldValue) + "', updatedat=CURRENT_TIMESTAMP WHERE customfieldid='" + objField.Id + "' AND contactid='" + objField.contactId + "' AND subscriptionid='" + objField.subscriptionId + "';");
                                    }


                                } else {
                                    //Not found so do nothing
                                }                                

                            }

                        }

                    }
                }

                return objNew;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Remove(Guid contactid)
        {
            try
            {
                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

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

                        //Execute Command
                        _databaseManager.executeNonQuery("DELETE FROM public.contactcustomfield where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + contactid + "'");

                    }
                }


            }
            catch (Exception ex)
            {

            }
        }

    }

    public class contactFileService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public contactFileService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
      
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<string> Get(Guid contactid)
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

                        //Read the database - List Files and Binary (Load the actual fileData)
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, contactid, filename, filetype, filesize, filetag, createdbyid, createdat, updatedat FROM public.contactfile WHERE subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + contactid + "' ;");


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

        public async Task<contactFileModel[]> Create(contactFileModel[] objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            string strNewFileValue = "";

            try
            {

                //Process to create a new file
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

                        string strQueryString = "";

                        //This ensures that you cannot try to add values for subscriptions you have no access to
                        foreach (contactFileModel objFile in objNew)
                        {

                            //Found so it is allowed, insert data
                            strNewFileValue = await _databaseManager.executeQueryWithByte("INSERT INTO public.contactfile (subscriptionid, contactid, filename, filetype, filesize, filetag, filedata, createdbyid, createdat, updatedat) VALUES ('" + objFile.subscriptionId + "', '" + objFile.contactId + "', '" + _databaseManager.sqlCheck(objFile.fileName) + "', '" + _databaseManager.sqlCheck(objFile.fileType) + "', " + objFile.fileSize + ", '" + _databaseManager.sqlCheck(objFile.fileTag) + "', @FileByte, '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;", objFile.fileData);

                            //Update Object
                            objFile.Id = Guid.Parse(strNewFileValue);
                            objFile.createdById = objAuthIdentity.authUserId;

                        }

                    }
                }

                return objNew;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Remove(Guid contactid, Guid fileid)
        {
            try
            {
                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

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

                        //Execute Command
                        if (contactid == Guid.Empty) {
                            _databaseManager.executeNonQuery("DELETE FROM public.contactfile where subscriptionid IN (" + strSubscriptionIds + ") and id='" + fileid + "'");
                        } else {
                            _databaseManager.executeNonQuery("DELETE FROM public.contactfile where subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + contactid + "'");
                        }

                    }
                }


            }
            catch (Exception ex)
            {

            }
        }

        public async Task<DataTable> Download(Guid fileid)
        {
            try
            {
                databaseSettings _databaseManager = new databaseSettings(objConfiguration);
                DataTable _dataTable;

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //No Auth Sent, return empty JSon
                    //Do nothing
                    return null;
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        //No Security found for User to Auth / Give permissions, return empty JSon
                        //Do nothing
                        return null;
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

                        //Query Database and return file as a stream
                        if (strSubscriptionIds == "") {
                            //Do nothing
                            return null;
                        } else {
                            _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, contactid, filename, filedata, filetype, filesize, filetag, createdbyid, createdat, updatedat FROM public.contactfile WHERE subscriptionid IN (" + strSubscriptionIds + ") and id='" + fileid + "' ;");
                            return _dataTable;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
