using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;

namespace GrayDuck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class subscriptionController : ControllerBase
    {

        readonly IConfiguration objConfiguration;
                
        //Initialize Configuration so we can use it as needed
        public subscriptionController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
        }

        // GET: api/subscription
        [HttpGet]
        public async Task<ActionResult<subscriptionModel>> Get()
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
                        subscriptionService objSubscriptionService = new subscriptionService(objConfiguration, objAuthIdentity);

                        return Ok(await objSubscriptionService.Get());
                    }
                    else
                    {

                        return Unauthorized("Error, Access denied to perform this action. Confirm your subcritpionId and access rights.");
                    }

                }


            }
            catch (Exception ex) {
                return BadRequest("Error, " + ex.Message);
            }
        }


        // GET: api/subscription/5
        [HttpGet("{id}", Name = "GetSubscription")]
        public async Task<ActionResult<subscriptionModel>> Get(Guid id)
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
                        subscriptionService objSubscriptionService = new subscriptionService(objConfiguration, objAuthIdentity);

                        return Ok(await objSubscriptionService.Get(id));
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


        // POST: api/subscription
        [HttpPost]
        public async Task <ActionResult<subscriptionSignupModel>> Post(subscriptionModel objItem)
        {
            //try
            //{


            //}
            //catch (Exception ex)
            //{
            //    return BadRequest("Error, " + ex.Message);
            //}

            subscriptionService objSubscriptionService = new subscriptionService(objConfiguration, null);
            subscriptionSignupModel objNewSubscription;

            //objSubscriptionService.Create(objItem);
            //return CreatedAtRoute("GetSubscription", new { id = objItem.Id.ToString() }, objItem);

            //Custom SignUp Process - For new Subscriptions only
            if (objItem.name == "string")
            {
                return BadRequest("Error, Invalid subscritpion name provided.");
            }
            else
            {
                objNewSubscription = await objSubscriptionService.SignUp(objItem); //Dedicated Signup Function for new subscriptions

                return objNewSubscription;
            }

        }


        // PUT: api/subscription/5
        [HttpPut("{id}")]
        public async Task <IActionResult> Put(string id, subscriptionModel objItem)
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
                        //subscriptionService objSubscriptionService = new subscriptionService(objConfiguration, objAuthIdentity);

                        //return Ok(objSubscriptionService.Get());

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


        // DELETE: api/subscription/5
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
                        //Check Security - for the posted subscriptionId does the user have access to do this function
                        //Since the Auth has been confirmed we know there has to be a security group (at least 1 so we will not get a NULL / Nothing Object)
                        bool boolAuthorized = false;
                        foreach (subscriptionSecurityModel objAuthSecurity in objAuthIdentity.authSecurity)
                        {
                            if (objAuthSecurity.subscriptionDelete == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            subscriptionService _subscriptionService = new subscriptionService(objConfiguration, objAuthIdentity);

                            long rowsAffected = 0;

                            rowsAffected = await _subscriptionService.Remove(id);

                            if (rowsAffected == 0)
                            {
                                return BadRequest("Error, 0 records deleted.");
                            }
                            else
                            {
                                return Ok();
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

    }
}
