using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class activitiesModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public Guid typeId { get; set; }

        public Guid accountId { get; set; }
        public Guid contactId { get; set; }


        [Required(ErrorMessage = "Title is required.")]
        public string title { get; set; }
        public string description { get; set; }

        public Guid activityscoringId { get; set; }

        //Array of Custom Field Data
        public activitiescustomfieldModel[] customFields { get; set; }


        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class activitiesDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE activities(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    typeId UUID,
        //    accountId UUID,
        //    contactId UUID,
        //    activityscoringId UUID,
        //    title VARCHAR NOT NULL,
        //    description VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);


    }

    public class activityTypesModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Type name is required.")]
        public string typename { get; set; }
        public string summary { get; set; }

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class activityTypesDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extension for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE activitytypes(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    typename VARCHAR NOT NULL,
        //    summary VARCHAR,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);


    }

}
