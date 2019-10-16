// ! Your application

//BUSCA -------------------------------------------------------------

// auto complete
function CampoData(campo, soMask) {
    $(document).ready(function () {
        var c = $(campo);
        if (c == null) return;
        if (soMask != true)
            c.datepicker($.datepicker.regional['pt-br']);
        else c.mask('99/99/9999');
    });
}

function CampoHorario(campo) {
    $(document).ready(function () {
        $(campo).mask('99:99');
    });
}

function CampoTelefone(campo) {
    $(document).ready(function () {
        $(campo).mask('(99)9999-9999?9');
    });

}

function CampoRamal(campo) {
    $(document).ready(function () {
        $(campo).mask('9999');
    });
}

function CampoRG(campo) {
    $(document).ready(function () {
        $(campo).mask('99.999.999-9');
    });
}

function CampoPlaca(campo) {
    $(document).ready(function () {
        $(campo).mask('***-9999');
    });
}

function CampoCPF(campo) {
    ///	<summary>
    ///     &#10;Mascara de CPF
    ///	</summary>
    ///	<param name="elems" type="Elements">
    ///     &#10;Componente
    ///	</param>
    $(document).ready(function () {
        $(campo).mask('999.999.999-99');
    });
}


function CampoCNPJ(campo) {
    ///	<summary>
    ///     &#10;Mascara de CNPJ
    ///	</summary>
    ///	<param name="elems" type="Elements">
    ///     &#10;Componente
    ///	</param>
    $(document).ready(function () {
        $(campo).mask('99.999.999/9999-99');
        $(campo).blur(function () {
            validarCNPJ(this);
        });
    });
}

function validarCNPJ(campo) {
    var CNPJ = campo.value;

    if (!CNPJ) { return false; }

    erro = new String;
    if (CNPJ == "00.000.000/0000-00") { erro += "CNPJ inválido\n\n"; }
    CNPJ = CNPJ.replace(".", "");
    CNPJ = CNPJ.replace(".", "");
    CNPJ = CNPJ.replace("-", "");
    CNPJ = CNPJ.replace("/", "");

    var a = [];
    var b = new Number;
    var c = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    for (i = 0; i < 12; i++) {
        a[i] = CNPJ.charAt(i);
        b += a[i] * c[i + 1];
    }
    if ((x = b % 11) < 2) {
        a[12] = 0
    } else {
        a[12] = 11 - x
    }
    b = 0;
    for (y = 0; y < 13; y++) {
        b += (a[y] * c[y]);
    }
    if ((x = b % 11) < 2) {
        a[13] = 0;
    } else {
        a[13] = 11 - x;
    }
    if ((CNPJ.charAt(12) != a[12]) || (CNPJ.charAt(13) != a[13])) { erro += "Dígito verificador com problema!"; }

    if (erro.length > 0) {
        alert("CNPJ inválido.");
        campo.focus();
    }
}

function CampoCEP(campo) {
    $(document).ready(function () {
        $(campo).mask('99999-999');
    });
}

function CampoDia(campo) {
    $(document).ready(function () {
        $(campo).mask('99');
        $(campo).blur(function () {
            if ($(this).val() != "") {
                if ($(this).val() > 31 || $(this).val() < 1) {
                    alert("Digite um valor entre 1 e 31");
                    $(this).val("");
                }
            }
        });
    });
}

function CampoMes(campo) {
    $(document).ready(function () {
        $(campo).mask('99');
        $(campo).blur(function () {
            if ($(this).val() != "") {
                if ($(this).val() > 12 || $(this).val() < 1) {
                    alert("Digite um valor entre 1 e 12");
                    $(this).val("");
                }
            }
        });
    });
}

function CampoAno(campo) {
    $(document).ready(function () {
        $(campo).mask('9999');
        $(campo).blur(function () {
            if ($(this).val() != "") {
                if ($(this).val() > 9999 || $(this).val() < 1900) {
                    alert("Digite um valor entre 1900 e 9999");
                    $(this).val("");
                }
            }
        });
    });
}

function CampoSenha(campo) {
    $(document).ready(function () {
        $(campo).get(0).type = "password";
    });
}

function CampoSexo(campo) {
    $(document).ready(function () {
        var id = $(campo).attr("id");

        var valor = $(campo).val();

        var del = $(campo);

        $(campo).parent().append("<select id='" + id + "' name='" + id + "'></select>");

        del.remove();

        $("#" + id).append("<option value='M'>Masculino</option>");
        $("#" + id).append("<option value='F'>Feminino</option>");
        $("#" + id).append("<option>Selecione o Sexo</option>");

        if (valor != null) {
            if (valor == "Masculino")
                $("#" + id).children("option").first().attr("selected", "selected");
            else
                $("#" + id).children("option").last().attr("selected", "selected");
        }
    });
}

function CampoSexoAmbos(campo) {
    $(document).ready(function () {
        var id = $(campo).attr("id");

        var valor = $(campo).val();

        var del = $(campo);

        $(campo).parent().append("<select id='" + id + "' name='" + id + "'></select>");

        del.remove();
        $("#" + id).append("<option value='A'>Ambos</option>");
        $("#" + id).append("<option value='M'>Masculino</option>");
        $("#" + id).append("<option value='F'>Feminino</option>");
        $("#" + id).append("<option>Selecione o Sexo</option>");

        if (valor != null) {
            if (valor == "Masculino")
                $("#" + id).children("option").first().attr("selected", "selected");
            else
                $("#" + id).children("option").last().attr("selected", "selected");
        }
    });
}

function CampoDecimal(campo, prefix) {
    if (prefix == true) {
        prefixo = 'R$ ';
    } else {
        prefixo = '';
    }
    $(campo).priceFormat({
        prefix: prefixo,
        centsSeparator: ',',
        thousandsSeparator: '',
        clearPrefix: true
    });
}

function CampoDecimal2(campo, prefix) {
    $(document).ready(function() {
        if (prefix == true) {
            prefixo = 'R$ ';
        } else {
            prefixo = '';
        }
        $(campo).priceFormat({
            prefix: prefixo,
            centsSeparator: ',',
            thousandsSeparator: '.',
            clearPrefix: true
        });
    });
}

function CampoRecebeData(campo) {
    var recebedata = new Date();
    if (recebedata.getDate() < 10) {
        var dia = '0' + recebedata.getDate();
    } else { dia = recebedata.getDate(); }

    if (recebedata.getMonth() < 10) {
        var mes = '0' + (recebedata.getMonth() + 1);
    } else { var mes = recebedata.getMonth() + 1; }


    var dataFormatada = dia + '/' + mes + '/' + recebedata.getFullYear();

    $(campo).val(dataFormatada);
}

function numeroParaMoeda(n, c, d, t) {
    c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? "," : d, t = t == undefined ? "." : t, s = n < 0 ? "-" : "", i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
}

function moedaParaNumero(valor) {
    return isNaN(valor) == false ? parseFloat(valor) : parseFloat(valor.replace("R$", "").replace(".", "").replace(",", "."));
}

function convertDataJson(e) {
    if (e != null) {
        var value = new Date(parseInt(e.replace(/(^.*\()|([+-].*$)/g, '')));
        var dataj = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
        return dataj;
    }
    return "";
}


/* Portuguese initialisation for the jQuery UI date picker plugin. */
jQuery(function ($) {
    $.datepicker.regional['pt-BR'] = {
        closeText: 'Fechar',
        prevText: '<Anterior',
        nextText: 'Seguinte',
        currentText: 'Hoje',
        monthNames: ['Janeiro', 'Fevereiro', 'Mar&ccedil;o', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
        monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
    'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
        dayNames: ['Domingo', 'Segunda-feira', 'Ter&ccedil;a-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'S&aacute;bado'],
        dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'S&aacute;b'],
        dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'S&aacute;b'],
        weekHeader: 'Sem',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['pt-BR']);
});

jQuery(function() {
    $.validator.addMethod('date', function(value, element) {
        if (this.optional(element)) {
            return true;
        }

        var ok = true;
        try {
            $.datepicker.parseDate('dd/mm/yy', value);
        } catch(err) {
            ok = false;
        }
        return ok;
    });
});

(function ($, window, document, undefined) {


    // Put all JS you need additionally to script.js here

})(jQuery, this, document);