<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TRV.aspx.cs" Inherits="TRV" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 300px;
        }

        .auto-style7 {
            text-align: center;
            width: 300px;
        }

        .auto-style8 {
            position: relative;
            width: 100%;
            -ms-flex-preferred-size: 0;
            flex-basis: 0;
            -ms-flex-positive: 1;
            flex-grow: 1;
            min-width: 0;
            max-width: 100%;
            left: 0px;
            top: 0px;
            padding-left: 15px;
            padding-right: 15px;
        }
    </style>
</head>

<body>
    <div class="container">
        <form id="form1" runat="server" novalidate="novalidate">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-xs-12 col-sm-10">
                    <div class="col border">
                        <asp:Label ID="Label1" runat="server" Text="Место установки:"></asp:Label>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:RadioButtonList ID="spvRadioButtonList1" runat="server" style="margin-left: 0px"
                                    AutoPostBack="True" required="required"
                                    OnSelectedIndexChanged="spvRadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>ЦТП</asp:ListItem>
                                    <asp:ListItem>ИТП</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:CustomValidator ID="spvCustomValidator1" runat="server"
                                    ControlToValidate="spvRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="CustomValidator" ForeColor="Red"
                                    OnServerValidate="spvCustomValidator1_ServerValidate" SetFocusOnError="True"
                                    ValidateEmptyText="True"></asp:CustomValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label2" runat="server" Text="Наличие регулятора перепада давления:">
                                </asp:Label>
                                <asp:RadioButtonList ID="rpvRadioButtonList1" required="required" runat="server"
                                    AutoPostBack="True">
                                    <asp:ListItem>Да</asp:ListItem>
                                    <asp:ListItem>Нет</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:CustomValidator ID="rpvCustomValidator1" runat="server"
                                    ControlToValidate="rpvRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="CustomValidator" ForeColor="Red"
                                    OnServerValidate="rpvCustomValidator1_ServerValidate" SetFocusOnError="True"
                                    ValidateEmptyText="True"></asp:CustomValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label3" runat="server"
                                    Text="Область применения (система) / Схема присоединения">
                                </asp:Label>:<br />
                                <asp:RadioButton ID="aaRadioButton1" runat="server" Text="Горячее водоснабжение"
                                    AutoPostBack="True" required="required"
                                    OnCheckedChanged="aaRadioButton1_CheckedChanged" />
                                <asp:RadioButtonList ID="aa1RadioButtonList1" required="required" CssClass="childRadio"
                                    runat="server" RepeatDirection="Horizontal" Enabled="False" AutoPostBack="True"
                                    OnSelectedIndexChanged="aa1RadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>Открытая</asp:ListItem>
                                    <asp:ListItem>Закрытая (через теплообменник)</asp:ListItem>
                                </asp:RadioButtonList>

                                <asp:RadioButton ID="aaRadioButton2" runat="server" Text="Отопление" AutoPostBack="True"
                                    required="required" OnCheckedChanged="aaRadioButton2_CheckedChanged" />
                                <asp:RadioButtonList ID="aa2RadioButtonList1" required="required" CssClass="childRadio"
                                    runat="server" RepeatDirection="Horizontal" Enabled="False" AutoPostBack="True"
                                    OnSelectedIndexChanged="aa2RadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>Зависимая</asp:ListItem>
                                    <asp:ListItem>Независимая (через теплообменник)</asp:ListItem>
                                </asp:RadioButtonList>

                                <asp:RadioButton ID="aaRadioButton3" runat="server" Text="Вентиляция"
                                    AutoPostBack="True" required="required"
                                    OnCheckedChanged="aaRadioButton3_CheckedChanged" />
                                <asp:RadioButtonList ID="aa3RadioButtonList1" required="required" CssClass="childRadio"
                                    runat="server" RepeatDirection="Horizontal" Enabled="False" AutoPostBack="True"
                                    OnSelectedIndexChanged="aa3RadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>Зависимая</asp:ListItem>
                                    <asp:ListItem>Независимая (через теплообменник)</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:CustomValidator ID="aaCustomValidator8" runat="server"
                                    ControlToValidate="aa1RadioButtonList1" Display="Dynamic"
                                    ErrorMessage="CustomValidator" ForeColor="Red" SetFocusOnError="True"
                                    ValidateEmptyText="True" OnServerValidate="aaCustomValidator8_ServerValidate">
                                </asp:CustomValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label4" runat="server" Text="Тип клапана:"></asp:Label>
                                <asp:RadioButtonList ID="tvRadioButtonList1" runat="server" AutoPostBack="True"
                                    required="required"
                                    OnSelectedIndexChanged="tvRadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>2-х ходовой седельный</asp:ListItem>
                                    <asp:ListItem>3-х ходовой смесительный</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:CustomValidator ID="tv1CustomValidator1" runat="server"
                                    ControlToValidate="tvRadioButtonList1" Display="Dynamic"
                                    ErrorMessage="CustomValidator" ForeColor="Red"
                                    OnServerValidate="tv1CustomValidator1_ServerValidate" SetFocusOnError="True"
                                    ValidateEmptyText="True"></asp:CustomValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label5" runat="server" Text="Рабочая среда:"></asp:Label>
                                <div class="row">
                                    <div class="col-3">
                                        <asp:RadioButtonList ID="ws2RadioButtonList1" runat="server" AutoPostBack="True"
                                            required="required"
                                            OnSelectedIndexChanged="ws2RadioButtonList1_SelectedIndexChanged">
                                            <asp:ListItem>Вода</asp:ListItem>
                                            <asp:ListItem>Этиленгликоль</asp:ListItem>
                                            <asp:ListItem>Пропиленгликоль</asp:ListItem>
                                            <asp:ListItem Enabled="False">Водяной пар</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="col-9">
                                        <br />
                                        <asp:TextBox ID="ws2TextBox1" runat="server" step="0.01" Enabled="False"
                                            type="number" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label6" runat="server" Text="% (от 5% до 65%)"></asp:Label>
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:CustomValidator ID="CustomValidator16" runat="server"
                                            ControlToValidate="ws2TextBox1" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator16_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                        <asp:TextBox ID="ws2TextBox2" runat="server" Enabled="False" step="0.01"
                                            type="number" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="Label7" runat="server"
                                            Text="&#8451; (от 0&#8451; до 150&#8451;)">
                                        </asp:Label>&nbsp;&nbsp;&nbsp;
                                        <asp:CustomValidator ID="CustomValidator17" runat="server"
                                            ControlToValidate="ws2TextBox2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator17_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                    <div class="col">
                                        <asp:CustomValidator ID="ws2CustomValidator1" runat="server"
                                            ControlToValidate="ws2RadioButtonList1" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="ws2CustomValidator1_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="lpv1" runat="server">
                                    <asp:Label ID="Label15" runat="server" Text="Потери давления в системе:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label9" runat="server" Text="&#916;Pсист = "></asp:Label>
                                        <asp:TextBox ID="lpvTextBox2" runat="server" Enabled="False" step="0.01"
                                            type="number" TextMode="Number" required="required" CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:DropDownList ID="lpvDropDownList2" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="lpvDropDownList2_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator18" runat="server"
                                            ControlToValidate="lpvDropDownList2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator18_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="col border">

                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="lpv2" runat="server">
                                    <asp:Label ID="Label11" runat="server" Text="Потери давления в теплообменнике:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label10" runat="server" Text="&#916;Pто = "></asp:Label>
                                        &nbsp;&nbsp;
                                        <asp:TextBox ID="lpvTextBox21" runat="server" Enabled="False" step="0.01"
                                            type="number" required="required" TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="lpvDropDownList21" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="lpvDropDownList21_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator19" runat="server"
                                            ControlToValidate="ws2TextBox2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator19_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="lpv5" runat="server">

                                    <asp:Label ID="Label17" runat="server" Text="Давление пара перед клапаном:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label18" runat="server" Text="P'1 = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        &nbsp;<asp:TextBox ID="lpv5TextBox1" runat="server" step="0.00000000001"
                                            Enabled="False"></asp:TextBox>&nbsp;
                                        <asp:DropDownList ID="lpv5DropDownList1" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lpv5DropDownList1_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server"
                                            ControlToValidate="lpv5DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="CustomValidator1_ServerValidate">
                                        </asp:CustomValidator>
                                    </div>
                                    <asp:Label ID="Label19" runat="server"
                                        Text="Требуемое давление пара после клапана:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label20" runat="server" Text="Р(треб) ="></asp:Label>
                                        <asp:TextBox ID="lpv5TextBox2" runat="server" step="0.00000000001"
                                            Enabled="False" TextMode="Number"></asp:TextBox>
                                        &nbsp;<asp:DropDownList ID="lpv5DropDownList2" runat="server" Enabled="False"
                                            OnSelectedIndexChanged="lpv5DropDownList2_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator2" runat="server"
                                            ControlToValidate="lpv5DropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator2_ServerValidate"
                                            SetFocusOnError="True">
                                        </asp:CustomValidator>
                                    </div>
                                    <asp:Label ID="Label25" runat="server" Text="Тип пара:">
                                    </asp:Label>
                                    <div class="auto-style8">
                                        <asp:RadioButtonList ID="lpv5RadioButtonList1" runat="server"
                                            AutoPostBack="True"
                                            OnSelectedIndexChanged="lpv5RadioButtonList1_SelectedIndexChanged">
                                            <asp:ListItem>Перегретый</asp:ListItem>
                                            <asp:ListItem>Насыщеный</asp:ListItem>
                                        </asp:RadioButtonList>

                                        <asp:CustomValidator ID="CustomValidator21" runat="server"
                                            ControlToValidate="lpv5RadioButtonList1" Display="Dynamic"
                                            EnableClientScript="False" ErrorMessage="CustomValidator" ForeColor="Red"
                                            SetFocusOnError="True" ValidateEmptyText="True"
                                            OnServerValidate="CustomValidator21_ServerValidate"></asp:CustomValidator>
                                    </div>
                                    <asp:Label ID="Label57" runat="server" Text="Температура пара через клапан:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label58" runat="server" Text="T1 = "></asp:Label>

                                        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="lpv5TextBox3"
                                            runat="server" step="0.00000000001" Enabled="False" type="number"
                                            TextMode="Number" CausesValidation="True"
                                            OnTextChanged="lpv5TextBox3_TextChanged">
                                        </asp:TextBox>
                                        <asp:Label ID="Label59" runat="server" Text=" &#8451;"></asp:Label>
                                        <asp:CustomValidator ID="CustomValidator3" runat="server"
                                            ControlToValidate="lpv5TextBox3" Display="Dynamic"
                                            EnableClientScript="False" ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator3_ServerValidate" SetFocusOnError="True"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div class="col border">

                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="lpv3" runat="server">
                                    <asp:Label ID="Label14" runat="server" Text="Расчётные потери давления на клапане:">
                                    </asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label13" runat="server" Text="&#916;Pкл = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;<asp:TextBox ID="lpvTextBox1" runat="server" Enabled="False"
                                            step="0.01" type="number" required="required" TextMode="Number">
                                        </asp:TextBox>
                                        <asp:DropDownList ID="lpvDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="lpvDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="lpvCustomValidator1" runat="server"
                                            ControlToValidate="lpvDropDownList1" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="lpvCustomValidator1_ServerValidate"
                                            ValidateEmptyText="True">
                                        </asp:CustomValidator>
                                        <asp:Label ID="Label55" runat="server" ForeColor="Red"
                                            Text="Неверно указано значение давления" Visible="False"></asp:Label>
                                        <asp:Label ID="Label56" runat="server" ForeColor="Red"
                                            Text="Суммарные потери давления в теплообменнике и на клапане превышают давление перед клапаном"
                                            Visible="False"></asp:Label>
                                        <br />


                                    </div>
                                    <div class="col-12">
                                        <asp:Label ID="Label54" runat="server"
                                            Text="(для корректной работы клапана потери давления на нем должны быть не менее, чем потери давления на регулируемом участке, т.е. в теплообменнике при независимой схеме присоединения или в системе при зависимой схеме присоединения)">
                                        </asp:Label>
                                        <br />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                            <ContentTemplate>
                                <div class="panel-hide" id="calcv" runat="server">
                                    <asp:Label ID="Label12" runat="server"
                                        Text="Расчёт регулирующего клапана на кавитацию:"></asp:Label>
                                    <div class="col">
                                        <asp:Label ID="Label26" runat="server" Text="Давление перед клапаном:">
                                        </asp:Label>
                                        <br />
                                        <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;<asp:TextBox ID="calcvTextBox1" runat="server" Enabled="False"
                                            step="0.01" type="number" required="required" TextMode="Number">
                                        </asp:TextBox>
                                        <asp:DropDownList ID="calcvDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="calcvDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>

                                        &nbsp;<asp:CustomValidator ID="calcvCustomValidator1" runat="server"
                                            ErrorMessage="CustomValidator" ControlToValidate="calcvDropDownList1"
                                            Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="calcvCustomValidator1_ServerValidate"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                        <asp:Label ID="Label22" runat="server"
                                            Text="Максимальная температура теплоносителя через клапан:">
                                        </asp:Label><br />
                                        <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                        &nbsp;&nbsp;<asp:TextBox ID="calcvTextBox2" runat="server" step="0.01"
                                            Enabled="False" type="number" required="required" TextMode="Number"
                                            CausesValidation="True">
                                        </asp:TextBox>
                                        <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                        &nbsp;<asp:CustomValidator ID="calcvCustomValidator2" runat="server"
                                            ErrorMessage="CustomValidator" ControlToValidate="calcvTextBox2"
                                            Display="Dynamic" ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="calcvCustomValidator2_ServerValidate"
                                            ValidateEmptyText="True"></asp:CustomValidator>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="col border">
                        <div>
                            <asp:Label ID="Label16" runat="server" Text="Расход через клапан:"></asp:Label>
                            <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButton ID="fvRadioButton1" runat="server"
                                        Text="Задать максимальную величину расхода через клапан:" AutoPostBack="True"
                                        OnCheckedChanged="fvRadioButton1_CheckedChanged" /><br />
                                    <div class="col">
                                        <asp:Label ID="Label28" runat="server" Text="Gкл = "></asp:Label>
                                        <asp:TextBox ID="fvTextBox1" runat="server" step="0.00001" Enabled="False"
                                            TextMode="Number"></asp:TextBox>
                                        <asp:DropDownList ID="fvDropDownList1" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="fvDropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>м3/ч</asp:ListItem>
                                            <asp:ListItem>л/с</asp:ListItem>
                                            <asp:ListItem>л/мин</asp:ListItem>
                                            <asp:ListItem>л/ч</asp:ListItem>
                                            <asp:ListItem>кг/с</asp:ListItem>
                                            <asp:ListItem>кг/ч</asp:ListItem>
                                            <asp:ListItem>т/ч</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;<asp:CustomValidator ID="CustomValidator12" runat="server"
                                            ControlToValidate="fvDropDownList1" ErrorMessage="CustomValidator"
                                            ForeColor="Red" OnServerValidate="CustomValidator12_ServerValidate"
                                            SetFocusOnError="True" ValidateEmptyText="True"></asp:CustomValidator>
                                        <br />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div>
                            <asp:UpdatePanel ID="UpdatePanel14" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButton ID="fvRadioButton2" runat="server"
                                        Text="Вычислить максимальную величину расхода через клапан:" AutoPostBack="True"
                                        OnCheckedChanged="fvRadioButton2_CheckedChanged" />
                                    <div class="col table-responsive-md">
                                        <table class="table table-bordered col">
                                            <thead>
                                                <tr>
                                                    <th scope="col"></th>
                                                    <th scope="col"></th>
                                                    <th scope="col" class="auto-style1">Температура
                                                        теплоносителя в подающем трубопроводе</th>
                                                    <th scope="col">Температура теплоносителя в обратном трубопроводе
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <th rowspan="2" scope="row" class="align-middle">Параметры теплосети
                                                    </th>
                                                    <td>Зима</td>
                                                    <td class="auto-style7">
                                                        <asp:Label ID="Label27" runat="server" Text="T1 = "></asp:Label>
                                                        <asp:TextBox ID="fvTextBox2" runat="server" step="0.01"
                                                            Enabled="False" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label29" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:Label ID="Label32" runat="server" Text="T2 = "></asp:Label>
                                                        <asp:TextBox ID="fvTextBox3" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label33" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Лето</td>
                                                    <td class="auto-style7">
                                                        <asp:Label ID="Label30" runat="server" Text="T'1 = ">
                                                        </asp:Label>
                                                        <asp:TextBox ID="fvTextBox4" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label31" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:Label ID="Label34" runat="server" Text="T'2 = ">
                                                        </asp:Label>
                                                        <asp:TextBox ID="fvTextBox5" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label35" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th rowspan="2" scope="row" class="align-middle">Параметры системы
                                                    </th>
                                                    <td>Отопления</td>
                                                    <td class="auto-style7">
                                                        <asp:Label ID="Label36" runat="server" Text="T21 = ">
                                                        </asp:Label>
                                                        <asp:TextBox ID="fvTextBox6" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label37" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:Label ID="Label38" runat="server" Text="T22= "></asp:Label>
                                                        <asp:TextBox ID="fvTextBox7" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label39" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Вентиляции</td>
                                                    <td class="auto-style7">
                                                        <asp:Label ID="Label40" runat="server" Text="T11 = ">
                                                        </asp:Label>
                                                        <asp:TextBox ID="fvTextBox8" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label41" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:Label ID="Label42" runat="server" Text="T12= "></asp:Label>
                                                        <asp:TextBox ID="fvTextBox9" runat="server" Enabled="False"
                                                            step="0.01" type="number" Width="60px" required="required"
                                                            TextMode="Number"></asp:TextBox>
                                                        <asp:Label ID="Label43" runat="server" Text=" &#8451;">
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                        <asp:CustomValidator ID="tvCustomValidator1" runat="server"
                                            ControlToValidate="tvRadioButtonList1" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red" SetFocusOnError="True"
                                            OnServerValidate="tvCustomValidator1_ServerValidate"></asp:CustomValidator>



                                        <br />



                                        <asp:Label ID="Label45" runat="server" Text="Тепловая мощность Q = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fvTextBox10" runat="server" Enabled="False" step="0.01"
                                            required="required" OnTextChanged="fvTextBox10_TextChanged"
                                            TextMode="Number">
                                        </asp:TextBox>
                                        <asp:DropDownList ID="fvDropDownList2" runat="server" AutoPostBack="True"
                                            Enabled="False"
                                            OnSelectedIndexChanged="fvDropDownList2_SelectedIndexChanged">
                                            <asp:ListItem class="dropdown-item">выбрать</asp:ListItem>
                                            <asp:ListItem Value="кВт" class="dropdown-item">кВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">МВт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Вт</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">Гкал/ч</asp:ListItem>
                                            <asp:ListItem class="dropdown-item">ккал/ч</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="CustomValidator20" runat="server"
                                            ControlToValidate="fvDropDownList2" Display="Dynamic"
                                            ErrorMessage="CustomValidator" ForeColor="Red"
                                            OnServerValidate="CustomValidator20_ServerValidate"></asp:CustomValidator>
                                        <br />

                                        <asp:Label ID="Label46" runat="server" Text="Максимальный расход Gкл = ">
                                        </asp:Label>
                                        <asp:TextBox ID="fvTextBox11" runat="server" Enabled="False" step="0.01"
                                            ReadOnly="True"></asp:TextBox>
                                        <asp:Label ID="Label48" runat="server" Text=" кг/ч"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label8" runat="server" ForeColor="Red" Text=""></asp:Label>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel15" runat="server">
                            <ContentTemplate>
                                <div class="col panel-hide" id="tdRBL" runat="server">
                                    <asp:Label ID="Label44" runat="server" Text="Характеристики электропривода:">
                                    </asp:Label><br />
                                    <asp:Label ID="Label47" runat="server" Text="Напряжение питания:"></asp:Label>
                                    <asp:RadioButtonList ID="tdRadioButtonList1" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="tdRadioButtonList1_SelectedIndexChanged"
                                        Enabled="False">
                                        <asp:ListItem Selected="True">230 VAC</asp:ListItem>
                                        <asp:ListItem>24 VAC / VDC</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="Label49" runat="server" Text="Управление:"></asp:Label>
                                    <asp:RadioButtonList ID="tdRadioButtonList2" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="tdRadioButtonList2_SelectedIndexChanged"
                                        Enabled="False">
                                        <asp:ListItem Selected="True">Трёхпозиционное</asp:ListItem>
                                        <asp:ListItem>Аналоговое 4-20 mA (2-10 V)</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="Label50" runat="server" Text="Наличие датчика положения 4-20 mA:">
                                    </asp:Label>
                                    <asp:RadioButtonList ID="tdRadioButtonList3" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="tdRadioButtonList3_SelectedIndexChanged"
                                        Enabled="False">
                                        <asp:ListItem>Да</asp:ListItem>
                                        <asp:ListItem Selected="True">Нет</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="Label51" runat="server"
                                        Text="Наличие возвратного механизма (функция безопасности):">
                                    </asp:Label>
                                    <asp:RadioButtonList ID="tdRadioButtonList4" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="tdRadioButtonList4_SelectedIndexChanged"
                                        Enabled="False">
                                        <asp:ListItem> Да</asp:ListItem>
                                        <asp:ListItem Selected="True"> Нет</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-2">
                    <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                        <ContentTemplate>
                            <asp:Image ID="vPictureBox" runat="server" class="valve-image" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-10">
                    <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="LabelError" runat="server" Font-Bold="True" Font-Size="Medium"
                                Font-Strikeout="False" ForeColor="Red"></asp:Label>

                            <asp:Button ID="vButton" runat="server" type="submit" Text="Рассчитать"
                                OnClick="vButton_Click" Width="100%" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-12">
                    <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="Label52" runat="server" Enabled="False" Text="Результаты расчёта"
                                Visible="False" Font-Size="Medium" Font-Bold="True"></asp:Label>

                            <div class="col non-padding">
                                <asp:Label ID="ws2ResultLabel" runat="server" Text="Label" Visible="False"></asp:Label>
                            </div>

                            <div class="col non-padding">
                                <asp:Label ID="maxt2ResultLabel" runat="server" Text="maxt2ResultLabel" Visible="False">
                                </asp:Label>
                            </div>
                            <div class="col non-padding">
                                <asp:Label ID="maxp2ResultLabel" runat="server" Text="Label" Visible="False">
                                </asp:Label>
                            </div>
                            <div class="col non-padding">
                                <asp:Label ID="calcvDNLabel" runat="server" Visible="False"></asp:Label>
                            </div>
                            <div class="col non-padding">
                                <asp:Label ID="calcvCapacityLabel" runat="server" Visible="False"></asp:Label>
                            </div>

                            <div class="table-responsive-lg" onclick="ShowBTN()">
                                <asp:GridView ID="GridView2" CssClass="table table-result trv" runat="server"
                                    Font-Size="X-Small" Visible="False"
                                    OnSelectedIndexChanged="GridView2_SelectedIndexChanged"
                                    AutoGenerateSelectButton="True">


                                    <RowStyle Font-Size="Small" />
                                    <SelectedRowStyle BackColor="#ff7d00" Font-Bold="False" ForeColor="White" />
                                </asp:GridView>
                            </div>
                            <div class="col non-padding">
                                <asp:Label ID="Label53" runat="server" CssClass="show-btn" Text="Объект:"
                                    Visible="False">
                                </asp:Label>

                                <asp:TextBox ID="objTextBox1" runat="server" Enabled="False" Visible="False"
                                    Width="1020px">
                                </asp:TextBox>
                            </div>
                            <script>
                                function ShowBTN() {
                                    var element = document.getElementById('Label53');
                                    var btn2 = document.getElementById('Button2');
                                    //var btn3 = document.getElementById('Button3');
                                    btn2.classList.add("show-btn");
                                    //btn3.classList.add("show-btn");
                                };
                            </script>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="col non-padding padding-top-bottom">
                        <asp:Button ID="Button2" runat="server" Text="Сохранить в PDF"
                            CssClass="btn btn-primary hide-btn" OnClick="Button2_Click" />
                        <%--<asp:Button ID="Button3" runat="server" Text="Сохранить в Excel"
                                CssClass="btn btn-primary hide-btn" Display="None" OnClick="Button3_Click" />--%>
                    </div>
                </div>
            </div>
        </form>
    </div>

</body>

</html>