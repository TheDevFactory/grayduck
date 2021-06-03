using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GrayDuck.Services
{
    public class accountService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public accountService(IConfiguration _configuration, identityModel _AuthIdentity)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, name, description, accountstatus, industry, siccode, website, annualrevenue, accountscore, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.account WHERE subscriptionid IN (" + strSubscriptionIds + ") ORDER BY createdat DESC;");

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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, externalid, name, description, accountstatus, industry, siccode, website, annualrevenue, accountscore, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdbyid, createdat, updatedat, assignedtoid FROM public.account WHERE subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "';");

                    }
                }

                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<accountModel> Create(accountModel objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            accountModel objNewAccount = new accountModel();

            string strExistingAccountNameGuid = "";
            string strExistingAccountExternalIdGuid = "";
            string strNewAccountId = "";

            bool bolProceed = false;
            string strProcessMessage = "";

            try
            {

                //Process to create new account for subscription


                //See if the account exists for either the name or externalId
                if (String.IsNullOrEmpty(objNew.name))
                {

                }
                else
                {
                    strExistingAccountNameGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.account where subscriptionId='" + objNew.subscriptionId + "' and name='" + _databaseManager.sqlCheck(objNew.name) + "'");
                }
                if (String.IsNullOrEmpty(objNew.externalId) || objNew.externalId == "0")
                {

                }
                else
                {
                    strExistingAccountExternalIdGuid = await _databaseManager.executeScalarReturn("SELECT Id from public.account where subscriptionId='" + objNew.subscriptionId + "' and externalid='" + _databaseManager.sqlCheck(objNew.externalId) + "'");
                }


                if (strExistingAccountNameGuid == "" && strExistingAccountExternalIdGuid == "")
                {
                    bolProceed = true;
                    strProcessMessage = "";
                }
                else
                {
                    bolProceed = false;
                    strProcessMessage = "Existing account found for this name / externalId.";

                    //Return Existing Account Id
                    //Name wins over externalId - always
                    if (String.IsNullOrEmpty(strExistingAccountNameGuid))
                    {
                        if (String.IsNullOrEmpty(strExistingAccountExternalIdGuid))
                        {

                        }
                        else
                        {
                            objNewAccount = objNew;
                            objNewAccount.Id = Guid.Parse(strExistingAccountExternalIdGuid);
                        }
                    }
                    else
                    {
                        objNewAccount = objNew;
                        objNewAccount.Id = Guid.Parse(strExistingAccountNameGuid);
                    }

                }


                //If we are allowed to proceed the process can start
                if (bolProceed == true)
                {
                    //1. Create New Account
                    strNewAccountId = await _databaseManager.executeQuery("INSERT INTO public.account (subscriptionid, externalid, name, description, accountstatus, industry, siccode, website, annualrevenue, billingstreet, billingcity, billingstate, billingcountry, billingpostcode, isactive, createdbyid, createdat, updatedat, assignedtoid) VALUES ('" + objNew.subscriptionId + "','" + _databaseManager.sqlCheck(objNew.externalId) + "','" + _databaseManager.sqlCheck(objNew.name) + "','" + _databaseManager.sqlCheck(objNew.description) + "','" + _databaseManager.sqlCheck(objNew.accountStatus) + "','" + _databaseManager.sqlCheck(objNew.industry) + "','" + _databaseManager.sqlCheck(objNew.sicCode) + "','" + _databaseManager.sqlCheck(objNew.website.ToLower()) + "'," + objNew.annualRevenue + ",'" + _databaseManager.sqlCheck(objNew.billingStreet) + "','" + _databaseManager.sqlCheck(objNew.billingCity) + "','" + _databaseManager.sqlCheck(objNew.billingState) + "', '" + _databaseManager.sqlCheck(objNew.billingCountry) + "', '" + _databaseManager.sqlCheck(objNew.billingPostCode) + "', " + objNew.isActive + ", '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, '" + objNew.assignedToId + "') RETURNING Id;");

                    if (strNewAccountId == "" || strNewAccountId.StartsWith("Error"))
                    {
                        bolProceed = false;
                        strProcessMessage += "Unable to create new account. " + strNewAccountId;
                    }
                    else
                    {
                        bolProceed = true;
                        strProcessMessage += "New account created. ";
                        objNewAccount = objNew;
                        objNewAccount.Id = Guid.Parse(strNewAccountId);
                        objNewAccount.createdById = objAuthIdentity.authUserId;

                        //If we have a new account we can insert the custom field values
                        if (objNew.customFields is null)
                        {
                            //No Custom Fields Found    
                        }
                        else
                        {
                            //Custom Fields Found
                            foreach (accountcustomfieldModel objField in objNew.customFields)
                            {

                                await _databaseManager.executeQuery("INSERT INTO public.accountcustomfield (subscriptionid, customfieldid, accountid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objNew.subscriptionId + "', '" + objField.Id + "', '" + strNewAccountId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                            }
                        }

                    }
                }

                return objNewAccount;

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

                        ////Execute Command
                        //rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.account where subscriptionid IN (" + strSubscriptionIds + ") and id='" + id + "'");

                        //accountfile
                        //rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.accountfile where subscriptionid IN (" + strSubscriptionIds + ") and accountid='" + id + "';");


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

    public class accountCustomFieldsService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public accountCustomFieldsService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<string> Get(Guid accountid)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, accountid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.accountcustomfield WHERE subscriptionid IN (" + strSubscriptionIds + ") and accountid='" + accountid + "';");

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

        public async Task<accountcustomfieldModel[]> Create(accountcustomfieldModel[] objNew)
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
                            foreach (accountcustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {
                                    //Build Query Stringt to Execute
                                    if (strQueryString == "")
                                    {
                                        strQueryString = " subscriptionid=" + objField.subscriptionId + " and accountid='" + objField.accountId + "' and customfieldid='" + objField.Id + "' ";
                                    }
                                    else
                                    {
                                        strQueryString += " OR subscriptionid=" + objField.subscriptionId + " and accountid='" + objField.accountId + "' and customfieldid='" + objField.Id + "' ";
                                    }
                                }
                                else
                                {
                                    //Not found so do nothing
                                }

                            }

                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, customfieldid, accountid, fieldname, fieldvalue, createdbyid, createdat, updatedat FROM public.accountcustomfield WHERE " + strQueryString + " ;");




                        //Build Allowed Subscription List
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {

                            //1 - Create Custom Field for each object we receive
                            //    This ensures that you cannot try to add values for subscriptions you have no access to
                            foreach (accountcustomfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {

                                    //Before we INSERT see if we need to do an UPDATE if the field already exists for this contact
                                    DataRow[] foundRows = _dataTable.Select("customfieldid='" + objField.Id + "' AND accountid='" + objField.accountId + "' AND subscriptionid='" + objField.subscriptionId + "'");
                                    if (foundRows.Length == 0)
                                    {
                                        //Found so it is allowed, insert data
                                        strNewFieldValue = await _databaseManager.executeQuery("INSERT INTO public.accountcustomfield (subscriptionid, customfieldid, contactid, fieldname, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objField.subscriptionId + "', '" + objField.Id + "', '" + objField.accountId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                                        //Update Object
                                        objField.Id = Guid.Parse(strNewFieldValue);
                                        objField.createdById = objAuthIdentity.authUserId;
                                    }
                                    else
                                    {
                                        //Existing Record found so, update data
                                        strNewFieldValue = await _databaseManager.executeQuery("UPDATE public.accountcustomfield SET fieldvalue='" + _databaseManager.sqlCheck(objField.fieldValue) + "', updatedat=CURRENT_TIMESTAMP WHERE customfieldid='" + objField.Id + "' AND accountid='" + objField.accountId + "' AND subscriptionid='" + objField.subscriptionId + "';");
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

        public void Remove(Guid accountid)
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
                        _databaseManager.executeNonQuery("DELETE FROM public.accountcustomfield where subscriptionid IN (" + strSubscriptionIds + ") and accountid='" + accountid + "'");

                    }
                }


            }
            catch (Exception ex)
            {

            }
        }

    }

    public class accountFileService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public accountFileService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<string> Get(Guid accountid)
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, accountid, filename, filetype, filesize, filetag, createdbyid, createdat, updatedat FROM public.contactfile WHERE subscriptionid IN (" + strSubscriptionIds + ") and accountid='" + accountid + "' ;");


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

        public async Task<accountFileModel[]> Create(accountFileModel[] objNew)
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
                        foreach (accountFileModel objFile in objNew)
                        {

                            //Found so it is allowed, insert data
                            strNewFileValue = await _databaseManager.executeQueryWithByte("INSERT INTO public.accountfile (subscriptionid, accountid, filename, filetype, filesize, filetag, filedata, createdbyid, createdat, updatedat) VALUES ('" + objFile.subscriptionId + "', '" + objFile.accountId + "', '" + _databaseManager.sqlCheck(objFile.fileName) + "', '" + _databaseManager.sqlCheck(objFile.fileType) + "', " + objFile.fileSize + ", '" + _databaseManager.sqlCheck(objFile.fileTag) + "', @FileByte, '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING Id;", objFile.fileData);

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

        public void Remove(Guid accountid, Guid fileid)
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
                        if (accountid == Guid.Empty)
                        {
                            _databaseManager.executeNonQuery("DELETE FROM public.accountfile where subscriptionid IN (" + strSubscriptionIds + ") and id='" + fileid + "'");
                        }
                        else
                        {
                            _databaseManager.executeNonQuery("DELETE FROM public.accountfile where subscriptionid IN (" + strSubscriptionIds + ") and accountid='" + accountid + "'");
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
                        if (strSubscriptionIds == "")
                        {
                            //Do nothing
                            return null;
                        }
                        else
                        {
                            _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, accountid, filename, filedata, filetype, filesize, filetag, createdbyid, createdat, updatedat FROM public.accountfile WHERE subscriptionid IN (" + strSubscriptionIds + ") and id='" + fileid + "' ;");
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
