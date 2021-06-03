using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class subscriptionUserModel
    {

        public Guid Id { get; set; }

        public subscriptionUserAccessModel[] subscriptionAccess { get; set; }

        [Required(ErrorMessage = "Email is required.")] 
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }

        public string mobile { get; set; } //Mobile number can be used for message sending and account security
        public string apiToken { get; set; } //API Token used to authenticate user via API... each user must get a unique API token


        public bool isActive { get; set; } = true;
        public bool isDeleted { get; set; } = false;

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class subscriptionUserAccessModel 
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Security Group is required.")]
        public Guid subscriptionSecurityId { get; set; }

        [Required(ErrorMessage = "Subscription User is required.")]
        public Guid subscriptionUserId { get; set; }

    }

    public class subscriptionUserDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE subscriptionUser(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    email VARCHAR NOT NULL,
        //    password VARCHAR NOT NULL,
        //    mobile VARCHAR,
        //    apiToken VARCHAR,
        //    isActive BOOLEAN,
        //    isDeleted BOOLEAN,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

    public class subscriptionUserAccessDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.

        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE subscriptionUserAccess(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    subscriptionSecurityId UUID,
        //    subscriptionUserId UUID,
        //    PRIMARY KEY (Id)
        //);

    }

}
