<%@ Page Title="Document Type" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="DocumentType.aspx.vb" Inherits="DocumentType" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.4.1.js"></script>
    <script>
        $(document).ready(function () {
            $('#BtnAdd').click(function () {
                $("#MainContent_TxtNewDesc").val("");
                $("#DivEdit").hide();
                $("#DivAdd").show(); 
                $('#MainContent_BtnSave').show();
                $('#MainContent_BtnUpdate').hide();
            });

            $('#BtnEdit').click(function () {

                var KeyVal = $("#MainContent_txtCode").val();

                if (KeyVal == "") {
                    alert("Please select item to edit.");
                    $("#myModalEdit").modal('hide');
                } else {
                    $("#myModalEdit").modal('show');
                    $("#DivEdit").show();
                    $("#DivAdd").hide();
                    $('#MainContent_BtnUpdate').show();
                    $('#MainContent_BtnSave').hide(); 
                } 
            });

            $('#BtnDelete').click(function () {

                var KeyVal = $("#MainContent_txtCode").val(); 

                if (KeyVal == "") {
                    alert("Please select item to delete.");
                    $("#myModalDel").modal('hide');
                } else {
                    $("#myModalDel").modal('show'); 
                }

            });
            
        });
        
    </script>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <br />
        <h3>Document Type</h3>
        <br />

        <div class="row">
            <div class="col-sm-6">
                <div class="btn-group">
                    <button type="button" name="BtnAdd" id="BtnAdd" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModalEdit">Add</button>
                    <button type="button" id="BtnEdit" class="btn btn-primary btn-sm" >Edit</button>
                    <button type="button" id="BtnDelete" class="btn btn-primary btn-sm" >Delete</button>
                    <asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Refresh"></asp:Button> 
                </div>
            </div>
            <div class="col-sm-6"></div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <asp:GridView ID="tblDocumentType" runat="server" AllowPaging="True" BorderColor="#CCCCCC"
                        AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px" Font-Size="Small"
                        CssClass="table table-sm table-bordered table-striped" PageSize="10" EnableModelValidation="True"
                        SelectedRowStyle-CssClass="btn btn-info">

                        <Columns>
                            <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <HeaderStyle Width="30px"></HeaderStyle>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Type_Cd" HeaderText="System Code">
                                <HeaderStyle Width="200px"></HeaderStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="Descr" HeaderText="Description">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>

                            <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="Select">
                                <ItemStyle CssClass="labelC" Width="40px" />
                                <ControlStyle CssClass="btn btn-primary btn-sm" />
                            </asp:CommandField>

                        </Columns>
                        <SelectedRowStyle CssClass="bg-warning" />
                        <PagerStyle Font-Size="8pt" />
                        <HeaderStyle CssClass="titleBar" />
                        <RowStyle CssClass="odd" />
                        <AlternatingRowStyle CssClass="even" />
                    </asp:GridView>
                </div>

            </div>
        </div>
    </div>


    <div id="myModalEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Document Type Reference</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div> 
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div id="DivEdit">
                                System Code:
                                <input type="text" runat="server" id="txtCode" name="txtCode" class="form-control col-12" value="" placeholder="System Generated"/>                      
                                Description:
                                <input type="text" runat="server" id="txtDescr" name="txtDescr" class="form-control col-12 border border-danger" value="" />                      
                            </div>
                            <div id="DivAdd">
                                Description:
                                <input type="text" runat="server" id="TxtNewDesc" name="TxtNewDesc" class="form-control col-12 border border-danger" value="" placeholder="Enter your description" />   
                            </div>
                            
                        </div>
                    </div> 
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSave" value="Save" Class="btn btn-sm btn-primary" runat="server" /> 
                    <input type="button" id="BtnUpdate" value="Submit" Class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="myModalDel" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">Document Type Reference</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div> 
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <h5 class="text-dark">Are you sure you want to delete the selected item?</h5>
                        </div>
                    </div> 
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnDelAction" value="YES" Class="btn btn-sm btn-primary" runat="server" /> 
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>


