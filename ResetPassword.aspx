<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResetPassword.aspx.vb" Inherits="ResetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password</title>
    <script src="Scripts/jquery-3.5.1.js"></script>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script> 
        function validate() {
            if (document.getElementById("txtPwd").value == "") {
                alert("Please enter your current password!");
                document.getElementById("txtPwd").focus()
                return false
            }
            else if (document.getElementById("txtNew").value == "") {
                alert("Please enter your new password!");
                document.getElementById("txtNew").focus()
                return false
            }
            else if (document.getElementById("txtConfirm").value == "") {
                alert("Please reenter your new password!");
                document.getElementById("txtConfirm").focus()
                return false
            }
        }
         
        $(document).ready(function () { 

            $("#cmdClear").on('click', function () {
                $('#length').removeClass('valid').addClass('invalid');
                $('#letter').removeClass('valid').addClass('invalid');
                $('#capital').removeClass('valid').addClass('invalid');
                $('#number').removeClass('valid').addClass('invalid');
            })

            $("#txtNew").keyup(function () {
                // keyup code here 
                //alert("lance test"); 

                var pswd = $(this).val();
                var PasswordPass = 0

                //validate the length
                if (pswd.length < 8) {
                    $('#length').removeClass('valid').addClass('invalid');
                } else {
                    $('#length').removeClass('invalid').addClass('valid');
                    PasswordPass = PasswordPass + 1;
                }


                if (pswd.match(/[a-z]/)) {
                    $('#letter').removeClass('invalid').addClass('valid');
                    PasswordPass = PasswordPass + 1;
                } else {
                    $('#letter').removeClass('valid').addClass('invalid');
                }


                //validate capital letter
                if (pswd.match(/[A-Z]/)) {
                    $('#capital').removeClass('invalid').addClass('valid');
                    PasswordPass = PasswordPass + 1;
                } else {
                    $('#capital').removeClass('valid').addClass('invalid');
                }

                //validate number/[!@#$%\^&*(){}[\]<>?/|\-]/
                if (pswd.match(/\d/)) {
                    $('#number').removeClass('invalid').addClass('valid');
                    PasswordPass = PasswordPass + 1;
                } else {
                    $('#number').removeClass('valid').addClass('invalid');
                }

                //validate number
                if (pswd.match(/[!@#$%\^&*(){}[\]<>?/|\-]/)) {
                    $('#specialChar').removeClass('invalid').addClass('valid');
                    PasswordPass = PasswordPass + 1;
                } else {
                    $('#specialChar').removeClass('valid').addClass('invalid');
                }


                if (PasswordPass == 5) {
                    $('#btnSubmit').removeAttr('disabled');
                } else {
                    $('#btnSubmit').attr('disabled', 'disabled');
                }

            });
        })
         
    </script>
     
    <style>
        .invalid {
            background: url(images/no.png) no-repeat 0 50%;
            padding-left: 0px;
            line-height: 24px;
            font-size:14px;
            color: #ec3f41;
        }

        .valid {
            background: url(images/yes.png) no-repeat 0 50%;
            padding-left: 0px;
            line-height: 24px;
            font-size:14px;
            color: #3a7d34;
        }

        .disableButton {
            background: #f2f2f2;
            height: 22px;
            cursor: pointer;
            padding: 3px;
            padding-left: 7px;
            padding-right: 7px;
            text-shadow: 1px 1px 1px #e5e5e5;
            outline: none;
            font-weight: bold;
            color: #cccccc;
        }

        .xBtn {
            background-color: #0a60cb;
            color: #fff;
        }

        .bg {
            background-color: #eeeded;
        }

        .divPad {
            margin-bottom: 15px
        }
    </style>
</head>
<body class="bg">
    <form id="form1" runat="server">
        <div class="container-sm">
            <div class="row">
                <div class="col-sm-4"></div>
                <div class="col-sm-4">
                    <br />
                    <br /> 
                    <br />
                    <ul class="list-group Exp">
                        <li class="list-group-item active" style="font-size: 16px; padding: 10px; margin: 0px">Account Recovery</li>
                        <li class="list-group-item" style="font-size: 12px;">
                            <div class="row">
                                <div class="col-sm-12 divPad">
                                    <h4><small class="text-info">Password must meet the following requirements:</small></h4>
                                    <ul>
                                        <li id="letter" class="invalid"><strong>1 letter</strong></li>
                                        <li id="capital" class="invalid"><strong>1 capital letter</strong></li>
                                        <li id="number" class="invalid"><strong>1 number</strong></li>
                                        <li id="specialChar" class="invalid"><strong>1 special characters</strong></li>
                                        <li id="length" class="invalid"><strong>8 characters</strong></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="row">

                                <div class="col-sm-12 divPad">
                                    <input class="form-control form-control-sm col-12" type="password" id="txtNew" runat="server" placeholder="New Password" autocomplete="off" />
                                </div>

                                <div class="col-sm-12 divPad">
                                    <input class="form-control form-control-sm col-12" type="password" id="txtConfirm" runat="server" placeholder="Re-type new password" autocomplete="off" />
                                </div>


                                <div class="col-sm-12"> 
                                    <asp:Button ID="btnSubmit" CssClass="btn btn-primary btn-sm btn-block" Enabled="false" runat="server" Text="Submit" />
                                </div>
                            </div>
                            <br />
                        </li>
                    </ul>
                </div>
                <div class="col-sm-4"></div>
            </div>
        </div>
    </form>
</body>
</html>
