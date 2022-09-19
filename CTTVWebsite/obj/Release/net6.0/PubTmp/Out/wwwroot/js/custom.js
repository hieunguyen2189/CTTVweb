$("#idInputForm").submit(function (e) {
    var re = new RegExp("^[a-zA-Z0-9_$\\-\\n\\s]*$");
    $('.classInputBox').each(function (i, obj) {
        var classterm = $(obj).val();
        if (re.test(classterm)) {
            console.log("Valid");
        } else {
            console.log("Invalid");
            $("#checkValidInput").show();
            $("#checkValidInput").delay(2000).fadeOut("slow");
            e.preventDefault();
        }
    });


    $('.classValidSelect').each(function (i, obj) {
        var classterm = $(obj).val();
        if (classterm == '' || classterm == null) {
            console.log("Invalid");
            $("#checkValidSelect").html("Please choose 'from date' and 'to date'");
            $("#checkValidSelect").show();
            $("#checkValidSelect").delay(2000).fadeOut("slow");
            e.preventDefault();
        }
    });


    
});
