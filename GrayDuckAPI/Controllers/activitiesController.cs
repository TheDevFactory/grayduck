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
    public class activitiesController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public activitiesController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }

        // GET: api/activities/5
        [HttpGet("{id}", Name = "GetActivity")]
        public async Task <ActionResult<activitiesModel>> Get(Guid id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIES";
                objAudit.eventType = "GetActivity (id)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetActivity";
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
                        activitiesService _activitiesService = new activitiesService(objConfiguration, objAuthIdentity);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIES";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Return Activity";
                        objAudit.targetTable = "activities";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        //return Ok(_activitiesService.Get(id));
                        return Ok();
                    }
                    else
                    {
                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIES";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Activity.";
                        objAudit.targetTable = "activities";

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


        // POST: api/activities
        [HttpPost]
        public async Task <ActionResult<activitiesModel>> Post(string contactemail, string contactmobile , activitiesModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIES";
                objAudit.eventType = "CreateActivity";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateActivity";
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
                            if (objAuthSecurity.subscriptionId == objItem.subscriptionId && objAuthSecurity.activityAdd == true)
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
                            objItem = await _activitiesService.Create(contactemail, contactmobile, objItem);

                            if (objItem == null) {
                                return BadRequest("Error, unable to create activity. Ensure you have the contact information included with your submission.");
                            } else {

                                //****************************************************************************
                                objAudit.objectType = "ACTIVITIES";
                                objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                                objAudit.objectId = objItem.Id;
                                objAudit.environmentUserId = objAuthIdentity.authUserId;
                                objAudit.environmentUserToken = objAuthIdentity.authToken;
                                objAudit.targetResult = "200";
                                objAudit.targetNewValue = "Create Activity";
                                objAudit.targetTable = "activities";

                                await objAuditLog.Create(objAudit);
                                //****************************************************************************

                                return CreatedAtRoute("GetActivity", new { id = objItem.Id.ToString() }, objItem);
                            }
                            
                        }
                        else
                        {
                            //****************************************************************************
                            objAudit.objectType = "ACTIVITIES";
                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = Guid.Empty;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "401";
                            objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Activity.";
                            objAudit.targetTable = "activities";

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


        // DELETE: api/activities/5
        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(string id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIES";
                objAudit.eventType = "DeleteActivity";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteActivity";
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
                        activitiesService _activitiesService = new activitiesService(objConfiguration, objAuthIdentity);

                        //_activitiesService.Remove(id);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIES";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Delete Activity";
                        objAudit.targetTable = "activities";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Ok();
                    }
                    else
                    {

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIES";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Activity.";
                        objAudit.targetTable = "activities";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

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
