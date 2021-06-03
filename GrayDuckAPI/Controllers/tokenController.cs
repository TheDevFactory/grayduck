using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrayDuck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tokenController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
        auditlogService objAuditLog;
        auditlogModel objAudit = new auditlogModel();

        //Initialize Configuration so we can use it as needed
        public tokenController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
            objAuditLog = new auditlogService(objConfiguration, null);
        }


        // POST api/<tokenController>
        [HttpPost]
        public async Task<IActionResult> PostAsync(string email, string password)
        {

            try
            {
                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "USER";
                objAudit.eventType = "AuthenticateUser (email,password)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "AuthenticateUser";
                objAudit.targetMethod = HttpContext.Request.Method;
                objAudit.targetTable = "subscriptionuser";
                //objAudit.targetResult = "200";
                //objAudit.targetNewValue = "";
                //*************************************************************************************


                //New Auth Object - Perform Authentication (email and password)
                subscriptionUserService objUserAuthentication = new subscriptionUserService(objConfiguration, null);
                identityModel objAuthIdentity;

                objAuthIdentity = await objUserAuthentication.AuthenticateUser(email, password);

                // Normally Identity handles sign in, but you can do it directly
                if (objAuthIdentity.authMessage == "SUCCESS")
                {
                    
                    objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                    objAudit.objectId = objAuthIdentity.authUserId;
                    objAudit.environmentUserId = objAuthIdentity.authUserId;
                    objAudit.environmentUserToken = objAuthIdentity.authToken;
                    objAudit.targetResult = "200";
                    objAudit.targetNewValue = objAuthIdentity.authMessage;
                    await objAuditLog.Create(objAudit);

                    return Ok(objAuthIdentity);
                }
                else
                {
                    return Unauthorized(objAuthIdentity);
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }

        }

        // GET api/<tokenController>
        [HttpGet]
        public async Task<IActionResult> Get(string email, string password)
        {

            try
            {

                //New Auth Object - Perform Authentication (email and password)
                subscriptionUserService objUserAuthentication = new subscriptionUserService(objConfiguration, null);
                identityModel objAuthIdentity;

                objAuthIdentity = await objUserAuthentication.AuthenticateUser(email, password);

                // Normally Identity handles sign in, but you can do it directly
                if (objAuthIdentity.authMessage == "SUCCESS")
                {
                    return Ok(objAuthIdentity);
                }
                else
                {
                    return Unauthorized(objAuthIdentity);
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }

        }

        //PUT api/<tokenController>
        
        /// <summary>
        /// Put token function will generate a new token only.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that this PUT is only used to generate a new token. Updating or saving a user profile uses a POST command.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put()
        {
            try
            {

                //************** Generate Audit Log Results ************** ****************************
                //objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                //objAudit.objectId = objAuthIdentity.authUserId;
                //objAudit.environmentUserId = objAuthIdentity.authUserId;
                //objAudit.environmentUserToken = objAuthIdentity.authToken;
                objAudit.objectType = "USER";
                objAudit.eventType = "AuthenticateUser (token)";
                objAudit.environmentMachine = HttpContext.Connection?.RemoteIpAddress?.ToString();
                objAudit.environmentDomain = HttpContext.Request.Host.Value.ToString();
                objAudit.environmentCulture = CultureInfo.CurrentCulture.Name;
                objAudit.targetAPI = HttpContext.Request.Path.ToString();
                objAudit.targetAction = "AuthenticateUser";
                objAudit.targetMethod = HttpContext.Request.Method;
                objAudit.targetTable = "subscriptionuser";
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
                        //Check Security - for the posted PUT the userId is use dto generate the new Token
                        //Since the Auth has been confirmed we know there has to be a security group (at least 1 so we will not get a NULL / Nothing Object)
                        bool boolAuthorized = false;
                        foreach (subscriptionSecurityModel objAuthSecurity in objAuthIdentity.authSecurity)
                        {
                            //Mark that it is Authorized as they are in a security group. So the user is valid + has a security group
                            boolAuthorized = true;
                            break;
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            objAudit.subscriptionId = objAuthIdentity.authSecurity[0].subscriptionId;
                            objAudit.objectId = objAuthIdentity.authUserId;
                            objAudit.environmentUserId = objAuthIdentity.authUserId;
                            objAudit.environmentUserToken = objAuthIdentity.authToken;
                            objAudit.targetResult = "200";
                            objAudit.targetNewValue = objAuthIdentity.authMessage;
                            await objAuditLog.Create(objAudit);

                            return Ok();
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


        // DELETE api/<tokenController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string email, string apitoken)
        {
            try
            {

                //if (HttpContext.Request.Headers.TryGetValue("Authorization", out var objAuthHeaderValue) == false)
                //    // No Header Auth Info
                //    return Unauthorized();
                //else
                //{
                //    // ***********************************************************************************
                //    // Get the auth token
                //    //string authToken = Request.Headers.Authorization.Parameter;
                //    string authToken = HttpContext.Request.Headers["Authorization"][0];
                //    authToken = authToken.Replace("Bearer ", ""); //Clean up Bearer Auth Value

                //    //// Decode the token from BASE64
                //    //string decodedToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                //    //// Extract username and password from decoded token
                //    //string username = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                //    //string token = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                //    // ***********************************************************************************


                //    // ***********************************************************************************
                //    //New Auth Object - Perform Authentication
                //    subscriptionUserService objUserAuthentication = new subscriptionUserService(objConfiguration, null);
                //    identityModel objAuthIdentity;

                //    objAuthIdentity = objUserAuthentication.AuthenticateUser("", Guid.Parse(authToken));

                //    // Normally Identity handles sign in, but you can do it directly
                //    if (objAuthIdentity.authMessage == "SUCCESS")
                //    {
                //        subscriptionService objSubscriptionService = new subscriptionService(objConfiguration, objAuthIdentity);

                //        return Ok(objSubscriptionService.Get());
                //    }
                //    else
                //    {
                //        return BadRequest(objAuthIdentity.authResult);
                //    }

                //}

                //Return Success Message
                return Ok("Token Removed");

            }
            catch (Exception ex)
            {
                return BadRequest("Error, " + ex.Message);
            }


        }
    }
}
