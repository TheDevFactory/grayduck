using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuck.Models
{
    public class subscriptionSecurityModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subscription is required.")]
        public Guid subscriptionId { get; set; } //Set Custom Security Permission Set per subscription // We pre-load some default groups

        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; } //Permission set name


        public Guid createdById { get; set; } //User Id that created this security model       
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;


        //Create Permissions that can be stored and applied to users
        public Boolean subscriptionManageAll { get; set; } = false; //Gives the ability to access any and all subscriptions (super user)

        public Boolean serverlogView { get; set; } = false;
        public Boolean serverlogDelete { get; set; } = false;

        public Boolean subscriptionView { get; set; } = false;
        public Boolean subscriptionAdd { get; set; } = false;
        public Boolean subscriptionEdit { get; set; } = false;
        public Boolean subscriptionDelete { get; set; } = false;


        public Boolean subscriptionUserView { get; set; } = false;
        public Boolean subscriptionUserAdd { get; set; } = false;
        public Boolean subscriptionUserEdit { get; set; } = false;
        public Boolean subscriptionUserDelete { get; set; } = false;


        public Boolean accountView { get; set; } = false;
        public Boolean accountAdd { get; set; } = false;
        public Boolean accountEdit { get; set; } = false;
        public Boolean accountDelete { get; set; } = false;


        public Boolean contactView { get; set; } = false;
        public Boolean contactAdd { get; set; } = false;
        public Boolean contactEdit { get; set; } = false;
        public Boolean contactDelete { get; set; } = false;


        public Boolean activityView { get; set; } = false;
        public Boolean activityAdd { get; set; } = false;
        public Boolean activityEdit { get; set; } = false;
        public Boolean activityDelete { get; set; } = false;


        public Boolean activityTypesView { get; set; } = false;
        public Boolean activityTypesAdd { get; set; } = false;
        public Boolean activityTypesEdit { get; set; } = false;
        public Boolean activityTypesDelete { get; set; } = false;


        public Boolean scoringView { get; set; } = false;
        public Boolean scoringAdd { get; set; } = false;
        public Boolean scoringEdit { get; set; } = false;
        public Boolean scoringDelete { get; set; } = false;

        public Boolean customFieldsView { get; set; } = false;
        public Boolean customFieldsAdd { get; set; } = false;
        public Boolean customFieldsEdit { get; set; } = false;
        public Boolean customFieldsDelete { get; set; } = false;


        public Boolean auditlogView { get; set; } = false;
        public Boolean auditlogDelete { get; set; } = false;

    }


    public class subscriptionSecurityDatabase
    {

        //Create and Update the database tables for this model.
        //First we ensure the extnsion for uuid processing exists in the database.. then we create the table.


        //CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

        //CREATE TABLE subscriptionsecurity(
        //    Id UUID DEFAULT uuid_generate_v4 (),
        //    subscriptionId UUID,
        //    name VARCHAR NOT NULL,
        //    createdById UUID,
        //    createdAt TIMESTAMP,
        //    updatedAt TIMESTAMP,
        //    subscriptionManageAll BOOLEAN,
        //    serverlogView BOOLEAN,
        //    serverlogDelete BOOLEAN,
        //    subscriptionView BOOLEAN,
        //    subscriptionAdd BOOLEAN,
        //    subscriptionEdit BOOLEAN,
        //    subscriptionDelete BOOLEAN,
        //    subscriptionUserView BOOLEAN,
        //    subscriptionUserAdd BOOLEAN,
        //    subscriptionUserEdit BOOLEAN,
        //    subscriptionUserDelete BOOLEAN,
        //    accountView BOOLEAN,
        //    accountAdd BOOLEAN,
        //    accountEdit BOOLEAN,
        //    accountDelete BOOLEAN,
        //    contactView BOOLEAN,
        //    contactAdd BOOLEAN,
        //    contactEdit BOOLEAN,
        //    contactDelete BOOLEAN,
        //    activityView BOOLEAN,
        //    activityAdd BOOLEAN,
        //    activityEdit BOOLEAN,
        //    activityDelete BOOLEAN,
        //    activityTypesView BOOLEAN,
        //    activityTypesAdd BOOLEAN,
        //    activityTypesEdit BOOLEAN,
        //    activityTypesDelete BOOLEAN,
        //    scoringView BOOLEAN,
        //    scoringAdd BOOLEAN,
        //    scoringEdit BOOLEAN,
        //    scoringDelete BOOLEAN,
        //    customFieldsView BOOLEAN,
        //    customFieldsAdd BOOLEAN,
        //    customFieldsEdit BOOLEAN,
        //    customFieldsDelete BOOLEAN,
        //    auditlogView BOOLEAN,
        //    auditlogDelete BOOLEAN,
        //    PRIMARY KEY (Id)
        //);

    }

}
