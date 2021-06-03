using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public class contactController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public contactController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }


        /// <summary>
        /// Searches the contact list for subscription.
        /// </summary>
        /// <remarks>
        /// Note that a set of paramaters can be passed to limit the results.
        /// Results should be limited to a maximum of 10,000 records.
        /// </remarks>
        [HttpGet("search")]
        public async Task<ActionResult<List<contactModel>>> Search(string firstname, string lastname, string mobile, string email, string customfield1, string customfieldvalue1, string customfield2, string customfieldvalue2)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "SearchContacts";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "SearchContacts";
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
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contact List";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _contactService.Search(firstname, lastname,mobile,email,customfield1,customfieldvalue1,customfield2,customfieldvalue2));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contact List.";
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


        /// <summary>
        /// Retrieves contact list for subscription.
        /// </summary>
        /// <remarks>
        /// Note that a set of paramaters can be passed to limit the results.
        /// Results should be limited to a maximum of 1,000 records showing newest contacts first.
        /// </remarks>
        [HttpGet]
        public async Task <ActionResult<List<contactModel>>> Get(int limit = 100, string orderby = "createdat DESC")
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "GetContacts";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetContacts";
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
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contacts";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _contactService.Get(limit,orderby));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contacts.";
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


        /// <summary>
        /// Retrieves contact by id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "GetContact")]
        public async Task <ActionResult<contactModel>> Get(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "GetContact (id)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetContact";
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
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contact";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _contactService.Get(id));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contact.";
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


        /// <summary>
        ///  Creates new contact for subscription.
        /// </summary>
        [HttpPost]
        public async Task <ActionResult<contactModel>> Post(contactModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "CreateContact";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateContact";
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
                            if (objAuthSecurity.subscriptionId == objItem.subscriptionId && objAuthSecurity.contactAdd == true) {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true) {
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);
                            objItem = await _contactService.Create(objItem);

                            if (objItem == null)
                            {
                                return BadRequest("Error, unable to create contact.");
                            }
                            else
                            {

                                //****************************************************************************
                                objAudit.objectType = "CONTACT";
                                objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                objAudit.objectId = objItem.Id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "200";
                                objAudit.targetNewValue = "Create Contact";
                                objAudit.targetTable = "contact";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return CreatedAtRoute("GetContact", new { id = objItem.Id.ToString() }, objItem);
                            }

                        } else {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Contact.";
                            objAudit.targetTable = "contact";

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
        /// Updates existing contact by id.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task <IActionResult> Put(Guid id, contactModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "UpdateContact";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UpdateContact";
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
                            if (objAuthSecurity.subscriptionId == objItem.subscriptionId && objAuthSecurity.contactEdit == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            //_contactService.Update(id, objItem);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = objItem.Id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Update Contact";
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
                            objAudit.objectId = objItem.Id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Update Contact.";
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


        /// <summary>
        /// Deletes existing contact by id.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
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
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "DeleteContact";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteContact";
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
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            long rowsAffected = 0;

                            rowsAffected = await _contactService.Remove(id);

                            if (rowsAffected == 0)
                            {

                                //****************************************************************************
                                objAudit.objectType = "CONTACT";
                                objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                objAudit.objectId = id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "400";
                                objAudit.targetNewValue = "Error, 0 records deleted. Delete Contact.";
                                objAudit.targetTable = "contact";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return BadRequest("Error, 0 records deleted.");
                            }
                            else
                            {

                                //****************************************************************************
                                objAudit.objectType = "CONTACT";
                                objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                objAudit.objectId = id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "200";
                                objAudit.targetNewValue = "Delete Contact";
                                objAudit.targetTable = "contact";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return Ok();
                            }
                           
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Contact.";
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






        /// <summary>
        /// Updates existing contact by id.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        [HttpPut("{id}/status", Name = "UpdateContactStatus")]
        public async Task<IActionResult> UpdateContactStatus(Guid id, string Status, string subscriptionId)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACT";
                objAudit.eventType = "UpdateContactStatus";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UpdateContactStatus";
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
                            if (objAuthSecurity.subscriptionId == Guid.Parse(subscriptionId) && objAuthSecurity.contactEdit == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            contactService _contactService = new contactService(objConfiguration, objAuthIdentity);

                            _contactService.UpdateStatus(id, Status, Guid.Parse(subscriptionId));

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = Guid.Parse(subscriptionId);
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Update Contact Status [" + Status + "]";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok();
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = Guid.Parse(subscriptionId);
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Update Contact Status.";
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





        /// <summary>
        /// Retrieves contact files by contact id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>        
        [HttpGet("{id}/files", Name = "GetContactFiless")]
        public async Task<ActionResult<contactFileModel>> GetFiles(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "CONTACTFILE";
                objAudit.eventType = "GetContactFiles (contactid)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetContactFiles";
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
                            contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contact File List";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _contactFileService.Get(id));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contact File List.";
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


        /// <summary>
        /// Uploads a file for a contact record. Limited to 200 MB in file size.
        /// </summary>
        /// <remarks>
        /// Note that the contactid and subscriptionid is a GUID and not an integer.
        /// The Upload file size is limited to 200 MB.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="file"> Binary </param>
        /// <param name="fileTag"></param>
        /// <param name="subscriptionId"></param>
        [HttpPost("{id}/files", Name = "UploadContactFiless")]
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
                objAudit.objectType = "CONTACTFILE";
                objAudit.eventType = "UploadContactFile";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UploadContactFile";
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
                            if (objAuthSecurity.contactEdit == true && objAuthSecurity.subscriptionId == Guid.Parse(subscriptionId))
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
                                contactFileModel[] objItem = new contactFileModel[1];

                                objItem[0] = new contactFileModel
                                {
                                    contactId = id,
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


                                contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);
                                objItem = await _contactFileService.Create(objItem);

                                //****************************************************************************
                                objAudit.objectType = "CONTACT";
                                objAudit.subscriptionId = Guid.Parse(subscriptionId);
                                objAudit.objectId = id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "201";
                                objAudit.targetNewValue = "Uploaded File For Contact (" + fullFileName + ")";
                                objAudit.targetTable = "contact";

                                await objAuditLog.Create(objAudit);


                                objAudit.objectType = "CONTACTFILE";
                                objAudit.objectId = objItem[0].Id;
                                objAudit.targetTable = "contactfile";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return CreatedAtRoute("GetContactFiless", new { id = id }, objItem);

                            }

                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = Guid.Parse(subscriptionId);
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Upload Contact File.";
                            objAudit.targetTable = "contact";

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
        /// Retrieves contact activities by contact id.
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        /// <param name="id"></param>        
        [HttpGet("{id}/activities", Name = "GetContactActivitiess")]
        public async Task<ActionResult<contactFileModel>> GetActivities(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIES";
                objAudit.eventType = "GetContactActivities (contactid)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetContactActivitiess";
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
                            if (objAuthSecurity.activityView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            activitiesService _activitiesService = new activitiesService(objConfiguration, objAuthIdentity);

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = "Return Contact Activity List";
                            objAudit.targetTable = "contact";

                            await objAuditLog.Create(objAudit);
                            //****************************************************************************

                            return Ok(await _activitiesService.GetContactItems(id));
                        }
                        else
                        {

                            //****************************************************************************
                            objAudit.objectType = "CONTACT";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = id;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Contact Activity List.";
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
