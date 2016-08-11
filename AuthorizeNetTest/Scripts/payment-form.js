var $form = $('#payment-form');
$form.find('.subscribe').on('click', pay);


function pay(e) {

    //alert('payment!');

    /* Abort if invalid form data */
    if (!validator.form()) {
        return;
    }

    /* Visual feedback */
    $form.find('.subscribe').html('Validating <i class="fa fa-spinner fa-pulse"></i>').prop('disabled', true);

    var expiry = $form.find('#cardExpiry').payment('cardExpiryVal');

    var ccData = {
        number: $form.find('#cardNumber').val().replace(/\s/g, ''),
        cvc: $form.find('#cardCVC').val(),
        exp_month: expiry.month,
        exp_year: expiry.year
    };

    $('#cardExpiryMonth').val(ccData.exp_month);
    $('#cardExpiryYear').val(ccData.exp_year);

    $form.submit();
}

/* Fancy restrictive input formatting via jQuery.payment library*/
//$('input[name=cardNumber]').payment('formatCardNumber');
//$('input[name=cardCVC]').payment('formatCardCVC');
//$('input[name=cardExpiry').payment('formatCardExpiry');
$('input#cardNumber').payment('formatCardNumber');
$('input#cardCVC').payment('formatCardCVC');
$('input#cardExpiry').payment('formatCardExpiry');

/* Form validation using Stripe client-side validation helpers */
jQuery.validator.addMethod("cardNumber", function (value, element) {
    //return this.optional(element) || Stripe.card.validateCardNumber(value);
    return this.optional(element);
}, "Please specify a valid credit card number.");

jQuery.validator.addMethod("cardExpiry", function (value, element) {
    /* Parsing month/year uses jQuery.payment library */
    value = $.payment.cardExpiryVal(value);
    //return this.optional(element) || Stripe.card.validateExpiry(value.month, value.year);
    return this.optional(element);
}, "Invalid expiration date.");

jQuery.validator.addMethod("cardCVC", function (value, element) {
    //return this.optional(element) || Stripe.card.validateCVC(value);
    return this.optional(element);
}, "Invalid CVC.");

validator = $form.validate({
    rules: {
        cardNumber: {
            required: true,
            cardNumber: true
        },
        cardExpiry: {
            required: true,
            cardExpiry: true
        },
        cardCVC: {
            required: true,
            cardCVC: true
        }
    },
    highlight: function (element) {
        $(element).closest('.form-control').removeClass('success').addClass('error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-control').removeClass('error').addClass('success');
    },
    errorPlacement: function (error, element) {
        $(element).closest('.form-group').append(error);
    }
});

paymentFormReady = function () {

    //if ($form.find('[name=cardNumber]').hasClass("success") &&
    //    $form.find('[name=cardExpiry]').hasClass("success") &&
    //    $form.find('[name=cardCVC]').val().length > 1) {
    //    return true;
    //} else {
    //    return false;
    //}
    if ($form.find('#cardNumber').hasClass("success") &&
        $form.find('#cardExpiry').hasClass("success") &&
        $form.find('#cardCVC').val().length > 1) {
        return true;
    } else {
        return false;
    }
}


var paymentResult = {
    isSuccessful: $('#payment-is-successful').val(),
    resultCode: $('#payment-result-code').val(),
    authCode: $('#payment-auth-code').val(),
    errorCode: $('#payment-error-code').val(),
    errorMessage: $('#payment-error-message').val()
};

if (paymentResult.isSuccessful) {

    if (paymentResult.isSuccessful == "True" && paymentResult.authCode) {
        alert('Payment Successful!\r\nAUTHCODE: ' + paymentResult.authCode);
    }
    else if (paymentResult.isSuccessful == "True") {
        alert('Payment Successful!');
    }
    else {
        alert('Error!\r\nERROR CODE: ' + paymentResult.errorCode + '\r\nERROR: ' + paymentResult.errorMessage);
    }

    $form.validate().element("#cardNumber");
    $form.validate().element("#cardExpiry");
    $form.validate().element("#cardCVC");
}




$form.find('.subscribe').prop('disabled', true);
var readyInterval = setInterval(function () {
    if (paymentFormReady()) {
        $form.find('.subscribe').prop('disabled', false);
        clearInterval(readyInterval);
    }
}, 250);