//------------- Login.js -------------//
$(document).ready(function() {

	//validate login form 
    $("#login-form").validate({
        ignore: null,
        ignore: 'input[type="hidden"]',
        errorPlacement: function( error, element ) {
            var place = element.closest('.input-group');
            if (!place.get(0)) {
                place = element;
            }
            if (place.get(0).type === 'checkbox') {
                place = element.parent();
            }
            if (error.text() !== '') {
                place.after(error);
            }
        },
        errorClass: 'help-block',
        rules: {
            usuario: {
                required: true
            },
            password: {
                required: true,
                minlength: 5
            }
        },
        messages: {
            password: {
                required: "Digite sua senha",
                minlength: "Minimo de 5 caracteres"
            },
            usuario: {
                required: "Digite o seu Usuário"
            },
        },
        highlight: function( label ) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function( label ) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        }
    });
	
});