<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title></title>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server">
            <div class="row jumbotron">
                
                <asp:Button ID="Button1" CssClass="btn btn-primary btn-lg btn-block btn-indexbtn" runat="server" Text="Программа подбора регулятора давления прямого действия" BackColor="#FF6600" ForeColor="White" PostBackUrl="~/RDT.aspx" />
                <asp:Button ID="Button2" CssClass="btn btn-primary btn-lg btn-block btn-index" runat="server" Text="Программа подбора регулирующего клапана" BackColor="#FF6600" ForeColor="White" PostBackUrl="~/TRV.aspx" />

            </div>
        </form>
    </div>
</body>
</html>
