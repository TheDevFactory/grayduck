using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class customfieldModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }


        [Required(ErrorMessage = "Field Name is required.")]
        public string fieldName { get; set; }

        [Required(ErrorMessage = "Field Type is required.")]
        public string fieldType { get; set; } //DateTime, Date, Email, Float, Integer, Percentage, URL, Phone, Textarea, Textshort, Boolean
        public Boolean fieldRequired { get; set; } = false;
        public int fielLenght { get; set; }
        public int fieldOrder { get; set; } = 0;
        public string fieldValue { get; set; } = "";


        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }


    public class customfieldDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE customfield(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    fieldName VARCHAR NOT NULL,
        //    fieldType VARCHAR NOT NULL,
        //    fieldRequired BOOLEAN,
        //    fieldLenght BIGINT,
        //    fieldOrder BIGINT,
        //    fieldvalue VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }



    public class contactcustomfieldModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Field Id is required.")]
        public Guid customfieldId { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public Guid contactId { get; set; }

        public string fieldName { get; set; } = "";
        public string fieldValue { get; set; } = "";

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class contactcustomfieldDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE contactcustomfield(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    customfieldId UUID,
        //    contactId UUID,
        //    fieldname VARCHAR,
        //    fieldvalue VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

    public class accountcustomfieldModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Field Id is required.")]
        public Guid customfieldId { get; set; }

        [Required(ErrorMessage = "Account is required.")]
        public Guid accountId { get; set; }

        public string fieldName { get; set; } = "";
        public string fieldValue { get; set; } = "";

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class accountcustomfieldDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE accountcustomfield(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    customfieldId UUID,
        //    accountId UUID,
        //    fieldname VARCHAR,
        //    fieldvalue VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

    public class activitiescustomfieldModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Field Id is required.")]
        public Guid customfieldId { get; set; }

        [Required(ErrorMessage = "Activity is required.")]
        public Guid activityId { get; set; }

        public string fieldName { get; set; } = "";
        public string fieldValue { get; set; } = "";

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class activitiescustomfieldDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE activitiestcustomfield(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    customfieldId UUID,
        //    activityId UUID,
        //    fieldname VARCHAR,
        //    fieldvalue VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

}
