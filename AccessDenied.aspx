<%@ Page Title="Access denied" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="AccessDenied.aspx.vb" Inherits="AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <br />
        <h1 class="text-danger"><%: Title %>.</h1>
        <br />
        <h4 class="text-info">You don't have permission to access this page.</h4>
        <br /><br />
    </div>
</asp:Content>

