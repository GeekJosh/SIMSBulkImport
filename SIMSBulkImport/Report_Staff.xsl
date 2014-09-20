<?xml version='1.0'?>

<!--   Author      : Matt Smith                                                             -->
<!--   Date        : 2012-03-10                                                             -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <title>Import results - <xsl:value-of select="SIMS_Bulk_Import/Properties/Title" /></title>
        <style type="text/css">
          body {
          margin: 0px;
          padding: 1em;
          }

          h1 {
          padding-top:8px;
          }

          p.footer {
          font-size: 0.8em;
          }

          table {
          border-collapse: collapse;
          }

          table, th, td {
          border: 1px solid #666;
          padding: 0.3em;
          }

          .toplefttd {
          font-weight:bold;
          width: 10em;
          }

          .toprighttd {
          width: 7em;
          text-align: center;
          }

          .title {
          font-weight:bold;
          }

          .logo {
          width:400px;
          height:60px;
          padding-left:55px;
          background-image: url(data:image/png;base64,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);
          background-repeat: no-repeat;
          }
        </style>
      </head>
      <body>
          <div class="logo">
            <h1>
              <xsl:value-of select="SIMS_Bulk_Import/Properties/Title" />
            </h1>
          </div>
          <table>
          <tr>
            <td class="toplefttd">Version: </td>
            <td class="toprighttd">
              <xsl:value-of select="SIMS_Bulk_Import/Properties/Version" />
            </td>
          </tr>
          <tr>
            <td class="toplefttd">Date:</td>
            <td class="toprighttd">
              <xsl:value-of select="SIMS_Bulk_Import/Properties/Date" />
            </td>
          </tr>
          <tr>
            <td class="toplefttd">Type:</td>
            <td class="toprighttd">Staff</td>
          </tr>
        </table>
        <br />
        <table class="resultTable">
          <tr>
            <td class="title">Surname</td>
            <td class="title">Forename</td>
            <td class="title">Gender</td>
            <td class="title">Staff Code</td>
            <td class="title">Date of birth</td>
            <td class="title">PersonID</td>
            <td class="title">Result</td>
            <td class="title">Item</td>
            <td class="title">Value</td>
            <td class="title">Notes</td>
          </tr>
          <xsl:for-each select="SIMS_Bulk_Import/Staff_Import_Results">
            <tr>
              <td>
                <xsl:value-of select="Surname" />
              </td>
              <td>
                <xsl:value-of select="Forename" />
              </td>
              <td>
                <xsl:value-of select="Gender" />
              </td>
              <td>
                <xsl:value-of select="Staff_Code" />
              </td>
              <td>
                <xsl:value-of select="DOB" />
              </td>
              <td>
                <xsl:value-of select="PersonID" />
              </td>
              <td>
                <xsl:value-of select="Result" />
              </td>
              <td>
                <xsl:value-of select="Item" />
              </td>
              <td>
                <xsl:value-of select="Value" />
              </td>
              <td>
                <xsl:value-of select="Notes" />
              </td>
            </tr>
          </xsl:for-each>
        </table>
        <br />
        <p class="footer">
          <xsl:value-of select="SIMS_Bulk_Import/Properties/Copyright" />
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>