<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="RecurringSetup.aspx.vb" Inherits="Department" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.5.1.js"></script>
    <script>
        function OperModal() {
            $('#MainContent_TxtValidFrom').val("");
            $('#MainContent_TxtValidTo').val("");
            $('#MainContent_TxtAmount').val("");
            $('#EarningsForm').modal();
        }
        $(document).ready(function () {
            $('#MainContent_BtnDelete').click(function () { 
                $('#DeleteMessage').modal();
            });

            $('#MainContent_BtnEdit').click(function () {
                $('#EarningsForm').modal();
            });

            $('#BtnUpload').click(function () {
                $('#UploadFiles').modal();
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Recurring Setup</h3>
        <br />
        <div class="row divPad" id="EmpFilter" runat="server">
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Select employee to view recurring pay element:</small>
                    <asp:DropDownList ID="CmdEmployeeList" runat="server" Width="" CssClass="form-control form-control-sm" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="col-sm-3"></div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-12">
                <div class="col-sm-6">
                    <div class="btn-group">
                        <button type="button" id="BtnAdd" class="btn btn-primary btn-sm" runat="server">Add</button>
                        <button type="button" id="BtnEdit" class="btn btn-primary btn-sm" runat="server">Edit</button>
                        <button type="button" id="BtnDelete" runat="server" class="btn btn-primary btn-sm">Delete</button>
                        <button type="button" id="BtnExport" runat="server" class="btn btn-primary btn-sm">Export to Excel</button>
                        <button type="button" id="BtnUpload" class="btn btn-primary btn-sm">Upload Pay-Element Instruction</button>
                        <asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Reload"></asp:Button>
                    </div>
                </div>
                <div class="col-sm-6"></div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <asp:GridView ID="TblRecurringList" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="Small"
                        AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                        CssClass="table table-sm table-bordered table-striped" PageSize="15" EnableModelValidation="True"
                        SelectedRowStyle-CssClass="btn btn-info">

                        <Columns>
                            <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <HeaderStyle Width="30px"></HeaderStyle>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Id" HeaderText="Sys Id" HeaderStyle-CssClass="hideGridColumn"
                                ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="50px" />

                            <asp:BoundField DataField="PayElementId" HeaderText="Element Code" HeaderStyle-CssClass="hideGridColumn"
                                ItemStyle-CssClass="hideGridColumn" ItemStyle-Width="200px" />

                            <asp:BoundField DataField="Descr" HeaderText="Description">
                                <HeaderStyle Width="200px"></HeaderStyle>
                            </asp:BoundField>

                            <asp:BoundField DataField="Amount" HeaderText="Amount">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ValidFrom" HeaderText="Valid From">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ValidTo" HeaderText="Valid To">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>
                            <asp:BoundField DataField="IsActive" HeaderText="Is Active">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CreatedBy" HeaderText="Created By">
                                <ItemStyle CssClass="labelL" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DateCreated" HeaderText="Date Created">
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
                    <%=PayDetails %>
                </div>

            </div>
        </div>
    </div>

    <div id="EarningsForm" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="LblModalTitle" runat="server" Text="Upload Master Data"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <small>Select recurring pay element:</small>
                            <asp:DropDownList ID="CmdEarningsList" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <small>Enter validity date From and To:</small>
                            <div class="form-inline">
                                <asp:TextBox ID="TxtValidFrom" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                                &nbsp; To &nbsp;
                                <asp:TextBox ID="TxtValidTo" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <small>Enter amount per month:</small>
                            <div class="form-inline">
                                <asp:TextBox ID="TxtAmount" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="0.00"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <small>Status:</small>
                            <div class="form-inline">
                                <asp:DropDownList ID="CmdEarningsStatus" runat="server" Width="" CssClass="form-control form-control-sm col-5" ></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSubmitSave" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <div id="DeleteMessage" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label1" runat="server" Text="Delete pay element"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12 text-danger">
                            Are you you want to delete this item?
                                <br />
                                If Yes, click the submit button.
                             
                           
                        </div>
                    </div>
                   
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSubmitDelete" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <div id="UploadFiles" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label2" runat="server" Text="Upload Pay Element Instructions"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="alert alert-info">
                                <strong>Note:&nbsp;</strong>Please make sure that the Pay Element Template Sheet is named “Sheet1”.
                            </div>
                        </div>

                        <div class="col-12">
                           
                            <label class="text-primary">Pay Element Recurring Template: </label>
                            <asp:FileUpload ID="TxtFileName" runat="server" CssClass="form-control-file border" />
                            <br /> 
                        </div>
                    </div>

                </div>

                <div class="modal-footer">
                    <input type="button" id="BtnSubmitFileInstruction" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

