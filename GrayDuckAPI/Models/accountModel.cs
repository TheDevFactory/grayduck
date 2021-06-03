using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class accountModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")] 
        public Guid subscriptionId { get; set; }


        public string externalId { get; set; } //Allows subscriptions to be linked to external systems, CRM and others

        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }
        public string description { get; set; }
        public string accountStatus { get; set; }


        public string industry { get; set; }
        public string sicCode { get; set; }
        public string website { get; set; }
        public Int32 annualRevenue { get; set; }
        public Int16 accountScore { get; set; }

        public string billingStreet { get; set; }
        public string billingCity { get; set; }
        public string billingState { get; set; }
        public string billingCountry { get; set; }
        public string billingPostCode { get; set; }


        //Array of Custom Field Data
        public accountcustomfieldModel[] customFields { get; set; }


        public Boolean isActive { get; set; } = true;

        public Guid assignedToId { get; set; } //User Id that the record is assigned to
        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;


    }

    public class accountDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE account(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    externalId VARCHAR,
        //    name VARCHAR NOT NULL,
        //    description VARCHAR,
        //    accountStatus VARCHAR,
        //    industry VARCHAR,
        //    sicCode VARCHAR,
        //    website VARCHAR,
        //    annualRevenue MONEY,
        //    accountScore BIGINT,
        //    billingStreet VARCHAR,
        //    billingCity VARCHAR,
        //    billingState VARCHAR,
        //    billingCountry VARCHAR,
        //    billingPostCode VARCHAR,
        //    isActive BOOLEAN,
        //    assignedToId UUID,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);


    }

}
