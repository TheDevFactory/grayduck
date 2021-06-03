using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.Extensions.Configuration;

namespace GrayDuck.Services
{
    public class auditlogService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
        
        public auditlogService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;
            
            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, null);
        }

        public async Task<DataTable> Get() {
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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, objectid, objecttype, eventtype, environmentuserid, environmentusertoken, environmentmachine, environmentdomain, environmentculture, targetapi, targetaction, targetmethod, targettable, targetresult, targetnewvalue, createdat FROM public.auditlog WHERE subscriptionid IN (" + strSubscriptionIds + ") ORDER BY createdat DESC LIMIT 1000;");

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
                        _dataTable = await _databaseManager.executeReader("SELECT id, subscriptionid, objectid, objecttype, eventtype, environmentuserid, environmentusertoken, environmentmachine, environmentdomain, environmentculture, targetapi, targetaction, targetmethod, targettable, targetresult, targetnewvalue, createdat FROM public.auditlog WHERE subscriptionid IN (" + strSubscriptionIds + ") AND id='" + id + "' ;");

                    }
                }

                return _dataTable;

            }
            catch (Exception ex)
            {
         
                return null;
            }
        }

        public async Task<auditlogModel> Create(auditlogModel objNew)
        {

            databaseSettings _databaseManager = new databaseSettings(objConfiguration);
            auditlogModel objNewAuditLog = new auditlogModel();
            string strNewAuditLogId = "";

            try
            {

                //Process to create new audit log entry for subscription


                //1. Create New Audit Log Entry
                strNewAuditLogId = await _databaseManager.executeQuery("INSERT INTO public.auditlog (subscriptionid, objectid, objecttype, eventtype, environmentuserid, environmentusertoken, environmentmachine, environmentdomain, environmentculture, targetapi, targetaction, targetmethod, targettable, targetresult, targetnewvalue, createdat) VALUES ('" + objNew.subscriptionId + "','" + objNew.objectId + "','" + _databaseManager.sqlCheck(objNew.objectType) + "','" + _databaseManager.sqlCheck(objNew.eventType) + "','" + objNew.environmentUserId + "', '" + objNew.environmentUserToken + "', '" + _databaseManager.sqlCheck(objNew.environmentMachine) + "', '" + _databaseManager.sqlCheck(objNew.environmentDomain) + "', '" + _databaseManager.sqlCheck(objNew.environmentCulture) + "', '" + _databaseManager.sqlCheck(objNew.targetAPI) + "', '" + _databaseManager.sqlCheck(objNew.targetAction) + "', '" + _databaseManager.sqlCheck(objNew.targetMethod) + "', '" + _databaseManager.sqlCheck(objNew.targetTable) + "', '" + _databaseManager.sqlCheck(objNew.targetResult) + "', '" + _databaseManager.sqlCheck(objNew.targetNewValue) + "' , CURRENT_TIMESTAMP) RETURNING Id;");

                if (strNewAuditLogId == "" || strNewAuditLogId.StartsWith("Error"))
                {
                    //strProcessMessage += "Unable to create new audit loh entry. " + strNewAuditLogId;
                }
                else
                {
                    //strProcessMessage += "New audit log entry created. ";
                    objNewAuditLog = objNew;
                    objNewAuditLog.Id = Guid.Parse(strNewAuditLogId);
                    //objNewServerLog.createdById = objAuthIdentity.authUserId;
                }

                return objNewAuditLog;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<long> Remove()
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
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.auditlog WHERE subscriptionid IN (" + strSubscriptionIds + ");");

                    }
                }

                return rowsAffected;

            }
            catch (Exception ex)
            {
                return 0;
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
                        rowsAffected = await _databaseManager.executeNonQuery("DELETE FROM public.auditlog WHERE subscriptionid IN (" + strSubscriptionIds + ") AND id='" + id + "'");

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
