using GrayDuck.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Services
{
    public class activitiesScoringService
    {

        readonly IConfiguration objConfiguration;
        readonly identityModel objAuthIdentity;

        public activitiesScoringService(IConfiguration _configuration, identityModel _AuthIdentity)
        {
            objConfiguration = _configuration;
            objAuthIdentity = _AuthIdentity;

            ////Create New DataAccess Object
            //databaseSettings _databaseManager = new databaseSettings(_configuration, _ServerLog);
        }



    }
}
