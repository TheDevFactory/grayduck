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
    public class activitiesScoringController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public activitiesScoringController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }

        // GET: api/activitiesScoring
        [HttpGet]
        public async Task <ActionResult<List<activitiesScoringModel>>> Get()
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIESSCORING";
                objAudit.eventType = "GetActivitiesScoring";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetActivitiesScoring";
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
                        activitiesScoringService _activitiesScoringService = new activitiesScoringService(objConfiguration, objAuthIdentity);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Empty;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Return Activities Scoring";
                        objAudit.targetTable = "activitiesscoring";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        //return Ok(await _activitiesScoringService.Get());
                        return Ok();
                        
                    }
                    else
                    {

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Empty;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Activities Scoring.";
                        objAudit.targetTable = "activitiesscoring";

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


        // GET: api/activitiesScoring/5
        [HttpGet("{id}", Name = "GetActivitiesScoring")]
        public async Task <ActionResult<activitiesScoringModel>> Get(string id)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIESSCORING";
                objAudit.eventType = "GetActivitiesScoring";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "GetActivitiesScoring";
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
                        activitiesScoringService _activitiesScoringService = new activitiesScoringService(objConfiguration, objAuthIdentity);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Return Activities Scoring";
                        objAudit.targetTable = "activitiesscoring";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        //return Ok(_activitiesScoringService.Get(id));
                        return Ok();

                    }
                    else
                    {

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Empty;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Return Activities Scoring.";
                        objAudit.targetTable = "activitiesscoring";

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


        // POST: api/activitiesScoring
        [HttpPost]
        public async Task <ActionResult<activitiesScoringModel>> Post(activitiesScoringModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIESSCORING";
                objAudit.eventType = "CreateActivitiesScoring";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "CreateActivitiesScoring";
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
                        activitiesScoringService _activitiesScoringService = new activitiesScoringService(objConfiguration, objAuthIdentity);

                       //await _activitiesScoringService.Create(objItem);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = objItem.Id;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Create Activities Scoring";
                        objAudit.targetTable = "activitiesscoring";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return CreatedAtRoute("GetActivitiesScoring", new { id = objItem.Id.ToString() }, objItem);
                    }
                    else
                    {

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Empty;
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Create Activities Scoring.";
                        objAudit.targetTable = "activitiesscoring";

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


        // PUT: api/activitiesScoring/5
        [HttpPut("{id}")]
        public async Task <IActionResult> Put(string id, activitiesScoringModel objItem)
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "ACTIVITIESSCORING";
                objAudit.eventType = "UpdateActivitiesScoring";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "UpdateActivitiesScoring";
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
                        activitiesScoringService _activitiesScoringService = new activitiesScoringService(objConfiguration, objAuthIdentity);

                        //_activitiesScoringService.Update(id, objItem);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Update Activities Scoring";
                        objAudit.targetTable = "activitiesscoring";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Ok();
                    }
                    else
                    {
                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Update Activities Scoring.";
                        objAudit.targetTable = "activitiesscoring";

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


        // DELETE: api/activitiesScoring/5
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
                objAudit.objectType = "ACTIVITIESSCORING";
                objAudit.eventType = "DeleteActivitiesScoring";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "DeleteActivitiesScoring";
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
                        activitiesScoringService _activitiesScoringService = new activitiesScoringService(objConfiguration, objAuthIdentity);

                        //_activitiesScoringService.Remove(id);

                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "200";
                        objAudit.targetNewValue = "Delete Activities Scoring";
                        objAudit.targetTable = "activitiesscoring";

                        await objAuditLog.Create(objAudit);
                        //****************************************************************************

                        return Ok();
                    }
                    else
                    {
                        //****************************************************************************
                        objAudit.objectType = "ACTIVITIESSCORING";
                        objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        objAudit.objectId = Guid.Parse(id);
                        objAudit.environmentUserId = objAuthIdentity.authUserId;
                        objAudit.environmentUserToken = objAuthIdentity.authToken;
                        objAudit.targetResult = "401";
                        objAudit.targetNewValue = "Error, Access denied to perform this action. Confirm your subcritpionId and access rights. Delete Activities Scoring.";
                        objAudit.targetTable = "activitiesscoring";

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

    }
}
