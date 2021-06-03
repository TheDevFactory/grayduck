using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GrayDuck.Models;
using Microsoft.Extensions.Configuration;

namespace GrayDuck.Services
{
    public class subscriptionSecurityService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;
        public subscriptionSecurityService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }


        public async Task<DataTable> Get()
        {
            DataTable _dataTable = new DataTable();


            try
            {

                databaseSettings _databaseManager = new databaseSettings(objConfiguration);

                //Check AuthIdentity Security to only allow working with data that the user is allowed to
                if (objAuthIdentity == null)
                {
                    //Nothing to do
                }
                else
                {
                    if (objAuthIdentity.authSecurity.Count == 0)
                    {
                        ////No Security found for User to Auth / Give permissions, return empty JSon
                    }
                    else
                    {

                        //Build Allowed Subscription List
                        string strSubscriptionIds = "";
                        foreach (subscriptionSecurityModel row in objAuthIdentity.authSecurity)
                        {
                            if (strSubscriptionIds == "")
                            {
                                strSubscriptionIds = "'" + row.subscriptionId.ToString() + "'";
                            }
                            else
                            {
                                strSubscriptionIds += ",'" + row.subscriptionId.ToString() + "'";
                            }
                        }

                        //Read the database
                        _dataTable = await _databaseManager.executeReader("SELECT * FROM public.subscriptionsecurity WHERE subscriptionid IN (" + strSubscriptionIds + ") ORDER BY name ASC;");

                    }
                }

                //return strJSon;
                return _dataTable;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
