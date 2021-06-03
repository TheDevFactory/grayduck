using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class accountFileModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Account is required.")]
        public Guid accountId { get; set; }

        [Required(ErrorMessage = "File Name is required.")]
        public string fileName { get; set; }

        [Required(ErrorMessage = "File Type is required.")]
        public string fileType { get; set; }

        [Required(ErrorMessage = "File Size is required.")]
        public long fileSize { get; set; }

        public string fileTag { get; set; } //Custom Tag Values - UPPER CASE Only

        public byte[] fileData { get; set; } //Stores the RAW file data in Bytes

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }


    public class accountFileDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extension for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE accountfile(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    accountId UUID,
        //    fileName VARCHAR NOT NULL,
        //    fileType VARCHAR NOT NULL,
        //    fileSize VARCHAR NOT NULL,
        //    fileTag VARCHAR,
        //    fileData BYTEA,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);

    }

}
