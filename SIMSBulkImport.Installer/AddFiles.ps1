$filehead = "<?xml version=""1.0"" encoding=""UTF-8""?>`n<Wix xmlns=""http://schemas.microsoft.com/wix/2006/wi"">`n   <Fragment>`n        <ComponentGroup Id=""SIMSBulkImportComponents"" Directory=""INSTALLDIR"">`n"

$filecontent = ""

$fileEntries = Get-ChildItem "Installer\src"
foreach($fileName in $fileEntries) 
{ 
	$cleanFilename = $fileName -replace " ", ""
    $filecontent = $filecontent + "`n           <Component>`n               <File Id=""$cleanFilename"" Source=""src\$fileName"" />`n           </Component>"
}      

$filefoot = "`n`n		</ComponentGroup>`n	</Fragment>`n</Wix>"

$file = $filehead + $filecontent + $filefoot | Out-File Installer\SimsBulkImport.wxs -Encoding "UTF8"