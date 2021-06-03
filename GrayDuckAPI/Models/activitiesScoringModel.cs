using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class activitiesScoringModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; }

        [Required(ErrorMessage = "Activity Type is required.")]
        public string activityType { get; set; }
        public string description { get; set; }

        public Int16 score { get; set; }

        public Guid createdById { get; set; } //User Id that created this security model
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    public class activitiesScoringDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE activitiesScoring(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    activityType VARCHAR NOT NULL,
        //    description VARCHAR NOT NULL,
        //    score BIGINT,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    PRIMARY KEY (Id)
        //);


    }


}
