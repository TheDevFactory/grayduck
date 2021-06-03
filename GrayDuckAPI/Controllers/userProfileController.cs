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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrayDuckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userProfileController : ControllerBase
    {

        readonly IConfiguration objConfiguration;

        //Initialize Configuration so we can use it as needed
        public userProfileController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
        }       


        /// <summary>
        /// Retrieves user list for subscription.
        /// </summary>
        /// <remarks>
        /// Note that a set of paramaters can be passed to limit the results.
        /// Results should be limited to a maximum of 10,000 records.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<List<subscriptionUserModel>>> Get()
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
                            subscriptionUserService _subscriptionUserService = new subscriptionUserService(objConfiguration, objAuthIdentity);

                            return Ok(await _subscriptionUserService.Get());
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


        /// <summary>
        /// Retrieves users profile by id. 
        /// </summary>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// The logged on user has permission to view their own profile regardless of system permissions.
        /// </remarks>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "GetUserProfile")]
        public async Task<ActionResult<subscriptionUserModel>> Get(Guid id)
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

                        //Check if it is the user is requesting to view their own profile... if they are give them accesss.
                        if (id == objAuthIdentity.authUserId)
                        {
                            //Mark that it is Authorized
                            boolAuthorized = true;
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            subscriptionUserService _subscriptionUserService = new subscriptionUserService(objConfiguration, objAuthIdentity);

                            return Ok(await _subscriptionUserService.Get(id));
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


        /// <summary>
        ///  Creates new user profile for subscription.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<subscriptionUserModel>> Post(subscriptionUserModel objItem)
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
                            if (objAuthSecurity.subscriptionId == objAuthIdentity.authSecurity[0].subscriptionId && objAuthSecurity.subscriptionUserAdd == true)
                            {
                                //Mark that it is Authorized
                                boolAuthorized = true;
                                break;
                            }
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            subscriptionUserService _userService = new subscriptionUserService(objConfiguration, objAuthIdentity);
                            objItem = await _userService.Create(objItem);

                            if (objItem.Id == Guid.Empty) {

                                //Unable to create the user.. error message inside of teh API Token
                                return BadRequest(objItem.apiToken);

                            } else {

                                return CreatedAtRoute("GetUserProfile", new { id = objItem.Id.ToString() }, objItem);

                            }
                                 
                        }
                        else
                        {

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
        /// Updates existing user profile by id.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> Put(string action, string userid, string currentToken)
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
                            //if (objAuthSecurity.subscriptionId == objItem.subscriptionId && objAuthSecurity.contactEdit == true)
                            //{
                            //    //Mark that it is Authorized
                            //    boolAuthorized = true;
                            //    break;
                            //}

                            boolAuthorized = true;
                            break;
                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            //Perform Specific Action.. allowing us to use the PUT command for other things
                            if (action == "generatetoken") {

                                subscriptionUserAccessService _subscriptionUserAccessService = new subscriptionUserAccessService(objConfiguration, objAuthIdentity);
                                subscriptionSignupModel objsubscriptionSignupModel;

                                objsubscriptionSignupModel = await _subscriptionUserAccessService.GenerateToken(userid, currentToken);

                                if (objsubscriptionSignupModel.signupCode == "Success")
                                {

                                    return Ok(objsubscriptionSignupModel);
                                }
                                else
                                {
                                    return BadRequest(objsubscriptionSignupModel);
                                }

                            } else {
                                return BadRequest("Error, Invalid action supplied.");
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


        /// <summary>
        /// Deletes existing user profile by id.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Note that the id is a GUID and not an integer.
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
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
