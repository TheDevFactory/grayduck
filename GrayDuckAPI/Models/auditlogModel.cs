using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class auditlogModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")] 
        public Guid subscriptionId { get; set; }


        [Required(ErrorMessage = "Object Type is required.")]
        public string objectType { get; set; } //Example: CONTACT, ACCOUNT, USER

        [Required(ErrorMessage = "Object Id is required.")]
        public Guid objectId { get; set; } //Example: Record Id that this record entry is for (contactid, accountid and so on)


        [Required(ErrorMessage = "Event Type is required.")]
        public string eventType { get; set; } //Example: Custom name for Searching and filtering. Example: Delete File

        [Required(ErrorMessage = "User is required.")]
        public Guid environmentUserId { get; set; } //Example: User GUID
        public Guid environmentUserToken { get; set; } //Example: User Token used at the time of the action
        public string environmentMachine { get; set; } //Example: DELL_NICO or IP Address
        public string environmentDomain { get; set; } //Example: MICROSOFT\DOMAIN
        public string environmentCulture { get; set; } //Example: en-GB, en-US


        [Required(ErrorMessage = "Target API is required.")]
        public string targetAPI { get; set; } //Example: api/contact

        [Required(ErrorMessage = "Target Action is required.")]
        public string targetAction { get; set; } //Example: DeleteContact(id)
        public string targetMethod { get; set; } //Example: GET, POST, DELETE
        public string targetTable { get; set; } //Example: contact
        public string targetResult { get; set; } //Example: 200, 400, 502
        public string targetNewValue { get; set; } //Example: Json object with what we received from the Client call

        public DateTime createdAt { get; set; } = DateTime.Now;

    }

    public class auditlogDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE auditlog(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    objectId UUID,
        //    objectType VARCHAR,
        //    eventType VARCHAR,
        //    environmentUserId UUID,
        //    environmentUserToken UUID,
        //    environmentMachine VARCHAR,
        //    environmentDomain VARCHAR,
        //    environmentCulture VARCHAR,
        //    targetAPI VARCHAR,
        //    targetAction VARCHAR,
        //    targetMethod VARCHAR,
        //    targetTable VARCHAR,
        //    targetResult VARCHAR,
        //    targetNewValue VARCHAR,
        //    createdAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

}
