<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ExpiredToken.aspx.vb" Inherits="ExpiredToken" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-3.5.1.js"></script>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <br />
            <h1 class="text-danger">Token Expired</h1>
            <br />
            <div class="col-sm-12 text-left">
                <h4 class="text-info">Your token is already expired. Click the link below to recover your account.</h4>
            </div>
            <div class="col-sm-12 text-left">
                <asp:LinkButton ID="LinkForgotAcct" CssClass="btn-link" runat="server">Account recovery</asp:LinkButton>
            </div>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
