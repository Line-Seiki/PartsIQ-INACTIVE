function formValue(e){var i={};return $(e).find(".form-control").each(function(){var e=$(this).val(),a=$(this).attr("id");i[a]=e}),i}!function(n){var e=document.getElementsByClassName("needs-validation");n(".reset-form").on("click",function(){var e=n(this).attr("data-target");n(e)[0].reset(),n(e).find(".floating-label").length&&n(e).find(".floating-label").removeClass("enable-floating-label")}),Array.prototype.filter.call(e,function(a){a.addEventListener("submit",function(e){e.preventDefault(),!1===a.checkValidity()?e.stopPropagation():(e=formValue(a),alert("form is submitted.. See console logs"),console.log("form values",e)),a.classList.add("was-validated")},!1)});n("#demo-form").validate({ignore:[".select2-hidden-accessible"],focusInvalid:!1,rules:{"validation-fname1":{required:!0},"validation-email1":{required:!0,email:!0},"validation-gender1":{required:!0},"hobbies[]":{required:!0,minlength:2},"validation-select":{required:!0},"validation-message1":{required:!0,minlength:20,maxlength:100}},errorPlacement:function(e,a,i){return n("#alert-info").addClass("d-none"),n("#alert-warn").removeClass("d-none"),"hobbies[]"===n(a).attr("name")&&n(a).parent(".checkbox").siblings(".validation-error").removeClass("d-none"),"validation-gender1"===n(a).attr("name")&&n(a).parent(".radio").siblings(".validation-error").removeClass("d-none"),"validation-message1"===n(a).attr("name")&&("Please enter at least 20 characters."===n(e)[0].outerText&&n(a).siblings(".validation-error").removeClass("d-none").text("Please enter at least 20 characters."),"Please enter no more than 100 characters."===n(e)[0].outerText)?n(a).siblings(".validation-error").removeClass("d-none").text("Character limit range between 20 and 100"):n(a).siblings(".validation-error").removeClass("d-none"),!0},highlight:function(e){n(e).parents(".form-group").addClass("invalid-field")},unhighlight:function(e){n(e).parents(".form-group").removeClass("invalid-field"),"hobbies[]"===n(e).attr("name")&&n(e).parent(".checkbox").siblings(".validation-error").addClass("d-none"),("validation-gender1"===n(e).attr("name")?n(e).parent(".radio"):n(e)).siblings(".validation-error").addClass("d-none")},submitHandler:function(e){n("#alert-warn").addClass("d-none"),n("#alert-info").removeClass("d-none");var e=n(e).serializeArray(),i={};n(e).each(function(e,a){i[a.name]=a.value}),alert("form is submitted.. See console logs"),console.log("form values",i)}}),n("#signupForm").validate({focusInvalid:!1,rules:{"validation-fullname2":{required:!0},"validation-email2":{required:!0,email:!0},"validation-password3":{required:!0},"validation-cpassword4":{required:!0,equalTo:"#password3"},"validation-checkbox2[]":{required:!1}},errorPlacement:function(e,a){return n(a).siblings(".validation-error").removeClass("d-none"),"Please enter the same value again."===e[0].textContent&&n(a).siblings(".validation-error").text("Password Mismatch"),!0},highlight:function(e){n(e).parents(".form-group").addClass("invalid-field")},unhighlight:function(e){n(e).parents(".form-group").removeClass("invalid-field"),n(e).siblings(".validation-error").addClass("d-none")},submitHandler:function(e){var a=n(e).serializeArray(),i={};n(a).each(function(e,a){i[a.name]=a.value}),alert("Data has been submitted. Please see console log"),console.log("form data ===>",i),n(e)[0].reset(),n(".floating-label").removeClass("enable-floating-label")}}),n("#h-form").validate({focusInvalid:!1,rules:{"validation-email3":{required:!0,email:!0},"validation-password4":{required:!0},"validation-cpassword5":{required:!0,equalTo:"#password4"},"validation-website":{required:!0,url:!0},"validation-checkbox3[]":{required:!1}},errorPlacement:function(e,a){return n(a).siblings(".validation-error").removeClass("d-none"),"Please enter the same value again."===e[0].textContent&&n(a).siblings(".validation-error").text("Password Mismatch"),"Please enter a valid URL."===e[0].textContent&&n(a).siblings(".validation-error").text("Please enter a valid URL."),!0},highlight:function(e){n(e).parents(".form-group").addClass("invalid-field")},unhighlight:function(e){n(e).parents(".form-group").removeClass("invalid-field"),n(e).siblings(".validation-error").addClass("d-none")},submitHandler:function(e){var a=n(e).serializeArray(),i={};n(a).each(function(e,a){i[a.name]=a.value}),alert("Data has been submitted. Please see console log"),console.log("form data ===>",i),n(e)[0].reset(),n(".floating-label").removeClass("enable-floating-label")}}),n("#test").parsley().on("form:success",function(e){alert("form submitted")}).on("form:submit",function(){return!1})}(jQuery);