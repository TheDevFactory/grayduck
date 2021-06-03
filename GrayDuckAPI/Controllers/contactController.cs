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

        //Initialize Configuration so we can use it as needed
        public contactController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
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

                            return Ok(await _contactService.Search(firstname, lastname,mobile,email,customfield1,customfieldvalue1,customfield2,customfieldvalue2));
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
                            return Ok(await _contactService.Get(limit,orderby));
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

                            return Ok(await _contactService.Get(id));
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
        ///  Creates new contact for subscription.
        /// </summary>
        [HttpPost]
        public async Task <ActionResult<contactModel>> Post(contactModel objItem)
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
                                return CreatedAtRoute("GetContact", new { id = objItem.Id.ToString() }, objItem);
                            }

                        } else {

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

                            return Ok(await _contactFileService.Get(id));
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

                                return CreatedAtRoute("GetContactFiless", new { id = id }, objItem);

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

                            return Ok(id);
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
