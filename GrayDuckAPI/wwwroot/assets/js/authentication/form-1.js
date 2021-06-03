

var togglePassword = document.getElementById("toggle-password");

if (togglePassword) {
    togglePassword.addEventListener('click', function () {
        var x = document.getElementById("password");
        if (x.type === "password") {
            x.type = "text";
        } else {
            x.type = "password";
        }
    });
}



jQuery(document).ready(function ($) {

    $('#btnLoginAuth').on('click', function (e) {

        //Prevents form from submitting
        e.preventDefault();

        //Set Variables
        var varUSER = $('#email').val();
        var varPASS = $('#password').val();

        //Clear any existing data first
        document.getElementById('spanAuthResponse').innerHTML = '<div class="alert alert-info mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Info!</strong> Connecting to server for Authentication... </div>';

        //Execute Auth Against API (Auth Token)
        jQuery.ajax({
            url: '/api/token?email=' + varUSER1 + '&password=' + varPASS1,
            async: true,
            dataType: 'json',
            type: 'POST',
            timeout: 4000 // sets timeout to 4 seconds
        }).done(function (data, textStatus, jqXHR) {

            var jsonData = jqXHR.responseJSON;
            console.log(jsonData);

            //Store Object in Local Storage (Cache)
            localStorage.setItem('grayduckAUTH', JSON.stringify(jsonData));

            // Handle Success
            document.getElementById('spanAuthResponse').innerHTML = '<div class="alert alert-success mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Success!</strong> GrayDuck Auth succeeded. ' + jsonData.authResult + ' </div>';

            //Redirect to Dashboard Page
            window.location.replace("dashboard.html");

        }).fail(function (jqXHR, textStatus, errorThrown) {

            var jsonData = jqXHR.responseJSON;
   
            if (jqXHR.responseJSON == null | jqXHR.responseJSON === 'undefined') {
                document.getElementById('spanAuthResponse').innerHTML = '<div class="alert alert-danger mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Error!</strong> ' + textStatus + ' </div>';
            } else {
                // Handle Failure
                document.getElementById('spanAuthResponse').innerHTML = '<div class="alert alert-danger mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Error!</strong> ' + jsonData.authResult + ' </div>';

            //No redirect needed
            }

            //Store Object in Local Storage (Cache) -Clear all data since it was incorrect
            localStorage.removeItem('grayduckAUTH');

        });
               

    })

    $('#btnRegister').on('click', function (e) {

        //Prevents form from submitting
        e.preventDefault();

        //Clear any existing data first
        document.getElementById('spanRegistrationResponse').innerHTML = '<div class="alert alert-info mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Info!</strong> Connecting to server for Subscription SignUp... </div>';

        //Set Variables
        var varBUSINESS = $('#business').val();
        var varEMAIL = $('#email').val();
        var varMOBILE = $('#mobile').val();

        //JSon Object to Send Over
        var varJSONData = {
            name: varBUSINESS,
            email: varEMAIL,
            mobile: varMOBILE,
            officePhone: "",
            billingStreet: "",
            billingCity: "",
            billingState: "",
            billingCountry: "",
            billingPostCode: "",
            isActive: true,
        }

        //Execute Auth Against API (Auth Token)
        jQuery.ajax({
            url: '/api/subscription',
            async: true,
            type: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(varJSONData),
        }).done(function (data, textStatus, jqXHR) {

            var jsonData = jqXHR.responseJSON;
            console.log(jsonData);

            ////Store Object in Local Storage (Cache)
            //localStorage.setItem('grayduckAUTH', JSON.stringify(jsonData));

            // Handle Success / Error in 200 Request
            if (jsonData.signupCode === 'Success') {
                document.getElementById('spanRegistrationResponse').innerHTML = '<div class="alert alert-success mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Success!</strong> ' + jsonData.signupMessage + ' <br><br>You can now login using your email address<br> <strong>' + jsonData.email + '</strong>  <br>and the password<br> <strong>' + jsonData.password + '</strong> <br></div>';
            } else {
                document.getElementById('spanRegistrationResponse').innerHTML = '<div class="alert alert-danger mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Error!</strong> ' + jsonData.signupMessage + ' </div>';
            }

            ////Redirect to Dashboard Page
            //window.location.replace("dashboard.html");

        }).fail(function (jqXHR, textStatus, errorThrown) {

            var jsonData = jqXHR.responseJSON;
            console.log(jsonData);

            ////Store Object in Local Storage (Cache) -Clear all data since it was incorrect
            //localStorage.removeItem('grayduckAUTH');

            //// Handle Failure
            document.getElementById('spanRegistrationResponse').innerHTML = '<div class="alert alert-danger mb-4" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x close" data-dismiss="alert"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg></button> <strong>Error!</strong> ' + jsonData.signupMessage + ' </div>';

            ////No redirect needed

        });

    })

});