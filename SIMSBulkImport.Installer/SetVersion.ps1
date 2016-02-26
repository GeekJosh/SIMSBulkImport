$buildname = $args[0]
Write-Host $buildname
$buildPts = $buildname.Split('.')

$majorVer = $buildPts[0]
$minorVer = $buildPts[1]
$buildVer = $buildPts[2]

$outPut = "<?xml version=""1.0"" encoding=""utf-8""?>`n<Include>`n	<?define ProductName=""SIMS Bulk Import""?>`n	<?define ProductVersion.Major="""+$majorVer+"""?>`n	<?define ProductVersion.Minor="""+$minorVer+"""?>`n	<?define ProductVersion.Build="""+$buildVer+"""?>`n</Include>"
Write-Host $outPut
$outPut | Out-File Installer\Version.wxi -Encoding "UTF8"