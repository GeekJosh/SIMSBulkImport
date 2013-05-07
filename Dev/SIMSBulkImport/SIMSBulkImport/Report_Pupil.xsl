<?xml version='1.0'?>
<!--   Author      : Matt Smith                                                             -->
<!--   Date        : 2012-03-10                                                             -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <title>Import results - <xsl:value-of select="SIMS_Bulk_Import/Properties/Title"/></title>
        <style type="text/css">
          body {
            margin: 0px;
            padding: 1em;
          }

          h1 {

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

          td.toplefttd {
            font-weight:bold;
            width: 10em;
          }

          td.toprighttd {
            width: 7em;
            text-align: center;
          }

          td.resultTitle {
            font-weight: bold;
          }
        </style>
      </head>
      <body>
        <h1>
          <xsl:value-of select="SIMS_Bulk_Import/Properties/Title"/>
        </h1>
        <table>
          <tr>
            <td class="toplefttd">Version: </td>
            <td class="toprighttd"><xsl:value-of select="SIMS_Bulk_Import/Properties/Version"/></td>
          </tr>
          <tr>
            <td class="toplefttd">Date:</td>
            <td class="toprighttd"><xsl:value-of select="SIMS_Bulk_Import/Properties/Date"/></td>
          </tr>
          <tr>
            <td class="toplefttd">Type:</td>
            <td class="toprighttd">Pupil</td>
          </tr>
        </table>
        <br/>
        <table class="resultTable">
          <tr>
            <td class="resultTitle">Surname</td>
            <td class="resultTitle">Forename</td>
            <td class="resultTitle">Gender</td>
            <td class="resultTitle">Admission Number</td>
            <td class="resultTitle">Date of birth</td>
            <td class="resultTitle">Year</td>
            <td class="resultTitle">Registration</td>
            <td class="resultTitle">PersonID</td>
            <td class="resultTitle">Result</td>
            <td class="resultTitle">Item</td>
            <td class="resultTitle">Value</td>
            <td class="resultTitle">Notes</td>
          </tr>
          <xsl:for-each select="SIMS_Bulk_Import/Pupil_Import_Results">
          <tr>
            <td><xsl:value-of select="Surname"/></td>
            <td><xsl:value-of select="Forename"/></td>
            <td><xsl:value-of select="Gender"/></td>
            <td><xsl:value-of select="Admission-Number"/></td>
            <td><xsl:value-of select="Date-of-Birth"/></td>
            <td><xsl:value-of select="Year"/></td>
            <td><xsl:value-of select="Registration"/></td>
            <td><xsl:value-of select="PersonID"/></td>
            <td><xsl:value-of select="Result"/></td>
            <td><xsl:value-of select="Item"/></td>
            <td><xsl:value-of select="Value"/></td>
            <td><xsl:value-of select="Notes"/></td>
          </tr>
          </xsl:for-each>
        </table>
        <br/>
        <p class="footer">
          <xsl:value-of select="SIMS_Bulk_Import/Properties/Copyright"/>
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>