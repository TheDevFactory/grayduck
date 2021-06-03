using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GrayDuck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class contactCustomFieldsController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public contactCustomFieldsController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }


        // GET: api/contactCustomFields/5
        [HttpGet("{contactid}", Name = "GetContactCustomFields")]
        public async Task<ActionResult<contactcustomfieldModel>> Get(string contactid)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACTCUSTOMFIELD";
                objAudit.eventType = "GetContactCustomFields (contactid)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetContactCustomFields";
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
                            contactCustomFieldsService _contactcustomfieldService = new contactCustomFieldsService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(contactid);
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contact Custom Field List";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _contactcustomfieldService.Get(Guid.Parse(contactid)));
                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(contactid);
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contact Custom Field List.";
                            objAudit.targetTable = "contact";

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


        // POST: api/contactCustomFields
        [HttpPost]
        public async Task<ActionResult<contactcustomfieldModel>> Post(contactcustomfieldModel[] objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACTCUSTOMFIELD";
                objAudit.eventType = "CreateContactCustomField";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateContactCustomField";
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
                            contactCustomFieldsService _contactcustomfieldService = new contactCustomFieldsService(objConfiguration, objAuthIdentity);
                            objItem = await _contactcustomfieldService.Create(objItem);

                            //****************************************************************************
                            objAudit.objectType = "CONTACTCUSTOMFIELD";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(objItem[0].Id.ToString());
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Create Contact Custom Field";
                            objAudit.targetTable = "contactcustomfield";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            //return CreatedAtRoute("GetContactCustomFields", new { id = objItem.Id.ToString() }, objItem);
                            return Created("GetContactCustomFields", objItem);
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACTCUSTOMFIELD";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(objItem[0].Id.ToString());
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Contact Custom Field";
                            objAudit.targetTable = "contactcustomfield";

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


        // DELETE: api/contactCustomFields/5
        [HttpDelete("{contactid}")]
        public async Task<IActionResult> Delete(string contactid)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACTCUSTOMFIELD";
                objAudit.eventType = "CreateContactCustomField";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateContactCustomField";
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
                            contactCustomFieldsService _contactService = new contactCustomFieldsService(objConfiguration, objAuthIdentity);

                            _contactService.Remove(Guid.Parse(contactid));

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(contactid);
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Delete Contact Custom Fields";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok();
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Parse(contactid);
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Contact Custom Fields";
                            objAudit.targetTable = "contact";

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
