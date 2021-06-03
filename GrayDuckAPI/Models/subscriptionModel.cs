using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class subscriptionModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }

        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; }

        //[BsonElement("Name")]
        //public string name { get; set; }

        public string externalId { get; set; } //Allows subscriptions to be linked to external systems, CRM and others       

        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; } 
        public string mobile { get; set; }
        public string officePhone { get; set; }


        public string billingStreet { get; set; }
        public string billingCity { get; set; }
        public string billingState { get; set; }
        public string billingCountry { get; set; }
        public string billingPostCode { get; set; }


        public Boolean isActive { get; set; }
        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class subscriptionDatabase 
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE subscription(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    externalId VARCHAR,
        //    name VARCHAR NOT NULL,
        //    email VARCHAR,
        //    mobile VARCHAR,
        //    officePhone VARCHAR,
        //    billingStreet VARCHAR,
        //    billingCity VARCHAR,
        //    billingState VARCHAR,
        //    billingCountry VARCHAR,
        //    billingPostCode VARCHAR,
        //    isActive BOOLEAN,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }




    //Subscription Sign Up Model used for new Subscriptions only, allows a single call with the data in it.
    public class subscriptionSignupModel {

        //Data that can easily be used by the user
        public string Id { get; set; }
        public string externalId { get; set; } //Allows subscriptions to be linked to external systems, CRM and others    

        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; } //Email also used for login, API token generated and random password generated
        public string apitoken { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }


        public string signupCode { get; set; } //Internal Error Codes / Success Codes
        public string signupMessage { get; set; } //Human readable Error Message / Success Message

    }


}
