using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class identityModel
    {

        //public Guid Id { get; set; }

        //[Required(ErrorMessage = "Subscription is required.")]
        //public Guid subscriptionId { get; set; }

        //[Required(ErrorMessage = "Email is required.")]
        //public string email { get; set; }

        //[Required(ErrorMessage = "Password is required.")]
        //public string password { get; set; }


        public string email { get; set; }
        public Guid authToken { get; set; }

        public List<subscriptionSecurityModel> authSecurity { get; set; }

        public string authMessage { get; set; }
        public string authResult { get; set; }
        public Guid authUserId { get; set; }

    }
}
