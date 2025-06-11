<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TRV_ver.aspx.cs" Inherits="TRV_ver" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title>Программа подбора регулирующего клапана - Поверочный расчет</title>

</head>
<body>
    <div class="container">
        <form id="form1" runat="server" novalidate="novalidate">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-12">
                    <nav class="nav nav-tabs">
                        <a class="nav-link" href="/TRV.aspx">Стандартный расчет</a>
                        <a class="nav-link active" aria-current="page" href="#">Поверочный расчет</a>
                    </nav>
                </div>
                <div class="col-xs-12 col">
                    <div class="col border border-non-top">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label1" runat="server" Text="Тип клапана:"></asp:Label>
                                <br />
                                <asp:RadioButton ID="tvRadioButton1" runat="server" Text="2-х ходовой" AutoPostBack="True" OnCheckedChanged="RadioButton1_CheckedChanged" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="tvRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="tvRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>TRV</asp:ListItem>
                                        <asp:ListItem>TRV-T</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="tvRadioButton2" runat="server" Text="3-х ходовой" AutoPostBack="True" OnCheckedChanged="tvRadioButton2_CheckedChanged" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="tvRadioButtonList2" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="tvRadioButtonList2_SelectedIndexChanged">
                                        <asp:ListItem>TRV-3</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div>
                                    <asp:CustomValidator ID="tvCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="CustomValidator" OnServerValidate="tvCustomValidator1_ServerValidate"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label2" runat="server" Text="Номинальное давление PN, бар:"></asp:Label>
                                <div class="col-12 row">
                                    <asp:RadioButtonList ID="pnRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="pnRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>16</asp:ListItem>
                                        <asp:ListItem>25</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div>
                                    <asp:CustomValidator ID="pnCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="pnCustomValidator" OnServerValidate="pnCustomValidator1_ServerValidate" ControlToValidate="pnRadioButtonList1" ValidateEmptyText="True"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label3" runat="server" Text="Диаметр и пропускная способность клапана:"></asp:Label>
                                <div id="aaPanel0" class="col-12" runat="server">
                                    <div class="row">
                                        <div class="col-6 col-md-2">
                                            <asp:Label ID="dnLabel" runat="server" Text="DN, мм:"></asp:Label>
                                        </div>
                                        <div class="col-6 col-md-2">
                                            <asp:DropDownList ID="dnDropDownList1" runat="server" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="dnDropDownList1_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-6 col-md-2">
                                            <asp:Label ID="kvsLabel" runat="server" Text="Kvs, м3/ч:"></asp:Label>
                                        </div>
                                        <div class="col-6 col-md-2">
                                            <asp:DropDownList ID="kvsDropDownList1" runat="server" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="kvsDropDownList1_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div>
                                    <asp:CustomValidator ID="dnKvsCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="dnKvsCustomValidator" OnServerValidate="dnKvsCustomValidator1_ServerValidate"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel1" runat="server">
                                    <asp:Label ID="Label4" runat="server" Text="Рабочая среда:"></asp:Label>
                                    <div class="row">
                                        <div class="col-6 col-md-3">
                                            <asp:RadioButtonList ID="wsRadioButtonList1" runat="server"
                                                AutoPostBack="True" required="required" OnSelectedIndexChanged="wsRadioButtonList1_SelectedIndexChanged">
                                                <asp:ListItem>Вода</asp:ListItem>
                                                <asp:ListItem>Этиленгликоль</asp:ListItem>
                                                <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                                <asp:ListItem Enabled="False">Водяной пар</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <div class="col-12 sub-col">
                                                <asp:RadioButtonList ID="lpvRadioButtonList1" runat="server"
                                                    AutoPostBack="True"
                                                    Enabled="False">
                                                    <asp:ListItem>Перегретый</asp:ListItem>
                                                    <asp:ListItem>Насыщеный</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="col-6 col-md-6">
                                            <div class="row">
                                                <br></br>
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox1" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label6" runat="server" Text="% (от 5% до 65%)"></asp:Label>
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox2" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label7" runat="server"
                                                    Text="&#8451; (от 0&#8451; до 150&#8451;)">
                                                </asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div>
                                        <asp:CustomValidator ID="wsCustomValidator1" runat="server"
                                            ControlToValidate="wsRadioButtonList1" Display="Dynamic"
                                            EnableClientScript="False" ErrorMessage="CustomValidator"
                                            ForeColor="Red"
                                            SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="wsCustomValidator1_ServerValidate"></asp:CustomValidator>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel2" runat="server" visible="false">
                                    <div class="col border">
                                        <asp:Label ID="Label11" runat="server"
                                            Text="Потери давления на регулируемом участке (без учета регулирующего клапана):">
                                        </asp:Label>

                                        <div class="col row">
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label10" runat="server" Text="&#916;Ppy' = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lpvTextBox21" runat="server" Enabled="False" type="number" CssClass="textbox-non-right-radius"
                                                        required="required" TextMode="Number"></asp:TextBox>

                                                    <asp:DropDownList ID="lpvDropDownList21" runat="server" AutoPostBack="True" CssClass="dropdown-non-left-radius"
                                                        Enabled="False" OnSelectedIndexChanged="lpvDropDownList21_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 col-md-2">
                                                <asp:HyperLink ID="HyperLink1" CssClass="btn-link-pdf" Target="_new"
                                                    NavigateUrl="/Content/data/calcTRV.pdf" runat="server">Определение
                                                потерь давления на регулируемом участке</asp:HyperLink>
                                            </div>

                                        </div>
                                        <div>
                                            <asp:CustomValidator ID="lpvCustomValidator1" runat="server" ControlToValidate="lpvTextBox21" Display="Dynamic"
                                                ErrorMessage="CustomValidator" ForeColor="Red" SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="lpvCustomValidator1_ServerValidate"></asp:CustomValidator>
                                        </div>

                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel3" runat="server" visible="false">
                                    <div class="col border">
                                        <asp:Label ID="Label12" runat="server"
                                            Text="Расчёт регулирующего клапана на кавитацию:"></asp:Label>
                                        <div class="col row">
                                            <div class="col-12">
                                                <asp:Label ID="Label26" runat="server" Text="Давление перед клапаном:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="calcvTextBox1" runat="server" CssClass="textbox-non-right-radius"
                                                        Enabled="False" type="number" required="required" TextMode="Number">
                                                    </asp:TextBox>
                                                    <asp:DropDownList ID="calcvDropDownList1" runat="server" AutoPostBack="True" CssClass="dropdown-non-left-radius"
                                                        Enabled="False" OnSelectedIndexChanged="calcvDropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 row">
                                                <asp:CustomValidator ID="calcvCustomValidator1" runat="server"
                                                    ErrorMessage="CustomValidator" ControlToValidate="calcvDropDownList1"
                                                    Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                                    ValidateEmptyText="True" OnServerValidate="calcvCustomValidator1_ServerValidate"></asp:CustomValidator>
                                            </div>
                                            <div class="col-12">
                                                <asp:Label ID="Label22" runat="server"
                                                    Text="Максимальная температура теплоносителя через клапан:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="calcvTextBox2" runat="server" Enabled="False"
                                                        type="number" required="required" TextMode="Number"
                                                        CausesValidation="True">
                                                    </asp:TextBox>&nbsp;
                                                        <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-12 row">
                                                <asp:CustomValidator ID="calcvCustomValidator2" runat="server"
                                                    ErrorMessage="CustomValidator" ControlToValidate="calcvTextBox2"
                                                    Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                                    ValidateEmptyText="True" OnServerValidate="calcvCustomValidator2_ServerValidate"></asp:CustomValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel4" runat="server" visible="false">
                                    <div class="col border">

                                        <asp:Label ID="Label17" runat="server"
                                            Text="Давление пара перед клапаном (изб.):">
                                        </asp:Label>
                                        <div class="col row">
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label18" runat="server" Text="P'1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lpv5TextBox1" runat="server" TextMode="Number" CssClass="textbox-non-right-radius"
                                                        Enabled="False"></asp:TextBox>
                                                    <asp:DropDownList ID="lpv5DropDownList1" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lpv5DropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 row">
                                                <asp:CustomValidator ID="CustomValidator1" runat="server" Display="Dynamic"
                                                    ControlToValidate="lpv5DropDownList1" ErrorMessage="CustomValidator"
                                                    ForeColor="Red" SetFocusOnError="True" OnServerValidate="CustomValidator1_ServerValidate">
                                                </asp:CustomValidator>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-12">
                                                    <asp:Label ID="Label57" runat="server" Text="Температура пара через клапан:">
                                                    </asp:Label>
                                                </div>
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label58" runat="server" Text="T1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lpv5TextBox3"
                                                        runat="server" Enabled="False" type="number" TextMode="Number"
                                                        CausesValidation="True">
                                                    </asp:TextBox>&nbsp;
                                            <asp:Label ID="Label59" runat="server" Text=" &#8451;"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-12 row">
                                                <asp:CustomValidator ID="CustomValidator3" runat="server"
                                                    ControlToValidate="lpv5TextBox3" Display="Dynamic"
                                                    EnableClientScript="False" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="CustomValidator3_ServerValidate"></asp:CustomValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <div runat="server" id="aaPanel5" visible="false">
                                    <div class="col border">
                                        <asp:Label ID="Label16" runat="server" Text="Расход через клапан:"></asp:Label>
                                        <div class="flow-radio-label col row">
                                            <div>
                                                <asp:RadioButton ID="fvRadioButton1" runat="server"
                                                    Text="Задать максимальную величину расхода через клапан:"
                                                    AutoPostBack="True" Checked="True" />
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label28" runat="server" Text="Gкл = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="fvTextBox1" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="fvDropDownList1" runat="server" AutoPostBack="True" CssClass="dropdown-non-left-radius"
                                                        Enabled="False" OnSelectedIndexChanged="fvDropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>м³/ч</asp:ListItem>
                                                        <asp:ListItem>л/с</asp:ListItem>
                                                        <asp:ListItem>л/мин</asp:ListItem>
                                                        <asp:ListItem>л/ч</asp:ListItem>
                                                        <asp:ListItem>кг/с</asp:ListItem>
                                                        <asp:ListItem>кг/ч</asp:ListItem>
                                                        <asp:ListItem>т/ч</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 row">

                                                <asp:CustomValidator ID="fvCustomValidator1" runat="server" Display="Dynamic"
                                                    ControlToValidate="fvDropDownList1" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="fvCustomValidator1_ServerValidate"></asp:CustomValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="col-12">
                    <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="LabelError" runat="server" Font-Bold="True" Font-Size="Medium"
                                Font-Strikeout="False" ForeColor="Red"></asp:Label>
                            <asp:Button ID="trvCalc" runat="server" type="submit" Text="Рассчитать"
                                Width="100%" OnClick="trvCalc_Click" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-12">
                    <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                        <ContentTemplate>
                            <div id="resultPanel" runat="server">
                                <asp:Label ID="Label52" runat="server" Enabled="False" Text="Результаты расчёта"
                                    Visible="False" Font-Size="Medium" Font-Bold="True"></asp:Label>

                                <div class="col non-padding">
                                    <asp:Label ID="ws2ResultLabel" runat="server" Text="Label" Visible="False">
                                    </asp:Label>
                                </div>

                                <div class="col non-padding">
                                    <asp:Label ID="maxt2ResultLabel" runat="server" Text="maxt2ResultLabel"
                                        Visible="False">
                                    </asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="maxp2ResultLabel" runat="server" Text="Label" Visible="False">
                                    </asp:Label>
                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="labelOptyV" runat="server" Visible="False"></asp:Label>
                                </div>


                                <div class="table-responsive-lg" onclick="ShowBTN()">
                                    <asp:GridView ID="GridView2" CssClass="table table-result trv-ver" runat="server"
                                        Font-Size="X-Small"
                                        AutoGenerateSelectButton="True" OnSelectedIndexChanged="GridView2_SelectedIndexChanged" Style="left: 0px; top: 0px">
                                        <RowStyle Font-Size="Small" />
                                        <SelectedRowStyle BackColor="#ff7d00" Font-Bold="False" ForeColor="White" />
                                    </asp:GridView>

                                </div>
                                <div class="col non-padding">
                                    <asp:Label ID="Label53" runat="server" CssClass="show-btn" Text="Объект:"
                                        Visible="False">
                                    </asp:Label>

                                    <asp:TextBox ID="objTextBox1" runat="server" Enabled="False" Visible="False"
                                        CssClass="obj-field">
                                    </asp:TextBox>
                                </div>
                            </div>
                            <div class="col non-padding padding-top-bottom">
                                <asp:Button ID="trvSave" runat="server" Text="Сохранить в PDF" Visible="False"
                                    CssClass="btn btn-primary" OnClick="trvSave_Click" />

                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="trvSave" />
                        </Triggers>
                    </asp:UpdatePanel>

                </div>
                <div class="col-12">
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                        <ContentTemplate>
                            
                        </ContentTemplate>
                        
                    </asp:UpdatePanel>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
