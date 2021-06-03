using GrayDuck.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Services
{
    public class activitiesService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
        public activitiesService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }


        public async Task<DataTable> GetContactItems(Guid contactid)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, typeid, contactid, activityscoringid, title, description, createdbyid, createdat, updatedat FROM public.activities WHERE subscriptionid IN (" + strSubscriptionIds + ") and contactid='" + contactid + "' ORDER BY createdat DESC;");

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

        public async Task<activitiesModel> Create(string contactemail, string contactmobile, activitiesModel objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

            activitiesModel objNewActivity= new activitiesModel();

            string strNewActivityId = "";
            string strExistingContactEmailGuid = "";
            string strExistingContactMobileGuid = "";

            Guid contactid = Guid.Empty;
            Guid accountid = Guid.Empty;

            try
            {

                //Process to create new activity for subscription

                //0. If no accountid or contactid is present we can lookup the email or mobile supplied to match them up
                //See if the contact exists for either the email address or mobile number
                if (objNew.contactId == Guid.Empty)
                {

                    if (String.IsNullOrEmpty(contactemail))
                    {
                        //Do nothing
                    }
                    else
                    {
                        strExistingContactEmailGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.contact where subscriptionId='" + objNew.subscriptionId + "' and email='" + _databaseManager.sqlCheck(contactemail.ToLower()) + "' ");
                    }
                    if (String.IsNullOrEmpty(contactmobile))
                    {
                        //Do nothing
                    }
                    else
                    {
                        strExistingContactMobileGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.contact where subscriptionId='" + objNew.subscriptionId + "' and mobile='" + _databaseManager.sqlCheck(contactmobile.ToLower()) + "'");
                    }

                    //Email wins so take email Guid First
                    if (String.IsNullOrEmpty(strExistingContactEmailGuid)) {
                        if (String.IsNullOrEmpty(strExistingContactMobileGuid)) {
                            //Do nothing
                        }
                        else {
                            contactid = Guid.Parse(strExistingContactMobileGuid);
                        }
                    } else {
                        contactid = Guid.Parse(strExistingContactEmailGuid);
                    }

                }
                else {
                    //Set values as required
                    contactid = objNew.contactId;
                    accountid = objNew.accountId;
                }


                if (contactid == Guid.Empty) {
                    return null;
                } else {

                    //1. Create New activity
                    strNewActivityId = await _databaseManager.executeQuery("INSERT INTO public.activities (subscriptionid, typeid, accountid, contactid, title, description, activityscoringid, createdbyid, createdat, updatedat) VALUES ('" + objNew.subscriptionId + "', '" + objNew.typeId + "', '" + accountid + "','" + contactid + "','" + _databaseManager.sqlCheck(objNew.title) + "','" + _databaseManager.sqlCheck(objNew.description) + "','" + objNew.activityscoringId + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;");

                    if (strNewActivityId == "" || strNewActivityId.StartsWith("Error"))
                    {
                        //bolProceed = false;
                        //strProcessMessage += "Unable to create new activity. " + strNewActivityId;
                    }
                    else
                    {
                        //bolProceed = true;
                        //strProcessMessage += "New activity created. ";
                        objNewActivity = objNew;
                        objNewActivity.Id = Guid.Parse(strNewActivityId);
                        objNewActivity.createdById = objAuthIdentity.authUserId;

                        //If we have a new contact we can insert the custom field values
                        if (objNew.customFields is null)
                        {
                            //No Custom Fields Found    
                        }
                        else
                        {
                            //Custom Fields Found
                            foreach (activitiescustomfieldModel objField in objNew.customFields)
                            {
                                await _databaseManager.executeQuery("INSERT INTO public.activitiescustomfield (subscriptionid, customfieldid, activityid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objNew.subscriptionId + "', '" + objField.Id + "', '" + strNewActivityId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");
                            }
                        }

                        //Recalculate Score on Account or Contact with a any new activity (valid scoring activity)
                        //Scoring activity with a score <> 0 - 0 Does not affect the score so no need to recalculate
                        if ((contactid == Guid.Empty) || (objNew.activityscoringId == Guid.Empty) || (Guid.Parse(strNewActivityId) == Guid.Empty))
                        {
                            //Nothing to do since the contactId is empty OR scoringId is empty OR activityId is empty.
                            //All 3 are needed to run the calculation
                        }
                        else
                        {
                            //Call function to execute on this contact to recalculate contact score in teh background
                            _contactService.CalculateContactScore(contactid, Guid.Parse(strNewActivityId), objNew.activityscoringId);
                        }

                    }

                    return objNewActivity;

                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }

    }


    public class activitiesCustomFieldsService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
        public activitiesCustomFieldsService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
 
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }


        public async Task<string> Get(Guid activityid)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, activityid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.activitycustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and activityid='" + activityid + "';");

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

        public async Task<activitiescustomfieldModel[]> Create(activitiescustomfieldModel[] objNew)
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
                            foreach (activitiescustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {
                                    //Build Query Stringt to Execute
                                    if (strQueryString == "")
                                    {
                                        strQueryString = " subscriptionid=" + objField.subscriptionId + " and activityid='" + objField.activityId + "' and customfieldid='" + objField.Id + "' ";
                                    }
                                    else
                                    {
                                        strQueryString += " OR subscriptionid=" + objField.subscriptionId + " and activityid='" + objField.activityId + "' and customfieldid='" + objField.Id + "' ";
                                    }
                                }
                                else
                                {
                                    //Not found so do nothing
                                }

                            }

                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, activityid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.activitiescustomfield WHERE " + strQueryString + " ;");




                        //Build Allowed Subscription List
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {

                            //1 - Create Custom Field for each object we receive
                            //    This ensures that you cannot try to add values for subscriptions you have no access to
                            foreach (activitiescustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {

                                    //Before we INSERT see if we need to do an UPDATE if the field already exists for this contact
                                    DataRow[] foundRows = _dataTable.Select("customfieldid='" + objField.Id + "' AND activityid='" + objField.activityId + "' AND subscriptionid='" + objField.subscriptionId + "'");
                                    if (foundRows.Length == 0)
                                    {
                                        //Found so it is allowed, insert data
                                        strNewFieldValue = await _databaseManager.executeQuery("INSERT INTO public.activitiescustomfield (subscriptionid, customfieldid, activityid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objField.subscriptionId + "', '" + objField.Id + "', '" + objField.activityId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                                        //Update Object
                                        objField.Id = Guid.Parse(strNewFieldValue);
                                        objField.createdById = objAuthIdentity.authUserId;
                                    }
                                    else
                                    {
                                        //Existing Record found so, update data
                                        strNewFieldValue = await _databaseManager.executeQuery("UPDATE public.accountcustomfield SET fieldvalue='" + _databaseManager.sqlCheck(objField.fieldValue) + "', updatedat=CURRENT_TIMESTAMP WHERE customfieldid='" + objField.Id + "' AND activityid='" + objField.activityId + "' AND subscriptionid='" + objField.subscriptionId + "';");
                                    }


                                }
                                else
                                {
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

        public void Remove(Guid activityid)
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
                        _databaseManager.executeNonQuery("DELETE FROM public.activitiescustomfield where subscriptionid IN (" + strSubscriptionIds + ") and activityid='" + activityid + "'");

                    }
                }


            }
            catch (Exception ex)
            {

            }
        }

    }


    public class activityTypesService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
        public activityTypesService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
   
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, typename, summary, createdbyid, createdat, updatedat FROM public.activitytypes WHERE subscriptionid IN (" + strSubscriptionIds + ");");

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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, typename, summary, createdbyid, createdat, updatedat FROM public.activitytypes WHERE subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "';");

                    }
                }

                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<activityTypesModel> Create(activityTypesModel objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            activityTypesModel objNewType = new activityTypesModel();

            string strExistingTypeGuid = "";
            string strNewTypeId = "";

            bool bolProceed = false;
            string strProcessMessage = "";

            try
            {

                //Process to create new contact for subscription


                //See if the contact exists for either the email address or mobile number
                if (String.IsNullOrEmpty(objNew.typename))
                {

                }
                else
                {
                    strExistingTypeGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.activitytypes where subscriptionId='" + objNew.subscriptionId + "' and typename='" + _databaseManager.sqlCheck(objNew.typename.ToUpper()) + "' ");
                }



                if (strExistingTypeGuid == "")
                {
                    bolProceed = true;
                    strProcessMessage = "";
                }
                else
                {
                    bolProceed = false;
                    strProcessMessage = "Existing type found for this name.";

                    //Return Existing Type Id
                    if (String.IsNullOrEmpty(strExistingTypeGuid))
                    {

                    }
                    else
                    {
                        objNewType = objNew;
                        objNewType.Id = Guid.Parse(strExistingTypeGuid);
                    }

                }


                //If we are allowed to proceed the process can start
                if (bolProceed == true)
                {
                    //1. Create New Type
                    strNewTypeId = await _databaseManager.executeQuery("INSERT INTO public.activitytypes (subscriptionid, typename, summary, createdbyid, createdat, updatedat) VALUES ('" + objNew.subscriptionId + "', '" + _databaseManager.sqlCheck(objNew.typename.ToUpper()) + "','" + _databaseManager.sqlCheck(objNew.summary) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;");

                    if (strNewTypeId == "" || strNewTypeId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new activity type. " + strNewTypeId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New activity type created. ";
                        objNewType = objNew;
                        objNewType.Id = Guid.Parse(strNewTypeId);
                        objNewType.createdById = objAuthIdentity.authUserId;
                    }
                }

                return objNewType;

            }
            catch (Exception ex)
            {
                return null;
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
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.activitytypes where subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "'");

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
