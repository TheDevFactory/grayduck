using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using GrayDuck.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrayDuckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class fileController : ControllerBase
    {
        readonly IConfiguration objConfiguration;

        //Initialize Configuration so we can use it as needed
        public fileController(IConfiguration _configuration)
        {
            objConfiguration = _configuration;
        }

        // GET: api/file/5
        [HttpGet("{fileid}", Name = "DownloadFile")]
        public async Task<IActionResult> Get(string fileid, string objectType)
        {
            try
            {

                //objectType - Would be contact, account
                //fileid - Would be per type. Contact or Account

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

                            if (objectType == null) {
                                return BadRequest("Error, Unknown object type.");
                            } else {
                                switch (objectType.ToUpper())
                                {
                                    case "CONTACT":
                                        if (objAuthSecurity.contactView == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    case "ACCOUNT":
                                        if (objAuthSecurity.accountView == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    default:
                                        throw new InvalidOperationException("Unknown Object Type");
                                }
                            }

                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {
                            DataTable _dataTable;

                            switch (objectType.ToUpper())
                            {
                                case "CONTACT":

                                    // Get the data
                                    contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);
                                    _dataTable = await _contactFileService.Download(Guid.Parse(fileid));

                                    if (_dataTable == null) {

                                        return BadRequest("Error, Unknown download request."); 

                                    } else {
                                        string strfilename = "";
                                        string strfiletype = "";
                                        string strcontactid = "";
                                        byte[] b = null;

                                        //Loop over all rows (will only have one)
                                        foreach (DataRow row in _dataTable.Rows)
                                        {
                                            strfilename = row["filename"].ToString();
                                            strfiletype = row["filetype"].ToString();
                                            strcontactid = row["contactid"].ToString();

                                            //byte[] b = null;
                                            b = (byte[])row["filedata"];                                        
                                        }

                                        if (strfilename == "") {
                                            return BadRequest("Error, Unknown download request.");

                                        } else {
                                            return File(b, strfiletype, strfilename);
                                        }
                                    }                                  

                                case "ACCOUNT":

                                    // Get the data
                                    contactFileService _accountFileService = new contactFileService(objConfiguration, objAuthIdentity);
                                    _dataTable = await _accountFileService.Download(Guid.Parse(fileid));

                                    if (_dataTable == null)
                                    {

                                        return BadRequest("Error, Unknown download request.");

                                    }
                                    else
                                    {
                                        string strfilename = "";
                                        string strfiletype = "";
                                        string straccountid = "";
                                        byte[] b = null;

                                        //Loop over all rows (will only have one)
                                        foreach (DataRow row in _dataTable.Rows)
                                        {
                                            strfilename = row["filename"].ToString();
                                            strfiletype = row["filetype"].ToString();
                                            straccountid = row["accountid"].ToString();

                                            //byte[] b = null;
                                            b = (byte[])row["filedata"];
                                        }

                                        if (strfilename == "")
                                        {
                                            return BadRequest("Error, Unknown download request.");

                                        }
                                        else
                                        {
                                            return File(b, strfiletype, strfilename);

                                        }
                                    }

                                default:
                                    return BadRequest("Unknown Object Type");
                                    
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


        // DELETE: api/file/5
        [HttpDelete("{fileid}")]
        public async Task<IActionResult> Delete(string fileid, string objectType)
        {
            try
            {
                //objectType - Would be contact, account
                //fileid - Would be per type. Contact or Account

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

                            if (objectType == null)
                            {
                                return BadRequest("Error, Unknown object type.");
                            }
                            else
                            {

                                switch (objectType.ToUpper())
                                {
                                    case "CONTACT":
                                        if (objAuthSecurity.contactDelete == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    case "ACCOUNT":
                                        if (objAuthSecurity.accountDelete == true)
                                        {
                                            //Mark that it is Authorized
                                            boolAuthorized = true;
                                        }
                                        break;

                                    default:
                                        throw new InvalidOperationException("Unknown Object Type");
                                }

                            }

                        }

                        //If we are Authorized to Perform this action lets proceed
                        if (boolAuthorized == true)
                        {

                            switch (objectType.ToUpper())
                            {
                                case "CONTACT":
                                    contactFileService _contactFileService = new contactFileService(objConfiguration, objAuthIdentity);

                                    _contactFileService.Remove(Guid.Empty,Guid.Parse(fileid));

                                    return Ok();
                                    
                                case "ACCOUNT":
                                    return BadRequest("Unknown Object Type");
                                    
                                default:
                                    return BadRequest("Unknown Object Type");
                                    
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
