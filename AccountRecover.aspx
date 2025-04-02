<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AccountRecover.aspx.vb" Inherits="Acctrecover" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Account Recovery</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <style>
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
                        <li class="list-group-item active" style="font-size: 16px; padding: 10px; margin: 0px">Reset Password</li>
                        <li class="list-group-item" style="font-size: 12px;">
                            <div class="row">
                                <div class="col-sm-12 divPad">
                                    <h5><small class="text-info">Enter your employee code or vendor code and registered Ayala Land Premier email address to recover your account.</small></h5>
                                </div>
                            </div>
                            <div class="row">

                                <div class="col-sm-12 divPad">
                                    <input class="form-control form-control-sm col-12" type="text" id="TxtEmpCode" runat="server" placeholder="Employee code or Vendor code" autocomplete="off" />
                                </div>
                                <div class="col-sm-12 divPad">
                                    <input class="form-control form-control-sm col-12" type="text" id="TxtEmail" runat="server" placeholder="Email Address" autocomplete="off" />
                                </div>


                                <div class="col-sm-12">
                                    <asp:Button ID="btnSubmit" CssClass="btn btn-primary btn-sm btn-block xBtn" runat="server" Text="Submit" />
                                    <asp:Button ID="btnBack" CssClass="btn btn-primary btn-sm btn-block xBtn" runat="server" Text="Back to Login page" />
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
