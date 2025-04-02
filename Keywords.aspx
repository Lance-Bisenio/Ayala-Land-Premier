<%@ Page Title="Job Fields Reference" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="Keywords.aspx.vb" Inherits="Keywords" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        table {
            font-size: 12px
        }
    </style>
    <script>
        function invoke() {
            <%=vScript %>
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <br />
                <h5><%: Title %></h5>
            </div>
        </div>

        <div class="row">
            <div class="col-md-4">
                <div class="btn-group">
                    <asp:Button ID="btnAdd" CssClass="btn btn-primary btn-sm" runat="server" Text="Create New" />
                    <asp:Button ID="BtnEdit" CssClass="btn btn-primary btn-sm" runat="server" Text="Edit" />
                    <asp:Button ID="btnDelete" CssClass="btn btn-primary btn-sm" runat="server" Text="Delete" />
                    <asp:Button ID="BtnRefresh" CssClass="btn btn-primary btn-sm" runat="server" Text="Reload" />
                </div>
            </div>
            <div class="col-md-8">
                <div class="btn-group">
                    <asp:Button ID="BtnCreateProcess" CssClass="btn btn-primary btn-sm" runat="server" Text="Create Process Properties" />
                    <asp:Button ID="BtnEditProcessKeys" CssClass="btn btn-primary btn-sm" runat="server" Text="Edit" />
                    <asp:Button ID="Button3" CssClass="btn btn-primary btn-sm" runat="server" Text="Delete" />
                    <asp:Button ID="BtnReloadProcess" CssClass="btn btn-primary btn-sm" runat="server" Text="Reload" />
                </div>
            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-md-4">
                <asp:GridView ID="tblkeywords" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped" AllowPaging="True" PageSize="10">
                    <RowStyle CssClass="odd" />
                    <Columns>
                        <asp:BoundField DataField="keyword_id" HeaderText="Keyword ID">
                            <ItemStyle />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descr" HeaderText="Keyword Name">
                            <ItemStyle />
                        </asp:BoundField>
                        <asp:BoundField DataField="Data_Type" HeaderText="Data Type">
                            <ItemStyle />
                        </asp:BoundField>
                        <asp:BoundField DataField="Encoded_By" HeaderText="Encoded By">
                            <ItemStyle />
                        </asp:BoundField>
                        <asp:BoundField DataField="Date_Encoded" HeaderText="Date Encoded">
                            <ItemStyle />
                        </asp:BoundField>
                        <asp:CommandField ButtonType="Button" ShowSelectButton="True">
                            <ControlStyle CssClass="btn btn-primary btn-sm" />
                            <ItemStyle CssClass="labelC" Width="80px" />
                        </asp:CommandField>
                    </Columns>
                    <SelectedRowStyle CssClass="activeBar" />
                    <HeaderStyle CssClass="titleBar" />
                    <AlternatingRowStyle CssClass="even" />
                </asp:GridView>
            </div>
            <div class="col-md-8">
                <table class="table table-bordered table-striped">
                    <tr>
                        <td>Process Description</td>
                        <td>Properties</td>
                        <td style="width: 60px"></td>
                    </tr>
                    <%=ProccessKeywords %>
                </table>
            </div>
        </div>
    </div>



    <div id="myModal" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">

                <!-- Modal Header -->
                <div class="modal-header">
                    <h4 class="modal-title">Parameters</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>

                <!-- Modal body -->
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-12">
                            <small>Description:</small>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control form-control-sm border border-danger"
                                placeholder="Enter Description"></asp:TextBox>
                        </div>
                        <div class="col-md-12">
                            <small>Data Type:</small>
                            <asp:DropDownList ID="DDLDataTypeList" runat="server" CssClass="form-control form-control-sm border border-success">
                                <asp:ListItem Value="DATE">DATE</asp:ListItem>
                                <asp:ListItem Value="STRING">STRING</asp:ListItem>
                                <asp:ListItem Value="NUMERIC">NUMERIC</asp:ListItem>
                                <asp:ListItem Value="YES/NO">YES/NO</asp:ListItem>
                                <asp:ListItem Value="TEXTAREA">TEXTAREA</asp:ListItem>
                                <asp:ListItem Value="NUMERIC">NUMERIC</asp:ListItem>
                                <asp:ListItem Value="TIME">TIME</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-12">
                            <asp:Button ID="BtnSave" CssClass="btn btn-success btn-sm" runat="server" Text="Submit" />
                            <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Close</button>
                            <br />
                            <br />
                        </div>
                    </div>
                </div>
                <br />
                <br />
            </div>
        </div>
    </div>
    <div id="ModalPrcess" class="modal fade bd-example-modal-xl" tabindex="-1" role="dialog" aria-labelledby="myExtraLargeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">

                <!-- Modal Header -->
                <div class="modal-header">
                    <h4 class="modal-title">Process Properties</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>

                <!-- Modal body -->
                <div class="modal-body">
                    <div class="row">

                        <div class="col-md-12">
                            <small>Process:</small>
                            <asp:DropDownList ID="DDLProcessList" runat="server" CssClass="form-control form-control-sm border border-success">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-12">
                            <small>Category:</small>
                            <asp:DropDownList ID="DDLCategory" runat="server" CssClass="form-control form-control-sm border border-success">
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-12">
                            <small>Select Properties:</small>
                            <div class="col-md-12 border border-danger" style="overflow: auto; height: 250px">
                                <%=KeywordsList %>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-12">
                            <asp:Button ID="BtnSaveKeys" CssClass="btn btn-success btn-sm" runat="server" Text="Submit" />
                            <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal">Close</button>
                            <br />
                        </div>
                    </div>
                </div>
                <br />
                <br />
            </div>
        </div>
    </div>
</asp:Content>

