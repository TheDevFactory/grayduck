using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrayDuckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class fileController : ControllerBase
    {
        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public fileController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }

        // GET: api/file/5
        [HttpGet("{fileid}", Name = "DownloadFile")]
        public async Task<IActionResult> Get(string fileid, string objectType)
        {
            try
            {

                //objectType - Would be contact, account
                //fileid - Would be per type. Contact or Account


                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                //objAudit.objectType = "CONTACTFILE";
                objAudit.eventType = "DownloadFile (fileid,objectType)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DownloadFile";
                objAudit.targetMethod = HttpContext.Request.Method;
                //objAudit.targetTable = "subscriptionuser";
                //objAudit.targetResult = "200";
                //objAudit.targetNewValue = "";
                //*************************************************************************************


                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var objAuthHeaderValue) == false)
                    // No Header Auth Info
                    return Unauthorized("Error, No Authorization header found in the request.");
                else
                {
                    // ***********************************************************************************
                    // Get the auth token
                    //string authToken = Request.Headers.Authorization.Parameter;
                    string authToken = HttpContext.Request.Headers["Authorization"][0];
                    authToken = authToken.Replace("Bearer ", ""); //Clean up Bearer Auth Value
                    // ***********************************************************************************

                    // ***********************************************************************************
                    //New Auth Object - Perform Authentication
                    subscriptionUserService objUserAuthentication = new subscriptionUserService(objConfiguration, null);
                    identityModel objAuthIdentity;

                    objAuthIdentity = await objUserAuthentication.AuthenticateUser("", Guid.Parse(authToken));
                    // ***********************************************************************************

                    // Normally Identity handles sign in, but you can do it directly
                    if (objAuthIdentity.authMessage == "SUCCESS")
                    {
                        //Check Security - for the posted subscriptionId does the user have access to do this function
                        //Since the Auth has been confirmed we know there has to be a security group (at least 1 so we will not get a NULL / Nothing Object)
                        bool boolAuthorized = false;
                        foreach (subscriptionSecurityModel objAuthSecurity in objAuthIdentity.authSecurity)
                        {

                            if (objectType == null) {
                                return BadRequest("Error, Unknown object type.");
                            } else {
                                switch (objectType.ToUpper())
                                {
                                    case "CONTACT":
                                        if (objAuthSecurity.contactView == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    case "ACCOUNT":
                                        if (objAuthSecurity.accountView == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    default:
                                        throw new InvalidOperationException("Unknown Object Type");
                                }
                            }

                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            DataTable _dataTable;

                            switch (objectType.ToUpper())
                            {
                                case "CONTACT":

                                    // Get the data
                                    contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);
                                    _dataTable = await _contactFileService.Download(Guid.Parse(fileid));

                                    if (_dataTable == null) {

                                        //****************************************************************************
                                        objAudit.objectType = "CONTACTFILE";
                                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                        objAudit.objectId = Guid.Parse(fileid);
                                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                                        objAudit.targetResult = "400";
                                        objAudit.targetNewValue = "Error, Unknown download request.";
                                        objAudit.targetTable = "contactfile";
                                        
                                        await objAuditLog.Create(objAudit);
                                        //****************************************************************************

                                        return BadRequest("Error, Unknown download request."); 

                                    } else {
                                        string strfilename = "";
                                        string strfiletype = "";
                                        string strcontactid = "";
                                        byte[] b = null;

                                        //Loop over all rows (will only have one)
                                        foreach (DataRow row in _dataTable.Rows)
                                        {
                                            strfilename = row["filename"].ToString();
                                            strfiletype = row["filetype"].ToString();
                                            strcontactid = row["contactid"].ToString();

                                            //byte[] b = null;
                                            b = (byte[])row["filedata"];                                        
                                        }

                                        if (strfilename == "") {

                                            //****************************************************************************
                                            objAudit.objectType = "CONTACTFILE";
                                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                            objAudit.objectId = Guid.Parse(fileid);
                                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                                            objAudit.targetResult = "400";
                                            objAudit.targetNewValue = "Error, Unknown download request. Filename is empty.";
                                            objAudit.targetTable = "contactfile";

                                            await objAuditLog.Create(objAudit);
                                            //****************************************************************************

                                            return BadRequest("Error, Unknown download request.");

                                        } else {

                                            //****************************************************************************
                                            objAudit.objectType = "CONTACTFILE";
                                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                            objAudit.objectId = Guid.Parse(fileid);
                                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                                            objAudit.targetResult = "200";
                                            objAudit.targetNewValue = "Download File (" +strfilename + ")";
                                            objAudit.targetTable = "contactfile";

                                            await objAuditLog.Create(objAudit);

                                            //Seperate Log Entry on the contact record
                                            objAudit.objectType = "CONTACT";
                                            objAudit.objectId = Guid.Parse(strcontactid);
                                            objAudit.targetTable = "contact";
                                            await objAuditLog.Create(objAudit);
                                            //****************************************************************************

                                            return File(b, strfiletype, strfilename);

                                        }
                                    }                                  

                                case "ACCOUNT":

                                    // Get the data
                                    contactFileService _accountFileService = new contactFileService(objConfiguration, objAuthIdentity);
                                    _dataTable = await _accountFileService.Download(Guid.Parse(fileid));

                                    if (_dataTable == null)
                                    {

                                        //****************************************************************************
                                        objAudit.objectType = "ACCOUNTFILE";
                                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                        objAudit.objectId = Guid.Parse(fileid);
                                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                                        objAudit.targetResult = "400";
                                        objAudit.targetNewValue = "Error, Unknown download request.";
                                        objAudit.targetTable = "accountfile";

                                        await objAuditLog.Create(objAudit);
                                        //****************************************************************************

                                        return BadRequest("Error, Unknown download request.");

                                    }
                                    else
                                    {
                                        string strfilename = "";
                                        string strfiletype = "";
                                        string straccountid = "";
                                        byte[] b = null;

                                        //Loop over all rows (will only have one)
                                        foreach (DataRow row in _dataTable.Rows)
                                        {
                                            strfilename = row["filename"].ToString();
                                            strfiletype = row["filetype"].ToString();
                                            straccountid = row["accountid"].ToString();

                                            //byte[] b = null;
                                            b = (byte[])row["filedata"];
                                        }

                                        if (strfilename == "")
                                        {

                                            //****************************************************************************
                                            objAudit.objectType = "ACCOUNTFILE";
                                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                            objAudit.objectId = Guid.Parse(fileid);
                                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                                            objAudit.targetResult = "400";
                                            objAudit.targetNewValue = "Error, Unknown download request. Filename is empty.";
                                            objAudit.targetTable = "accountfile";

                                            await objAuditLog.Create(objAudit);
                                            //****************************************************************************

                                            return BadRequest("Error, Unknown download request.");

                                        }
                                        else
                                        {

                                            //****************************************************************************
                                            objAudit.objectType = "ACCOUNTFILE";
                                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                            objAudit.objectId = Guid.Parse(fileid);
                                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                                            objAudit.targetResult = "200";
                                            objAudit.targetNewValue = "Download File (" + strfilename + ")";
                                            objAudit.targetTable = "accountfile";

                                            await objAuditLog.Create(objAudit);

                                            //Seperate Log Entry on the contact record
                                            objAudit.objectType = "ACCOUNT";
                                            objAudit.objectId = Guid.Parse(straccountid);
                                            objAudit.targetTable = "account";
                                            await objAuditLog.Create(objAudit);
                                            //****************************************************************************

                                            return File(b, strfiletype, strfilename);

                                        }
                                    }

                                default:
                                    return BadRequest("Unknown Object Type");
                                    
                            }

                        }
                        else
                        {
                            return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                        }
                    }
                    else
                    {
                        return BadRequest(objAuthIdentity.authResult);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // DELETE: api/file/5
        [HttpDelete("{fileid}")]
        public async Task<IActionResult> Delete(string fileid, string objectType)
        {
            try
            {
                //objectType - Would be contact, account
                //fileid - Would be per type. Contact or Account

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                //objAudit.objectType = "CONTACTFILE";
                objAudit.eventType = "DeleteFile (fileid,objectType)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteFile";
                objAudit.targetMethod = HttpContext.Request.Method;
                //objAudit.targetTable = "subscriptionuser";
                //objAudit.targetResult = "200";
                //objAudit.targetNewValue = "";
                //*************************************************************************************

                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var objAuthHeaderValue) == false)
                    // No Header Auth Info
                    return Unauthorized("Error, No Authorization header found in the request.");
                else
                {
                    // ***********************************************************************************
                    // Get the auth token
                    //string authToken = Request.Headers.Authorization.Parameter;
                    string authToken = HttpContext.Request.Headers["Authorization"][0];
                    authToken = authToken.Replace("Bearer ", ""); //Clean up Bearer Auth Value
                    // ***********************************************************************************

                    // ***********************************************************************************
                    //New Auth Object - Perform Authentication
                    subscriptionUserService objUserAuthentication = new subscriptionUserService(objConfiguration, null);
                    identityModel objAuthIdentity;

                    objAuthIdentity = await objUserAuthentication.AuthenticateUser("", Guid.Parse(authToken));
                    // ***********************************************************************************

                    // Normally Identity handles sign in, but you can do it directly
                    if (objAuthIdentity.authMessage == "SUCCESS")
                    {
                        //Check Security - for the posted subscriptionId does the user have access to do this function
                        //Since the Auth has been confirmed we know there has to be a security group (at least 1 so we will not get a NULL / Nothing Object)
                        bool boolAuthorized = false;
                        foreach (subscriptionSecurityModel objAuthSecurity in objAuthIdentity.authSecurity)
                        {

                            if (objectType == null)
                            {
                                return BadRequest("Error, Unknown object type.");
                            }
                            else
                            {

                                switch (objectType.ToUpper())
                                {
                                    case "CONTACT":
                                        if (objAuthSecurity.contactDelete == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    case "ACCOUNT":
                                        if (objAuthSecurity.accountDelete == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    default:
                                        throw new InvalidOperationException("Unknown Object Type");
                                }

                            }

                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            switch (objectType.ToUpper())
                            {
                                case "CONTACT":
                                    contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);

                                    _contactFileService.Remove(Guid.Empty,Guid.Parse(fileid));

                                    //****************************************************************************
                                    objAudit.objectType = "CONTACTFILE";
                                    objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                    objAudit.objectId = Guid.Parse(fileid);
                                    objAudit.environmentUserId = objAuthIdentity.authUserId;
                                    objAudit.environmentUserToken = objAuthIdentity.authToken;
                                    objAudit.targetResult = "200";
                                    objAudit.targetNewValue = "File Deleted";
                                    objAudit.targetTable = "contactfile";

                                    await objAuditLog.Create(objAudit);
                                    //****************************************************************************

                                    return Ok();
                                    
                                case "ACCOUNT":
                                    return BadRequest("Unknown Object Type");
                                    
                                default:
                                    return BadRequest("Unknown Object Type");
                                    
                            }

                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "CONTACTFILE";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(fileid);
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "400";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete File.";
                            objAudit.targetTable = "contactfile";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                        }
                    }
                    else
                    {
                        return BadRequest(objAuthIdentity.authResult);
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }

    }
}
