using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GrayDuck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class accountController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public accountController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }


        // GET: api/account
        /// <summary>
        /// Retrieves account list for subscription.
        /// </summary>
        /// <remarks>
        /// Note that a set of paramaters can be passed to limit the results.
        /// Results should be limited to a maximum of 10,000 records.
        /// </remarks>
        [HttpGet]
        public async Task <ActionResult<List<accountModel>>> Get()
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNT";
                objAudit.eventType = "GetAccounts";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetAccounts";
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
                            if (objAuthSecurity.accountView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            accountService _accountService = new accountService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Accounts";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _accountService.Get());
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Accounts.";
                            objAudit.targetTable = "account";

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


        // GET: api/account/5
        /// <summary>
        /// Retrieves account by id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "GetAccount")]
        public async Task <ActionResult<accountModel>> Get(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNT";
                objAudit.eventType = "GetAccount (id)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetAccount";
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
                            if (objAuthSecurity.accountView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            accountService _accountService = new accountService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Account";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _accountService.Get(id));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Account.";
                            objAudit.targetTable = "account";

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


        // POST: api/account
        [HttpPost]
        public async Task <ActionResult<accountModel>> Post(accountModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNT";
                objAudit.eventType = "CreateAccount";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateAccount";
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
                       accountService _accountService = new accountService(objConfiguration, objAuthIdentity);  
                       await  _accountService.Create(objItem);


                        //****************************************************************************
                        objAudit.objectType = "ACCOUNT";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = objItem.Id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Create Account";
                        objAudit.targetTable = "account";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return CreatedAtRoute("GetAccount", new { id = objItem.Id.ToString() }, objItem);
                    }
                    else
                    {
                        //****************************************************************************
                        objAudit.objectType = "ACCOUNT";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Empty;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Account.";
                        objAudit.targetTable = "account";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // PUT: api/account/5
        [HttpPut("{id}")]
        public async Task <IActionResult> Put(string id, accountModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNT";
                objAudit.eventType = "UpdateAccount";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UpdateAccount";
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
                        accountService _accountService = new accountService(objConfiguration, objAuthIdentity);

                        //_accountService.Update(id,objItem);

                        ////****************************************************************************
                        //objAudit.objectType = "ACCOUNT";
                        //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        //objAudit.objectId = objItem.Id;
                        //objAudit.environmentUserId = objAuthIdentity.authUserId;
                        //objAudit.environmentUserToken = objAuthIdentity.authToken;
                        //objAudit.targetResult = "200";
                        //objAudit.targetNewValue = "Update Account";
                        //objAudit.targetTable = "account";

                        //await objAuditLog.Create(objAudit);
                        ////****************************************************************************

                        return Ok();
                    }
                    else
                    {

                        //****************************************************************************
                        objAudit.objectType = "ACCOUNT";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = objItem.Id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Update Account.";
                        objAudit.targetTable = "account";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // DELETE: api/account/5
        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNT";
                objAudit.eventType = "DeleteAccount";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteAccount";
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
                        accountService _accountService = new accountService(objConfiguration, objAuthIdentity);

                        long rowsAffected = 0;

                        rowsAffected = await _accountService.Remove(id);

                        if (rowsAffected == 0)
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "400";
                            objAudit.targetNewValue = "Error, 0 records deleted. Delete Account.";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return BadRequest("Error, 0 records deleted.");
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Delete Account";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok();
                        }

                    }
                    else
                    {
                        //****************************************************************************
                        objAudit.objectType = "ACCOUNT";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Account.";
                        objAudit.targetTable = "account";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }




        /// <summary>
        /// Retrieves account files by account id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>        
        [HttpGet("{id}/files", Name = "GetAccountFiless")]
        public async Task<ActionResult<contactFileModel>> GetFiles(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNTFILE";
                objAudit.eventType = "GetAccountFiles (accountid)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetAccountFiles";
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
                            if (objAuthSecurity.accountView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            accountFileService _accountFileService = new accountFileService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Account File List";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _accountFileService.Get(id));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Account File List.";
                            objAudit.targetTable = "account";

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


        /// <summary>
        /// Uploads a file for a account record. Limited to 200 MB in file size.
        /// </summary>
        /// <remarks>
        /// Note that the accountid and subscriptionid is a GUID and not an integer.
        /// The Upload file size is limited to 200 MB.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="file"> Binary </param>
        /// <param name="fileTag"></param>
        /// <param name="subscriptionId"></param>
        [HttpPost("{id}/files", Name = "UploadAccountFiless")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<ActionResult<contactFileModel[]>> PostFile(Guid id, IFormFile file, string fileTag, string subscriptionId)
        {
            try
            {
                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNTFILE";
                objAudit.eventType = "UploadAccountFile";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UploadAccountFile";
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
                            if (objAuthSecurity.accountEdit == true && objAuthSecurity.subscriptionId == Guid.Parse(subscriptionId))
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            if (file == null || file.Length == 0)
                            {
                                return BadRequest("Error, Invalid file received. Content is null or lenght is 0.");
                            }
                            else
                            {

                                //Ensure the file name is clean with no bad characters
                                string fullFileName = file.FileName.ToString();
                                fullFileName = fullFileName.Trim('"');

                                if (file.FileName.Contains("\\"))
                                {
                                    //Clean up file name
                                    fullFileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                                }
                                else
                                {
                                    //Nothing to do
                                }


                                //Create Object with File (1)
                                accountFileModel[] objItem = new accountFileModel[1];

                                objItem[0] = new accountFileModel
                                {
                                    accountId = id,
                                    fileTag = fileTag.ToUpper(),
                                    subscriptionId = Guid.Parse(subscriptionId),
                                    createdById = objAuthIdentity.authUserId
                                };

                                using (var target = new MemoryStream())
                                {
                                    //Copy File into a Stream so we can store at as an Byte Array
                                    file.CopyTo(target);

                                    objItem[0].fileData = target.ToArray();
                                    objItem[0].fileName = fullFileName;
                                    objItem[0].fileSize = file.Length;
                                    objItem[0].fileType = file.ContentType;
                                }


                                accountFileService _accountFileService = new accountFileService(objConfiguration, objAuthIdentity);
                                objItem = await _accountFileService.Create(objItem);

                                //****************************************************************************
                                objAudit.objectType = "ACCOUNT";
                                objAudit.subscriptionId = Guid.Parse(subscriptionId);
                                objAudit.objectId = id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "201";
                                objAudit.targetNewValue = "Uploaded File For Account (" + fullFileName + ")";
                                objAudit.targetTable = "account";

                                await objAuditLog.Create(objAudit);


                                objAudit.objectType = "ACCOUNTFILE";
                                objAudit.objectId = objItem[0].Id;
                                objAudit.targetTable = "accountfile";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return CreatedAtRoute("GetAccountFiless", new { id = id }, objItem);

                            }

                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = Guid.Parse(subscriptionId);
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Upload Account File.";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                        }

                    }
                    else
                    {
                        return Unauthorized(objAuthIdentity.authResult);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }






        /// <summary>
        /// Retrieves custom fields by account id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>    
        [HttpGet("{id}/customfields", Name = "GetAccountCustomFieldss")]
        public async Task<ActionResult<accountcustomfieldModel>> GetCustomFields(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNTCUSTOMFIELD";
                objAudit.eventType = "GetAccountCustomFields";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetAccountCustomField";
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
                            if (objAuthSecurity.contactView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            accountCustomFieldsService _accountcustomfieldService = new accountCustomFieldsService(objConfiguration, objAuthIdentity);


                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Account Custom Fields";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _accountcustomfieldService.Get(id));
                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId; ;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Get Account Custom Field.";
                            objAudit.targetTable = "account";

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


        /// <summary>
        /// Add Custom Field Values to Account.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>
        [HttpPost("{id}/customfields", Name = "CreateAccountCustomFieldss")]
        public async Task<ActionResult<accountcustomfieldModel>> PostCustomFields(Guid id,accountcustomfieldModel[] objItem)
        {
            try
            {


                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNTCUSTOMFIELD";
                objAudit.eventType = "CreateAccountCustomFields";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateAccountCustomField";
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
                            if (objAuthSecurity.contactAdd == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Create Account Custom Fields";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            accountCustomFieldsService _accountcustomfieldService = new accountCustomFieldsService(objConfiguration, objAuthIdentity);
                            objItem = await _accountcustomfieldService.Create(objItem);

                            //return CreatedAtRoute("GetContactCustomFields", new { id = objItem.Id.ToString() }, objItem);
                            return Created("GetAccountCustomFieldss", objItem);
                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId; ;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Account Custom Fields.";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                        }

                    }
                    else
                    {
                        return Unauthorized(objAuthIdentity.authResult);
                    }

                }

            }
            catch (Exception ex)
            {
                 return BadRequest("Error, " + ex.Message);
            }
        }


        /// <summary>
        /// Deletes all custom field values by account id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>    
        [HttpDelete("{id}/customfields", Name = "DeleteAccountCustomFieldss")]
        public async Task<IActionResult> DeleteCustomFields(Guid id)
        {
            try
            {


                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACCOUNTCUSTOMFIELD";
                objAudit.eventType = "DeleteAccountCustomFields";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteAccountCustomFields";
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
                            if (objAuthSecurity.contactDelete == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Delete Account Custom Fields";
                            objAudit.targetTable = "account";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            accountCustomFieldsService _accountService = new accountCustomFieldsService(objConfiguration, objAuthIdentity);

                            _accountService.Remove(id);

                            return Ok();
                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "ACCOUNT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId; ;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Account Custom Fields.";
                            objAudit.targetTable = "account";

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
