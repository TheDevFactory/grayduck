using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class contactModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; } = Guid.Empty;

        public Guid accountId { get; set; } = Guid.Empty;

        public string externalId { get; set; } = ""; //Allows subscriptions to be linked to external systems, CRM and others

        [Required(ErrorMessage = "Firstname is required.")]
        public string firstName { get; set; } = "";
        public string middelName { get; set; } = "";

        [Required(ErrorMessage = "Lastname is required.")]
        public string lastName { get; set; } = "";
        public string saluation { get; set; } = "";
        public string gender { get; set; } = "";

        public string email { get; set; } = "";
        public string mobile { get; set; } = "";
        public string officePhone { get; set; } = "";
        public string homePhone { get; set; } = "";


        public Boolean doNotCall { get; set; } = false;
        public string doNotCallReason { get; set; } = "";
        public DateTime doNotCallAt { get; set; } = DateTime.Now;

        public Boolean unsubscribed { get; set; } = false;
        public string unsubscribedReason { get; set; } = "";
        public DateTime unsubscribedAt { get; set; } = DateTime.Now;


        public string companyName { get; set; } = "";
        public string companyTitle { get; set; } = "";
        public string companyRole { get; set; } = "";
        public string companyDepartment { get; set; } = "";
        public string companyIndustry { get; set; } = "";
        public string companySICCode { get; set; } = "";
        public string companyWebsite { get; set; } = "";


        public Int16 contactScore { get; set; } = 0;
        public string contactSource { get; set; } = "";
        public string contactStatus { get; set; } = "";
        public Int16 contactRating { get; set; } = 0;

        //Array of Custom Field Data
        public contactcustomfieldModel[] customFields { get; set; }


        public Boolean isActive { get; set; } = true;

        public Guid assignedToId { get; set; } = Guid.Empty; //User Id that the record is assigned to

        public Guid createdById { get; set; } = Guid.Empty; //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class contactDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE contact(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    accountID UUID,
        //    externalId VARCHAR,
        //    firstName VARCHAR NOT NULL,
        //    middleName VARCHAR,
        //    lastName VARCHAR,
        //    salutation VARCHAR,
        //    gender VARCHAR,
        //    email VARCHAR,
        //    mobile VARCHAR,
        //    officePhone VARCHAR,
        //    homePhone VARCHAR,
        //    doNotCall BOOLEAN,
        //    doNotCallReason VARCHAR,
        //    doNotCallAt TIMESTAMP,
        //    unsubscribed BOOLEAN,
        //    unsubscribedReason VARCHAR,
        //    unsubscribedAt TIMESTAMP,
        //    companyName VARCHAR,
        //    companyTitle VARCHAR,
        //    companyRole VARCHAR,
        //    companyDepartment VARCHAR,
        //    companyIndustry VARCHAR,
        //    companySICCode VARCHAR,
        //    companyWebsite VARCHAR,
        //    contactScore BIGINT,
        //    contactSource VARCHAR,
        //    contactStatus VARCHAR,
        //    contactRating BIGINT,
        //    isActive BOOLEAN,
        //    assignedToId UUID,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);


    }

}
