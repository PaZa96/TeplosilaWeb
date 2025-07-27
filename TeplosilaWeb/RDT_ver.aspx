<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RDT_ver.aspx.cs" Inherits="RDT_ver" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title>Программа подбора регулятора давления прямого действия - Поверочный расчет</title>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-12">
                    <nav class="nav nav-tabs">
                        <a class="nav-link" href="/RDT.aspx">Стандартный расчет</a>
                        <a class="nav-link active" aria-current="page" href="#">Поверочный расчет</a>
                    </nav>
                </div>
                <div class="col-xs-12 col">
                    <div class="col border border-non-top">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="col-12 row">
                                    <asp:Label ID="Label4" runat="server" Text="Исполнение регулятора:"></asp:Label>
                                </div>
                                <asp:RadioButton ID="eorRadioButton1" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton1_CheckedChanged" Text="Регулятор перепада давления" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList1" Enabled="False" AutoPostBack="True" runat="server" OnSelectedIndexChanged="eorRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>RDT</asp:ListItem>
                                        <asp:ListItem>RDT-H</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton2" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton2_CheckedChanged" Text="Регулятор давления &quot;после себя&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList2" Enabled="False" AutoPostBack="True" runat="server" OnSelectedIndexChanged="eorRadioButtonList2_SelectedIndexChanged">
                                        <asp:ListItem>RDT-P</asp:ListItem>
                                        <asp:ListItem>RDT-PH</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton3" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton3_CheckedChanged" Text="Регулятор давления &quot;до себя&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList3" Enabled="False" runat="server" AutoPostBack="True" OnSelectedIndexChanged="eorRadioButtonList3_SelectedIndexChanged">
                                        <asp:ListItem>RDT-S</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="eorRadioButton4" runat="server" AutoPostBack="True" OnCheckedChanged="eorRadioButton4_CheckedChanged" Text="Регулятор &quot;перепуска&quot;" />
                                <div class="col-12">
                                    <asp:RadioButtonList ID="eorRadioButtonList4" Enabled="False" runat="server" AutoPostBack="True" OnSelectedIndexChanged="eorRadioButtonList4_SelectedIndexChanged">
                                        <asp:ListItem>RDT-B</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div>
                                    <asp:CustomValidator ID="eorCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="CustomValidator" OnServerValidate="eorCustomValidator1_ServerValidate"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label1" runat="server" Text="Диапазон настройки регулятора:"></asp:Label>
                                <div class="col-12 row">
                                    <asp:RadioButtonList ID="csrRadioButtonList1" runat="server" AutoPostBack="True">
                                        <asp:ListItem>0.1 (0,08...0,9 бар)</asp:ListItem>
                                        <asp:ListItem>1.1 (0,16...1,8 бар)</asp:ListItem>
                                        <asp:ListItem>1.2 (0,24...3,0 бар)</asp:ListItem>
                                        <asp:ListItem>1.3 (0,4...4,8 бар)</asp:ListItem>
                                        <asp:ListItem>2.1 (0,5...5,8 бар)</asp:ListItem>
                                        <asp:ListItem>2.2 (0,9...10,0 бар)</asp:ListItem>
                                        <asp:ListItem>2.3 (1,4...15,8 бар)</asp:ListItem>
                                    </asp:RadioButtonList>


                                </div>
                                <div class="col-12 row">
                                    <asp:CustomValidator ID="csrCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="pnCustomValidator" ControlToValidate="csrRadioButtonList1" ValidateEmptyText="True" OnServerValidate="csrCustomValidator1_ServerValidate"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label2" runat="server" Text="Номинальное давление PN, бар:"></asp:Label>
                                <div class="col-12 row">
                                    <asp:RadioButtonList ID="pnRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="pnRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>16</asp:ListItem>
                                        <asp:ListItem>25</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div>
                                    <asp:CustomValidator ID="pnCustomValidator1" runat="server" SetFocusOnError="True" Display="Dynamic" ForeColor="Red" ErrorMessage="pnCustomValidator" ControlToValidate="pnRadioButtonList1" ValidateEmptyText="True" OnServerValidate="pnCustomValidator1_ServerValidate"></asp:CustomValidator>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Диаметр и пропускная способность регулятора:"></asp:Label>
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
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <div id="aaPanel1" runat="server">
                                    <asp:Label ID="Label3" runat="server" Text="Рабочая среда:"></asp:Label>
                                    <div class="row">
                                        <div class="col-6 col-md-3">
                                            <asp:RadioButtonList ID="wsRadioButtonList1" runat="server"
                                                AutoPostBack="True" required="required" OnSelectedIndexChanged="wsRadioButtonList1_SelectedIndexChanged">
                                                <asp:ListItem>Вода</asp:ListItem>
                                                <asp:ListItem>Этиленгликоль</asp:ListItem>
                                                <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <div class="col-12 sub-col">
                                                <asp:RadioButtonList ID="lpvRadioButtonList1" runat="server"
                                                    AutoPostBack="True"
                                                    Enabled="False" Visible="False">
                                                    <asp:ListItem>Перегретый</asp:ListItem>
                                                    <asp:ListItem>Насыщеный</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="col-6 col-md-6">
                                            <div class="row">
                                                <br />
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox1" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number" placeholder=" от 5% до 65%"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label6" runat="server" Text="%"></asp:Label>
                                            </div>
                                            <div class="row">
                                                <asp:TextBox ID="wsTextBox2" runat="server" Enabled="False" type="number"
                                                    required="required" TextMode="Number" placeholder=" от 0&#8451; до 150&#8451;"></asp:TextBox>&nbsp;
                                                <asp:Label ID="Label7" runat="server"
                                                    Text="&#8451;">
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
                        <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                            <ContentTemplate>
                                <div id="lpPane1" runat="server" visible="false" class="col border">
                                    <asp:Label ID="Label20" runat="server" Text="Потери давления на клапане регулятора:">
                                    </asp:Label>
                                    <div class="border">
                                        <asp:Label ID="Label8" runat="server" Text="Регулятор перепада давления:">
                                        </asp:Label>
                                        <div class="col row">
                                            <div class="col-12">
                                                <asp:Label ID="lpLabel2" runat="server"
                                                    Text="Перепад давлений, поддерживаемый регулятором на регулируемом участке:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label55" runat="server" Text="ΔPру ="></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp1TextBox2" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="lp1DropDownList2" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp1DropDownList2_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator8" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp1DropDownList2" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="CustomValidator8_ServerValidate"></asp:CustomValidator>
                                            </div>

                                            <div class="col-12">
                                                <asp:Label ID="lpLabel3" runat="server"
                                                    Text="Давление в подающем трубопроводе "></asp:Label>
                                                <br />
                                                <asp:Label ID="Label60" runat="server"
                                                    Text="(перед регулятором, если он устанавливается на подающем трубопроводе, или в месте врезки импульсной трубки, если регулятор устанавливается на обратном трубопроводе):">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label56" runat="server" Text="P1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp1TextBox3" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="lp1DropDownList3" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp1DropDownList3_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator1" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp1DropDownList3" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                                            </div>

                                            <div class="col-12">
                                                <asp:Label ID="lpLabel4" runat="server" Text="Давление в обратном трубопроводе">
                                                </asp:Label>
                                                <br />
                                                <asp:Label ID="Label61" runat="server"
                                                    Text="(в месте врезки импульсной трубки, если регулятор устанавливается на подающем трубопроводе, или после регулятора, если он устанавливается на обратном трубопроводе):">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label57" runat="server" Text="P2 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp1TextBox4" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number" AutoPostBack="True"></asp:TextBox>
                                                    <asp:DropDownList ID="lp1DropDownList4" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp1DropDownList4_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator2" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp1DropDownList4" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="CustomValidator2_ServerValidate"></asp:CustomValidator>
                                            </div>
                                            <div class="col-12">
                                                <asp:Label ID="lpLabel5" runat="server"
                                                    Text="Максимальный перепад на регуляторе:"></asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-6 col-md-5">
                                                    <asp:Label ID="Label58" runat="server" Text="ΔPрд(max) = Р1 - Р2 - ΔРру =">
                                                    </asp:Label>
                                                </div>
                                                <div class="row">
                                                    <div>
                                                        <asp:TextBox ID="lp1TextBox5" runat="server" Enabled="False" TextMode="Number" AutoPostBack="True"
                                                            ReadOnly="True"></asp:TextBox>&nbsp;
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label59" runat="server" Text="бар"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <div class="col border" visible="false" runat="server" id="lpPane2">
                                    <asp:Label ID="Label9" runat="server" Text="Потери давления на клапане регулятора:">
                                    </asp:Label>
                                    <div class="border">
                                        <asp:Label ID="Label10" runat="server" Text="Регулятор давления <q>после себя</q>:">
                                        </asp:Label>

                                        <div class="col row">
                                            <div class="col-12">
                                                <asp:Label ID="Label11" runat="server" Text="Давление перед регулятором:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label13" runat="server" Text="P'1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp2TextBox1" runat="server" Enabled="False" TextMode="Number" CssClass="textbox-non-right-radius">
                                                    </asp:TextBox>
                                                    <asp:DropDownList ID="lp2DropDownList1" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp2DropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator3" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp2DropDownList1" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" OnServerValidate="CustomValidator3_ServerValidate"></asp:CustomValidator>
                                            </div>
                                            <div class="col-12">
                                                <asp:Label ID="Label14" runat="server"
                                                    Text="Требуемое давление после регулятора:"></asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label15" runat="server" Text="Р(треб) ="></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp2TextBox2" runat="server" Enabled="False" TextMode="Number" CssClass="textbox-non-right-radius">
                                                    </asp:TextBox>
                                                    <asp:DropDownList ID="lp2DropDownList2" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp2DropDownList2_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator4" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp2DropDownList2" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" OnServerValidate="CustomValidator4_ServerValidate"></asp:CustomValidator>
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
                                <div class="col border" visible="false" runat="server" id="lpPane3">
                                    <asp:Label ID="Label36" runat="server" Text="Потери давления на клапане регулятора:">
                                    </asp:Label>
                                    <div class="border">
                                        <asp:Label ID="Label17" runat="server" Text="Регулятор давления <q>до себя</q>:">
                                        </asp:Label>
                                        <div class="col row">
                                            <div class="col-12">
                                                <asp:Label ID="Label18" runat="server"
                                                    Text="Требуемое давление перед регулятором:"></asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label19" runat="server" Text="Р(треб) ="></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp3TextBox1" runat="server" Enabled="False" TextMode="Number" CssClass="textbox-non-right-radius">
                                                    </asp:TextBox>
                                                    <asp:DropDownList ID="lp3DropDownList1" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp3DropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator5" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp3DropDownList1" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" OnServerValidate="CustomValidator5_ServerValidate"></asp:CustomValidator>
                                            </div>
                                            <div class="col-12">
                                                <asp:Label ID="Label25" runat="server" Text="Давление после регулятора:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label27" runat="server" Text="Р'2 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp3TextBox2" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="lp3DropDownList2" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp3DropDownList2_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator6" runat="server"
                                                    ControlToValidate="lp3DropDownList2" Display="Dynamic"
                                                    ErrorMessage="CustomValidator" ForeColor="Red"
                                                    SetFocusOnError="True" OnServerValidate="CustomValidator6_ServerValidate">
                                                </asp:CustomValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                            <ContentTemplate>
                                <div class="col border" runat="server" visible="false" id="lpPane4">
                                    <asp:Label ID="Label40" runat="server" Text="Потери давления на клапане регулятора:">
                                    </asp:Label>
                                    <div class="border">
                                        <asp:Label ID="Label29" runat="server" Text="Регулятор <q>перепуска</q>:">
                                        </asp:Label>
                                        <div class="col row">
                                            <div class="col-12">
                                                <asp:Label ID="Label30" runat="server"
                                                    Text="Перепад давлений, поддерживаемый регулятором на регулируемом участке:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label31" runat="server" Text="ΔPру ="></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="lp4TextBox2" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="lp4DropDownList2" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                        AutoPostBack="True" OnSelectedIndexChanged="lp4DropDownList2_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="CustomValidator7" runat="server" Display="Dynamic"
                                                    ControlToValidate="lp4DropDownList2" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" OnServerValidate="CustomValidator7_ServerValidate"></asp:CustomValidator>
                                            </div>
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
                                                <asp:Label ID="Label26" runat="server" Text="Давление перед регулятором:">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="calcrTextBox1" runat="server" CssClass="textbox-non-right-radius"
                                                        Enabled="False" TextMode="Number">
                                                    </asp:TextBox>
                                                    <asp:DropDownList ID="calcrDropDownList1" runat="server" Enabled="False" CssClass="dropdown-non-left-radius"
                                                         AutoPostBack="True" OnSelectedIndexChanged="calcrDropDownList1_SelectedIndexChanged">
                                                        <asp:ListItem>выбрать</asp:ListItem>
                                                        <asp:ListItem>МПа</asp:ListItem>
                                                        <asp:ListItem>кПа</asp:ListItem>
                                                        <asp:ListItem>бар</asp:ListItem>
                                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="calcrCustomValidator1" runat="server"
                                                    ErrorMessage="CustomValidator" ControlToValidate="calcrDropDownList1"
                                                    Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                                    ValidateEmptyText="True" OnServerValidate="calcrCustomValidator1_ServerValidate"></asp:CustomValidator>
                                            </div>
                                            <div class="col-12">
                                                <asp:Label ID="Label22" runat="server"
                                                    Text="Температура теплоносителя через регулятор (максимальная):">
                                                </asp:Label>
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="calcrTextBox2" runat="server" Enabled="False"
                                                        type="number" TextMode="Number"
                                                        CausesValidation="True">
                                                    </asp:TextBox>&nbsp;
                                                        <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <asp:CustomValidator ID="calcrCustomValidator2" runat="server"
                                                    ErrorMessage="CustomValidator" ControlToValidate="calcrTextBox2"
                                                    Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                                    ValidateEmptyText="True" OnServerValidate="calcrCustomValidator2_ServerValidate"></asp:CustomValidator>
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
                                        <asp:Label ID="Label16" runat="server" Text="Расход через регулятор давления:"></asp:Label>
                                        <div class="flow-radio-label col row">
                                            <div>
                                                <asp:RadioButton ID="fprRadioButton1" runat="server"
                                                    Text="Задать максимальную величину расхода через регулятор давления:"
                                                    AutoPostBack="True" Checked="True" />
                                            </div>
                                            <div class="col-12 col-md-8 row">
                                                <div class="col-4 col-md-2">
                                                    <asp:Label ID="Label28" runat="server" Text="Gкл = "></asp:Label>
                                                </div>
                                                <div class="row">
                                                    <asp:TextBox ID="fprTextBox1" runat="server" Enabled="False" CssClass="textbox-non-right-radius"
                                                        TextMode="Number"></asp:TextBox>
                                                    <asp:DropDownList ID="fprDropDownList1" runat="server" AutoPostBack="True" CssClass="dropdown-non-left-radius"
                                                        Enabled="False" OnSelectedIndexChanged="fprDropDownList1_SelectedIndexChanged">
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

                                                <asp:CustomValidator ID="fprCustomValidator1" runat="server" Display="Dynamic"
                                                    ControlToValidate="fprDropDownList1" ErrorMessage="CustomValidator"
                                                    ForeColor="Red"
                                                    SetFocusOnError="True" ValidateEmptyText="True" OnServerValidate="fprCustomValidator1_ServerValidate"></asp:CustomValidator>
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
                            <asp:Button ID="rdtCalc" runat="server" type="submit" Text="Рассчитать"
                                Width="100%" OnClick="rdtCalc_Click" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-12">
                    <div class="col non-padding">
                        <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                            <ContentTemplate>
                                <div id="resultPanel" runat="server">
                                    <asp:Label ID="Label52" runat="server" Enabled="False" Text="Результаты расчёта"
                                        Visible="False" Font-Bold="True" Font-Size="Medium"></asp:Label>

                                    <div class="col non-padding">
                                        <asp:Label ID="ws1ResultLabel" runat="server" Visible="False"></asp:Label>
                                    </div>
                                    <div class="col non-padding">
                                        <asp:Label ID="maxt1ResultLabel" runat="server" Visible="False"></asp:Label>
                                    </div>
                                    <div class="col non-padding">
                                        <asp:Label ID="maxp1ResultLabel" runat="server" Visible="False"></asp:Label>
                                    </div>
 
                                    <div class="col non-padding">
                                        <asp:Label ID="labelOptyV" runat="server" Visible="False"></asp:Label>
                                    </div>
                                    <div class="table-responsive-lg">
                                        <asp:GridView ID="GridView1" CssClass="table table-result rdt-ver" runat="server"
                                            Font-Size="X-Small" Visible="False"
                                            AutoGenerateSelectButton="True" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" Style="left: 0px; top: 0px">
                                            <RowStyle Font-Size="Small" />
                                            <SelectedRowStyle BackColor="#ff7d00" Font-Bold="False" ForeColor="White" />
                                        </asp:GridView>
                                    </div>
                                    <div class="col non-padding">
                                        <div>
                                            <asp:Label ID="mtmLabelError" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Font-Strikeout="False" ForeColor="Red" Enabled="false"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label53" runat="server" CssClass="show-btn" Text="Объект:"
                                                Visible="False">
                                            </asp:Label>
                                        </div>
                                        <asp:TextBox ID="objTextBox1" runat="server" Enabled="False" Visible="False" CssClass="obj-field"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col non-padding padding-top-bottom">
                                    <asp:Button ID="rdtSave" runat="server" Text="Сохранить в PDF" Visible="False"
                                        CssClass="btn btn-primary" OnClick="rdtSave_Click" />
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="rdtSave" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
        </form>
    </div>
</body>
</html>
