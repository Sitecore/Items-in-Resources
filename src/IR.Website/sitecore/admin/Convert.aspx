<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Convert.aspx.cs" Inherits="IR.sitecore.admin.Convert" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <h1>Convert SQL Database items to Items-in-Resources format</h1>
      <asp:Button runat="server" OnClick="ConvertItems" Text="Convert all databases" OnClientClick="var that=this; setTimeout(function(){that.disabled='true'; that.value='Processing... Please wait.'; },10); return true;"/>
    </div>
    </form>
</body>
</html>
