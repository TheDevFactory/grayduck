using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace GrayDuck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class subscriptionSecurityController : ControllerBase
    {

        readonly IConfiguration objConfiguration;

        //Initialize Configuration so we can use it as needed
        public subscriptionSecurityController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
        }

        // GET: api/subscriptionSecurity
        [HttpGet]
        public async Task <ActionResult<List<subscriptionSecurityModel>>> Get()
        {
            try
            {

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
                            if (objAuthSecurity.subscriptionUserView == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            subscriptionSecurityService _subscriptionSecurityService = new subscriptionSecurityService(objConfiguration, objAuthIdentity);

                            return Ok(await _subscriptionSecurityService.Get());
                        }
                        else
                        {
                            return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                        }
                    }
                    else
                    {

                        return BadRequest();
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // GET: api/subscriptionSecurity/5
        [HttpGet("{id}", Name = "GetSubscriptionSecurity")]
        public async Task <ActionResult<subscriptionSecurityModel>> Get(Guid id)
        {
            try
            {

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
                        subscriptionSecurityService _subscriptionSecurityService = new subscriptionSecurityService(objConfiguration, objAuthIdentity);

                        //return Ok(_subscriptionSecurityService.Get(id));
                        return Ok();
                    }
                    else
                    {

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // POST: api/subscriptionSecurity
        [HttpPost]
        public async Task <ActionResult<subscriptionSecurityModel>> Post(subscriptionSecurityModel objItem)
        {
            try
            {

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
                        subscriptionSecurityService _subscriptionSecurityService = new subscriptionSecurityService(objConfiguration, objAuthIdentity);

                        //objItem = await _subscriptionSecurityService.Create(objItem);

                        ////****************************************************************************
                        //objAudit.objectType = "SUBSCRIPTIONSECURITY";
                        //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                        //objAudit.objectId = objItem.Id;
                        //objAudit.environmentUserId = objAuthIdentity.authUserId;
                        //objAudit.environmentUserToken = objAuthIdentity.authToken;
                        //objAudit.targetResult = "200";
                        //objAudit.targetNewValue = "Create Subscription Security";
                        //objAudit.targetTable = "subscriptionsecurity";

                        //await objAuditLog.Create(objAudit);
                        ////****************************************************************************

                        //return CreatedAtRoute("GetSubscriptionSecurity", new { id = objItem.Id.ToString() }, objItem);
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // PUT: api/subscriptionSecurity/5
        [HttpPut("{id}")]
        public async Task <IActionResult> Put(Guid id, subscriptionSecurityModel objItem)
        {
            try
            {

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
                        subscriptionSecurityService _subscriptionSecurityService = new subscriptionSecurityService(objConfiguration, objAuthIdentity);

                        //_subscriptionSecurityService.Update(id, objItem);

                        return Ok();
                    }
                    else
                    {

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // DELETE: api/subscriptionSecurity/5
        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(Guid id)
        {
            try
            {

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
                        subscriptionSecurityService _subscriptionSecurityService = new subscriptionSecurityService(objConfiguration, objAuthIdentity);

                        //_subscriptionSecurityService.Remove(id);

                        return Ok();
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
