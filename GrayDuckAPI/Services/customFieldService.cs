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
    public class customFieldService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public customFieldService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }

        public async Task<string> Get() {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            DataTable _dataTable;
            String strJSon;
            //List<object> _listObject = new List<object>();

            try
            {

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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, fieldname, fieldtype, fieldrequired, fieldlenght, fieldorder, fieldvalue, createdbyid, createdat, updatedat FROM public.customfield WHERE subscriptionid IN (" + strSubscriptionIds + ");");

                        //Convert to Json string
                        strJSon = JsonConvert.SerializeObject(_dataTable, Formatting.Indented);

                        //Convert Table to List Object
                        //_listObject = _dataTable.AsEnumerable().ToList<object>();
                    }
                }

                return strJSon;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> Get(Guid id) {
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, fieldname, fieldtype, fieldrequired, fieldlenght, fieldorder, fieldvalue, createdbyid, createdat, updatedat FROM public.customfield WHERE subscriptionid IN (" + strSubscriptionIds + ") AND id='" + id + "';");

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

        public async Task<customfieldModel[]> Create(customfieldModel[] objNew)
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
                            foreach (customfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {
                                    //Build Query Stringt to Execute
                                    if (strQueryString == "")
                                    {
                                        strQueryString = " subscriptionid=" + objField.subscriptionId + " and id='" + objField.Id + "' ";
                                    }
                                    else
                                    {
                                        strQueryString += " OR subscriptionid=" + objField.subscriptionId + " and id='" + objField.Id + "' ";
                                    }
                                }
                                else
                                {
                                    //Not found so do nothing
                                }

                            }

                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, fieldname, fieldtype, fieldrequired, fieldlenght, fieldorder, fieldvalue, createdbyid, createdat, updatedat FROM public.customfield WHERE " + strQueryString + " ;");




                        //Build Allowed Subscription List
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {

                            //1 - Create Custom Field for each object we receive
                            //    This ensures that you cannot try to add values for subscriptions you have no access to
                            foreach (customfieldModel objField in objNew)
                            {

                                if (row.subscriptionId == objField.subscriptionId)
                                {

                                    //Before we INSERT see if we need to do an UPDATE if the field already exists for this contact
                                    DataRow[] foundRows = _dataTable.Select("id='" + objField.Id + "' AND subscriptionid='" + objField.subscriptionId + "'");
                                    if (foundRows.Length == 0)
                                    {
                                        //Found so it is allowed, insert data
                                        strNewFieldValue = await _databaseManager.executeQuery("INSERT INTO public.customfield (subscriptionid, fieldname, fieldtype, fieldrequired, fieldlenght, fieldorder, fieldvalue, createdbyid, createdat, updatedat) VALUES ('" + objField.subscriptionId + "', '" + _databaseManager.sqlCheck(objField.fieldName) + "', '" + _databaseManager.sqlCheck(objField.fieldType) + "', " + objField.fieldRequired + ", " + objField.fielLenght + ", " + objField.fieldOrder + ", '" + _databaseManager.sqlCheck(objField.fieldValue) + "', '" + objAuthIdentity.authUserId + "' , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

                                        //Update Object
                                        objField.Id = Guid.Parse(strNewFieldValue);
                                        objField.createdById = objAuthIdentity.authUserId;
                                    }
                                    else
                                    {
                                        //Existing Record found so, update data
                                        strNewFieldValue = await _databaseManager.executeQuery("UPDATE public.customfield SET fieldname='" + _databaseManager.sqlCheck(objField.fieldName ) + "', fieldtype='" + _databaseManager.sqlCheck(objField.fieldType ) + "', fieldrequired=" + objField.fieldRequired + ", fieldlenght=" + objField.fielLenght + ", fieldorder=" + objField.fieldOrder + ", fieldvalue='" + _databaseManager.sqlCheck(objField.fieldValue) + "', updatedat=CURRENT_TIMESTAMP WHERE id='" + objField.Id + "' AND subscriptionid='" + objField.subscriptionId + "';");
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

    }
}
